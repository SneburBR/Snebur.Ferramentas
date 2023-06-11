using Community.VisualStudio.Toolkit;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{
    public static class SolutionsExtension
    {
        public async static Task<IEnumerable<Project>> GetStartupProjectsAsync(this Solutions solutions)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var dte = await DteEx.GetDTEAsync();
            if (dte.Solution.IsOpen)
            {
                try
                {
                    var sb = dte.Solution.SolutionBuild;
                    var projectsName = ((Array)sb.StartupProjects).Cast<string>().
                                                                   Select(x => Path.GetFileNameWithoutExtension(x)).
                                                                   ToList();

                    if (projectsName.Count > 0)
                    {
                        var allProjects = await solutions.GetAllProjectsAsync();
                        var projetosInicializacao = allProjects.Where(x => projectsName.Contains(x.Name, new IgnorarCasoSensivel()) || projectsName.Contains(x.Text, new IgnorarCasoSensivel())).ToList();
                        if (projetosInicializacao.Count < projectsName.Count)
                        {
                            var startupProjetNames = projetosInicializacao.Select(x => x.Name).ToList();
                            var projetosNaoEncontrados = projectsName.Where(x => !startupProjetNames.Contains(x, new IgnorarCasoSensivel())).ToList();
                            var descricaoProjetosNaoEncontrados = String.Join(", ", projetosNaoEncontrados);
                            LogVSUtil.LogErro($"Os projetos de inicialização {descricaoProjetosNaoEncontrados} não foram encontrados");
                        }
                        return projetosInicializacao;
                    }
                }
                catch(Exception erro)
                {
                    LogVSUtil.LogErro(erro);
                }
            }
            return new List<Project>();
        }

        public async static Task SaveAllProjetsAsync(this Solutions solutions)
        {
            var allProjects = await solutions.GetAllProjectsAsync();
            foreach (var project in allProjects)
            {
                await project.SaveAsync();
            }
        }

        public async static Task<PhysicalFile> AddExistingFileAsync(this PhysicalFolder folder, string fullPath)
        {
            var arquivos = await folder.AddExistingFilesAsync(fullPath);
            return arquivos.FirstOrDefault();
        }

        public static Task<PhysicalFile> GetPhysicalFileAsync(this Project project, string fullPath)
        {
            return SolutionUtil.GetPhysicalFileAsync(project.Children, fullPath);
        }

        public static Task<bool> TryBuildProjectAsync(this Build build,
                                                            Project project)
        {
            return TryBuildProjectAsync(build, project, false);
        }

        private static async Task<bool> TryBuildProjectAsync(Build build,
                                                             Project project,
                                                              bool isRecursivo)
        {
            Exception erro = null;
            try
            {
                if (isRecursivo)
                {
                    project = await VS.Solutions.FindProjectsAsync(project.Name);
                }
                var t = Stopwatch.StartNew();
                LogVSUtil.Log($"Compilando o projeto {project.Name}");
                var resultado = await build.BuildProjectAsync(project);
                if (resultado)
                {
                    LogVSUtil.Sucesso($"Projeto {project.Name} compilado", t);
                    return true;
                }
            }
            catch (Exception ex)
            {
                erro = ex;
                if (!isRecursivo)
                {
                    return await TryBuildProjectAsync(build, project, true);
                }
            }
            LogVSUtil.LogErro($"Falha ao compilando o projeto {project.Name}. {erro.Message}");
            return false;
        }

        public static Task<DocumentView> TryOpenAsync(this Documents documents, string file)
        {
            return documents.OpenAsync(file);
        }
    }


}