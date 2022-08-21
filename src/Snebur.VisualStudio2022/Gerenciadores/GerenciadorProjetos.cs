using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Depuracao;
using Snebur.Utilidade;
using Snebur.VisualStudio.Utilidade;
using static Snebur.VisualStudio.ExtensaoContantes;


//futuramente vamos normalizar os Error, passando um parametro da mapeamento do arquivo e linha
//detalhes aqui https://www.html5rocks.com/en/tutorials/developertools/sourcemaps/

namespace Snebur.VisualStudio
{
    public partial class GerenciadorProjetos
    {

        public SneburVisualStudio2022Package LocalPackage { get; }
        internal ConcurrentDictionary<string, ProjetoTypeScript> ProjetosTS { get; } = new ConcurrentDictionary<string, ProjetoTypeScript>();
        internal ConcurrentDictionary<string, ProjetoEstilo> ProjetosSass2 { get; } = new ConcurrentDictionary<string, ProjetoEstilo>();
        private HashSet<string> Extensoes { get; } = new HashSet<string> { EXTENSAO_TYPESCRIPT, EXTENSAO_SASS, EXTENSAO_CONTROLE_SHTML };

        private DTE2 DTE => GerenciadorProjetos.DTE_GLOBAL;
        private SolutionEvents SolutionEventsInterno => GerenciadorProjetos.SolutionEvents;
        private DocumentEvents DocumentEventsInterno => GerenciadorProjetos.DocumentEvents;
        private BuildEvents BuildEventsInterno => GerenciadorProjetos.BuildEvents;

        //private DebuggerEvents DebuggerEventsInterno => GerenciadorProjetos.DebuggerEvents;
        //private ProjectItemsEvents SolutionItemsEventsInterno => GerenciadorProjetos.SolutionItemsEvents;
        private bool IsLimparLogCompilandoInterno => GerenciadorProjetos.IsLimparLogCompilando;
        ///*private Dictionary<string, FileSystemWatcher> DicionarioArquivosEstiloCompliadoInterno =*/ GerenciadorProjetos.DicionarioArquivosEstiloCompliado;
        //private Dictionary<string, FileSystemWatcher> DicionariosArquivosShtmlInterno = GerenciadorProjetos.DicionariosArquivosShtml;

        private event EventHandler SoluacaoAbertaInterno;
        public Stopwatch TempoCompilacao;
        private bool IsCompilando { get; set; }

        private ConcurrentBag<string> ArquivosNotificacaoPendente { get; } = new ConcurrentBag<string>();


        private ServicoDepuracao ServicoDepuracao;

        private GerenciadorProjetos(SneburVisualStudio2022Package package)
        {
            this.LocalPackage = package;
        }


        private void Inicializar()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            GerenciadorProjetos.InicializarPropriedadesGlobal();

            this.SolutionEventsInterno.Opened += this.SolutionEvents_Opened;
            this.BuildEventsInterno.OnBuildBegin += this.BuildEvents_OnBuildBegin;
            this.BuildEventsInterno.OnBuildDone += this.BuildEvents_OnBuildDone;



            this.DocumentEventsInterno.DocumentOpened += this.DocumentEvents_DocumentOpened;
            this.DocumentEventsInterno.DocumentSaved += this.DocumentEvents_DocumentSaved;
            this.SolutionEventsInterno.BeforeClosing += this.SolutionEvents_BeforeClosing;


        }


        //private bool _isExecutandoSolucaoAberta = false;
        //private bool _isExecutandoCompilacaoIniciando = false;
        //private bool _isExecutandoCompilacaoConcluida = false;

        private void SolutionEvents_Opened()
        {
            _ = this.ExecutarAsync(this.SolucaoAbertaAsync);
        }


