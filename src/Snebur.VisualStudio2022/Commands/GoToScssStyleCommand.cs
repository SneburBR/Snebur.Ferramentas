using Community.VisualStudio.Toolkit;
using EnvDTE;
using EnvDTE80;
using Snebur.Utilidade;
using System.IO;

namespace Snebur.VisualStudio
{

    [Command(PackageIds.GoToScssStyleCommand)]
    internal sealed class GoToScssStyleCommand : BaseCommand<GoToScssStyleCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var dte = await DteEx.GetDTEAsync();
            if (dte.ActiveDocument != null)
            {

                var nomeArquivo = dte.ActiveDocument.Name;
                if (nomeArquivo.EndsWith(".shtml") || nomeArquivo.EndsWith(".shtml.ts"))
                {
                    var caminhoArquivoAtual = dte.ActiveDocument.FullName;
                    if (caminhoArquivoAtual.EndsWith(".ts"))
                    {
                        caminhoArquivoAtual = caminhoArquivoAtual.Substring(0, caminhoArquivoAtual.Length - 3);
                    }
                    var caminhoScsss = caminhoArquivoAtual + ".scss";
                    if (!File.Exists(caminhoScsss))
                    {
                        this.AdicionarAcaoParaCriarEstilo(dte, caminhoArquivoAtual, caminhoScsss);

                        return;
                    }
                    if (File.Exists(caminhoScsss))
                    {
                        dte.AbrirArquivo(caminhoScsss);
                    }
                }
            }
        }

        private void AdicionarAcaoParaCriarEstilo(DTE2 dte, string caminhoArquivoAtual, string caminhoScss)
        {
            var nomeArquivoScss = Path.GetFileName(caminhoScss);
            LogVSUtil.Alerta($"Arquivo {nomeArquivoScss} SCSS não existe");
            LogVSUtil.LogAcaoLink($"Clique aqui para adicionar {nomeArquivoScss}", (Action)(() =>
            {
                _ = this.AdicioanrArquivoAsync(caminhoArquivoAtual, caminhoScss);
            }));

        }

        private async Task AdicioanrArquivoAsync(string caminhoArquivoAtual,
                                                 string caminhoScss)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (File.Exists(caminhoScss))
            {
                return;
            }
            ArquivoUtil.CriarArquivoTexto(caminhoScss);
            var dte = await DteEx.GetDTEAsync();
            var caminhoLayout = ArquivoControleUtil.RetornarCaminhoShtml(caminhoArquivoAtual);
            var projetoItem = dte.Solution.FindProjectItem(caminhoLayout);
            if (projetoItem == null)
            {
                LogVSUtil.LogErro($"Não foi encontrado o arquivo do layout {Path.GetFileName(caminhoScss)}");
                return;
            }
            await GerenciadorProjetos.Instancia.AdicionarArquivoAsync(projetoItem.ProjectItems, caminhoScss);
        }
    }
}
