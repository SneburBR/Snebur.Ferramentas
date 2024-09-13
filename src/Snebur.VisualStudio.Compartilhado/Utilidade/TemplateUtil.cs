using Snebur.Utilidade;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Snebur.VisualStudio
{
    public static class TemplateUtil
    {
        public static void InserirTemplateArquivoNovo(ProjetoTypeScript projetoTS,
                                                      FileInfo arquivo)
        {
            if (arquivo.Exists)
            {
                var conteudo = ArquivoUtil.LerTexto(arquivo.FullName);
                if (String.IsNullOrWhiteSpace(conteudo))
                {
                    var extesionAndSubExtension = arquivo.GetPenultimateExtension();
                    switch (arquivo.Extension)
                    {
                        case ConstantesProjeto.EXTENSAO_CONTROLE_SHTML:
                            InsertTemplateShtml(projetoTS, arquivo);
                            break;
                        case ConstantesProjeto.EXTENSAO_CONTROLE_SHTML_SCSS:
                            InsertemplateScss(projetoTS, arquivo);
                            break;
                        case ConstantesProjeto.EXTENSAO_CONTROLE_SHTML_TYPESCRIPT:
                            InsertTemplateShtmlControle(projetoTS, arquivo);
                            break;
                        case ConstantesProjeto.EXTENSAO_TYPESCRIPT:
                            InsertTemplateTS(projetoTS, arquivo);
                            break;
                    }
                }
            }
        }

        private static void InsertTemplateShtml(ProjetoTypeScript projetoTS,
                                               FileInfo arquivo)
        {
            var nomeClass = TextoUtil.RemoverEspacos(arquivo.Name.Split('.').First());
            var nomeClassScss = CodigoUtil.Formatar(nomeClass, EnumFormatacaoCodigo.PascalCase, EnumFormatacaoCodigo.KebabCase);

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <title></title>");
            sb.AppendLine("\t<meta charset=\"utf-8\" />");
            sb.AppendLine("</head>");
            sb.AppendLine($"<body class=\"sn-{nomeClassScss}\">");
            sb.AppendLine("");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            LogVSUtil.Log($"Inserindo template shtml: {arquivo.Name}");

            LocalProjetoUtil.SalvarDominio(arquivo.FullName, sb.ToString());

            var caminhoShmlControle = arquivo.FullName  + ".ts";
            if(!File.Exists(caminhoShmlControle))
            {
                InsertTemplateShtmlControle(projetoTS, new FileInfo(caminhoShmlControle));
            }
        }

        private static void InsertTemplateShtmlControle(ProjetoTypeScript projetoTS,
                                                        FileInfo arquivo)
        {
            var nomeClass = TextoUtil.RemoverEspacos(arquivo.Name.Split('.').First());
            var nomeClassEntend = GetNomeClassExtend(arquivo.Name);
            var nomeTipo = "class";
            var extend = $"extends Snebur.UI.{nomeClassEntend}";

            SaveFileTypscript(projetoTS,
                               arquivo,
                               nomeTipo,
                               nomeClass,
                               nomeClassEntend);
        }

        private static string GetNomeClassExtend(string name)
        {

            if (name.Contains("Pagina"))
            {
                return "Pagina";
            }

            if (name.Contains("Janela"))
            {
                return "Janela";
            }

            if (name.Contains("Controle"))
            {
                return "ControleUsuario";
            }
            return "__DECONHECIDO__";
        }

        private static void InsertemplateScss(ProjetoTypeScript projetoTS, 
                                               FileInfo arquivo)
        {
            var nomeClassScss = CodigoUtil.Formatar(arquivo.Name, EnumFormatacaoCodigo.PascalCase, EnumFormatacaoCodigo.KebabCase);

            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine($"sn-{nomeClassScss}");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");

            LogVSUtil.Log($"Inserindo template scss: {arquivo.Name}");
            LocalProjetoUtil.SalvarDominio(arquivo.FullName, sb.ToString());
        }

        private static void InsertTemplateTS(ProjetoTypeScript projetoTS,
                                                     FileInfo arquivo)
        {
            var nomeTipo = arquivo.Name.ToLower().StartsWith("enum") ? "enum" : "class";
            var nomeClass = arquivo.Name.Split('.').First();

            var isViewModel = arquivo.Name.EndsWith("ViewModel.ts");
            var estenderViewModel = isViewModel ? "extends Snebur.Dominio.BaseViewModel " : String.Empty;

            SaveFileTypscript(projetoTS,
                               arquivo,
                               nomeTipo,
                               nomeClass,
                               estenderViewModel);
        }

        private static void SaveFileTypscript(ProjetoTypeScript projetoTS,
                                              FileInfo arquivo,
                                              string nomeTipo,
                                              string nomeClass,
                                              string extensao)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {projetoTS.NomeProjeto}");
            sb.AppendLine("{");
            sb.AppendLine($"\texport {nomeTipo} {nomeClass} {extensao}".TrimEnd());
            sb.AppendLine("\t{");
            sb.AppendLine("");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            LogVSUtil.Log($"Inserindo template {arquivo.GetPenultimateExtension()}: {arquivo.Name}");
            ArquivoUtil.SalvarTexto(arquivo.FullName, sb.ToString());
        }
    }
}
