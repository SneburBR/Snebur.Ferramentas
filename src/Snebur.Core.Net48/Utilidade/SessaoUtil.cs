using Snebur.Seguranca;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;

namespace Snebur.Utilidade
{
    public static class SessaoUtil
    {
        //public const string IDENTIFICADOR_SESSAO_USUARIO = "IdentificadorSessaoUsuario";
        public const string IDENTIFICADOR_APLICACAO = "IdentificadorAplicacao";
        //public const string IDENTIFICADOR_PROPRIETARIO = "IdentificadorProprietario";
        private const string CHAVE_CRIPTOGRAFIA = "248c6619-8119-45bd-ae2a-662512aff841";

        
        private static CredencialUsuario _credencialUsuario;

        private static object _bloqueioIdentificadorSessaoUsuario = new object();
        private static object _bloqueioAcessoArquivoAppData = new object();

        #region Informação da Sessão usuário

        //public static InformacaoSessaoUsuario RetornarInformacaoSessaoUsuarioAtual()
        //{
        //    return AplicacaoSnebur.Atual.InformacaoSessaoUsuarioAtual;
        //}

        //public static InformacaoSessaoUsuario RetornarInformacaoSessaoUsuarioAplicacao()
        //{
        //    var tipoAplicacao = AplicacaoSnebur.Atual.TipoAplicacao;
        //    //var ipInformacao = IpUtil.RetornarIpInformacao();
        //    var userAgent = SessaoUtil.RetornarUserAgent();
        //    var identificadorSessaoUsuario = AplicacaoSnebur.Atual.IdentificadorSessaoUsuario;
        //    //var identificadorProprietario = AplicacaoSnebur.Atual.IdentificadorProprietario;
        //    var identificadorAplicacao = AplicacaoSnebur.Atual.IdentificadorAplicacao;
        //    var sistemaOperacional = SessaoUtil.RetornarSistemaOperacional();
        //    var resolucao = SessaoUtil.RetornarResolucao();
        //    var versaoAplicacao = AplicacaoSnebur.Atual.VersaoAplicao.ToString();
        //    var nomeComptuador = Environment.MachineName;

        //    if (identificadorAplicacao == null)
        //    {
        //        throw new ArgumentNullException(nameof(identificadorAplicacao));
        //    }

        //    return new InformacaoSessaoUsuario
        //    {
        //        IdentificadorSessaoUsuario = identificadorSessaoUsuario,
        //        //IdentificadorProprietario = identificadorProprietario,
        //        IdentificadorAplicacao = identificadorAplicacao,
        //        TipoAplicacao = tipoAplicacao,
        //        //IPInformacao = ipInformacao,
        //        //IP = ipInformacao.IP,
        //        UserAgent = userAgent,
        //        Cultura = Thread.CurrentThread.CurrentCulture.Name,
        //        Idioma = CultureInfo.InstalledUICulture.Name,
        //        Navegador = new Navegador(),
        //        Plataforma = EnumPlataforma.PC,
        //        SistemaOperacional = sistemaOperacional,
        //        Resolucao = resolucao,
        //        VersaoAplicacao = versaoAplicacao,
        //        NomeComputador = nomeComptuador,
        //    };
        //}

        //private static string RetornarUserAgent()
        //{
        //    if (AplicacaoSnebur.Atual.IsAplicacaoAspNet)
        //    {
        //        return AplicacaoSnebur.Atual.AspNet.UserAgent;
        //    }
        //    return null;
        //}

        //public static DadosIPInformacao RetornarIpInformacao()
        //{
        //    if (_dadosIpInformacao == null)
        //    {
        //        throw new NotImplementedException();
        //        //_dadosIpInformacao = IpUtil.RetornarIPInformacao();
        //    }
        //    return _dadosIpInformacao;
        //}

      
        #endregion

        #region Identificador Sessão usuario

