//using EnvDTE;
//using EnvDTE80;
//using Microsoft.VisualStudio.Shell;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.Design;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Snebur.VisualStudio.Utilidade;

//namespace Snebur.VisualStudio
//{
//    internal sealed class FormatarDocumento
//    {
     
//        public const int CommandId = 256;

//        public static readonly Guid CommandSet = new Guid("566c3c0b-4cab-4b45-b07b-af5256dab60b");
        

//        private readonly ExtensaoSneburPackage package;

//        public static FormatarDocumento Instance
//        {
//            get;
//            private set;
//        }

//        private IServiceProvider ServiceProvider
//        {
//            get
//            {
//                return this.package;
//            }
//        }


//        private FormatarDocumento(ExtensaoSneburPackage package)
//        {
//            if (package == null)
//            {
//                throw new ArgumentNullException("package");
//            }

//            this.package = package;
//            if (this.ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
//            {
//                var menuCommandID = new CommandID(CommandSet, CommandId);
//                var menuItem = new MenuCommand(this.FormatarDocumento_MenuCommand, menuCommandID);
//                commandService.AddCommand(menuItem);
//            }
//        }

//        private void FormatarDocumento_MenuCommand(object sender, EventArgs e)
//        {
//            try
//            {
//                FormatarDocumento.FormatarDocumentoInterno();
//            }
//            catch (Exception ex)
//            {
//                LogVSUtil.LogErro(ex);
//            }
//        }


        
//        public static void FormatarDocumentoInterno()
//        {

//            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
//            var documento = dte.ActiveDocument;
//            if (documento != null)
//            {
//                try
//                {
//                    LogVSUtil.Logs?.Clear();
//                    dte.ExecuteCommand("Edit.RemoveAndSort");
//                    dte.ExecuteCommand("Edit.FormatDocument");
//                }
//                catch
//                {
//                }
                

//                var nomeArquivo = documento.Name;
//                var fi = new FileInfo(documento.FullName);

//                if (FormatarDocumentoUtil.ExtensoesSuportadas.Contains(fi.Extension))
//                {
//                    if (documento.Selection is TextSelection selecao)
//                    {
//                        var posicao = selecao.TopPoint;

//                        var posicaoLinha = selecao.TopPoint.Line;
//                        var posicaoColuna = selecao.TopPoint.LineCharOffset;

//                        selecao.SelectAll();


//                        var conteudo = selecao.Text;
//                        var isCsharp = fi.Extension.ToLower() == ".cs";
//                        var conteudoFormatado = FormatarDocumentoUtil.RetornarConteudoFormatado(conteudo,isCsharp);
//                        selecao.SelectAll();
//                        var totalLinhas = conteudoFormatado.TotalLinhas();
//                        if ((totalLinhas - 1) < posicaoLinha)
//                        {
//                            posicaoLinha = totalLinhas - 1;
//                        }
//                        selecao.Delete();
//                        selecao.Insert(conteudoFormatado);
//                        selecao.Collapse();

//                        selecao.MoveToLineAndOffset(posicaoLinha, posicaoColuna, true);
//                        selecao.Collapse();

//                        //selecao.StartOfDocument(true);
//                        //selecao.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstText);
//                    }
//                }

//            }
//        }

//        public static void Initialize(ExtensaoSneburPackage package)
//        {
//            Instance = new FormatarDocumento(package);
//        }

//    }
//}
