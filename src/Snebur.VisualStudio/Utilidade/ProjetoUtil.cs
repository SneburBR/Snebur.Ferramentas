using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using EnvDTE;
using EnvDTE80;
using EnvDTE100;
using Microsoft.VisualStudio.Shell;
using Snebur.VisualStudio.Projeto;
using System.IO;

namespace Snebur.VisualStudio.Utilidade
{
    public class ProjetoUtil
    {
        public const string CONFIGURACAO_DOMINIO = "dominio.json";
        public const string CONFIGURACAO_TYPESCRIPT = "tsconfig.json";
        public const string CONFIGURACAO_CONTEXTO_DADOS = "contextodados.json";
        public const string CONFIGURACAO_SASS = "compilerconfig.json";

        //ProjectKinds.vsProjectKindSolutionFolder
        public const string VS_PROJECT_KIND_SOLUTION_FOLDER = "{5e4e30c6-2d26-46db-a0f0-226bc5edb5aa}"; // ProjectKinds.vsProjectKindSolutionFolder;

        public static List<BaseProjeto> RetornarProjetos()
        {

            var projetos = new List<BaseProjeto>();
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;

            if (dte.Solution.Count > 0)
            {

                var projetosVS = ProjetoUtil.RetornarProjetosVisualStudio();

                //var UIH = (EnvDTE.UIHierarchy)dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Object;

                foreach (var projetoVS in projetosVS)
                {

                    var nomeArquivoProjeto = projetoVS.FileName;
                    if (!String.IsNullOrWhiteSpace(nomeArquivoProjeto) && (File.Exists(Path.GetFullPath(projetoVS.FileName))))
                    {
                        var caminhoProjeto = new FileInfo(projetoVS.FileName).Directory.FullName;
                        if (!Directory.Exists(caminhoProjeto))
                        {
                            throw new System.IO.DirectoryNotFoundException(String.Format("O caminho do projeto não foi encontradio {0}", caminhoProjeto));
                        }

                        var caminhoConfiguracaoDominio = Path.Combine(caminhoProjeto, CONFIGURACAO_DOMINIO);
                        var caminhoConfiguracaoTypeScript = Path.Combine(caminhoProjeto, CONFIGURACAO_TYPESCRIPT);
                        var caminhoConfiguracaoContextoDados = Path.Combine(caminhoProjeto, CONFIGURACAO_CONTEXTO_DADOS);
                        var caminhoConfiguracaoSass = Path.Combine(caminhoProjeto, CONFIGURACAO_SASS);

                        //Dominio
                        if (File.Exists(caminhoConfiguracaoDominio) && File.Exists(caminhoConfiguracaoTypeScript))
                        {
                            throw new NotSupportedException(string.Format("Não é suportado no mesmo projetos arquivos de configuração de dominio.json e tsconfig.json juntos: Projeto {0}", projetoVS.Name));
                        }

                        //TypeScript
                        if (File.Exists(caminhoConfiguracaoDominio))
                        {
                            LogMensagemUtil.Log("Compilando o projeto {0}", projetoVS.Name);
                            ProjetoUtil.CompilarProjeto(dte, projetoVS);

                            projetos.Add(new ProjetoDominio(projetoVS, caminhoProjeto, caminhoConfiguracaoDominio));

                        }

                        if (File.Exists(caminhoConfiguracaoTypeScript))
                        {
                            LogMensagemUtil.Log(String.Format("Projeto TypeScript encontrado : {0} ", projetoVS.Name));
                            projetos.Add(TypeScriptProjetoUtil.RetornarProjetoTypeScript(projetoVS, caminhoProjeto, caminhoConfiguracaoTypeScript));
                        }


                        //ContextoDados
                        if (File.Exists(caminhoConfiguracaoContextoDados))
                        {
                            LogMensagemUtil.Log(String.Format("Projeto ContextoDados encontrado : {0} ", projetoVS.Name));
                            projetos.Add(new ProjetoContextoDados(projetoVS, caminhoProjeto, caminhoConfiguracaoContextoDados));

                        }

                        //Sass
                        if (File.Exists(caminhoConfiguracaoSass))
                        {
                            LogMensagemUtil.Log(String.Format("Projeto sass encontrado : {0} ", projetoVS.Name));
                            projetos.Add(new ProjetoSass(projetoVS, caminhoProjeto, caminhoConfiguracaoSass));
                        }
                    }
                }


            }
            else
            {
                LogMensagemUtil.Log("Nenhum projeto encontrado");
            }
            return projetos;

        }




        public static List<Project> RetornarProjetosVisualStudio()
        {
            var projetos = new List<Project>();
            var ide = Package.GetGlobalService(typeof(DTE)) as DTE2;
            var item = ide.Solution.Projects.GetEnumerator();
            while (item.MoveNext())
            {
                var itemSolucao = item.Current as Project;

                if (itemSolucao != null)
                {
                    //if (itemSolucao.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                    
                    if (itemSolucao.Kind ==  VS_PROJECT_KIND_SOLUTION_FOLDER)
                    {
                        projetos.AddRange(ProjetoUtil.RetornarProjetosDaPasta(itemSolucao));
                    }
                    else
                    {
                        projetos.Add(itemSolucao);
                    }
                }
            }
            return projetos;
        }

        private static IEnumerable<Project> RetornarProjetosDaPasta(Project pastaSolucao)
        {
            var projetos = new List<Project>();
            for (var i = 1; i <= pastaSolucao.ProjectItems.Count; i++)
            {
                var subProjeto = pastaSolucao.ProjectItems.Item(i).SubProject;
                if (subProjeto != null)
                {
                    if (subProjeto.Kind == VS_PROJECT_KIND_SOLUTION_FOLDER)
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
            LogMensagemUtil.Log("Compilandos os projetos");
            try
            {
                dte.Solution.SolutionBuild.Build(true);
                return true;
            }
            catch (Exception erro)
            {
                throw new Exception("Não foi possivel compilar a solução ", erro);
            }
        }

        public static bool CompilarProjeto(DTE2 dte, Project projetoVS)
        {
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

    }
}
