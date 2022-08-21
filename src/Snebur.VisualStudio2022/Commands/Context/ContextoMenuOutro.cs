//using Microsoft.VisualStudio.Shell;
//using System;
//using System.ComponentModel.Design;
//using System.Linq;
//using System.Windows;

//namespace Snebur.VisualStudio.MenuSnebur
//{
//    internal sealed class ContextoMenuOutro : BaseComando
//    {
//        public const int CommandId = 0x0300;

//        //public static readonly Guid CommandSet = new Guid("83d949c2-9f3e-40df-9cf5-3ac650c95081");

//        public static ContextoMenuOutro Instance { get; private set; }
//        public static void Initialize(ExtensaoSneburPackage package)
//        {
//            Instance = new ContextoMenuOutro(package);
//        }

//        private ContextoMenuOutro(ExtensaoSneburPackage package)
//        {
//            if (package == null)
//            {
//                throw new ArgumentNullException("package");
//            }

//            this.package = package;

//            var commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
//            if (commandService != null)
//            {
//                var menuCommandID = new CommandID(ContextoMenuEstilo.CommandSet, CommandId);
//                var menuItem = new OleMenuCommand(this.MenuItemCallback, menuCommandID);
//                menuItem.BeforeQueryStatus += menuItem_BeforeQueryStatus;
//                commandService.AddCommand(menuItem);
//            }
//        }

//        private void MenuItemCallback(object sender, EventArgs e)
//        {
//            MessageBox.Show("OUTRO NAO IMPLEMENTADO");
//            //throw new NotImplementedException("PUBLICAR);
//        }

        
//        private void menuItem_BeforeQueryStatus(object sender, EventArgs e)
//        {
//            var button = (OleMenuCommand)sender;
//            button.Visible = button.Enabled = true;

//            var items = ProjectHelpers.GetSelectedItems();
//            var xxx = items.Count();

//            if (items.Count() != 1)
//            {
//                return;
//            }

//            var item = items.FirstOrDefault();

//            if (item == null || item.ContainingProject == null || item.Properties == null)
//                return;

//        }
//    }
//}
