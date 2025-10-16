using Snebur.Comunicacao;
using Snebur.Dominio;
using Snebur.Seguranca;
using Snebur.Servicos;
using Snebur.UI;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Timers;

#if NET48
using System.Configuration;
#endif

namespace Snebur
{
    public abstract partial class AplicacaoSnebur
    {
        #region Constantes

        private const int INTERVALO_NOTIFICAR_APLICACAO_ATIVA = 10;
        private const int TEMPO_VERIFICAR_INICIALIZACAO = 30;
        private const string NOME_CULTURA_PADRAO = "pt-BR";

        #endregion

        #region Propriedades 

        protected Dictionary<string, string> UrlsServico { get; } = new Dictionary<string, string>();
        private string _identificadorAplicacao;
        private string _nomeAplicacao;
        private string _nomeEmpresa;
        private Version _versaoAplicacao;
        private bool? _capturarPrimeiroErroAtivo;
        private CultureInfo _cultura;
        private string _ipPublico;

        private IServicoLogErro _servicoLogErro;
        private IServicoLogSeguranca _servicoLogSeguranca;
        private IServicoLogAplicacao _servicoLogAplicacao;
        private IServicoLogDesempenho _servicoLogDesempenho;
        private IServicoUsuario _servicoUsuario;
        private object _bloqueioInicializar = new object();

        private readonly System.Timers.Timer TimerAplicacaoAtiva;

        private TimeSpan? _diferencaDataHoraServidor;
        private NameValueCollection _appSettings;
        private NameValueCollection _connectionStrings;

        private bool IsAplicaoAspNet { get; }

        public virtual string UrlPingInternetConectada { get; set; }

        public IAlerta Alerta { get; set; }

        internal protected virtual dynamic DispatcherObject { get; } = null;
        public virtual bool IsMainThread => Thread.CurrentThread.ManagedThreadId == _mainThreadId;

        protected virtual bool IsNotificarLogAplicacaoInicializada { get; } = true;

        public virtual string UrlWebService => this.RetornarUrlServico("UrlWebService");
        public virtual string UrlServicoArquivo => this.RetornarUrlServico("UrlServicoArquivo");
        public virtual string UrlServicoDados => this.RetornarUrlServico("UrlServicoDados") ??
                                                 this.UrlWebService;
        public virtual string UrlServicoImagem => this.RetornarUrlServico("UrlServicoImagem") ??
                                                  this.UrlServicoArquivo;

        public virtual bool IsSeperarAppDataPorIdentificadorPropretario { get; } = true;
        public virtual bool IsAlicacaoCliente { get; } = false;

        public virtual string IdentificadorAplicacao
        {
            get
            {
                if (this._identificadorAplicacao == null)
                {
                    this._identificadorAplicacao = this.FuncaoRetornarIdentificadorAplicacao.Invoke();
                }
                return this._identificadorAplicacao;
            }
        }

        public virtual CultureInfo CulturaPadrao
        {
            get
            {
                if (this._cultura == null)
                {
                    this._cultura = this.FuncaoRetornarCulturaPadrao.Invoke();
                }
                return this._cultura;
            }
        }

        public virtual string IdentificadorProprietario
        {
            get
            {
                return this.FuncaoRetornarIdentificadorProprietario.Invoke();
            }
        }

        public virtual CredencialUsuario CredencialUsuario
        {
            get
            {
                return this.FuncaoRetornarCredencialUsuarioUsuario.Invoke();
            }
        }

        public virtual Guid IdentificadorSessaoUsuario
        {
            get
            {
                return this.FuncaoRetornarIdentificadorSessaoUsuario.Invoke();
            }
        }

        public bool IsUsuarioAnonimo => this.CredencialUsuario.Validar(CredencialAnonimo.Anonimo);

        public bool CapturarPrimeiroErroAtivo
        {
            get
            {
                if (!this._capturarPrimeiroErroAtivo.HasValue)
                {
                    this._capturarPrimeiroErroAtivo = false;
                    //this.ServicoErro.CapturarPrimeiroErro();
                }
                return this._capturarPrimeiroErroAtivo.Value;
            }
        }

        public string NomeAplicacao
        {
            get
            {
                if (this._nomeAplicacao == null)
                {
                    this._nomeAplicacao = ReflexaoUtil.RetornarNomeAplicacao();
                }
                return this._nomeAplicacao;
            }
        }

