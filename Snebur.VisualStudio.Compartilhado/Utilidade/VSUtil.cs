using System.IO;

namespace Snebur.VisualStudio
{
    public static class VSUtil
    {
        private const string PREFIXO_DIRETORIO_VISUAL_STUDIO = ".vs";

        internal static bool IsDiretorioVisualStudio(DirectoryInfo ditetorio)
        {
            while (ditetorio.Parent != null)
            {
                if (ditetorio.Name.StartsWith(PREFIXO_DIRETORIO_VISUAL_STUDIO))
                {
                    return true;
                }
                ditetorio = ditetorio.Parent;
            }
            return false;
        }

        internal static bool IsArquivoVisualStudio(string caminhoArquivo)
        {
            return VSUtil.IsDiretorioVisualStudio(new FileInfo(caminhoArquivo).Directory);
        }

        internal static bool IsArquivoVisualStudio(FileInfo arquivo)
        {
            return VSUtil.IsDiretorioVisualStudio(arquivo.Directory);
        }
    }
    //class VSUtil
    //{
    //    public static string WriteVisualStudioErrorList(MessageCategory category, string text, string code, string path, int line, int column)
    //    {
    //        if (_errorListProvider == null)
    //        {
    //            IServiceProvider serviceProvider =
    //            new ServiceProvider(VsPackage.ApplicationObject as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
    //            _errorListProvider = new ErrorListProvider(serviceProvider);
    //            _errorListProvider.ProviderName = "Factory Guide Errors";
    //            _errorListProvider.ProviderGuid = new Guid("67f1a08e-f8eb-4e22-9622-75df4ab7cd46");
    //            // _errorListProvider.ForceShowErrors();  
    //        }

    //        string textline = null;

    //        // Determine the task priority  
    //        TaskPriority priority = category == MessageCategory.Error ? TaskPriority.High : TaskPriority.Normal;
    //        TaskErrorCategory errorCategory = category == MessageCategory.Error ? TaskErrorCategory.Error : TaskErrorCategory.Warning;

    //        switch (errorCategory)
    //        {
    //            case TaskErrorCategory.Error:
    //                _errors++;
    //                break;
    //            case TaskErrorCategory.Warning:
    //                _warnings++;
    //                break;
    //        }

    //        // Check if this error is already in the error list, don't report more than once  
    //        bool alreadyReported = false;
    //        foreach (ErrorTask task in _errorListProvider.Tasks)
    //        {
    //            if (task.ErrorCategory == errorCategory &&
    //                task.Document == path &&
    //                task.Line == line - 1 &&
    //                task.Column == column - 1 &&
    //                task.Text == text)
    //            {
    //                alreadyReported = true;
    //                break;
    //            }
    //        }

    //        if (!alreadyReported)
    //        {
    //            // Add error to task list  
    //            ErrorTask task = new ErrorTask();
    //            task.Document = path;
    //            task.Line = line - 1; // The task list does +1 before showing this number.  
    //            task.Column = column - 1; // The task list does +1 before showing this number.  
    //            task.Text = text;
    //            task.Priority = priority; // High or Normal  
    //            task.ErrorCategory = errorCategory; // Error or Warning, no support for Message yet  
    //            task.Category = TaskCategory.BuildCompile;
    //            // task.HierarchyItem = hierarchy;  
    //            task.Navigate += new EventHandler(NavigateTo);
    //            if (VisualStudioExtensions.ContainsLink(text))
    //            {
    //                task.Help += new EventHandler(task_Help);
    //            }
    //            _errorListProvider.Tasks.Add(task);

    //            switch (errorCategory)
    //            {
    //                case TaskErrorCategory.Error:
    //                    _uniqueErrors++;
    //                    break;
    //                case TaskErrorCategory.Warning:
    //                    _uniqueWarnings++;
    //                    break;
    //            }

    //            string categoryString = category == MessageCategory.Error ? "error" : "warning";
    //            textline = MessageGeneration.Generate(category, text, code, path, line, column);
    //        }

