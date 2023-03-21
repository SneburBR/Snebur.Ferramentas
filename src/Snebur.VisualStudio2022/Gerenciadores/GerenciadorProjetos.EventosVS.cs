using Community.VisualStudio.Toolkit;
using Snebur.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static Snebur.VisualStudio.ConstantesProjeto;

namespace Snebur.VisualStudio
{
    public partial class GerenciadorProjetos
    {
        private bool _isEventoBuildAtivo = true;

        private void InicializarEventosVsCommunity()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            VS.Events.ProjectItemsEvents.AfterRenameProjectItems += this.ProjectItemsEvents_AfterRenameProjectItems;
            VS.Events.ProjectItemsEvents.AfterRemoveProjectItems += this.ProjectItemsEvents_AfterRemoveProjectItems;
            VS.Events.ProjectItemsEvents.AfterAddProjectItems += this.ProjectItemsEvents_AfterAddProjectItems;
            VS.Events.BuildEvents.ProjectBuildStarted += this.BuildEvents_ProjectBuildStarted;
        }

        public void AtivarEventosBuild()
        {
            this._isEventoBuildAtivo = true;
        }
        public void DesativarEventosBuild()
        {
            this._isEventoBuildAtivo = true;
        }
        private void BuildEvents_ProjectBuildStarted(ProjectTK obj)
        {
            if (this._isEventoBuildAtivo)
            {
                _ = this.CompilacaoIniciandoAsync(obj);
            }
        }

        private async Task CompilacaoIniciandoAsync(ProjectTK obj)
        {
            var tempoGeral = Stopwatch.StartNew();
            this.TempoCompilacao?.Stop();
            this.TempoCompilacao = Stopwatch.StartNew();
            var projetoTS = await this.RetornarProjetoTSAsync(new FileInfo(obj.FullPath));
            if(projetoTS!= null)
            {
                await projetoTS?.NormalizarReferenciasAsync();
            }
            await this.CompilacaoIniciandoAsync(tempoGeral);
        }

        private void ProjectItemsEvents_AfterRenameProjectItems(AfterRenameProjectItemEventArgs obj)
        {
            var itens = obj.ProjectItemRenames.Select(x => x.SolutionItem);
            _ = this.ItensRenomeadosAsync(itens);
        }

        private void ProjectItemsEvents_AfterRemoveProjectItems(AfterRemoveProjectItemEventArgs obj)
        {
            if (this._isRemovendoArquivo)
            {
                return;
            }
            _ = this.ArquivoRemovidosAsync(obj.ProjectItemRemoves);
        }

        private void ProjectItemsEvents_AfterAddProjectItems(IEnumerable<SolutionItem> itens)
        {
            if (this._isAdicionarArquivo)
            {
                return;
            }
            _ = this.ItensAdicionadoAsync(itens);
        }

        private async Task ArquivoRemovidosAsync(ProjectItemRemoveDetails[] projectItemRemoves)
        {
            var projetos = projectItemRemoves.Select(x => x.Project).Distinct(); ;
            foreach (var projeto in projetos)
            {
                if (projeto == null)
                {
                    continue;
                }
                var chave = BaseProjeto.RetornarChave(projeto.FullPath);
                if (this.ProjetosTS.TryGetValue(chave, out var projetoTS))
                {
                    await projetoTS.NormalizarReferenciasAsync();
                }
            }
        }

        private async Task ItensRenomeadosAsync(IEnumerable<SolutionItem> itens)
        {
            if (itens.Count() > 10)
            {
                await this.AtualizarProjetosAsync();
                return;
            }

            var projetosTS = new HashSet<ProjetoTypeScript>();
            foreach (var item in itens)
            {
                if (item == null)
                {
                    continue;
                }

                var arquivo = new FileInfo(item.FullPath);
                if (arquivo.Exists)
                {
                    var projetoTS = await this.ArquivoRenomeadoAsync(arquivo);
                    projetosTS.AddIsNotNull(projetoTS);
                }
            }
            await this.NormalizarProjetosAsync(projetosTS);
        }

        private async Task ItensAdicionadoAsync(IEnumerable<SolutionItem> itens)
        {

            if (itens.Count() > 10)
            {
                await this.AtualizarProjetosAsync();
                return;
            }

            var projetosTS = new HashSet<ProjetoTypeScript>();

            var arquivos = itens.Select(x => new FileInfo(x.FullPath));
            var arquivosLayout = arquivos.Where(x => x.Extension.Equals(EXTENSAO_CONTROLE_SHTML, StringComparison.InvariantCultureIgnoreCase));
            var arquivosCodigo = arquivos.Where(x => x.Extension.Equals(EXTENSAO_TYPESCRIPT) && !this.ExtensoesControle.Any(e => x.FullName.EndsWith(e)));

            foreach (var arquivo in arquivosLayout)
            {
                if (arquivo.Exists)
                {
                    var projetoTS = await this.AgruparArquivoAsync(arquivo);
                    projetosTS.AddIsNotNull(projetoTS);
                }
            }

            foreach (var arquivo in arquivosCodigo)
            {
                if (arquivo.Exists)
                {
                    var projetoTS = await this.InserirTemplateAsync(arquivo);
                    projetosTS.AddIsNotNull(projetoTS);
                }
            }
            await this.NormalizarProjetosAsync(projetosTS);
        }

        private async Task NormalizarProjetosAsync(HashSet<ProjetoTypeScript> projetosTS)
        {
            if (projetosTS.Count > 0)
            {
                await OutputWindow.OcuparAsync();
                foreach (var projetoTS in projetosTS)
                {
                    await projetoTS.NormalizarReferenciasAsync();
                }
                await OutputWindow.DesocuparAsync();
            }
        }



    }
}