        public string NomeEmpresa
        {
            get
            {
                if (this._nomeEmpresa == null)
                {
                    this._nomeEmpresa = ReflexaoUtil.RetornarNomeEmpresa();
                }
                return this._nomeEmpresa;
            }
        }

        public string CaminhoExecutavel
        {
            get
            {
                return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            }
        }

        public virtual EnumAmbienteServidor AmbienteServidor => this.FuncaoRetornarAmbienteServidor.Invoke();

        public string DiretorioAplicacao
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public Version VersaoAplicao
        {
            get
            {
                if (this._versaoAplicacao == null)
                {
                    this._versaoAplicacao = SistemaUtil.VersaoAplicacao;
                }
                return this._versaoAplicacao;
            }
        }

        private object BloqueioDataHoraServidor = new object();
        public TimeSpan DiferencaDataHoraUtcServidor
        {
            get
            {
                if (!this._diferencaDataHoraServidor.HasValue)
                {
                    lock (this.BloqueioDataHoraServidor)
                    {
                        if (!this._diferencaDataHoraServidor.HasValue)
                        {
                            var dataHoraServidor = this.FuncaoRetornarDataHoraUtcServidor.Invoke();
                            this._diferencaDataHoraServidor = dataHoraServidor - DateTime.UtcNow;
                        }
                    }
                }
                return this._diferencaDataHoraServidor.Value;
            }
        }

        public IServicoLogErro ServicoErro
        {
            get
            {
                if (this._servicoLogErro == null && this.FuncaoRetornarServicoLogErro != null)
                {
                    this._servicoLogErro = this.FuncaoRetornarServicoLogErro.Invoke();

                }
                return this._servicoLogErro;
            }
        }

        public IServicoLogSeguranca ServicoSeguranca
        {
            get
            {
                if (this._servicoLogSeguranca == null && this.FuncaoRetornarServicoLogSeguranca != null)
                {
                    this._servicoLogSeguranca = this.FuncaoRetornarServicoLogSeguranca.Invoke();
                }
                return this._servicoLogSeguranca;
            }
        }

        public IServicoLogAplicacao ServicoLogAplicacao
        {
            get
            {
                if (this._servicoLogAplicacao == null && this.FuncaoRetornarServicoLogAplicacao != null)
                {
                    this._servicoLogAplicacao = this.FuncaoRetornarServicoLogAplicacao.Invoke();
                }
                return this._servicoLogAplicacao;
            }
        }

        public IServicoLogDesempenho ServicoDesempenho
        {
            get
            {
                if (this._servicoLogDesempenho == null && this.FuncaoRetornarServicoLogDesempenho != null)
                {
                    this._servicoLogDesempenho = this.FuncaoRetornarServicoLogDesempenho.Invoke();
                }
                return this._servicoLogDesempenho;
            }
        }

        public IServicoUsuario ServicoUsuario
        {
            get
            {
                if (this._servicoUsuario == null && this.FuncaoRetornarServicoUsuario != null)
                {
                    this._servicoUsuario = this.FuncaoRetornarServicoUsuario.Invoke();
                }
                return this._servicoUsuario;
            }
        }

        public abstract EnumTipoAplicacao TipoAplicacao { get; }
        //{
        //    get
        //    {
        //        //if(this.HttpContext!= null)
        //        //{
        //        //    return EnumTipoAplicacao.DotNet_WebService;
        //        //}

        //        if (!this._tipoAplicacao.HasValue)
        //        {
        //            this._tipoAplicacao = SistemaUtil.TipoAplicacao;
        //        }
        //        return this._tipoAplicacao.Value;
        //    }
        //}

        public string NomeComputador
        {
            get
            {
                return Environment.MachineName;
            }
        }

        public bool AtivarLogServicoOnline { get; private set; }

        public bool Inicializada { get; private set; }

        internal TimeSpan TempoVerificarInicializacao
        {
            get
            {
                if (DebugUtil.IsAttached)
                {
                    // return TimeSpan.FromHours(10);
                }
                return TimeSpan.FromSeconds(TEMPO_VERIFICAR_INICIALIZACAO);
            }
        }

