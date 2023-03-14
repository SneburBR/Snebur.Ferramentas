using System.Collections.Generic;
using System.IO;
using System.Linq;
using Snebur.Publicacao;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    public  static partial class ProjetoTypeScriptUtil
    {
        public static readonly object BloqueioManipuladorArquivos = new object();
        public static void AtualizarScriptsDebug(IEnumerable<ProjetoTypeScript> projetosTypescript)
        {
            foreach (var projeto in projetosTypescript)
            {
                ProjetoTypeScriptUtil.AtualizarScriptsDebug(projetosTypescript,projeto);
            }
        }

        public static void AtualizarScriptsDebug(IEnumerable<ProjetoTypeScript> projetosTypescript, ProjetoTypeScript projeto)
        {
            lock (ProjetoTypeScriptUtil.BloqueioManipuladorArquivos)
            {
                if (projeto.ConfiguracaoProjeto.IsDebugScriptsDepedentes)
                {
                    foreach (var (nomeDepedencia, caminhoRelativo) in projeto.Dependencias.Select(x => (x.Key, x.Value)))
                    {
                        var projetoDepedencia = projetosTypescript.Where(x => x.NomeProjeto == nomeDepedencia).SingleOrDefault();
                        if (projetoDepedencia != null)
                        {
                            var caminhoScriptDebug = Path.Combine(projeto.CaminhoProjeto,
                                                                  ConstantesPublicacao.NOME_PASTA_BUILD,
                                                                  projetoDepedencia.ArquivoScriptCompilado.Name);
                            if (projetoDepedencia != null)
                            {
                                var caminhoScript = projetoDepedencia.CaminhoSaidaPadrao;
                                ProjetoTypeScriptUtil.AtualizarScriptDebug(caminhoScript, caminhoScriptDebug);
                            }
                        }
                    }
                }
            }
            
        }

     

        public static void AtualizarScriptDebug(string caminhoScript,
                                                string caminhoScriptDebug)
        {
            if (File.Exists(caminhoScriptDebug) && File.Exists(caminhoScript))
            {
                var dataHoraScriptNormalizado = ProjetoTypeScriptUtil.RetornarDataHoraScriptNormalizado(caminhoScript);
                var dataHoraScriptNormalizadoDebug = ProjetoTypeScriptUtil.RetornarDataHoraScriptNormalizado(caminhoScriptDebug);

                if (dataHoraScriptNormalizado != null &&
                    dataHoraScriptNormalizado > NormalizarCompilacaoJavascript.DATA_NORMALIZACAO_IMPLEMENTADA)
                {
                    if (dataHoraScriptNormalizadoDebug != null && 
                        (dataHoraScriptNormalizado > dataHoraScriptNormalizadoDebug))
                    {
                        ProjetoTypeScriptUtil.CopiarScripts(caminhoScript, caminhoScriptDebug);
                    }
                }
            }

            if (!File.Exists(caminhoScriptDebug) && File.Exists(caminhoScript))
            {
                ProjetoTypeScriptUtil.CopiarScripts(caminhoScript, caminhoScriptDebug);
            }
        }

        public static void CopiarScripts(string caminhoScriptOrigem, string caminhoScriptDestino)
        {
            var caminhoScriptMap = Path.ChangeExtension(caminhoScriptOrigem, ".js.map");
            var caminhoDeclaracao = Path.ChangeExtension(caminhoScriptOrigem, ".d.ts");
            var caminhoDeclaracaoMap = Path.ChangeExtension(caminhoScriptOrigem, ".d.ts.map");

            var caminhoScriptMapDestino = Path.ChangeExtension(caminhoScriptDestino, ".js.map");
            var caminhoDeclaracaoDestino = Path.ChangeExtension(caminhoScriptDestino, ".d.ts");
            var caminhoDeclaracaoMapDestino = Path.ChangeExtension(caminhoScriptDestino, ".d.ts.map");

            ArquivoUtil.CopiarArquivo(caminhoScriptOrigem, caminhoScriptDestino, true, true, true);
            ArquivoUtil.CopiarArquivo(caminhoScriptMap, caminhoScriptMapDestino, true, true, true);
            ArquivoUtil.CopiarArquivo(caminhoDeclaracao, caminhoDeclaracaoDestino, true, true, true);
            ArquivoUtil.CopiarArquivo(caminhoDeclaracaoMap, caminhoDeclaracaoMapDestino, true, true);
        }

        public static string NamespaceRaiz(string nomeProjeto)
        {
            var isSnebur = nomeProjeto.Contains("Snebur") ||
                             nomeProjeto.Contains("Sigi");

            //return (isSnebur) ? "Snebur" :
            //                      "Snebur";

            return "Snebur";

        }
        public static string NamespaceReflexao(string nomeProjeto)
        {
            var namaspaceRaiz = NamespaceRaiz(nomeProjeto);
            return $"{namaspaceRaiz}.Reflexao";


        }
    }
}
