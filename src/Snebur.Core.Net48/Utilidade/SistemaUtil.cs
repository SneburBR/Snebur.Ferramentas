using Snebur.Dominio;
using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Snebur.Utilidade
{
    public static class SistemaUtil
    {
        private const string NOME_QUALIFICADO_TIPO_TESTCLASSATTRIBUTE = "Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute, Microsoft.VisualStudio.TestPlatform.TestFramework";

        private static Version _versaoAplicacao;
        private static string _versaoAplicacaoString;
        private static Dimensao _resolucao;
        private static SistemaOperacional _sistemaOperacional;

        public static string CaminhoAplicacao
        {
            get
            {
                return System.Reflection.Assembly.GetEntryAssembly().Location;
            }
        }

        //public static EnumTipoAplicacao TipoAplicacao
        //{
        //    get
        //    {
        //        return LazyUtil.RetornarValorLazyComBloqueio(ref _tipoAplicacao,
        //                                                SistemaUtil.RetornarTipoAplicacaoInterno);

        //    }
        //}

        public static SistemaOperacional SistemaOperacional
        {
            get
            {
                return LazyUtil.RetornarValorLazyComBloqueio(ref _sistemaOperacional,
                                                            RetornarSistemaOperacional);

            }
        }

        public static Dimensao Resolucao
        {
            get
            {
                return LazyUtil.RetornarValorLazyComBloqueio(ref _resolucao,
                                                            RetornarResolucao);

            }
        }

        public static bool IsWindowsXp
        {
            get
            {
                if (DebugUtil.IsAttached)
                {
                    return false;
                }
                var os = Environment.OSVersion;
                return (os.Platform == PlatformID.Win32NT) && os.Version.Major == 5;
            }
        }

        public static string VersaoAplicacaoString => LazyUtil.RetornarValorLazyComBloqueio(ref _versaoAplicacaoString,
                                                                                      () => VersaoAplicacao.ToString());
        public static Version VersaoAplicacao => LazyUtil.RetornarValorLazyComBloqueio(ref _versaoAplicacao,
                                                                                 ReflexaoUtil.RetornarVersaoEntrada);
        public static bool IsAplicacao64Bits => Environment.Is64BitProcess;

        public static bool IsAdministrator
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
                }
                return false;
            }
        }

        //private static EnumTipoAplicacao RetornarTipoAplicacaoInterno()
        //{

        //    if (System.Reflection.Assembly.GetEntryAssembly() == null)
        //    {
        //        throw new Exception("O tip da aplicação deve ser implementado na AplicacaoSnebur");
        //        //if (SistemaUtil.IsAplicacaoUnidadeTeste())
        //        //{
        //        //    return EnumTipoAplicacao.DotNet_UnitTest;
        //        //}
        //        //if (AplicacaoSnebur._aplicacao?.HttpContext != null)
        //        //{
        //        //    return EnumTipoAplicacao.DotNet_WebService;
        //        //}
        //    }

        //    if (IsAplicacaoWindowsService())
        //    {
        //        //return EnumTipoAplicacao.DotNet_WindowService;
        //        return EnumTipoAplicacao.DotNet_WebService;
        //    }
        //    return EnumTipoAplicacao.DotNet_Wpf;
        //    //return EnumTipoAplicacao.DotNet_WebService;
        //}

        //private static bool IsAplicacaoUnidadeTeste()
        //{
        //    try
        //    {
        //        var tipoTestClass = Type.GetType(SistemaUtil.NOME_QUALIFICADO_TIPO_TESTCLASSATTRIBUTE);
        //        return tipoTestClass != null;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //private static bool IsAplicacaoWindowsService()
        //{
        //    return new WindowsServiceDetectar().IsAplicacaoWindowsService();
        //}

        public static string RetornarCodinomeSistemaOperacional()
        {
            var sistemaOperacional = Environment.OSVersion;
            var versao = sistemaOperacional.Version;

            if (sistemaOperacional.Platform == PlatformID.Win32Windows)
            {
                throw new Exception("Sistema operacional não suportado");
            }
            if (sistemaOperacional.Platform == PlatformID.Win32NT)
            {
                switch (versao.Major)
                {
                    case 5:

                        switch (versao.Minor)
                        {
                            case 0:

                                return "2000";

                            case 1:

                                return "XP";

                            case 2:

                                return "20003";

                            default:

                                return "2000/XP/2003";
                        }
                    case 6:

                        switch (versao.Minor)
                        {
                            case 0:

                                return "Vista/2008";

                            case 1:

                                return "7/2008 R2";

                            case 2:

                                return "8";

                            case 3:

                                return "8.1";

                            default:

                                return "Vista/7/8";
                        }
                    case 10:

                        return "10";

                    default:

                        throw new ErroNaoSuportado("Major  não suportado " + versao.Major);
                }
            }
            throw new ErroNaoSuportado("Plataforma não suportado  " + sistemaOperacional.Platform.ToString());
        }

        public static bool IsPossuiPermisaoAdministrador()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return false;
            }

            using (var identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private static SistemaOperacional RetornarSistemaOperacional()
        {
            return new SistemaOperacional(EnumSistemaOperacional.Windows, "Windows",
                                          Environment.OSVersion.Version.ToString(),
                                          RetornarCodinomeSistemaOperacional());
        }

        private static Dimensao RetornarResolucao()
        {

            if (System.Reflection.Assembly.GetEntryAssembly() != null)
            {
                var nomeTipoSystemParameters = "System.Windows.SystemParameters, PresentationFramework";
                var tipoSystemaParameters = Type.GetType(nomeTipoSystemParameters);
                if (tipoSystemaParameters != null)
                {
                    var propreidadeLargura = tipoSystemaParameters.GetProperty("PrimaryScreenWidth");
                    var propreidadeAltura = tipoSystemaParameters.GetProperty("PrimaryScreenHeight");

                    var largura = ConverterUtil.Converter<int>(propreidadeLargura.GetValue(null));
                    var altura = ConverterUtil.Converter<int>(propreidadeAltura.GetValue(null));

                    _resolucao = new Dimensao((int)largura, (int)altura);
                }
            }
            return new Dimensao(0, 0);
        }

        private class WindowsServiceDetectar
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr GetStdHandle(int nStdHandle);
            private const int STD_OUTPUT_HANDLE = -11;
            private readonly IntPtr iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);

            public bool IsAplicacaoWindowsService()
            {
                return (this.iStdOut == IntPtr.Zero);
            }
        }
    }
}