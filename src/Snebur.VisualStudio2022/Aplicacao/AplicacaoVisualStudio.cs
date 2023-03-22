using Community.VisualStudio.Toolkit;
using Snebur.Dominio;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Threading.Tasks;

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

            try
            {
                var projeto = await baseProjeto.GetProjectAsync();
                if (projeto != null)
                {
                    await VS.Build.BuildProjectAsync(projeto);
                }

            }
            catch (Exception erro)
            {
                throw new Exception(String.Format("Não foi possível compilar o projeto {0} ", baseProjeto.NomeProjeto), erro);
            }
        }

        protected override async Task<IEnumerable<string>> RetornarTodosArquivosProjetoAsync(object projetoVS,
                                                                                             string caminhoProjeto,
                                                                                             bool isLowerCase)
        {
            if (projetoVS is null)
            {
                throw new ArgumentNullException(nameof(projetoVS));
            }

            if (caminhoProjeto is null)
            {
                throw new ArgumentNullException(nameof(caminhoProjeto));
            }
             
            if(projetoVS is Project projeto)
            {
                return await SolutionUtil.RetornarTodosArquivosAsync(projeto, isLowerCase);
            }

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (projetoVS is EnvDTE.Project project)
            {
                throw new Exception("Tipo do projeto não suportado EnvDTE.Project");
                //return ProjetoUtil.RetornarTodosArquivos(project, isLowerCase);
            }
             
            throw new NotSupportedException($"{projetoVS.GetType()} não suportado");
        }
        

        public override bool CheckAccess()
        {
            return ThreadHelper.CheckAccess();
        }
    }
}
