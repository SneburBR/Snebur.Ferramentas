﻿using Community.VisualStudio.Toolkit;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace Snebur.VisualStudio.Commands
{
    [Command(PackageIds.GoToCodeCommand)]
    internal sealed class GoToCodeCommand : BaseCommand<GoToCodeCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            try
            {
                var dte = await VSEx.GetDTEAsync();
                if (dte.ActiveDocument != null)
                {
                    var nomeArquivo = dte.ActiveDocument.Name;
                    var isExtensaoShtml = nomeArquivo.EndsWith(ExtensaoContantes.EXTENSAO_CONTROLE_SHTML);
                    var isExtensaoScss = nomeArquivo.EndsWith(ExtensaoContantes.EXTENSAO_CONTROLE_SHTML_ESTILO);

                    if (!(isExtensaoShtml || isExtensaoScss))
                    {
                        try
                        {
                            dte.ExecuteCommand("View.ViewCode");
                        }
                        catch
                        {
                        }
                        return;
                    }

                    var caminhoArquivoAtual = dte.ActiveDocument.FullName;
                    if (isExtensaoScss)
                    {
                        caminhoArquivoAtual = Path.Combine(Path.GetDirectoryName(caminhoArquivoAtual),
                                                           Path.GetFileNameWithoutExtension(caminhoArquivoAtual));
                    }

                    var caminhoCodigo = caminhoArquivoAtual + ".ts";
                    if (File.Exists(caminhoCodigo))
                    {
                        var htmlApresentacao = isExtensaoShtml ? dte.ActiveDocument.RetornarTexto() : null;
                        dte.AbrirArquivo(caminhoCodigo);

                        var documentoCodigo = dte.ActiveDocument;
                        if (isExtensaoShtml &&
                            documentoCodigo.Name.EndsWith(ExtensaoContantes.EXTENSAO_CONTROLE_SHTML_TYPESCRIPT))
                        {
                            var declaracao = new DeclaracaoComponentes(dte,
                                                                       caminhoArquivoAtual,
                                                                       htmlApresentacao,
                                                                       documentoCodigo);
                            declaracao.Declarar();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }
        }
    }
}
