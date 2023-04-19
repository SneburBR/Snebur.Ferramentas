using EnvDTE;
using EnvDTE80;
using Snebur.VisualStudio.Utilidade;
using System.IO;

namespace Snebur.VisualStudio
{
    public static class TsNewStringFormatUtil
    {
        public static async Task FormatarNovoStringFormatAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = await DteEx.GetDTEAsync();
            var documento = dte.ActiveDocument;

            if (documento != null)
            {
                var nomeArquivo = documento.Name;
                var fi = new FileInfo(documento.FullName);

                if (FormatarDocumentoUtil.ExtensoesSuportadas.Contains(fi.Extension))
                {
                    if (documento.Selection is TextSelection selecao)
                    {
                        var posicao = selecao.TopPoint;
                        var posicaoLinha = selecao.TopPoint.Line;
                        var posicaoColuna = selecao.TopPoint.LineCharOffset;

                        selecao.SelectAll();

                        var conteudo = selecao.Text;
                        var isCsharp = fi.Extension.ToLower() == ".cs";

                        var objSubstituir = new SubstituicaoNovoStringFormatTS(conteudo, isCsharp);
                        var conteudoFormatado = objSubstituir.RetornarConteudo();

                        //selecao.SelectAll();
                        var totalLinhas = conteudoFormatado.TotalLinhas();
                        if ((totalLinhas - 1) < posicaoLinha)
                        {
                            posicaoLinha = totalLinhas - 1;
                        }
                        selecao.Delete();
                        selecao.Insert(conteudoFormatado);
                        selecao.Collapse();

                        selecao.MoveToLineAndOffset(posicaoLinha, posicaoColuna, true);
                        selecao.Collapse();


                        if (conteudoFormatado.Contains(SubstituicaoNovoStringFormatTS.PESQUISAR))
                        {
                            selecao.FindText(SubstituicaoNovoStringFormatTS.PESQUISAR);
                            selecao.SelectLine();
                        }
                    }
                }
            }
        }
    }
}