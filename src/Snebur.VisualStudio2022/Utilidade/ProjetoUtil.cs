﻿using EnvDTE;
using EnvDTE80;
using Snebur.Utilidade;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{
    public static class ProjetoUtil
    {
        public static bool IsProjetoZyoncore { get; private set; }

        //ProjectKinds.vsProjectKindSolutionFolder
        //public const string VS_PROJECT_KIND_SOLUTION_FOLDER = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}"; // ProjectKinds.vsProjectKindSolutionFolder;

        public static async Task<List<BaseProjeto>> RetornarProjetosAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var projetos = new List<BaseProjeto>();
            var dte = await VSEx.GetDTEAsync();
            if (dte.Solution.Count > 0)
            {

                var projetosVS = await ProjetoUtil.RetornarProjetosVisualStudioAsync();

                //var UIH = (EnvDTE.UIHierarchy)dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Object;

                foreach (var projetoVS in projetosVS)
                {
                    try
                    {
                        var projetoVS2 = (DteExtensao.Project)projetoVS;
                        var nomeArquivoProjeto = projetoVS.FileName;
                        if (!String.IsNullOrWhiteSpace(nomeArquivoProjeto) && (File.Exists(Path.GetFullPath(projetoVS.FileName))))
                        {
                            var arquivoProjeto = new FileInfo(projetoVS.FileName);
                            var diretorioProjeto = arquivoProjeto.Directory;
                            if (!arquivoProjeto.Exists)
                            {
                                throw new DirectoryNotFoundException($"O arquivo do projeto não foi encontrado {arquivoProjeto.FullName}");
                            }

                            var caminhoConfiguracaoDominio = Path.Combine(diretorioProjeto.FullName, ConstantesProjeto.CONFIGURACAO_DOMINIO);
                            var caminhoConfiguracaoTypeScript = Path.Combine(diretorioProjeto.FullName, ConstantesProjeto.CONFIGURACAO_TYPESCRIPT);
                            var caminhoConfiguracaoContextoDados = Path.Combine(diretorioProjeto.FullName, ConstantesProjeto.CONFIGURACAO_CONTEXTO_DADOS);
                            var caminhoConfiguracaoRegrasNegocio = Path.Combine(diretorioProjeto.FullName, ConstantesProjeto.CONFIGURACAO_REGRIAS_NEGOCIO);
                            var caminhoConfiguracaoSass = Path.Combine(diretorioProjeto.FullName, ConstantesProjeto.CONFIGURACAO_SASS);
                            var caminhoConfiguracaoServicos = Path.Combine(diretorioProjeto.FullName, ConstantesProjeto.CONFIGURACAO_SERVICOS);
                            var caminhoConfiguracaoWebConfig = Path.Combine(diretorioProjeto.FullName, ConstantesProjeto.CONFIGURACAO_WEB_CONFIG);
                            var caminhoConfiguracaoAppSettings = Path.Combine(diretorioProjeto.FullName, ConstantesProjeto.CONFIGURACAO_APP_SETTINGS);

                            //Domínio
                            if (File.Exists(caminhoConfiguracaoDominio) && File.Exists(caminhoConfiguracaoTypeScript))
                            {
                                throw new NotSupportedException(String.Format("Não é suportado no mesmo projetos arquivos de configuração de dominio.json e tsconfig.json juntos: Projeto {0}", projetoVS.Name));
                            }

                            if (File.Exists(caminhoConfiguracaoDominio))
                            {
                                var configuracaoDominio = ProjetoDominio.RetornarConfiguracaoDominio(caminhoConfiguracaoDominio);
                                projetos.Add(new ProjetoDominio(projetoVS2,
                                                                configuracaoDominio,
                                                                arquivoProjeto,
                                                                caminhoConfiguracaoDominio));
                                LogVSUtil.Log($"Compilando o projeto {projetoVS.Name}");

                            }

                            if (File.Exists(caminhoConfiguracaoTypeScript))
                            {
                                //TypeScript
                                LogVSUtil.Log($"Projeto TypeScript encontrado : {projetoVS.Name} ");
                                var configuracao = ProjetoTypeScriptUtil.RetornarConfiguracaoProjetoTypeScript(caminhoConfiguracaoTypeScript);
                                projetos.Add(ProjetoTypeScriptUtil.RetornarProjetoTypeScript(configuracao,
                                                                                             projetoVS2,
                                                                                             arquivoProjeto,
                                                                                             caminhoConfiguracaoTypeScript));
                            }

                            //ContextoDados
                            if (File.Exists(caminhoConfiguracaoContextoDados))
                            {
                                LogVSUtil.Log(String.Format("Projeto ContextoDados encontrado : {0} ", projetoVS.Name));
                                var configuracao = ProjetoContextoDados.RetornarConfiguracao(caminhoConfiguracaoContextoDados);
                                projetos.Add(new ProjetoContextoDados(projetoVS2, configuracao, arquivoProjeto, caminhoConfiguracaoContextoDados));
                            }

                            //RegrasNegocio
                            if (File.Exists(caminhoConfiguracaoRegrasNegocio))
                            {
                                LogVSUtil.Log(String.Format("Projeto regra de negócios encontrado : {0} ", projetoVS.Name));

                                var configuracao = ProjetoRegrasNegocioUtil.RetornarConfiguracao(caminhoConfiguracaoRegrasNegocio);
                                var caminhoBase = Path.GetDirectoryName(caminhoConfiguracaoRegrasNegocio);
                                var caminhoExtensaoTS = configuracao.RetornarCaminhoExtensaoTypeScriptCompleto(caminhoBase);
                                var caminhoExtensaoCS = configuracao.RetornarCaminhoExtensaoCSharpCompleto(caminhoBase);
                                if (File.Exists(caminhoExtensaoTS))
                                {
                                    projetos.Add(new ProjetoRegrasNegocioTypeScript(projetoVS2,
                                                                                    configuracao,
                                                                                    arquivoProjeto,
                                                                                    caminhoConfiguracaoContextoDados));
                                }

                                if (File.Exists(caminhoExtensaoCS))
                                {
                                    projetos.Add(new ProjetoRegrasNegocioCSharp(projetoVS2,
                                                                                configuracao,
                                                                                arquivoProjeto,
                                                                                caminhoConfiguracaoContextoDados));
                                }

                            }

                            //Serviço
                            if (File.Exists(caminhoConfiguracaoServicos))
                            {
                                LogVSUtil.Log(String.Format("Projeto servicos encontrado : {0} ", projetoVS.Name));
                                var configuracaoServicoes = ProjetoServicosTypescript.RetornarConfiguracao(caminhoConfiguracaoServicos);
                                projetos.Add(new ProjetoServicosTypescript(projetoVS2,
                                                                           configuracaoServicoes,
                                                                           arquivoProjeto,
                                                                           caminhoConfiguracaoServicos));

                                projetos.Add(new ProjetoServicosDotNet(projetoVS2,
                                                                        configuracaoServicoes,
                                                                        arquivoProjeto,
                                                                        caminhoConfiguracaoServicos));
                            }

                            //Sass
                            if (File.Exists(caminhoConfiguracaoSass))
                            {
                                LogVSUtil.Log(String.Format("Projeto sass encontrado : {0} ", projetoVS.Name));
                                var configuracaoSass = ProjetoEstilo.RetornarConfiguracao(caminhoConfiguracaoSass);
                                if (configuracaoSass != null && !configuracaoSass.IsIgnorar)
                                {
                                    projetos.Add(new ProjetoEstilo(projetoVS2,
                                                                   configuracaoSass,
                                                                   arquivoProjeto,
                                                                   caminhoConfiguracaoSass));
                                }
                            }

                            if (File.Exists(caminhoConfiguracaoWebConfig) &&
                                File.Exists(caminhoConfiguracaoAppSettings))
                            {
                                LogVSUtil.Log(String.Format("Projeto web service : {0} ", projetoVS.Name));

                                var configuracaoWeb = ProjetoWeb.RetornarConfiguracao(caminhoConfiguracaoWebConfig,
                                                                                      caminhoConfiguracaoTypeScript);
                                switch (configuracaoWeb)
                                {
                                    case ConfiguracaoProjetoWebApresentacao configuracaoProjetoWebApresentacao:

                                        var projetoTypescript = projetos.OfType<ProjetoTypeScript>().Where(x => x.ProjetoVS == projetoVS).Single();

                                        projetos.Add(new ProjetoWebApresentacao(projetoVS2,
                                                                                configuracaoProjetoWebApresentacao,
                                                                                projetoTypescript,
                                                                                arquivoProjeto,
                                                                                caminhoConfiguracaoWebConfig));

                                        break;

                                    case ConfiguracaoProjetoWebService configuracaoWebService:

                                        projetos.Add(new ProjetoWebService(projetoVS2,
                                                                           configuracaoWebService,
                                                                           arquivoProjeto,
                                                                           caminhoConfiguracaoWebConfig));
                                        break;
                                    default:
                                        break;
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LogVSUtil.LogErro(ex.Message);
                    }
                }

                foreach (var projeto in projetos)
                {
                    projeto.UniqueName = ((Project)projeto.ProjetoVS).UniqueName;
                }
            }
            else
            {
                LogVSUtil.Log("Nenhum projeto encontrado");
            }
            return projetos;

        }

        internal static bool IsProjetoTypescript(string caminhoProjeto)
        {
            if (!String.IsNullOrEmpty(caminhoProjeto))
            {
                var caminhoTSConfig = Path.Combine(caminhoProjeto, "tsconfig.json");
                return File.Exists(caminhoTSConfig);
            }
            return false;
        }

        public static void DefinirProjetosInicializacao()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            GerenciadorProjetos.Instancia.DiretorioProjetoTypescriptInicializacao = null;

            var sb = dte.Solution.SolutionBuild;
            //var sc = sb.ActiveConfiguration;
            //var xxx = dte.Solution.FullName;
            //var aaaa = dte.Solution.FileName;

            var projetosStartup = (Array)sb.StartupProjects;

            if (projetosStartup.Length > 0)
            {
                DefinirProjetosInicializacao(dte, projetosStartup);
            }
        }

        private static void DefinirProjetosInicializacao(DTE2 dte,
                                                        Array projetosStartup)
        {
            foreach (var projetoStartup in projetosStartup)
            {
                if (projetoStartup is string caminhoRelativoProjeto)
                {
                    var diretorioSolucacao = Path.GetDirectoryName(dte.Solution.FullName);
                    //var caminhoRelativoProjeto = (string)projetosStartup.GetValue(0);
                    var caminhoProjeto = Path.GetFullPath(Path.Combine(diretorioSolucacao, caminhoRelativoProjeto));
                    var diretorioProjeto = Path.GetDirectoryName(caminhoProjeto);
                    var caminhoTS = Path.Combine(diretorioProjeto, ConstantesProjeto.CONFIGURACAO_TYPESCRIPT);
                    if (File.Exists(caminhoTS))
                    {
                        try
                        {

                            var configuracaoTypescript = JsonUtil.Deserializar<ConfiguracaoProjetoTypeScriptFramework>(ArquivoUtil.LerTexto(caminhoTS));
                            GerenciadorProjetos.Instancia.ConfiguracaoProjetoTypesriptInicializacao = configuracaoTypescript;
                            if (configuracaoTypescript.IsDebugScriptsDepedentes)
                            {
                                GerenciadorProjetos.Instancia.DiretorioProjetoTypescriptInicializacao = diretorioProjeto;
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogVSUtil.LogErro(ex);
                        }
                    }

                }

            }
        }

        //private static readonly string[] PastarIgnorar = new string[] { "build", "pr" };
        private static readonly HashSet<string> PastarIgnorar = new() { "build", "packages", "node_modules", ".vs", ".git", "test",
                                                                       "tests","lib", "obj", "bin", "Recursos",
                                                                       "resources", "Properties"};

        public async static Task<List<Project>> RetornarProjetosVisualStudioAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var projetos = new List<Project>();
            var dte = await VSEx.GetDTEAsync();
            var item = dte.Solution.Projects.GetEnumerator();
            while (item.MoveNext())
            {
                var itemSolucao = item.Current as Project;
                if (itemSolucao != null && itemSolucao.Name != null)
                {
                    //if (itemSolucao.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                    if (itemSolucao.Kind == VsConstantes.VsProjectItemKindPhysicalFolder  )
                    {
                        if (ProjetoUtil.PastarIgnorar.Contains(itemSolucao.Name))
                        {
                            continue;
                        }
                        
                    }

                    if (itemSolucao.Kind == VsConstantes.VsProjectItemKindSolutionFolder)
                    {
                        projetos.AddRange(ProjetoUtil.RetornarProjetosDaPasta(itemSolucao));
                    }
                    else
                    {
                        try
                        {
                            if (itemSolucao.ProjectItems?.Count > 0 &&
                               !String.IsNullOrWhiteSpace(itemSolucao.FullName))
                            {
                                projetos.Add(itemSolucao);
                            }
                        }
                        catch
                        {

                        }

                    }
                }
            }
            ProjetoUtil.IsProjetoZyoncore = projetos.Any(x => x.Name.Contains("Zyoncore"));
            return projetos;
        }

        private static IEnumerable<Project> RetornarProjetosDaPasta(Project pastaSolucao)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var projetos = new List<Project>();
            for (var i = 1; i <= pastaSolucao.ProjectItems.Count; i++)
            {
                var projectItem = pastaSolucao.ProjectItems.Item(i);
                var nomeItem = projectItem.Name;
                var filhos = projectItem.ProjectItems;

                var subProjeto = pastaSolucao.ProjectItems.Item(i).SubProject;
                var o = projectItem.Object;

                if (subProjeto != null)
                {
                    if (subProjeto.Kind == VsConstantes.VsProjectItemKindSolutionFolder)
                    {
                        projetos.AddRange(ProjetoUtil.RetornarProjetosDaPasta(subProjeto));
                    }
                    else
                    {
                        projetos.Add(subProjeto);
                    }
                }
            }
            return projetos;
        }


        public static bool CompilarSolucao(DTE2 dte)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();

            LogVSUtil.Log("Compilandos os projetos");
            try
            {
                dte.Solution.SolutionBuild.Build(true);
                return true;
            }
            catch (Exception erro)
            {
                throw new Exception("Não foi possível compilar a solução ", erro);
            }
        }

        public static bool CompilarProjeto(DTE2 dte, Project projetoVS)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                dte.Solution.SolutionBuild.BuildProject("Debug", projetoVS.UniqueName, true);
                return true;
            }
            catch (Exception erro)
            {
                throw new Exception(String.Format("Não foi possivel compilar o projeto {0} ", projetoVS.Name), erro);
            }
        }

        public static HashSet<string> RetornarTodosArquivos(Project projetoVS, bool isLowerCase)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return ProjetoUtil.RetornarTodosArquivos(projetoVS.ProjectItems, isLowerCase);
        }

        public static HashSet<string> RetornarTodosArquivos(ProjectItems items, bool isLowerCase)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var arquivos = new HashSet<string>();
            if (items != null)
            {
                foreach (ProjectItem item in items)
                {
                    if (item.Kind == VsConstantes.VsProjectItemKindPhysicalFolder)
                    {
                        if (ProjetoUtil.PastarIgnorar.Contains(item.Name))
                        {
                            continue;
                        }

                    }

                    if (item.FileCount > 0)
                    {
                        for (short i = 0; i < item.FileCount; i++)
                        {
                            try
                            {
                                var caminhoArquivo = item.FileNames[i];
                                if (caminhoArquivo != null)
                                {
                                    if (isLowerCase)
                                    {
                                        caminhoArquivo = caminhoArquivo.ToLower();
                                    }
                                    arquivos.Add(caminhoArquivo);
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                    if (item.ProjectItems != null && item.ProjectItems.Count > 0)
                    {
                        arquivos.AddRange(ProjetoUtil.RetornarTodosArquivos(item.ProjectItems,
                                                                            isLowerCase));
                    }
                }
            }
            return arquivos;
        }

        public static List<(ProjectItem ProjectItem, List<string> Arquivos)> RetornarProjectItemsArquivo(ProjectItems items, bool caixaBaixa)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var retorno = new List<(ProjectItem, List<string>)>();
            if (items != null)
            {
                foreach (ProjectItem item in items)
                {
                    var arquivos = new List<string>();
                    if (item.FileCount > 0)
                    {
                        for (short i = 0; i < item.FileCount; i++)
                        {
                            try
                            {
                                var caminhoArquivo = item.FileNames[i];
                                if (caminhoArquivo != null)
                                {
                                    if (caixaBaixa)
                                    {
                                        caminhoArquivo = caminhoArquivo.ToLower();
                                    }
                                    arquivos.Add(caminhoArquivo);
                                }
                            }
                            catch
                            {

                            }
                        }
                    }

                    if (arquivos.Count > 0)
                    {
                        retorno.Add((item, arquivos));
                    }

                    if (item.ProjectItems != null && item.ProjectItems.Count > 0)
                    {
                        retorno.AddRange(ProjetoUtil.RetornarProjectItemsArquivo(item.ProjectItems, caixaBaixa));
                    }
                }
            }
            return retorno;
        }



        internal static string RetornarChave(Project projeto)
        {
            return Path.GetFileName(projeto.FileName);
        }

        internal static ProjectItem RetornarProjetoItem(Project projeto, string nome)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return RetornarProjetoItem(projeto.ProjectItems, nome);
        }

        internal static ProjectItem RetornarProjetoItem(ProjectItems items, string nome)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (items != null)
            {
                foreach (ProjectItem item in items)
                {
                    var nomeItem = item.Name;
                    if (nomeItem == nome)
                    {
                        return item;
                    }
                    var itemFilho = RetornarProjetoItem(item.ProjectItems, nome);
                    if (itemFilho != null)
                    {
                        return itemFilho;
                    }
                }
            }
            return null;

        }




    }
}
