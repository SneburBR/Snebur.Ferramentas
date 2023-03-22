using Community.VisualStudio.Toolkit;
using Snebur.Depuracao;
using Snebur.Linq;
using Snebur.Utilidade;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Snebur.VisualStudio.ConstantesProjeto;

//futuramente vamos normalizar os Error, passando um parâmetro do mapeamento do arquivo e linha
//detalhes aqui https://www.html5rocks.com/en/tutorials/developertools/sourcemaps/
namespace Snebur.VisualStudio
{
    public partial class GerenciadorProjetos : BaseGerenciadoProjetos<GerenciadorProjetos>
    {
        private bool _isInicializado;
        private bool _isExecutando = false;
        private DateTime _dataHoraUltimaVerificacao;
        private ServicoDepuracao _servicoDepuracao;

        private bool IsLimparLogCompilandoInterno => GerenciadorProjetos.IsLimparLogCompilando;
        public bool IsCompilando { get; private set; }

        public Stopwatch TtempoCompilacao { get; private set; }

        public SneburVisualStudio2022Package LocalPackage { get; private set; }
        internal ConcurrentDictionary<string, ProjetoTypeScript> ProjetosTS { get; } = new ConcurrentDictionary<string, ProjetoTypeScript>();
        internal ConcurrentDictionary<string, ProjetoSass> ProjetosSass { get; } = new ConcurrentDictionary<string, ProjetoSass>();
        private HashSet<string> ExtensoesWeb { get; } = new HashSet<string> { EXTENSAO_TYPESCRIPT, EXTENSAO_SASS, EXTENSAO_CONTROLE_SHTML };
        private HashSet<string> ExtensoesControlesSnebur { get; } = new HashSet<string> { EXTENSAO_CONTROLE_SHTML_TYPESCRIPT, EXTENSAO_CONTROLE_SHTML, EXTENSAO_CONTROLE_SHTML_ESTILO };

        private HashSet<string> _extensoesEncodindUt8;
        public HashSet<string> ExtensoesEncodindUt8 => 
            LazyUtil.RetornarValorLazyComBloqueio(ref this._extensoesEncodindUt8,this.RetornarExtesoesEncodingUt8);
         
        public event EventHandler SoluacaoAberta;

        public GerenciadorProjetos() : base()
        {
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

        private Task MensagemArquivoAlteradoAsync(string caminhoArquivo)
        {

            return Task.Run(() =>
            {
                var fi = new FileInfo(caminhoArquivo);
                if (fi.Extension == EXTENSAO_CONTROLE_SHTML ||
                    fi.Extension == EXTENSAO_ESTILO ||
                    fi.Extension == EXTENSAO_SASS
                    /*  ||fi.Extension == EXTENSAO_TYPESCRIPT*/)
                {
                    LogVSUtil.Log($"Arquivo alterado {fi.Name}");

                    this.NotificarArquivoAlterado(new FileInfo(caminhoArquivo));
                }
            });

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
                this._servicoDepuracao.EnviarMensagemParaTodos(mensagem);
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
                    return null;
            }
        }

        public async Task IniciarServicoDepuracaoAsync()
        {
            if (this._servicoDepuracao.Estado != EnumEstadoServicoDepuracao.Ativo)
            {
                await this._servicoDepuracao.IniciarAsync();
            }
        }

