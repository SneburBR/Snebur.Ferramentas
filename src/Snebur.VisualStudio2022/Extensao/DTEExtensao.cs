
using EnvDTE;
using EnvDTE80;
using System;
using System.IO;

namespace Snebur.VisualStudio
{
    internal static class DTEExtensao
    {
        internal static Document AbrirArquivo(this DTE2 dte, string caminhoArquivo)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (!dte.get_IsOpenFile(Constants.vsViewKindCode, caminhoArquivo) &&
                    !dte.get_IsOpenFile(Constants.vsViewKindTextView, caminhoArquivo))
                {
                    try
                    {
                        var vsViewKind = RetornarVsViewKind(caminhoArquivo);
                        var janela = dte.OpenFile(vsViewKind, caminhoArquivo);
                        janela.Visible = true;
                        janela.SetFocus();
                    }
                    catch
                    {
                        var janela = dte.OpenFile(EnvDTE.Constants.vsViewKindTextView, caminhoArquivo);
                        janela.Visible = true;
                        janela.SetFocus();
                    }
                }
                var documento = dte.Documents.Item(caminhoArquivo);
                documento?.Activate();
                return documento;

            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro($"Falha ao abrir documento {caminhoArquivo}", ex);
                return null;
            }

        }

        internal static string RetornarVsViewKind(string caminhoArquivo)
        {
            var extensao = Path.GetExtension(caminhoArquivo);
            switch (extensao)
            {
                case ".xaml":

                    return Constants.vsViewKindDesigner;

                //case ".html":
                //case ".shtml":

                //    return Constants.vsDocumentKindHTML;

                case ".sql":
                case ".txt":

                    return Constants.vsViewKindTextView;

                default:
                    return Constants.vsViewKindCode;
            }

        }

        internal static Project RetornarProjetoAtivo(this DTE2 dte)
        {
            Project projetoAtivo = null;
            var projetosAtivos = dte.ActiveSolutionProjects as Array;
            if (projetosAtivos != null && projetosAtivos.Length > 0)
            {
                projetoAtivo = projetosAtivos.GetValue(0) as Project;
            }
            return projetoAtivo;
        }

        internal static string RetornarTexto(this Document documento)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (documento.Selection is TextSelection selecao)
            {
                selecao.SelectAll();
                var texto = selecao.Text;
                selecao.StartOfDocument();
                return texto;

            }
            return String.Empty;
        }

        internal static bool IsDebug(this DTE2 dte)
        {
            if (dte.Debugger != null)
            {
                return true;
            }

            return false;
        }

        public static ProjectItem RetornarProjectItemPai(this ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var encontrarPai = new EncontrarPai(projectItem);
            return encontrarPai.RetornarProjectItemPai();
        }

        private class EncontrarPai
        {
            private ProjectItem ProjectItem;

            public Project Projecto { get; }

            public EncontrarPai(ProjectItem projectItem)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                this.ProjectItem = projectItem;
                this.Projecto = projectItem.ContainingProject;
            }

            public ProjectItem RetornarProjectItemPai()
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                foreach (ProjectItem item in this.Projecto.ProjectItems)
                {
                    var projectItem = this.VarrerProjectItens(item);
                    if (projectItem != null)
                    {
                        return projectItem;
                    }

                }
                throw new Exception("O project item pai não foi encontrado ");
            }

            private ProjectItem VarrerProjectItens(ProjectItem projectItem)
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                foreach (ProjectItem item in projectItem.ProjectItems)
                {
                    var nome = item.Name;
                    if (item == this.ProjectItem)
                    {
                        return projectItem;
                    }
                    var projectItemFilho = this.VarrerProjectItens(item);
                    if (projectItemFilho != null)
                    {
                        return projectItemFilho;
                    }
                }
                return null;

            }
        }
    }
}