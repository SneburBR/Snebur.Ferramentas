using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    public static class AssemblyInfoUtil
    {
        private const string PROCURAR_LINHA_VERSAO = "[assembly: AssemblyVersion";
        private const string PROCURAR_LINHA_VERSAO_ARQUIVO = "[assembly: AssemblyFileVersion";

        public static Version RetornarVersaoProjeto(string caminhoProjeto)
        {
            var caminhoAssemblyInfo = RetornarCaminhoAssemblyInfo(caminhoProjeto);
            return RetornarVersaoAssemblyInfo(caminhoProjeto, caminhoAssemblyInfo);
        }

        public static Version RetornarVersaoAssemblyInfo(string caminhoProjeto,
                                                         string caminhoAssemblyInfo)
        {
            if (!File.Exists(caminhoAssemblyInfo))
            {
                throw new FileNotFoundException(caminhoAssemblyInfo);
            }

            var linhas = File.ReadAllLines(caminhoAssemblyInfo, Encoding.UTF8);
            var linhaVersao = linhas.Where(x => x.Trim().StartsWith(PROCURAR_LINHA_VERSAO)).FirstOrDefault();
            var linhaVersaoArquivo = linhas.Where(x => x.Trim().StartsWith(PROCURAR_LINHA_VERSAO_ARQUIVO)).FirstOrDefault();


            var versao = RetornarVersaoDataLinha(caminhoProjeto, linhaVersao);
            if (versao == null)
            {
                return null;
            }

            var versaoArquivo = RetornarVersaoDataLinha(caminhoProjeto, linhaVersaoArquivo);
            if (versaoArquivo == null)
            {
                return versao;
            }

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

        private static Version RetornarVersaoDataLinha(string caminhoProjeto, string linhaVersao)
        {
            var versaoString = ExpressaoUtil.RetornarExpressaoAbreFecha(linhaVersao, true);
            versaoString = versaoString.Replace("\"", String.Empty);
            if (versaoString == "Vsix.Version")
            {
                return RetornarVersaoVisx(caminhoProjeto);
            }

            if (Version.TryParse(versaoString, out var versao))
            {
                return new Version(versao.Major, versao.Minor, versao.Build, versao.Revision);
            }
            return null;
        }

        private static Version RetornarVersaoVisx(string caminhoProjeto)
        {
            var caminhoVisk = Path.Combine(caminhoProjeto, "source.extension.vsixmanifest");
            if (File.Exists(caminhoVisk))
            {
                using (var fs = StreamUtil.OpenRead(caminhoVisk))
                {
                    var xml = XDocument.Load(fs);

                    var metaElement = xml.Root.Elements().Select(x => x).Where(x => x.Name.LocalName == "Metadata").First();
                    var idenElement = metaElement.Elements().Where(x => x.Name.LocalName == "Identity").First();
                    var versionAttribute = idenElement.Attributes().Where(x => x.Name.LocalName == "Version").First();
                    var versaoString = versionAttribute.Value;
                    
                    if (Version.TryParse(versaoString, out var versao))
                    {
                        return versao;
                    }
                }
            }
            return null;
        }

        private static void IncrementarVersaoVisk(string caminhoVisk, Version novaVersao)
        {
            if (File.Exists(caminhoVisk))
            {
                XDocument xml;
                using (var fs = StreamUtil.OpenRead(caminhoVisk))
                {
                    xml = XDocument.Load(fs);
                    var metaElement = xml.Root.Elements().Select(x => x).Where(x => x.Name.LocalName == "Metadata").First();
                    var idenElement = metaElement.Elements().Where(x => x.Name.LocalName == "Identity").First();
                    var versionAttribute = idenElement.Attributes().Where(x => x.Name.LocalName == "Version").First();
                    var versaoString = versionAttribute.Value;

                    if (Version.TryParse(versaoString, out var versao))
                    {
                        versionAttribute.Value = novaVersao.ToString();
                    }
                }
                using(var fw = StreamUtil.OpenWrite(caminhoVisk))
                {
                    xml.Save(fw);
                }
            }
        }

        public static void InscrementarVersao(string caminhoProjeto,
                                              string caminhoAssemblyInfo)
        {
            var versao = RetornarVersaoAssemblyInfo(caminhoProjeto, caminhoAssemblyInfo);
            var agora = DateTime.Now;

            var ano = Int32.Parse(DateTime.Now.Year.ToString().Substring(2, 2));
            //var versaoData = Convert.ToInt32($"{DateTime.Now.Month:00}{DateTime.Now.Day:00}");
            var novaVersao = new Version(ano, agora.Month, agora.Day, versao.Revision + 1);

            var caminhoVisk = Path.Combine(caminhoProjeto, "source.extension.vsixmanifest");
            if (File.Exists(caminhoVisk))
            {
                IncrementarVersaoVisk(caminhoVisk, novaVersao);
                return;
            }
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

     

        public static string RetornarCaminhoAssemblyInfo(string caminhoProjeto)
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
