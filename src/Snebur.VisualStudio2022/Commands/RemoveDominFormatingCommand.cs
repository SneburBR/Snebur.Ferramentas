using Community.VisualStudio.Toolkit;
using EnvDTE;

namespace Snebur.VisualStudio.Commands
{
    [Command(PackageIds.RemoveDominFormatingCommand)]
    internal sealed class RemoveDominFormatingCommand : BaseCommand<RemoveDominFormatingCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = await VSEx.GetDTEAsync();
            var document = await VS.Documents.GetActiveDocumentViewAsync();
            var documento = dte.ActiveDocument;
            if (documento != null)
            {

                var nomeArquivo = documento.Name;
                if (nomeArquivo.EndsWith(".cs"))
                {
                    if (documento.Selection is TextSelection selecao)
                    {
                        selecao.SelectAll();

                        var conteudo = selecao.Text;
                        var linhas = conteudo.ToLines();
                        var inicioRegionCampoPrivados = false;
                        int inicieInicio = 0;
                        int inicieFim = 0;
                        for (var i = 0; i < linhas.Count; i++)
                        {
                            var linha = linhas[i];
                            if (linha.Contains("#region Campos Privados"))
                            {
                                inicioRegionCampoPrivados = true;
                                inicieInicio = i;
                            }
                            if (inicioRegionCampoPrivados)
                            {

                                if (linha.Contains("#endregion"))
                                {
                                    inicieFim = i;
                                    inicioRegionCampoPrivados = false;
                                }
                            }
                            if (linha.Contains("this.RetornarValorPropriedade"))
                            {
                                linhas[i] = linha.Substring(0, linha.IndexOf("{")) + "{ get; set; }";
                            }

                        }

                        if (inicieInicio > 0 && inicieFim > 0)
                        {
                            linhas.RemoveRange(inicieInicio - 1, (inicieFim - inicieInicio) + 2);
                        }
                        var novoContexto = String.Join(System.Environment.NewLine, linhas);
                        selecao.SelectAll();
                        selecao.Delete();
                        selecao.Insert(novoContexto);
                        //selecao.Text = novoContexto;
                        selecao.Collapse();

                        selecao.StartOfDocument(true);
                        selecao.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstText);
                    }
                }
            }
        }

    }
}
