using Community.VisualStudio.Toolkit;
using EnvDTE;
using EnvDTE80;
using Snebur.Depuracao;
using Snebur.Linq;
using Snebur.Utilidade;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Snebur.VisualStudio.ConstantesProjeto;


//futuramente vamos normalizar os Error, passando um parametro da mapeamento do arquivo e linha
//detalhes aqui https://www.html5rocks.com/en/tutorials/developertools/sourcemaps/

namespace Snebur.VisualStudio
{
    public partial class GerenciadorProjetos : BaseGerenciadoProjetos<GerenciadorProjetos>
    {
        public SneburVisualStudio2022Package LocalPackage { get; private set; }
        internal ConcurrentDictionary<string, ProjetoTypeScript> ProjetosTS { get; } = new ConcurrentDictionary<string, ProjetoTypeScript>();
        internal ConcurrentDictionary<string, ProjetoSass> ProjetosSass { get; } = new ConcurrentDictionary<string, ProjetoSass>();
        private HashSet<string> Extensoes { get; } = new HashSet<string> { EXTENSAO_TYPESCRIPT, EXTENSAO_SASS, EXTENSAO_CONTROLE_SHTML };
        private HashSet<string> ExtensoesControle { get; } = new HashSet<string> { EXTENSAO_CONTROLE_SHTML_TYPESCRIPT, EXTENSAO_CONTROLE_SHTML, EXTENSAO_CONTROLE_SHTML_ESTILO };
        private DTE2 DTE => GerenciadorProjetos.DTE_GLOBAL;
        private EnvDTE.SolutionEvents SolutionEventsInterno => GerenciadorProjetos.SolutionEvents;
        private EnvDTE.DocumentEvents DocumentEventsInterno => GerenciadorProjetos.DocumentEvents;
        private EnvDTE.BuildEvents BuildEventsInterno => GerenciadorProjetos.BuildEvents;

        //private DebuggerEvents DebuggerEventsInterno => GerenciadorProjetos.DebuggerEvents;
        //private ProjectItemsEvents SolutionItemsEventsInterno => GerenciadorProjetos.SolutionItemsEvents;
        private bool IsLimparLogCompilandoInterno => GerenciadorProjetos.IsLimparLogCompilando;
        ///*private Dictionary<string, FileSystemWatcher> DicionarioArquivosEstiloCompliadoInterno =*/ GerenciadorProjetos.DicionarioArquivosEstiloCompliado;
        //private Dictionary<string, FileSystemWatcher> DicionariosArquivosShtmlInterno = GerenciadorProjetos.DicionariosArquivosShtml;

        private event EventHandler SoluacaoAbertaInterno;
        public Stopwatch TempoCompilacao;
        private bool IsCompilando { get; set; }

        //private ConcurrentBag<string> ArquivosNotificacaoPendente { get; } = new ConcurrentBag<string>();


        private ServicoDepuracao ServicoDepuracao;
         
        private bool _isInicializado;
        private DateTime DataHoraUltimaVerificacao;
        public GerenciadorProjetos() : base()
        {
            this.SoluacaoAbertaInterno += this.GerenciadorProjetos_SoluacaoAberta;
        }

        public async Task InicializarAsync(SneburVisualStudio2022Package package)
        {

            if (this._isInicializado)
            {
                throw new Exception("Gerenciador de projetos já inicializado");
            }

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            this.LocalPackage = package;
            //await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            GerenciadorProjetos.InicializarPropriedadesGlobal();

            this.InicializarEventosVsCommunity();
            this.InicializarEventosDTE();
            this._isInicializado = true;
            await this.InializarServidoDepuracaoAsync();
        }
        
    

        #region Depuração

        #region  Observadores -- FileSystemWatcher

          
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