    //        return textline;
    //    }

    //    private static void NavigateTo(object sender, EventArgs arguments)
    //    {
    //        Microsoft.VisualStudio.Shell.Task task = sender as Microsoft.VisualStudio.Shell.Task;

    //        if (task == null)
    //        {
    //            throw new ArgumentException("sender");
    //        }

    //        // If the name of the file connected to the task is empty there is nowhere to navigate to  
    //        if (String.IsNullOrEmpty(task.Document))
    //        {
    //            return;
    //        }

    //        IServiceProvider serviceProvider =
    //new ServiceProvider(VsPackage.ApplicationObject as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);

    //        IVsUIShellOpenDocument openDoc = serviceProvider.GetService(typeof(IVsUIShellOpenDocument)) as IVsUIShellOpenDocument;

    //        if (openDoc == null)
    //        {
    //            return;
    //        }

    //        IVsWindowFrame frame;
    //        Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp;
    //        IVsUIHierarchy hier;
    //        uint itemid;
    //        Guid logicalView = VSConstants.LOGVIEWID_Code;

    //        if (ErrorHandler.Failed(openDoc.OpenDocumentViaProject(
    //            task.Document, ref logicalView, out sp, out hier, out itemid, out frame))
    //            || frame == null
    //        )
    //        {
    //            return;
    //        }

    //        object docData;
    //        frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocData, out docData);

    //        // Get the VsTextBuffer  
    //        VsTextBuffer buffer = docData as VsTextBuffer;
    //        if (buffer == null)
    //        {
    //            IVsTextBufferProvider bufferProvider = docData as IVsTextBufferProvider;
    //            if (bufferProvider != null)
    //            {
    //                IVsTextLines lines;
    //                ErrorHandler.ThrowOnFailure(bufferProvider.GetTextBuffer(out lines));
    //                buffer = lines as VsTextBuffer;
    //                Debug.Assert(buffer != null, "IVsTextLines does not implement IVsTextBuffer");

    //                if (buffer == null)
    //                {
    //                    return;
    //                }
    //            }
    //        }

    //        // Finally, perform the navigation.  
    //        IVsTextManager mgr = serviceProvider.GetService(typeof(VsTextManagerClass)) as IVsTextManager;
    //        if (mgr == null)
    //        {
    //            return;
    //        }

    //        mgr.NavigateToLineAndColumn(buffer, ref logicalView, task.Line, task.Column, task.Line, task.Column);
    //    }

    //    /// <summary>  
    //    /// Determines whether the task text contains a url, we assume that url is help.  
    //    /// </summary>  
    //    /// <param name="text">The task text.</param>  
    //    /// <returns>  
    //    ///     <c>true</c> if the text contains a link, assume its a help link; otherwise, <c>false</c>.  
    //    /// </returns>  
    //    static bool ContainsLink(string text)
    //    {
    //        if (text == null)
    //        {
    //            throw new ArgumentNullException("text");
    //        }

    //        Match urlMatches = Regex.Match(text,
    //                    @"((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)");
    //        return urlMatches.Success;
    //    }

    //    /// <summary>  
    //    /// Handles the Help event of the task control.  
    //    /// </summary>  
    //    /// <param name="sender">The Task to parse for a guidance link.</param>  
    //    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>  
    //    static void task_Help(object sender, EventArgs e)
    //    {
    //        Microsoft.VisualStudio.Shell.Task task = sender as Microsoft.VisualStudio.Shell.Task;

    //        if (task == null)
    //        {
    //            throw new ArgumentException("sender");
    //        }

    //        string url = null;
    //        Match urlMatches = Regex.Match(task.Text,
    //                    @"((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)");
    //        if (urlMatches.Success)
    //        {
    //            url = urlMatches.Captures[0].Value;
    //        }

    //        if (url != null)
    //        {
    //            VsPackage.ApplicationObject.ItemOperations.Navigate(url,
    //                                              vsNavigateOptions.vsNavigateOptionsDefault);
    //        }
    //    }
    //}
}