        public void PararServicoDepuracao()
        {
            this._servicoDepuracao.Parar();
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

        private void ReiniciarInterno()
        {
            _ = this.ReiniciarAsync();
        }


        private bool _isProjetosAtualizados;
        private bool _isAtualizando;

        private async Task AtualizarProjetosAsync(bool isAtualizarAguardar = false)
        {
            var t = Stopwatch.StartNew();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            try
            {

                await OutputWindow.OcuparAsync();


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
                var projetosVS = await VS.Solutions.GetAllProjectsAsync(ProjectStateFilter.Loaded);
                //ProjetoUtil.RetornarProjetosVisualStudioAsync();

                foreach (var projetoVS in projetosVS)
                {
                    try
                    {

                        if (!String.IsNullOrWhiteSpace(projetoVS.FullPath))
                        {
                            await projetoVS.SaveAsync();
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

                    var projetosStartup = await VS.Solutions.GetStartupProjectsAsync();
                    if (projetosStartup?.Count() > 0)
                    {
                        var projetosTS = this.ProjetosTS.Values;
                        foreach (var statupProject in projetosStartup)
                        {
                            var chave = BaseProjeto.RetornarChave(statupProject.FullPath);
                            if (this.ProjetosTS.TryGetValue(chave, out var projetoTS))
                            {
                                await projetoTS.NormalizarReferenciasAsync();
                                ProjetoTypeScriptUtil.AtualizarScriptsDebug(projetosTS, projetoTS);
                            }
                        }
                    }

                }

                this._isProjetosAtualizados = (projetosVS.Count() > 0);

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
                await OutputWindow.DesocuparAsync();
                this._isAtualizando = false;
            }
        }

        protected async Task AtualizarProjetoTypescriptAsync(Project projetoVS)
        {


            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var arquivoProjeto = new FileInfo(projetoVS.FullPath);
            var caminhoProjeto = arquivoProjeto.Directory.FullName;
            var caminhoConfiguracao = Path.Combine(caminhoProjeto, ConstantesProjeto.CONFIGURACAO_TYPESCRIPT);
            if (File.Exists(caminhoConfiguracao))
            {
                var configuracaoTS = ProjetoTypeScriptUtil.RetornarConfiguracaoProjetoTypeScript(caminhoConfiguracao);
                if (!configuracaoTS?.IsIgnorar ?? false)
                {
                    var projetoVM = new ProjetoViewModel(projetoVS.FullPath,
                                                         projetoVS);

                    var projetoTS = ProjetoTypeScriptUtil.RetornarProjetoTypeScript(configuracaoTS,
                                                                                    projetoVM,
                                                                                    arquivoProjeto,
                                                                                    caminhoConfiguracao);

                    LogVSUtil.Log($"Iniciando gerenciador de projeto typescript '{projetoTS.NomeProjeto}'");
                    var todosArquivo = await SolutionUtil.RetornarTodosArquivosAsync(projetoVS, true);
                    projetoTS.TodosArquivos = todosArquivo;
                    await projetoTS.NormalizarReferenciasAsync();
                    this.AtualizarProjetoTS(projetoTS);
                }
            }
        }

        public void AtualizarProjetoSass(Project projetoVS)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var chave = SolutionUtil.RetornarChave(projetoVS);
            if (!this.ProjetosSass.ContainsKey(chave))
            {
                var arquivioProjeto = new FileInfo(projetoVS.FullPath);
                var caminhoProjeto = arquivioProjeto.Directory.FullName;
                var caminhoConfiguracao = Path.Combine(caminhoProjeto, ConstantesProjeto.CONFIGURACAO_SASS);
                if (File.Exists(caminhoConfiguracao))
                {
                    var configuracaoSass = ProjetoSass.RetornarConfiguracao(caminhoConfiguracao);
                    if (!configuracaoSass.IsIgnorar)
                    {
                        var projetoVM = new ProjetoViewModel(projetoVS.FullPath,
                                                             projetoVS);

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
            return Task.Run(() =>
           {
               this._servicoDepuracao = new ServicoDepuracao();
               this._servicoDepuracao.EventoLog += this.ServicoDepuracao_Log;
           });
        }

       

        private static void InicializarPropriedadesGlobal()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
        }

        public static void Reiniciar()
        {
            GerenciadorProjetos.Instancia.ReiniciarInterno();
        }

        private HashSet<string> RetornarExtesoesEncodingUt8()
        {
            var extensoes = new HashSet<string>();
            extensoes.AddRange(this.ExtensoesWeb);
            extensoes.AddRange(this.ExtensoesControlesSnebur);
            extensoes.AddRange(new string[] { ".cs", ".aspx", ".ashx",".config",
                                              ".json", ".js", ".html"  } );
            return extensoes;
        }

        #region Dispensar

        public async Task ReiniciarAsync()
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