                this.NotificarArquivoAlterado(new FileInfo(caminhoArquivo));
            }
        }

        private void NotificarArquivoAlterado(FileInfo arquivo)
        {
            var mensagns = this.RetornarMensagensArquivoAlterado(arquivo);

            foreach (var mensagem in mensagns)
            {
                if (mensagem is MensagemControleAlterado mensagemControle &&
                    mensagemControle.IsScript)
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
  
        private List<Mensagem> RetornarMensagensArquivoAlterado(FileInfo arquivo)
        {
            var extensao = arquivo.Extension.ToLower();
            var mensagens = new List<Mensagem>();
            if (extensao == EXTENSAO_SASS)
            {

                var projetosSass = this.ProjetosSass.Values.Where(x => x.ArquivosScss.Contains(arquivo.Name.ToLower())).ToList();
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

        public async Task IniciarServicoDepuracaoAsync()
        {
            if (this.ServicoDepuracao.Estado != EnumEstadoServicoDepuracao.Ativo)
            {
                await this.ServicoDepuracao.IniciarAsync();
            }
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


        private bool _isProjetosAtualizados;
        private bool _isAtualizando;

        private async Task AtualizarProjetosAsync(bool isAtualizarAguardar = false)
        {
            var t = Stopwatch.StartNew();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            try
            {
                await OutputWindow.Instance?.OcuparAsync();


                if (this._isAtualizando)
                {
                    if (!isAtualizarAguardar)
                    {
                        return;
                    }

                    while (this._isAtualizando)
                    {
                        await Task.Delay(500);
                    }
                }

                this._isAtualizando = true;
                var projetosVS = await ProjetoUtil.RetornarProjetosVisualStudioAsync();
                foreach (var projetoVS in projetosVS)
                {
                    try
                    {

                        if (!String.IsNullOrWhiteSpace(projetoVS.FileName))
                        {
                            projetoVS.Save();
                            await this.AtualizarProjetoTypescriptAsync(projetoVS);
                            this.AtualizarProjetoSass(projetoVS);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogVSUtil.LogErro(ex);
                    }
                }

                if (!ConfiguracaoVSUtil.IsNormalizandoTodosProjetos)
                {
                    var sb = this.DTE.Solution.SolutionBuild;
                    var projetosStartup = (Array)sb.StartupProjects;
                    if (projetosStartup?.Length > 0)
                    {
                        var projetosTS = this.ProjetosTS.Values;
                        foreach (string caminhoProjeto in projetosStartup)
                        {
                            var chave = BaseProjeto.RetornarChave(caminhoProjeto);
                            if (this.ProjetosTS.TryGetValue(chave, out var projetoTS))
                            {
                                await projetoTS.NormalizarReferenciasAsync();
                                ProjetoTypeScriptUtil.AtualizarScriptsDebug(projetosTS, projetoTS);
                            }
                        }
                    }

                }

                this._isProjetosAtualizados = (projetosVS.Count > 0);

                LogVSUtil.Sucesso($"Todos os projetos TS ou SASS atualizado t: {t.Elapsed.TotalSeconds}s", t);
                //this.DipensarProjetosDescarregados(projetosVS);
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro($"Falha ao atualizar projetos t: {t.Elapsed.TotalSeconds}s", ex);
                this._isAtualizando = false;
            }
            finally
            {
                await OutputWindow.Instance?.DesocuparAsync();
                this._isAtualizando = false;
            }
        }
         
        protected async Task AtualizarProjetoTypescriptAsync(Project projetoVS)
        {


            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var arquivoProjeto = new FileInfo(projetoVS.FileName);
            var caminhoProjeto = arquivoProjeto.Directory.FullName;
            var caminhoConfiguracao = Path.Combine(caminhoProjeto, ConstantesProjeto.CONFIGURACAO_TYPESCRIPT);
            if (File.Exists(caminhoConfiguracao))
            {
                var configuracaoTS = ProjetoTypeScriptUtil.RetornarConfiguracaoProjetoTypeScript(caminhoConfiguracao);
                if (!configuracaoTS?.IsIgnorar ?? false)
                {
                    var propriedadesVM = projetoVS.Properties.RetornarPropriedadesViewModel();
                    var projetoVM = new ProjetoViewModel(projetoVS, propriedadesVM);
                    var projetoTS = ProjetoTypeScriptUtil.RetornarProjetoTypeScript(configuracaoTS,
                                                                                    projetoVM,
                                                                                    arquivoProjeto,
                                                                                    caminhoConfiguracao);

                    LogVSUtil.Log($"Iniciando gerenciador de projeto typescript '{projetoTS.NomeProjeto}'");
                    var todosArquivo = ProjetoUtil.RetornarTodosArquivos(projetoVS, true);
                    projetoTS.TodosArquivos = todosArquivo;
                    await projetoTS.NormalizarReferenciasAsync();
                    this.AtualizarProjetoTS(projetoTS);
                }
            }

        }

        public void AtualizarProjetoSass(Project projetoVS)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!this.ProjetosSass.ContainsKey(ProjetoUtil.RetornarChave(projetoVS)))
            {
                var arquivioProjeto = new FileInfo(projetoVS.FileName);
                var caminhoProjeto = arquivioProjeto.Directory.FullName;
                var caminhoConfiguracao = Path.Combine(caminhoProjeto, ConstantesProjeto.CONFIGURACAO_SASS);
                if (File.Exists(caminhoConfiguracao))
                {
                    var configuracaoSass = ProjetoSass.RetornarConfiguracao(caminhoConfiguracao);
                    if (!configuracaoSass.IsIgnorar)
                    {
                        var propriedadesVM = projetoVS.Properties.RetornarPropriedadesViewModel();
                        var projetoVM = new ProjetoViewModel(projetoVS, propriedadesVM);
                        var projetoSass = new ProjetoSass(projetoVM, configuracaoSass, arquivioProjeto, caminhoConfiguracao);

                        this.AtualizarProjetoSass(projetoSass);

                    }
                }
            }
        }

        public override void AtualizarProjetoSass(ProjetoSass projetoSass)
        {
            this.ProjetosSass.TryRemove(projetoSass.Chave, out var _);
            this.ProjetosSass.TryAdd(projetoSass.Chave, projetoSass);
        }

       

   

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

        private Task InializarServidoDepuracaoAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                this.ServicoDepuracao = new ServicoDepuracao();
                this.ServicoDepuracao.EventoLog += this.ServicoDepuracao_Log;
            },
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.Default);
        }

        private void GerenciadorProjetos_SoluacaoAberta(object sender, EventArgs e)
        {
            GerenciadorProjetos.SoluacaoAberta?.Invoke(sender, e);
        }

        private static void InicializarPropriedadesGlobal()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            GerenciadorProjetos.DTE_GLOBAL = dte;
            GerenciadorProjetos.SolutionEvents = dte.Events.SolutionEvents;
            GerenciadorProjetos.DocumentEvents = dte.Events.DocumentEvents;
            GerenciadorProjetos.BuildEvents = dte.Events.BuildEvents;
        }

        public static void Reiniciar()
        {
            GerenciadorProjetos.Instancia.ReiniciarServidorReiniciarInterno();
        }

        #region Dispensar

        //private bool _isBloqueioDispensar = false;
        private void SolutionEvents_BeforeClosing()
        {

            this.DispensarProjetos();
        }

        private async Task ReiniciarInternoAsync()
        {
            this.DispensarProjetos();
            await this.AtualizarProjetosAsync();
        }

        public override void AtualizarProjetoTS(ProjetoTypeScript projeto)
        {
            this.ProjetosTS.TryRemove(projeto.Chave, out var _);
            this.ProjetosTS.TryAdd(projeto.Chave, projeto);
        }

        private void DispensarProjetos()
        {
            this.ProjetosTS.Clear();
            this.ProjetosSass.Clear();
        }

        #endregion

    }

    public enum EnumCondicaoAtualizarProjeto
    {
        Tudo = 1,
        AntesCompilar = 2,
        DepoisCompilar = 3,
    }

}