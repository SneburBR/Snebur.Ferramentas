//using EnvDTE;
//using EnvDTE80;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.InteropServices;

//namespace Snebur.VisualStudio
//{
//    public static class ProjectHelpers
//    {
//        private static DTE2 _dte = SneburVisualStudio2022Package._dte;

//        public static string GetConfigFile(this Project project)
//        {
//            string folder = project.GetRootFolder();

//            if (string.IsNullOrEmpty(folder))
//                return null;

//            return Path.Combine(folder, Contantes.CONFIG_FILENAME);
//        }

//        //public static void CheckFileOutOfSourceControl(string file)
//        //{
//        //    if (!File.Exists(file) || _dte.Solution.FindProjectItem(file) == null)
//        //        return;

//        //    if (_dte.SourceControl.IsItemUnderSCC(file) && !_dte.SourceControl.IsItemCheckedOut(file))
//        //        _dte.SourceControl.CheckOutItem(file);

//        //    FileInfo info = new FileInfo(file);
//        //    info.IsReadOnly = false;
//        //}

//        public static IEnumerable<ProjectItem> GetSelectedItems()
//        {
//            var items = (Array)_dte.ToolWindows.SolutionExplorer.SelectedItems;

//            foreach (UIHierarchyItem selItem in items)
//            {
//                ProjectItem item = selItem.Object as ProjectItem;

//                if (item != null)
//                    yield return item;
//            }
//        }

//        //public static IEnumerable<string> GetSelectedItemPaths()
//        //{
//        //    foreach (ProjectItem item in GetSelectedItems())
//        //    {
//        //        if (item != null && item.Properties != null)
//        //            yield return item.Properties.Item("FullPath").Value.ToString();
//        //    }
//        //}

//        public static string GetRootFolder(this Project project)
//        {
//            if (project == null || string.IsNullOrEmpty(project.FullName))
//                return null;

//            string fullPath;

//            try
//            {
//                fullPath = project.Properties.Item("FullPath").Value as string;
//            }
//            catch (ArgumentException)
//            {
//                try
//                {
//                    // MFC projects don't have FullPath, and there seems to be no way to query existence
//                    fullPath = project.Properties.Item("ProjectDirectory").Value as string;
//                }
//                catch (ArgumentException)
//                {
//                    // Installer projects have a ProjectPath.
//                    fullPath = project.Properties.Item("ProjectPath").Value as string;
//                }
//            }

//            if (string.IsNullOrEmpty(fullPath))
//                return File.Exists(project.FullName) ? Path.GetDirectoryName(project.FullName) : null;

//            if (Directory.Exists(fullPath))
//                return fullPath;

//            if (File.Exists(fullPath))
//                return Path.GetDirectoryName(fullPath);

//            return null;
//        }

//        public static void AddFileToProject(this Project project, string file, string itemType = null)
//        {
//            if (project.IsKind(ProjectTypes.ASPNET_5, ProjectTypes.DOTNET_Core))
//                return;

//            if (_dte.Solution.FindProjectItem(file) == null)
//            {
//                ProjectItem item = project.ProjectItems.AddFromFile(file);
//                item.SetItemType(itemType);
//            }
//        }

//        public static void SetItemType(this ProjectItem item, string itemType)
//        {
//            try
//            {
//                if (item == null || item.ContainingProject == null)
//                    return;

//                if (string.IsNullOrEmpty(itemType)
//                    || item.ContainingProject.IsKind(ProjectTypes.WEBSITE_PROJECT)
//                    || item.ContainingProject.IsKind(ProjectTypes.UNIVERSAL_APP))
//                    return;

//                item.Properties.Item("ItemType").Value = itemType;
//            }
//            catch (Exception ex)
//            {
//                LogVSUtil.LogErro(ex);
//            }
//        }

//        public static void AddNestedFile(string parentFile, string newFile, string itemType = null)
//        {
//            ProjectItem item = _dte.Solution.FindProjectItem(parentFile);

//            try
//            {
//                if (item == null
//                    || item.ContainingProject == null
//                    || item.ContainingProject.IsKind(ProjectTypes.ASPNET_5))
//                    return;

//                if (item.ProjectItems == null || item.ContainingProject.IsKind(ProjectTypes.UNIVERSAL_APP))
//                {
//                    item.ContainingProject.AddFileToProject(newFile);
//                }
//                else if (_dte.Solution.FindProjectItem(newFile) == null)
//                {
//                    item.ProjectItems.AddFromFile(newFile);
//                }

//                ProjectItem newItem = _dte.Solution.FindProjectItem(newFile);
//                newItem.SetItemType(itemType);
//            }
//            catch (Exception ex)
//            {
//                LogVSUtil.LogErro(ex);
//            }
//        }