        private static ConcurrentDictionary<string, Guid> IdentificadoresSessaoUsuario { get; set; } = new ConcurrentDictionary<string, Guid>();

        public static Guid RetornarIdentificadorSessaoUsuario()
        {
            var identificadorUsuario = AplicacaoSnebur.Atual.CredencialUsuario.IdentificadorUsuario;
            if (IdentificadoresSessaoUsuario.ContainsKey(identificadorUsuario))
            {
                return IdentificadoresSessaoUsuario[identificadorUsuario];
            }

            lock (_bloqueioIdentificadorSessaoUsuario)
            {
                if (!IdentificadoresSessaoUsuario.ContainsKey(identificadorUsuario))
                {
                    var caminhoArquivo = RetornarCaminhoArquivoIdentificadorSessaoUsuario();
                    var identificadorSessaoUsuario = Guid.Empty;
                    if (File.Exists(caminhoArquivo))
                    {
                        identificadorSessaoUsuario = RetornarConteudoAppData<Guid>(caminhoArquivo);
                    }
                    if (identificadorSessaoUsuario == Guid.Empty)
                    {
                        identificadorSessaoUsuario = Guid.NewGuid();
                        SalvarIdentificadorSessaoUsuario(identificadorSessaoUsuario);
                    }
                    IdentificadoresSessaoUsuario.TryAdd(identificadorUsuario, identificadorSessaoUsuario);
                }
            }
            return RetornarIdentificadorSessaoUsuario();
        }

        public static void SalvarIdentificadorSessaoUsuario( Guid identificadorSessaoUsuario)
        {
            var identificadorUsuario = AplicacaoSnebur.Atual.CredencialUsuario.IdentificadorUsuario;
            if (IdentificadoresSessaoUsuario.ContainsKey(identificadorUsuario))
            {
                IdentificadoresSessaoUsuario.TryRemove(identificadorUsuario, out _);
            }
            var caminhoArquivo = RetornarCaminhoArquivoIdentificadorSessaoUsuario();
            SalvarConteudoAppData(identificadorSessaoUsuario, caminhoArquivo);
        }

        private static string RetornarCaminhoArquivoIdentificadorSessaoUsuario()
        {
            var repositorio = ConfiguracaoUtil.CaminhoAppDataAplicacaoSemVersao;
            var nomeArquivo = RetornarNomeArquivoIdentificadorSessaoUsuario();
            return CaminhoUtil.Combine(repositorio, $"{nomeArquivo}-sid.dat");
        }

        private static string RetornarNomeArquivoIdentificadorSessaoUsuario()
        {
            var credencialUsuario = AplicacaoSnebur.Atual.CredencialUsuario;
            //if (DebugUtil.IsAttached)
            //{
            return TextoUtil.RetornarSomentesLetrasNumeros(credencialUsuario.IdentificadorUsuario).ToLower();
            //}
            //return Md5Util.RetornarHash(credencialUsuario.IdentificadorUsuario);
        }

        private static string RetornarCaminhoArquivoCredencialUsuario()
        {
            var repositorio = ConfiguracaoUtil.CaminhoAppDataAplicacaoSemVersao;
            return CaminhoUtil.Combine(repositorio, "user.dat");
        }

        public static void InicializarNovaSessaoUsuario()
        {
            LimparCredencialUsuario();
            LimparIdentificadoresSessaoUsuario();
            SalvarIdentificadorSessaoUsuario(Guid.NewGuid());
        }

        public static void LimparIdentificadoresSessaoUsuario()
        {
            lock (_bloqueioAcessoArquivoAppData)
            {
                var repositorio = ConfiguracaoUtil.CaminhoAppDataAplicacaoSemVersao;
                var arquivos = Directory.GetFiles(repositorio, "*.dat").Where(x => Path.GetFileNameWithoutExtension(x).EndsWith("-sid")).ToList();
                foreach (var arquivo in arquivos)
                {
                    ArquivoUtil.DeletarArquivo(arquivo, false, true);
                }
                IdentificadoresSessaoUsuario.Clear();
            }
        }
        #endregion

