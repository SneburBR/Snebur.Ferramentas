//using Microsoft.VisualStudio.Shell;
//using System;
//using System.ComponentModel.Design;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Windows;

//namespace Snebur.VisualStudio.MenuSnebur
//{
//    internal sealed class ContextoMenuEstilo : BaseComando
//    {
//        public const int CommandId = 0x0200;

//        public static readonly Guid CommandSet = new Guid("83d949c2-9f3e-40df-9cf5-3ac650c95081");

//        public static ContextoMenuEstilo Instance { get; private set; }
//        public static void Initialize(ExtensaoSneburPackage package)
//        {
//            Instance = new ContextoMenuEstilo(package);
//        }

//        private ContextoMenuEstilo(ExtensaoSneburPackage package)
//        {
//            if (package == null)
//            {
//                throw new ArgumentNullException("package");
//            }

//            this.package = package;

//            var commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
//            if (commandService != null)
//            {
//                var menuCommandID = new CommandID(CommandSet, CommandId);
//                var menuItem = new OleMenuCommand(this.MenuItemCallback, menuCommandID);
//                menuItem.BeforeQueryStatus += menuItem_BeforeQueryStatus;
//                commandService.AddCommand(menuItem);
//            }
//        }

//        private void MenuItemCallback(object sender, EventArgs e)
//        {

//            var caminhoArquivoSelecionado = this.RetornarCaminhoSelecionado();
//            if(caminhoArquivoSelecionado!= null)
//            {
//                var caminhoEstilo = this.RetornarCaminhoArquivoEstilo(caminhoArquivoSelecionado);
//                if(caminhoEstilo!= null)
//                {
//                    if (File.Exists(caminhoEstilo))
//                    {
//                        MessageBox.Show("Remover estilo");
//                    }
//                    else
//                    {
//                        MessageBox.Show("Adicioanr estilo");
//                    }

//                }
//            }
//            //throw new NotImplementedException();
//        }

//        private void menuItem_BeforeQueryStatus(object sender, EventArgs e)
//        {
//            var button = (OleMenuCommand)sender;
//            button.Visible = button.Enabled = false;

//            var caminhoArquivoSelecionado = this.RetornarCaminhoSelecionado();

//            if (caminhoArquivoSelecionado != null)
//            {
//                var isSuportado = caminhoArquivoSelecionado.EndsWith(ExtensaoContantes.EXTENSAO_CONTROLE_SHTML_TYPESCRIPT) ||
//                                  caminhoArquivoSelecionado.EndsWith(ExtensaoContantes.EXTENSAO_CONTROLE_SHTML) ||
//                                  caminhoArquivoSelecionado.EndsWith(ExtensaoContantes.EXTENSAO_CONTROLE_SHTML_ESTILO);

//                if (isSuportado)
//                {
//                    var caminhoEstilo = this.RetornarCaminhoArquivoEstilo(caminhoArquivoSelecionado);
//                    if(caminhoEstilo!= null)
//                    {
//                        button.Visible = button.Enabled = true;
//                        button.Text = File.Exists(caminhoEstilo) ? "Remover camada de estilo (sass)" :
//                                                                   "Adicionar camada de estilo (sass)";
//                    }
//                }
//            }
//        }

//        private string RetornarCaminhoSelecionado()
//        {
//            var items = ProjectHelpers.GetSelectedItems();
//            if (items.Count() != 1)
//            {
//                return null;
//            }

//            var item = items.SingleOrDefault();
//            if (item == null || item.ContainingProject == null || item.Properties == null)
//            {
//                return null;
//            }
//            var configFile = item.ContainingProject.GetConfigFile();
//            return item.Properties.Item("FullPath").Value.ToString().ToLower();
//        }

//        private string RetornarCaminhoArquivoEstilo(string caminhoArquivo)
//        {
//            if (caminhoArquivo.Contains(ExtensaoContantes.EXTENSAO_CONTROLE_SHTML))
//            {
//                if (!caminhoArquivo.EndsWith(ExtensaoContantes.EXTENSAO_CONTROLE_SHTML))
//                {
//                    caminhoArquivo = Path.GetFileNameWithoutExtension(caminhoArquivo);
//                }
//                if (caminhoArquivo.EndsWith(ExtensaoContantes.EXTENSAO_CONTROLE_SHTML))
//                {
//                    return $"{caminhoArquivo}{Path.GetExtension(ExtensaoContantes.EXTENSAO_CONTROLE_SHTML_ESTILO)}";
//                }
//            }
//            return null;
         
//        }
//    }
//}
