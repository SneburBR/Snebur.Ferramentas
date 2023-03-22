using Community.VisualStudio.Toolkit;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{
    public static class BaseProjetoExtensao
    {
        public static Task<Project> GetProjectAsync(this BaseProjeto baseProjeto)
        {
            return VS.Solutions.FindProjectsAsync(baseProjeto.NomeProjeto);
            //if(baseProjeto.ProjetoViewModel.ProjetoVS is Project project)
            //{
            //    return project;
            //}
            throw new Exception($"  baseProjeto.ProjetoViewModel.ProjetoVS '{baseProjeto.ProjetoViewModel.ProjetoVS}'  não suportado");
        }
    }

}