        #region Credencial Usuario

        public static CredencialUsuario RetornarCredencialUsuario()
        {
            if (_credencialUsuario == null)
            {
                lock (_bloqueioAcessoArquivoAppData)
                {
                    if (_credencialUsuario == null)
                    {
                        _credencialUsuario = RetornarCredencialUsuarioInterno();
                    }
                }
            }
            return _credencialUsuario;
        }

        public static void LimparCredencialUsuario()
        {
            lock (_bloqueioAcessoArquivoAppData)
            {
                var caminhoArquivo = RetornarCaminhoArquivoCredencialUsuario();
                ArquivoUtil.DeletarArquivo(caminhoArquivo);
                _credencialUsuario = null;
            }
        }

        private static CredencialUsuario RetornarCredencialUsuarioInterno()
        {
            lock (_bloqueioAcessoArquivoAppData)
            {
                var caminhoArquivo = RetornarCaminhoArquivoCredencialUsuario();
                if (File.Exists(caminhoArquivo))
                {
                    try
                    {
                        var credencia = RetornarConteudoAppData<CredencialUsuario>(caminhoArquivo);
                        if (credencia != null)
                        {
                            return credencia;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtil.ErroAsync(ex);
                        ArquivoUtil.DeletarArquivo(caminhoArquivo);
                        InicializarNovaSessaoUsuario();
                    }
                }
                return CredencialAnonimo.Anonimo;
            }
        }

        public static void SalvarCredencialUsuario(CredencialUsuario credencial)
        {
            lock (_bloqueioAcessoArquivoAppData)
            {
                LimparCredencialUsuario();
                var caminhoArquivo = RetornarCaminhoArquivoCredencialUsuario();
                SalvarConteudoAppData(credencial, caminhoArquivo);
                AplicacaoSnebur.Atual.NotificarCredencialAlterada();
                _credencialUsuario = null;
            }
        }
        #endregion

        #region Conteudo do AppData

        private static bool IsCriptografarConteudoAppData { get; } = true;

        private static void SalvarConteudoAppData(object obj, string caminhoArquivo)
        {
            lock (_bloqueioAcessoArquivoAppData)
            {
                var conteudo = RetornarConteudoSalvar(obj);
                ArquivoUtil.DeletarArquivo(caminhoArquivo, false, true);
                File.WriteAllText(caminhoArquivo, conteudo, Encoding.UTF8);
            }
        }

        private static string RetornarConteudoSalvar(object obj)
        {
            var conteudo = JsonUtil.Serializar(obj, EnumTipoSerializacao.Javascript);
            if (IsCriptografarConteudoAppData)
            {
                return CriptografiaUtil.Criptografar(CHAVE_CRIPTOGRAFIA, conteudo);
            }
            return conteudo;
        }

        private static T RetornarConteudoAppData<T>(string caminhoArquivo)
        {
            lock (_bloqueioAcessoArquivoAppData)
            {
                if (File.Exists(caminhoArquivo))
                {
                    try
                    {
                        var conteudo = RetornarConteudoLeitura(caminhoArquivo);
                        return JsonUtil.Deserializar<T>(conteudo, EnumTipoSerializacao.Javascript);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.ErroAsync(ex);
                        ArquivoUtil.DeletarArquivo(caminhoArquivo);

                    }
                }
                return default;
            }
        }

        private static string RetornarConteudoLeitura(string caminhoArquivo)
        {
            if (IsCriptografarConteudoAppData)
            {
                var conteudo = File.ReadAllText(caminhoArquivo, Encoding.UTF8);
                return CriptografiaUtil.Descriptografar(CHAVE_CRIPTOGRAFIA, conteudo);
            }
            return File.ReadAllText(caminhoArquivo, Encoding.UTF8);
        }
        #endregion
    }
}