//        public static bool IsKind(this Project project, params string[] kindGuids)
//        {
//            foreach (var guid in kindGuids)
//            {
//                if (project.Kind.Equals(guid, StringComparison.OrdinalIgnoreCase))
//                    return true;
//            }

//            return false;
//        }

//        public static void DeleteFileFromProject(string file)
//        {
//            ProjectItem item = _dte.Solution.FindProjectItem(file);
//            if (item == null)
//                return;
//            try
//            {
//                item.Delete();
//            }
//            catch (Exception ex)
//            {
//                LogVSUtil.LogErro(ex);
//            }
//        }

//        public static IEnumerable<Project> GetAllProjects()
//        {
//            return _dte.Solution.Projects
//                  .Cast<Project>()
//                  .SelectMany(GetChildProjects)
//                  .Union(_dte.Solution.Projects.Cast<Project>())
//                  .Where(p => { try { return !string.IsNullOrEmpty(p.FullName); } catch { return false; } });
//        }

//        private static IEnumerable<Project> GetChildProjects(Project parent)
//        {
//            try
//            {
//                if (!parent.IsKind(Contantes.vsProjectKindSolutionFolder) && parent.Collection == null)  // Unloaded
//                    return Enumerable.Empty<Project>();

//                if (!string.IsNullOrEmpty(parent.FullName))
//                    return new[] { parent };
//            }
//            catch (COMException)
//            {
//                return Enumerable.Empty<Project>();
//            }

//            return parent.ProjectItems
//                    .Cast<ProjectItem>()
//                    .Where(p => p.SubProject != null)
//                    .SelectMany(p => GetChildProjects(p.SubProject));
//        }

//        public static bool IsSolutionLoaded()
//        {
//            if (_dte.Solution == null)
//                return false;

//            return GetAllProjects().Any();
//        }

//        public static Project GetActiveProject()
//        {
//            try
//            {
//                Window2 window = _dte.ActiveWindow as Window2;
//                Document doc = _dte.ActiveDocument;

//                if (window != null && window.Type == vsWindowType.vsWindowTypeDocument)
//                {
//                    // if a document is active, use the document's containing directory
//                    if (doc != null && !string.IsNullOrEmpty(doc.FullName))
//                    {
//                        ProjectItem docItem = _dte.Solution.FindProjectItem(doc.FullName);

//                        if (docItem != null && docItem.ContainingProject != null)
//                            return docItem.ContainingProject;
//                    }
//                }

//                Array activeSolutionProjects = _dte.ActiveSolutionProjects as Array;

//                if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
//                    return activeSolutionProjects.GetValue(0) as Project;

//                if (doc != null && !string.IsNullOrEmpty(doc.FullName))
//                {
//                    var item = _dte.Solution?.FindProjectItem(doc.FullName);

//                    if (item != null)
//                        return item.ContainingProject;
//                }
//            }
//            catch (Exception ex)
//            {
//                LogVSUtil.LogErro("Error getting the active project" + ex);
//            }

//            return null;
//        }

//        public static bool IsConfigFile(this ProjectItem item)
//        {
//            if (item == null || item.Properties == null || item.ContainingProject == null)
//                return false;

//            var sourceFile = item.Properties.Item("FullPath").Value.ToString();
//            return Path.GetFileName(sourceFile).Equals(Contantes.CONFIG_FILENAME, StringComparison.OrdinalIgnoreCase);
//        }
//    }

//    public static class ProjectTypes
//    {
//        public const string ASPNET_5 = "{9f80621a-6787-4849-bfd4-b9436647065d}";
//        public const string DOTNET_Core = "{7506b20b-d702-4186-b92b-923dd68b00ec}";
//        public const string WEBSITE_PROJECT = "{c0b83a02-6d9f-4491-86b0-2f87765d46ea}";
//        public const string UNIVERSAL_APP = "{fbfbef3f-5ab3-4e7c-a54b-f1c39a3023ce}";
//        public const string NODE_JS = "{8bc8b082-c053-4102-b7c9-92ac96339488}";
//    }

//    class Contantes
//    {
//        public const string CONFIG_FILENAME = "compilerconfig.json";
//        public const string DEFAULTS_FILENAME = "compilerconfig.json.defaults";
//        public const string VSIX_NAME = "Web Compiler";
//        public const string NUGET_ID = "BuildWebCompiler";

//        public const string vsProjectKindSolutionFolder = "{5e4e30c6-2d26-46db-a0f0-226bc5edb5aa}";
//    }
//}