        public string CaminhoAppDataAplicacao
        {
            get
            {
                return ConfiguracaoUtil.CaminhoAppDataAplicacao;
            }
        }

        public virtual NameValueCollection AppSettings
        {
            get
            {
                return LazyUtil.RetornarValorLazyComBloqueio(ref this._appSettings, this.RetornarAppSettings);
            }
        }

        public virtual NameValueCollection ConnectionStrings
        {
            get
            {
                return LazyUtil.RetornarValorLazyComBloqueio(ref this._connectionStrings, this.RetornarConnectionStrings);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual InformacaoSessao InformacaoSessao => this.RetornarInformacaoSessaoUsuario();
        public virtual string IpPublico => this.RetornarIpPublico();

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public virtual InformacaoSessaoUsuario InformacaoSessaoUsuarioRequisicaoAtual => SessaoUtil.RetornarInformacaoSessaoUsuarioAplicacao();

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public virtual Guid IdentificadorSessaoUsuarioRequisicaoAtual => this.IdentificadorSessaoUsuario;

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public virtual CredencialUsuario CredencialUsuarioRequisicaoAtual => this.CredencialUsuario;

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public virtual string IdentificadorProprietarioRequisicaoAtual => this.IdentificadorProprietario;

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public virtual string UserAgent => null;

        //public abstract string RetornarIpDaRequisicao();

        #endregion

        #region Propriedade funções
        protected Func<CultureInfo> FuncaoRetornarCulturaPadrao { get; set; } = RetornarCultauraPadrao;

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected Func<string> FuncaoRetornarIdentificadorProprietario { get; set; } = RetornarIdentificadorProprietario;

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected Func<string> FuncaoRetornarIdentificadorAplicacao { get; set; } = RetornarIdentificadorAplicacao;

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected Func<Guid> FuncaoRetornarIdentificadorSessaoUsuario { get; set; } = SessaoUtil.RetornarIdentificadorSessaoUsuario;

        protected Func<CredencialUsuario> FuncaoRetornarCredencialUsuarioUsuario { get; set; } = SessaoUtil.RetornarCredencialUsuario;

        protected Func<DateTime> FuncaoRetornarDataHoraUtcServidor { get; set; } = RetornarDataHoraUtcServidor;

        //protected Func<IInformacaoSessao> FuncaoRetornarInformacaoSessaoUsuario { get; set; } = SessaoUtil.RetornarInformacaoSessaoUsuarioAtual;

        protected Action AcaoIniciarNovaSessaoUsuario { get; set; } = SessaoUtil.InicializarNovaSessaoUsuario;

        protected Func<IServicoLogErro> FuncaoRetornarServicoLogErro { get; set; } = RetornarServicoErro;

        protected Func<IServicoLogSeguranca> FuncaoRetornarServicoLogSeguranca { get; set; } = RetornarServicoLogSeguranca;

        protected Func<IServicoLogAplicacao> FuncaoRetornarServicoLogAplicacao { get; set; } = RetornarServicoLogAplicacao;

        protected Func<IServicoLogDesempenho> FuncaoRetornarServicoLogDesempenho { get; set; } = RetornarServicoLogDesempenho;

        protected Func<IServicoUsuario> FuncaoRetornarServicoUsuario { get; set; } = null;

        public Func<BaseInformacaoAdicionalServicoCompartilhado> FuncaoRetornarInformacaoAdicionalServicoCompartilhado { get; set; } = null;

        public Func<EnumAmbienteServidor> FuncaoRetornarAmbienteServidor { get; protected set; } = RetornarAmbienteServidor;

        public bool IgnorarErros
        {
            get
            {
                return Convert.ToBoolean(this.AppSettings["IgnorarErros"]);
            }
        }

        public event EventHandler NovaSessaoUsuarioInicializada;
        public event EventHandler CredencialAlterada;

        public bool IsAplicacaoAspNet { get; }
        public IAplicacaoSneburAspNet AspNet { get; }

        #endregion

        #region Construtor
        protected AplicacaoSnebur()
        {
            lock (_bloqueio)
            {
                if (_aplicacao != null)
                {
                    throw new Exception($"Já existe uma aplicação inicializada: {_aplicacao?.GetType().Name}");
                }
                _aplicacao = this;
            }

            AppDomain.CurrentDomain.UnhandledException += this.Aplicacao_UnhandledException;
            //AppDomain.CurrentDomain.FirstChanceException += this.Aplicacao_FirstChanceException;
            AppDomain.CurrentDomain.ProcessExit += this.Aplicacao_ProcessExit;

            this.TimerAplicacaoAtiva = new System.Timers.Timer((int)TimeSpan.FromMinutes(INTERVALO_NOTIFICAR_APLICACAO_ATIVA).TotalMilliseconds);
            this.TimerAplicacaoAtiva.Elapsed += this.Aplicacao_NotificarAplicacaoAtiva;

            Thread.CurrentThread.CurrentCulture = this.CulturaPadrao;
            Thread.CurrentThread.CurrentUICulture = this.CulturaPadrao;

            //WPF
            //Implementar classe AplicacaoSneburWpf que herdade AplicacaoSnebur
            //FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            this.InicializarComunicacao();

            if (this is IAplicacaoSneburAspNet aspNet)
            {
                this.IsAplicacaoAspNet = true;
                this.AspNet = aspNet;
            }
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public virtual void InicializarAsync()
        {
            ThreadUtil.ExecutarAsync(this.Inicializar);
        }

        public virtual void Inicializar()
        {
            if (!this.Inicializada)
            {
                lock (this._bloqueioInicializar)
                {
                    if (!this.Inicializada)
                    {
                        this.TimerAplicacaoAtiva.Start();
                        this.InicializarServicoUsuario();

                        if (this.IsNotificarLogAplicacaoInicializada &&
                            !DebugUtil.IsAttached)
                        {
                            LogUtil.LogAsync("Aplicação inicializada");
                            LogUtil.LogAplicacaoAtiva();
                        }
                    }
                }
            }
        }
        #endregion

        #region Comunicação

        private void InicializarComunicacao()
        {



//#if NET40
//            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
//#else
//            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 |
//                                                   SecurityProtocolType.Tls12
//#if NET45 == false
//                                                   | SecurityProtocolType.Tls13;
//#else
//                                                   ;
//#endif
//#endif

//            //Ignorar erros da certificados https, isso pode ocorres caso da DataHora do computador cliente do usuario esteja mais 12 horas de diferença com servidor
//            ServicePointManager.ServerCertificateValidationCallback = delegate (object obj,
//                                                                                          System.Security.Cryptography.X509Certificates.X509Certificate X509certificate,
//                                                                                          System.Security.Cryptography.X509Certificates.X509Chain chain,
//                                                                                          System.Net.Security.SslPolicyErrors errors)
//            {
//                return true;
//            };

            //this.DefinirConfiguracoesHttps();
        }
        //Conexões https
//        private void DefinirConfiguracoesHttps()
//        {
//            try
//            {
//                ServicePointManager.Expect100Continue = true;
//#if NET40
//                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
//#endif

//#if NET45
//                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |
//                                                       SecurityProtocolType.Tls11 |
//                                                       SecurityProtocolType.Tls ;   
//#endif

//#if NET48_OR_GREATER || NET6_0_OR_GREATER
//                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 |
//                                                       SecurityProtocolType.Tls12 |
//                                                       SecurityProtocolType.Tls11 |
//                                                       SecurityProtocolType.Tls;
//#endif

//                ServicePointManager.DefaultConnectionLimit = 256;

//#if NET6_0_OR_GREATER == false

//                var assemblyNet = Assembly.GetAssembly(typeof(System.Net.Configuration.SettingsSection));
//                if (assemblyNet != null)
//                {
//                    var aSettingsType = assemblyNet.GetType("System.Net.Configuration.SettingsSectionInternal");
//                    if (aSettingsType != null)
//                    {
//                        object anInstance = aSettingsType.InvokeMember("Section",
//                          BindingFlags.Static |
//                          BindingFlags.GetProperty |
//                          BindingFlags.NonPublic, null, null, new object[] { });

//                        if (anInstance != null)
//                        {
//                            FieldInfo aUseUnsafeHeaderParsing = aSettingsType.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic | BindingFlags.Instance);
//                            if (aUseUnsafeHeaderParsing != null)
//                            {
//                                aUseUnsafeHeaderParsing.SetValue(anInstance, true);
//                            }
//                        }
//                    }
//                }
//#endif
//            }
//            catch
//            {
//            }
//        }
#endregion

        #region ServicoUsuario

        private void InicializarServicoUsuario()
        {
            var servicoUsuario = this.ServicoUsuario;
            if (servicoUsuario != null)
            {
                if (!servicoUsuario.SessaoUsuarioAtiva(this.CredencialUsuario, this.IdentificadorSessaoUsuario))
                {
                    this.IniciarNovaSessaoAnonima();
                }
            }
        }

        public virtual void IniciarNovaSessaoAnonima()
        {
            this.AcaoIniciarNovaSessaoUsuario.Invoke();
            this.NotificarNovaSessaoUsuarioInicializada();
        }
        public virtual void NotificarNovaSessaoUsuarioInicializada()
        {
            this.NovaSessaoUsuarioInicializada?.Invoke(this, EventArgs.Empty);
        }

        public virtual void NotificarCredencialAlterada()
        {
            this.CredencialAlterada?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Log aplicação inicializada

        private void VerificarAplicacaoInicializada()
        {
            if (!this.Inicializada)
            {
                //if (DebugUtil.IsAttached)
                //{
                //    throw new Exception("Chamar o método Inicializar logo apos instanciar a AplicacaoSnebur");
                //}
                this.Inicializar();
            }
        }

        private void AtivarLogAplicacao()
        {
            LogUtil.AtivarLogServicoOnlineAsync((resultado) =>
            {
                this.AtivarLogServicoOnline = resultado;
            });
        }
        #endregion

        #region Manipuladores de erros

        private void Aplicacao_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            if (this.CapturarPrimeiroErroAtivo)
            {
                throw new NotImplementedException();
            }
        }

        private void Aplicacao_ProcessExit(object sender, EventArgs e)
        {
            if (this.Inicializada)
            {
                var informacaoAdicional = ServicoCompartilhadoUtil.RetornarInformacaoAdicionalServicoCompartilhado();
                LogUtil.Log("Aplicação finalizada", informacaoAdicional, false);
            }
        }

        private void Aplicacao_NotificarAplicacaoAtiva(object sender, ElapsedEventArgs e)
        {
            LogUtil.LogAplicacaoAtivaAsync();
        }

        private void Aplicacao_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
            {
                try
                {
                    LogUtil.ErroGlobal(e.ExceptionObject as Exception);
                }
                catch
                {
                }
            }
        }
        #endregion

        #region Métodos públicos

        public void AtualizarDiferencaDataHoraServidor()
        {
            var dataHoraServidor = this.FuncaoRetornarDataHoraUtcServidor.Invoke();
            this._diferencaDataHoraServidor = dataHoraServidor - DateTime.UtcNow;
        }
        #endregion

        #region Métodos privados - manipuladores

        private static string RetornarIdentificadorAplicacao()
        {
            if (Atual.AppSettings[SessaoUtil.IDENTIFICADOR_APLICACAO] != null)
            {
                return Atual.AppSettings[SessaoUtil.IDENTIFICADOR_APLICACAO];
            }
            var assemblyEntrada = ReflexaoUtil.AssemblyEntrada;
            if (assemblyEntrada != null)
            {
                var assemblyName = new AssemblyName(assemblyEntrada.FullName);
                var identificadorAplicacao = assemblyName.Name;

                const string NET50 = ".Net50";
                if (identificadorAplicacao.EndsWith(NET50))
                {
                    throw new Exception("Renomear o nome do assembly" + identificadorAplicacao);
                }

                return identificadorAplicacao;
            }
            throw new Erro("Não foi possível retornar o identificador da aplicação");
        }

        private static string RetornarIdentificadorProprietario()
        {
            if (Atual.AppSettings[ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO] != null)
            {
                return Atual.AppSettings[ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO];
            }
            return ConfiguracaoUtil.IDENTIFICADOR_PROPRIETARIO_GLOBAL;
        }

        private static CultureInfo RetornarCultauraPadrao()
        {
            return new CultureInfo(NOME_CULTURA_PADRAO);
        }

        private static DateTime RetornarDataHoraUtcServidor()
        {
            return DateTime.UtcNow;
        }

        private static IServicoLogErro RetornarServicoErro()
        {
            return new ServicoErroLocal();
        }

        private static IServicoLogSeguranca RetornarServicoLogSeguranca()
        {
            return new ServicoLogSegurancaLocal();
        }

        private static IServicoLogAplicacao RetornarServicoLogAplicacao()
        {
            return new ServicoLogAplicacaoLocal();
        }

        private static IServicoLogDesempenho RetornarServicoLogDesempenho()
        {
            return new ServicoLogDesempenhoLocal();
        }

        private static EnumAmbienteServidor RetornarAmbienteServidor()
        {
            return ConfiguracaoUtil.AmbienteServidor;
        }

        public InformacaoSessao RetornarInformacaoSessaoUsuario()
        {
            var tipoAplicacao = this.TipoAplicacao;
            var userAgent = this.AspNet?.UserAgent;
            var identificadorSessaoUsuario = this.IdentificadorSessaoUsuario;
            var identificadorAplicacao = this.IdentificadorAplicacao;
            //var ipInformacao = IpUtil.RetornarIpInformacao();
            //var identificadorProprietario = AplicacaoSnebur.Atual.IdentificadorProprietario;

            var sistemaOperacional = SistemaUtil.SistemaOperacional;
            var resolucao = SistemaUtil.Resolucao;
            var versaoAplicacao = this.VersaoAplicao.ToString();
            var nomeComptuador = Environment.MachineName;

            if (identificadorAplicacao == null)
            {
                throw new ArgumentNullException(nameof(identificadorAplicacao));
            }

            return new InformacaoSessao
            {
                //IdentificadorSessaoUsuario = identificadorSessaoUsuario,
                //IdentificadorProprietario = identificadorProprietario,
                IdentificadorAplicacao = identificadorAplicacao,
                TipoAplicacao = tipoAplicacao,
                //IPInformacao = ipInformacao,
                //IP = ipInformacao.IP,
                UserAgent = userAgent,
                Cultura = Thread.CurrentThread.CurrentCulture.Name,
                Idioma = CultureInfo.InstalledUICulture.Name,
                Navegador = new Navegador(),
                Plataforma = EnumPlataforma.PC,
                SistemaOperacional = sistemaOperacional,
                Resolucao = resolucao,
                VersaoAplicacao = versaoAplicacao,
                NomeComputador = nomeComptuador,
            };
        }

        #endregion

        protected virtual string RetornarUrlServico(string chaveConfiguracao)
        {
            if (!this.UrlsServico.ContainsKey(chaveConfiguracao))
            {
                lock ( this.UrlsServico.SyncLock())
                {
                    if (!this.UrlsServico.ContainsKey(chaveConfiguracao))
                    {
                        var urlServico = this.AppSettings[chaveConfiguracao];
                        if (!String.IsNullOrWhiteSpace(urlServico))
                        {
                            urlServico = AmbienteServidorUtil.NormalizarUrl(this.AppSettings[chaveConfiguracao]);
                        }
                        this.UrlsServico.Add(chaveConfiguracao, urlServico);
                    }
                }
            }
            return this.UrlsServico[chaveConfiguracao];
        }

        protected virtual string RetornarIpPublico()
        {
            if(this._ipPublico == null)
            {
                this._ipPublico = IpUtil.RetornarIpPublico();
            }
            return this._ipPublico;
            //throw new NotImplementedException();
            //return IpUtil.RetornarIPInformacaoRequisicao(isRetornarNullNaoEncotnrado).IP;
        }

        public virtual bool IsCredencialAvalistaPermirteContinuarSessaoInvalido(CredencialUsuario crendencial)
        {
            return false;
        }

        public virtual IUsuario RetornarUsuario(object contextoDados,
                                                   CredencialUsuario credencial)
        {
            return null;
        }

#if NET48

        protected virtual NameValueCollection RetornarAppSettings()
        {
            var appSettins = new NameValueCollection();
            foreach (var chave in ConfigurationManager.AppSettings.AllKeys)
            {
                appSettins.Add(chave, ConfigurationManager.AppSettings[chave]);
            }
            return appSettins;
        }

        protected virtual NameValueCollection RetornarConnectionStrings()
        {
            var connectionStrings = new NameValueCollection();
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                connectionStrings.Add(connectionString.Name, connectionString.ConnectionString);
            }
            return connectionStrings;
        }

       

#endif

    }
}