using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Snebur.VisualStudio
{
    public static class ProjetoUtil
    {
        public static string[] RetornarTodosCaminhosProjetosReferenciados(string caminhoProjeto)
        {
            if (!IsProjetoCsCharp(caminhoProjeto))
            {
                throw new ArgumentException($"O caminho informado não é de um projeto C# {caminhoProjeto}");
            }
            var caminhosReferencias = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var resultados = new List<string>();

            RetornarCaminhosRecursivamente(caminhoProjeto, caminhosReferencias, resultados);

            return resultados.ToArray();

        }

        private static void RetornarCaminhosRecursivamente(string caminhoProjeto,
                                                          HashSet<string> caminhosReferencias,
                                                          List<string> resultados)
        {
            if (caminhosReferencias.Contains(caminhoProjeto))
            {
                return; // Evita loops circulares
            }

            caminhosReferencias.Add(caminhoProjeto);
            resultados.Add(caminhoProjeto);

            var referencias = RetornarReferenciasDoProjeto(caminhoProjeto);

            foreach (var referencia in referencias)
            {
                RetornarCaminhosRecursivamente(referencia, caminhosReferencias, resultados);
            }
        }

        public static List<string> RetornarReferenciasExternas(string caminhoProjeto) { 

            XDocument projeto = XDocument.Load(caminhoProjeto);

            // Busca todas as referências que possuem HintPath
            var referencias = projeto.Descendants("Reference")
                .Where(r => r.Element("HintPath") != null)
                .Select(r => new
                {
                    Nome = r.Attribute("Include")?.Value,
                    HintPath = r.Element("HintPath")?.Value
                })
               .Select(x => x.HintPath)
               .Where(x => !String.IsNullOrWhiteSpace(x));

            var arquivosLink = projeto.Descendants("Compile")
                             .Where(c => c.Attribute("Include") != null)
                             .Select(c => (
                                 Tipo: "Compile",
                                 IncludePath: c.Attribute("Include")?.Value,
                                 Link: c.Attribute("Link")?.Value
                             ))
                             .Select(x => x.IncludePath)
                             .Where(x => !String.IsNullOrWhiteSpace(x));

            return referencias.Concat(arquivosLink)
                              .ToList();

        }

        private static List<string> RetornarReferenciasDoProjeto(string caminhoProjeto)
        {
            if (!File.Exists(caminhoProjeto))
            {
                LogVSUtil.LogErro($" Não foi encontrado caminho do projeto {caminhoProjeto}");
                return default;
            }

            var diretorioBase = Path.GetDirectoryName(caminhoProjeto);
            var referencias = new List<string>();

            var documento = XDocument.Load(caminhoProjeto);

            // Define the namespace
            XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";

            var elementosReferencia = documento.Descendants("ProjectReference");
            var elementosReferenciaNS = documento.Descendants(ns + "ProjectReference");

            var todasReferencias = elementosReferencia.Concat(elementosReferenciaNS);

            foreach (var elemento in todasReferencias)
            {
                var atributoInclude = elemento.Attribute("Include");
                if (atributoInclude != null)
                {
                    var caminhoRelativo = atributoInclude.Value;
                    var caminhoAbsoluto = Path.GetFullPath(Path.Combine(diretorioBase, caminhoRelativo));

                    if (IsProjetoCsCharp(caminhoAbsoluto))
                    {
                        referencias.Add(caminhoAbsoluto);
                    }
                }
            }
            return referencias;
        }

        public static bool IsProjetoTypescript(string caminhoProjeto)
        {
            if (!String.IsNullOrEmpty(caminhoProjeto))
            {
                var caminhoTSConfig = Path.Combine(caminhoProjeto, "tsconfig.json");
                return File.Exists(caminhoTSConfig);
            }
            return false;
        }

        public static bool IsProjetoCsCharp(string caminhoProjeto)
        {
            return caminhoProjeto.EndsWith(".csproj");
        }

        public static void IncrementarVersao(string caminhoProjeto,
                                             bool isIncrementarTodasReferencias)
        {
            if (!Debugger.IsAttached && isIncrementarTodasReferencias)
            {
                var caminhosReferencias = RetornarTodosCaminhosProjetosReferenciados(caminhoProjeto);
                foreach (var caminhoReferencia in caminhosReferencias)
                {
                    AssemblyInfoUtil.InscrementarVersao(caminhoReferencia);
                }
            }
            else
            {
                AssemblyInfoUtil.InscrementarVersao(caminhoProjeto);
            }
        }

        public static EnumTipoCsProj RetornarTipoCsProjet(string caminhoProjetoCsProj)
        {
            using (var fs = StreamUtil.OpenRead(caminhoProjetoCsProj))
            {
                var xml = new XmlDocument();
                xml.Load(fs);

                var atributoSdk = xml.GetElementsByTagName("Project")[0].Attributes["Sdk"];
                if (atributoSdk != null && atributoSdk.Value == "Microsoft.NET.Sdk")
                {
                    return EnumTipoCsProj.MicrosoftSdk;
                }
                return EnumTipoCsProj.Tradicional;
            }
        }

 
    }
} 
 