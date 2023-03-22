using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using EnvDTE80;
using System.IO;
using Snebur.Utilidade;
using Snebur.VisualStudio.Utilidade;
using Community.VisualStudio.Toolkit;

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
                    var caminhoEstilo = caminhoArquivoAtual + ".scss";
                    if (File.Exists(caminhoEstilo))
                    {
                        dte.AbrirArquivo(caminhoEstilo);
                    }
                }
            }
        }
    }
}
