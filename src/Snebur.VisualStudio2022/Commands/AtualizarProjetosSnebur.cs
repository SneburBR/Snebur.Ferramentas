//using Microsoft.VisualStudio.Shell;
//using Microsoft.VisualStudio.Shell.Interop;
//using System;
//using System.ComponentModel.Design;

//namespace Snebur.VisualStudio
//{
//    internal sealed class AtualizarProjetosSnebur : BaseComando
//    {

//        public const int CommandId = 256;


//        public static readonly Guid CommandSet = new Guid("558b9ec9-be5c-484b-b0f5-a4289d52e4a5");

//        //private readonly ExtensaoSneburPackage package;

//        private AtualizarProjetosSnebur(ExtensaoSneburPackage package)
//        {
//            if (package == null)
//            {
//                throw new ArgumentNullException("package");
//            }

//            this.package = package;

//            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
//            if (commandService != null)
//            {
//                var menuCommandID = new CommandID(CommandSet, CommandId);
//                var menuItem = new MenuCommand(this.AtualizarProjetos_MenuCommand, menuCommandID);
//                commandService.AddCommand(menuItem);
//            }
//        }

//        public static AtualizarProjetosSnebur Instance
//        {
//            get;
//            private set;
//        }

//        //private IServiceProvider ServiceProvider
//        //{
//        //    get
//        //    {
//        //        return this.package;
//        //    }
//        //}

//        public static void Initialize(ExtensaoSneburPackage package)
//        {
//            Instance = new AtualizarProjetosSnebur(package);
//        }


//        private void AtualizarProjetos_MenuCommand(object sender, EventArgs e)
//        {
//            ThreadHelper.ThrowIfNotOnUIThread();

//            var janela = this.package.FindToolWindow(typeof(JanelaSnebur), 0, true);
           
//            if ((null == janela) || (null == janela.Frame))
//            {
//                throw new NotSupportedException("A janela snebur não foi encontrada");
//            }

//            var windowFrame = janela.Frame as IVsWindowFrame;

//            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
//            JanelaSneburControl.Instacia?.NormalizarProjetosReferencias();

//        }
//    }
//}