        private void BuildEvents_OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            _ = this.ExecutarAsync(this.CompilacaoIniciandoAsync);
        }

        private DateTime DataHoraUltimaVerificacao;

        private void BuildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            var t = Stopwatch.StartNew();
            LogVSUtil.Log("Executando tarefas depois da compilação");
            BuildDoneAsync().Wait();
            LogVSUtil.Sucesso("Tarefas depois da compilação", t);
        }

        private async Task BuildDoneAsync()
        {
            if (GerenciadorProjetos.DiretorioProjetoTypescriptInicializacao == null)
            {
                ProjetoUtil.DefinirProjetosInicializacao();
            }
            var tempo = DateTime.Now - this.DataHoraUltimaVerificacao;
            if (tempo.TotalSeconds > 15)
            {
                if (await this.ExecutarAsync(this.CompilacaoConcluidaAsync))
                {
                    this.DataHoraUltimaVerificacao = DateTime.Now;
                }

            }
            else
            {
                this.NormalizarScripts();
            }
        }

        private async Task SolucaoAbertaAsync()
        {
            await this.AtualizarProjetosAsync();
            this.SoluacaoAbertaInterno?.Invoke(this, EventArgs.Empty);
        }

        private async Task CompilacaoIniciandoAsync()
        {
            if (LogVSUtil.IsNormalizandoTodosProjetos)
            {
                return;
            }

            this.IsCompilando = true;

            if (this.TempoCompilacao != null) this.TempoCompilacao.Stop();
            this.TempoCompilacao = Stopwatch.StartNew();
            if (this.IsLimparLogCompilandoInterno)
            {
                //LogVSUtil.ClearAsync();
            }

            try
            {
                var tempoGeral = Stopwatch.StartNew();
                var tempoGerenciador = Stopwatch.StartNew();

                await this.AtualizarProjetosAsync();
                tempoGerenciador.Stop();
                LogVSUtil.Sucesso("Gerenciador atualizado", tempoGerenciador);

                //var solutionsBuild = (SolutionBuild2)this.DTE.Solution.SolutionBuild;
                //(Array)solutionsBuild.StartupProjects

                var projetosVS = await ProjetoUtil.RetornarProjetosVisualStudioAsync();

                foreach (var projetoVS in projetosVS)
                {
                    var identificador = ProjetoUtil.RetornarChave(projetoVS);
                    if (this.ProjetosTS.TryGetValue(identificador, out ProjetoTypeScript projetoTS))
                    {
                        this.ServicoDepuracao?.SalvarPorta(projetoTS.CaminhoProjeto);
                    }

                }

                tempoGeral.Stop();
                LogVSUtil.Sucesso($"Processos antes de compilar", tempoGeral);
            }
            catch (FileNotFoundException ex)
            {
                LogVSUtil.LogErro(ex);
                await this.ReiniciarInternoAsync();
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }
        }

        private async Task CompilacaoConcluidaAsync()
        {
            this.NormalizarScripts();
            //this.GerarScriptsMinificados();
            //this.GerarScriptsProtegidos();


            if (this.TempoCompilacao != null)
            {
                //this.TempoCompilacao.Stop();
                LogVSUtil.Sucesso("Compilação finalizada", this.TempoCompilacao);
            }

            this.IsCompilando = false;
            this.NotificarArquivosAlteradoPendentes();
        }

        private void NormalizarScripts()
        {
            foreach (var projetoTS in this.ProjetosTS.Values)
            {
                using (var normalizarCompilacao = new NormalizarCompilacaoJavascript(projetoTS))
                {
                    normalizarCompilacao.Normalizar();
                }
            }
        }

        #region Depuracao

        #region  Observadores -- FileSystemWatcher

        private ObservadorArquivoProjeto RetornarObservadorArquivoProjeto(string identificadorProjeto)
        {
            lock ((GerenciadorProjetos.DicionarioObservadoresArquivo as ICollection).SyncRoot)
            {
                if (!GerenciadorProjetos.DicionarioObservadoresArquivo.ContainsKey(identificadorProjeto))
                {
                    GerenciadorProjetos.DicionarioObservadoresArquivo.Add(identificadorProjeto, new ObservadorArquivoProjeto(identificadorProjeto));
                }
                return GerenciadorProjetos.DicionarioObservadoresArquivo[identificadorProjeto];
            }
        }

        private void ObservadorArquivo_Error(object sender, ErrorEventArgs e)
        {
            LogVSUtil.LogErro(e.GetException());
        }

        private async Task MensagemArquivoAlteradoAsync(Document documento)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var caminhoArquivo = documento.FullName;
            var fi = new FileInfo(caminhoArquivo);
            if (fi.Extension == EXTENSAO_CONTROLE_SHTML ||
                fi.Extension == EXTENSAO_ESTILO ||
                fi.Extension == EXTENSAO_SASS
                /*  ||fi.Extension == EXTENSAO_TYPESCRIPT*/)
            {
                LogVSUtil.Log($"Arquivo alterado {fi.Name}");


                if (this.IsCompilando)
                {
                    this.ArquivosNotificacaoPendente.Add(caminhoArquivo);
                }
                else
                {
                    this.NotificarArquivoAlterado(new FileInfo(caminhoArquivo));

                }
            }
        }

        private async void NotificarArquivoAlterado(FileInfo arquivo)
        {
            var mensagns = this.RetornarMensagensArquivoAlterado(arquivo);

            foreach (var mensagem in mensagns)
            {
                if (mensagem is MensagemControleAlterado mensagemControle && mensagemControle.IsScript)
                {
                    return;
                }
                //{
                //    var projetoTS = this.ProjetosTS.Values.Where(x => x.Arquivos.Contains(arquivo.FullName)).FirstOrDefault();
                //    if (projetoTS == null)
                //    {
                //        return;
                //    }

                //    var compilarRuntime = new CompilarTypescriptRuntime(projetoTS, arquivo);
                //    if (!(await compilarRuntime.CompilarAsync()))
                //    {
                //        return;
                //    }
                //    mensagemControle.UrlScriptRuntime = compilarRuntime.UrlScriptRuntime;
                //    mensagemControle.CaminhoConstrutor = compilarRuntime.CaminhoTipo;

                //}
                this.ServicoDepuracao.EnviarMensagemParaTodos(mensagem);
            }
        }

        private void NotificarArquivosAlteradoPendentes()
        {
            var caminhosArquivo = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            while (this.ArquivosNotificacaoPendente.TryTake(out string caminhoArquivo))
            {
                caminhosArquivo.Add(caminhoArquivo);
            }

            foreach (var caminhoArquivo in caminhosArquivo)
            {
                this.NotificarArquivoAlterado(new FileInfo(caminhoArquivo));
            }
        }

        private List<Mensagem> RetornarMensagensArquivoAlterado(FileInfo arquivo)
        {
            var extensao = arquivo.Extension.ToLower();
            var mensagens = new List<Mensagem>();
            if (extensao == EXTENSAO_SASS)
            {

                var projetosSass = this.ProjetosSass2.Values.Where(x => x.ArquivosScss.Contains(arquivo.Name.ToLower())).ToList();
                foreach (var projetoSass in projetosSass)
                {
                    mensagens.Add(new MensagemEstiloCssAlterado
                    {
                        NomeArquivo = projetoSass.ArquivoEstiloCompilado.Name
                    });
                }
            }
            else
            {
                var mensagem = this.RetornarMensagemArquivoAlterado(arquivo, extensao);
                if (mensagem != null)
                {
                    mensagens.Add(mensagem);
                }

            }
            return mensagens;

        }
        private Mensagem RetornarMensagemArquivoAlterado(FileInfo arquivo,
                                                         string extensao)
        {
            switch (extensao)
            {
                case EXTENSAO_ESTILO:

                    return new MensagemEstiloCssAlterado
                    {
                        NomeArquivo = arquivo.Name
                    };

                case EXTENSAO_CONTROLE_SHTML:

                    return new MensagemControleAlterado
                    {
                        NomeControle = Path.GetFileNameWithoutExtension(arquivo.Name)
                    };

                case EXTENSAO_TYPESCRIPT:

                    if (arquivo.Name.ToLower().EndsWith(EXTENSAO_CONTROLE_SHTML_TYPESCRIPT))
                    {
                        return new MensagemControleAlterado
                        {
                            NomeControle = arquivo.Name.Replace(EXTENSAO_CONTROLE_SHTML_TYPESCRIPT, String.Empty),
                            IsScript = true,
                        };
                    }
                    return null;


                case EXTENSAO_SCRIPT:

                    return new MensagemScriptAlterado
                    {
                        NomeArquivo = arquivo.Name
                    };


                default:

                    //ignorar arquivo
                    return null;

            }

        }

        public void IniciarServicoDepuracao()
        {
            this.ServicoDepuracao.Iniciar();
        }

        public void PararServicoDepuracao()
        {
            this.ServicoDepuracao.Parar();
        }

        public Task<object> GetServiceAsync(Type serviceType)
        {
            return this.LocalPackage.GetServiceAsync(serviceType);
        }

        #endregion

        private void ServicoDepuracao_Log(object sender, MensagemEventArgs<MensagemLog> e)
        {
            var mensagemLog = e.Mensagem;
            var mensagemCompleta = $"Origem: {e.SessaoConectada.Sessao.Identificador} - {e.Mensagem.Mensagem}";
            LogVSUtil.Log(mensagemCompleta, mensagemLog.TipoLog);
        }

        #endregion

        private void ReiniciarServidorReiniciarInterno()
        {
            _ = this.ReiniciarInternoAsync();
        }


        private bool _isInicializado;
        private async Task AtualizarProjetosAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            try
            {

                var projetosVS = await ProjetoUtil.RetornarProjetosVisualStudioAsync();
                foreach (var projetoVS in projetosVS)
                {
                    try
                    {
                        if (!String.IsNullOrWhiteSpace(projetoVS.FileName))
                        {
                            projetoVS.Save();
                            this.AtualizarProjetoTypescript(projetoVS);
                            this.AtualizarProjetoSass(projetoVS);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogVSUtil.LogErro(ex);
                    }
                }

                if (!LogVSUtil.IsNormalizandoTodosProjetos)
                {
                    var sb = DTE.Solution.SolutionBuild;
                    var projetosStartup = (Array)sb.StartupProjects;
                    if (projetosStartup?.Length > 0)
                    {
                        var projetosTS = this.ProjetosTS.Values;
                        foreach (string caminhoProjeto in projetosStartup)
                        {
                            var chave = Path.GetFileName(caminhoProjeto);
                            if (this.ProjetosTS.ContainsKey(chave))
                            {
                                var projetoTS = this.ProjetosTS[chave];
                                projetoTS.NormalizarReferencias(false);
                                ProjetoTypeScriptUtil.AtualizarScriptsDebug(projetosTS, projetoTS);
                            }
                        }
                    }

                }

                _isInicializado = (projetosVS.Count > 0);


                this.DipensarProjetosDescarregados(projetosVS);
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }
        }


        private void AtualizarProjetoTypescript(Project projetoVS)
        {
            if (!this.ProjetosTS.ContainsKey(ProjetoUtil.RetornarChave(projetoVS)))
            {
                var caminhoProjeto = new FileInfo(projetoVS.FileName).Directory.FullName;
                var caminhoConfiguracao = Path.Combine(caminhoProjeto, ProjetoUtil.CONFIGURACAO_TYPESCRIPT);
                if (File.Exists(caminhoConfiguracao))
                {
                    var configuracaoTS = ProjetoTypeScriptUtil.RetornarConfiguracaoProjetoTypeScript(caminhoConfiguracao);
                    if (!configuracaoTS?.IsIgnorar ?? false)
                    {
                        var projetoTS = ProjetoTypeScriptUtil.RetornarProjetoTypeScript(configuracaoTS, this.DTE, projetoVS, caminhoProjeto, caminhoConfiguracao);
                        LogVSUtil.Log($"Iniciando gerenciador de projeto typescript '{projetoTS.NomeProjeto}'");
                        var todosArquivo = ProjetoUtil.RetornarTodosArquivos(projetoVS, true);
                        projetoTS.TodosArquivos = todosArquivo;
                        this.ProjetosTS.TryAdd(ProjetoUtil.RetornarChave(projetoVS), projetoTS);
                    }
                }
            }


        }

        private void AtualizarProjetoSass(Project projetoVS)
        {
            if (!this.ProjetosSass2.ContainsKey(ProjetoUtil.RetornarChave(projetoVS)))
            {
                var caminhoProjeto = new FileInfo(projetoVS.FileName).Directory.FullName;
                var caminhoConfiguracao = Path.Combine(caminhoProjeto, ProjetoUtil.CONFIGURACAO_SASS);
                if (File.Exists(caminhoConfiguracao))
                {
                    var configuracaoSass = ProjetoEstilo.RetornarConfiguracao(caminhoConfiguracao);
                    if (!configuracaoSass.IsIgnorar)
                    {
                        var projetoSass = new ProjetoEstilo(configuracaoSass, this.DTE, projetoVS, caminhoProjeto, caminhoConfiguracao);
                        this.ProjetosSass2.TryAdd(ProjetoUtil.RetornarChave(projetoVS), projetoSass);
                    }
                }
            }
        }

        #region Novo Arquivo

        #endregion

        #region Dispensar

        private bool _isBloqueioDispensar = false;
        private void SolutionEvents_BeforeClosing()
        {
            _ = this.ExecutarAsync(this.DispensarAsync);
        }

        private async Task ReiniciarInternoAsync()
        {
            await this.DispensarAsync();
            await this.AtualizarProjetosAsync();
        }

        private void DipensarProjetosDescarregados(List<Project> projetosVS)
        {
            var chaves = projetosVS.Select(x => ProjetoUtil.RetornarChave(x)).ToHashSet();
            var chavesProjetoDispensar = this.ProjetosTS.Where(x => !chaves.Contains(x.Key)).Select(x => x.Key).ToList();

            foreach (var chave in chavesProjetoDispensar)
            {
                this.DispensarProjeto(chave);
            }
        }

        private async Task DispensarAsync()
        {
            var projetosVS = await ProjetoUtil.RetornarProjetosVisualStudioAsync();
            foreach (var projetoVS in projetosVS)
            {
                this.DispensarProjeto(ProjetoUtil.RetornarChave(projetoVS));
            }
        }

        private void DispensarProjeto(string identificadorProjeto)
        {
            if (this.ProjetosTS.ContainsKey(identificadorProjeto))
            {
                if (GerenciadorProjetos.DicionarioObservadoresArquivo.TryGetValue(identificadorProjeto, out ObservadorArquivoProjeto observadorProjeto))
                {
                    observadorProjeto.Dispose();
                }

                if (this.ProjetosTS.TryRemove(identificadorProjeto, out ProjetoTypeScript projetoTS))
                {
                    LogVSUtil.Log($"Dispensando projeto  '{projetoTS.NomeProjeto}'");
                    projetoTS.ArquivosTS?.Clear();
                    projetoTS.ArquivosTypeScript?.Clear();
                    projetoTS.ArquivosTypeScriptOrdenados?.Clear();
                    projetoTS.Dispose();
                }

                if (this.ProjetosSass2.TryRemove(identificadorProjeto, out ProjetoEstilo projetoSass))
                {
                    LogVSUtil.Log($"Dispensando projeto  '{projetoTS.NomeProjeto}'");
                    projetoSass.Dispose();
                }
            }
        }

        #endregion

        private bool _isExecutando = false;


        public async Task<bool> ExecutarAsync(Func<Task> acao)
        {
            if (!this._isExecutando)
            {
                this._isExecutando = true;
                try
                {
                    await acao.Invoke();
                    return true;
                }
                catch (Exception ex)
                {
                    LogVSUtil.LogErro(ex);
                }
                finally
                {
                    this._isExecutando = false;
                }
            }
            return false;
        }

        public async Task<bool> ExecutarAsync<T>(Func<T, Task> acao, T parametro1)
        {
            if (!this._isExecutando)
            {
                this._isExecutando = true;
                try
                {
                    await acao?.Invoke(parametro1);
                    return true;
                }
                catch (Exception ex)
                {
                    LogVSUtil.LogErro(ex);
                }
                finally
                {
                    this._isExecutando = false;
                }
            }
            return false;
        }



    }

    public enum EnumCondicaoAtualizarProjeto
    {
        Tudo = 1,
        AntesCompilar = 2,
        DepoisCompilar = 3,
    }

}