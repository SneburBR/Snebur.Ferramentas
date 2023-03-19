using Snebur.Dominio;
using System.Collections.Generic;

namespace Snebur.VisualStudio
{
    internal class AplicacaoVisualStudio : BaseAplicacaoVisualStudio
    {
        public override EnumTipoAplicacao TipoAplicacao => EnumTipoAplicacao.ExtensaoVisualStudio;
 
        public AplicacaoVisualStudio()
        {
        }

        protected override IConfiguracaoGeral RetornarConfiguracaoGeral()
        {
            return VisualStudio.ConfiguracaoGeral.Instance;
        }

        protected override ILogVS RetornarLogVS()
        {
            return VisualStudio.LogVS.Instance;
        }

        protected override IGerenciadorProjetos RetornarGerenciadorProjetos()
        {
            return VisualStudio.GerenciadorProjetos.Instancia;
        }

        protected override async Task CompilarProjetoAsync(BaseProjeto baseProjeto)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var projetos = new List<BaseProjeto>();
            var dte = await VSEx.GetDTEAsync();
            try
            {
                dte.Solution.SolutionBuild.BuildProject("Debug", 
                                                        baseProjeto.UniqueName, 
                                                        true);
            }
            catch (Exception erro)
            {
                throw new Exception(String.Format("Não foi possível compilar o projeto {0} ", baseProjeto.NomeProjeto), erro);
            }
        }

        protected override IEnumerable<string> RetornarTodosArquivosProjeto(object projetoVS, string caminhoProjeto, bool isLowerCase)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();

            if (projetoVS is Project project)
            {
                return ProjetoUtil.RetornarTodosArquivos(project, isLowerCase);
            }
            throw new NotSupportedException();
        }
    }
}
