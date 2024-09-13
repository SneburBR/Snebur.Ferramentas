//futuramente vamos normalizar os Error, passando um parametro da mapeamento do arquivo e linha
//detalhes aqui https://www.html5rocks.com/en/tutorials/developertools/sourcemaps/

using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Snebur.VisualStudio
{
    public static class ArquivoControleUtil
    {
        public static FileInfo RetornarArquivoShtml(FileInfo arquivo)
        {
            var caminhoShtml = RetornarCaminhoShtml(arquivo);
            return new FileInfo(caminhoShtml);
        }

        public static int RetornarOrdenacao(FileInfo arquivo)
        {
            var name = arquivo.Name;
            if (name.EndsWith(ConstantesProjeto.EXTENSAO_CONTROLE_SHTML, StringComparison.InvariantCultureIgnoreCase))
            {
                return 0;
            }
            if (name.EndsWith(ConstantesProjeto.EXTENSAO_CONTROLE_SHTML_SCSS, StringComparison.InvariantCultureIgnoreCase))
            {
                return 1;
            }
            if (name.EndsWith(ConstantesProjeto.EXTENSAO_CONTROLE_SHTML_TYPESCRIPT, StringComparison.InvariantCultureIgnoreCase))
            {
                return 2;
            }
            return 3;
        }

        public static string RetornarCaminhoShtml(string caminhoArquivo)
        {
            return RetornarCaminhoShtml(new FileInfo(caminhoArquivo));
        }

        public static string RetornarCaminhoShtml(FileInfo arquivo)
        {
            if (!IsArquivoControle(arquivo))
            {
                throw new Exception("Arquivo não suportado " + arquivo.FullName);
            }

            if (arquivo.Extension.Equals(ConstantesProjeto.EXTENSAO_CONTROLE_SHTML, StringComparison.InvariantCultureIgnoreCase))
            {
                return arquivo.FullName;
            }
            return Path.Combine(arquivo.Directory.FullName, Path.GetFileNameWithoutExtension(arquivo.FullName));
        }

        public static IEnumerable<FileInfo> RetornarArquivosControle(IEnumerable<FileInfo> arquivos)
        {
            return arquivos.Where(fi => ArquivoControleUtil.IsArquivoControle(fi));
        }

        public static bool IsArquivoControle(FileInfo arquivo)
        {
            return ConstantesProjeto.ExtensoesControlesSnebur.Any(e => arquivo.Name.EndsWith(e, StringComparison.CurrentCultureIgnoreCase));
        }
    }

    public static class TypescriptDefinicaoUtil
    {
        internal static IEnumerable<string> RetornarCaminhosClasses(string caminhoDefinicacao)
        {
            var content = File.ReadAllText(caminhoDefinicacao);
            return ExtractNamespacesAndClasses(content);
        }

        static IEnumerable<string> ExtractNamespacesAndClasses(string content)
        {
            var namespacePattern = @"declare\s+namespace\s+([\w\.]+)\s*\{";
            var classPattern = @"\b(?:class|abstract\s+class|enum)\s+([\w]+)\s";

            var namespaceMatches = Regex.Matches(content, namespacePattern);
            var classesPerNamespace = new Dictionary<string, List<string>>();

            foreach (Match namespaceMatch in namespaceMatches)
            {
                var namespaceName = namespaceMatch.Groups[1].Value;
                var namespaceStartIndex = namespaceMatch.Index + namespaceMatch.Length;
                var subContext = content.Substring(namespaceMatch.Index);
                var namespaceContent = ExpressaoUtil.RetornarExpressaoAbreFecha(subContext, false, '{', '}');

                var namespaceEndIndex = content.IndexOf("}", namespaceStartIndex);
 
                //string namespaceContent = content.Substring(namespaceStartIndex, namespaceEndIndex - namespaceStartIndex);
                var classMatches = Regex.Matches(namespaceContent, classPattern);

                foreach (Match classMatch in classMatches)
                {
                    string className = classMatch.Groups[1].Value;

                    if (!classesPerNamespace.ContainsKey(namespaceName))
                    {
                        classesPerNamespace[namespaceName] = new List<string>();
                    }

                    classesPerNamespace[namespaceName].Add(className);
                }
            }

            var caminhosClasses = new HashSet<string>();
            foreach (var dominio in classesPerNamespace.Keys)
            {
                var classes = classesPerNamespace[dominio];
                foreach(var classe in classes)
                {
                    var caminho = $"{dominio}.{classe}";
                    caminhosClasses.Add(caminho);
                }
            }
            return caminhosClasses;
        }
    }
}