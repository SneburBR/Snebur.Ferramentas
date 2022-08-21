using System;
using System.IO;
using System.Linq;
using System.Text;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    internal static class AssemblyInfoUtil
    {
        private const string PROCURAR_LINHA_VERSAO = "[assembly: AssemblyVersion";
        private const string PROCURAR_LINHA_VERSAO_ARQUIVO = "[assembly: AssemblyFileVersion";

        public static Version RetornarVersaoProjeto(string caminhoProjeto)
        {
            var caminhoAssemblyInfo = RetornarCaminhoAssemblyInfo(caminhoProjeto);
            return RetornarVersaoAssemblyInfo(caminhoAssemblyInfo);
        }

        public static Version RetornarVersaoAssemblyInfo(string caminhoAssemblyInfo)
        {
            if (!File.Exists(caminhoAssemblyInfo))
            {
                throw new FileNotFoundException(caminhoAssemblyInfo);
            }

            var linhas = File.ReadAllLines(caminhoAssemblyInfo, Encoding.UTF8);
            var linhaVersao = linhas.Where(x => x.Trim().StartsWith(PROCURAR_LINHA_VERSAO)).FirstOrDefault();
            var linhaVersaoArquivo = linhas.Where(x => x.Trim().StartsWith(PROCURAR_LINHA_VERSAO_ARQUIVO)).FirstOrDefault();

            var versao = RetornarVersaoDataLinha(linhaVersao);
            var versaoArquivo = RetornarVersaoDataLinha(linhaVersaoArquivo);
            if (versao != versaoArquivo)
            {
                if (versao > versaoArquivo)
                {
                    return versao;
                }
                return versaoArquivo;
            }
            return versao;
        }

        private static Version RetornarVersaoDataLinha(string linhaVersao)
        {
            var versaoString = ExpressaoUtil.RetornarExpressaoAbreFecha(linhaVersao, true);
            versaoString = versaoString.Replace("\"", String.Empty);
            var versao = new Version(versaoString);
            var novaVersao = new Version(versao.Major, versao.Minor, versao.Build, versao.Revision);
            return novaVersao;
        }

        public static void InscrementarVersao(string caminhoAssemblyInfo)
        {
            var versao = RetornarVersaoAssemblyInfo(caminhoAssemblyInfo);
            var versaoMinor = Int32.Parse(DateTime.Now.Year.ToString().Substring(2, 2));
            var versaoData = Convert.ToInt32($"{DateTime.Now.Month:00}{DateTime.Now.Day:00}");
            var novaVersao = new Version(versao.Major, versaoMinor, versaoData, versao.Revision + 1);

            if (File.Exists(caminhoAssemblyInfo))
            {
                var linhas = File.ReadAllLines(caminhoAssemblyInfo, Encoding.UTF8);
                for (var i = 0; i < linhas.Count(); i++)
                {
                    var linha = linhas[i];
                    if (linha.Trim().StartsWith(PROCURAR_LINHA_VERSAO))
                    {

                        var novaLinha = $"{PROCURAR_LINHA_VERSAO}(\"{novaVersao.Major}.{novaVersao.Minor}.{novaVersao.Build}.{novaVersao.Revision}\")]";
                        linhas[i] = novaLinha;
                    }
                    if (linha.Trim().StartsWith(PROCURAR_LINHA_VERSAO_ARQUIVO))
                    {
                        linhas[i] = $"{PROCURAR_LINHA_VERSAO_ARQUIVO}(\"{novaVersao.Major}.{novaVersao.Minor}.{novaVersao.Build}.{novaVersao.Revision}\")]";
                    }
                }
                File.WriteAllLines(caminhoAssemblyInfo, linhas, Encoding.UTF8);
            }
        }

        internal static string RetornarCaminhoAssemblyInfo(string caminhoProjeto)
        {
            var caminhoAssembly = Path.Combine(caminhoProjeto, "Properties/AssemblyInfo.cs");
            if (File.Exists(caminhoAssembly))
            {
                return caminhoAssembly;

            }
            caminhoAssembly = Path.Combine(caminhoProjeto, "AssemblyInfo.cs");
            if (File.Exists(caminhoAssembly))
            {
                return caminhoAssembly;

            }
            return null;
            //throw new FileNotFoundException($"Não foi encontrado o arquivo AssemblyInfo.cs\r\n {caminhoAssembly}");
        }
    }

  
}
