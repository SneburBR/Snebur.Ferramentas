using Community.VisualStudio.Toolkit;
using Snebur.Linq;
using Snebur.Utilidade;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            VS.Events.SolutionEvents.OnAfterOpenSolution += this.SolutionEvents_OnAfterOpenSolution;
            VS.Events.SolutionEvents.OnBeforeCloseSolution += this.SolutionEvents_OnBeforeCloseSolution;
            VS.Events.SolutionEvents.OnBeforeUnloadProject += this.SolutionEvents_OnBeforeUnloadProject;
            VS.Events.SolutionEvents.OnAfterLoadProject += this.SolutionEvents_OnAfterLoadProject;

            VS.Events.BuildEvents.ProjectBuildStarted += this.BuildEvents_ProjectBuildStarted;
            VS.Events.BuildEvents.ProjectBuildDone += this.BuildEvents_ProjectBuildDone;

            VS.Events.DocumentEvents.Opened += this.DocumentEvents_Opened;
            VS.Events.DocumentEvents.BeforeDocumentWindowShow += this.DocumentEvents_BeforeDocumentWindowShow;
            VS.Events.DocumentEvents.Saved += this.DocumentEvents_Saved;
        }

        private void SolutionEvents_OnBeforeUnloadProject(Project project)
        {
            _ = SolutionUtil.DefinirProjetosInicializacaoAsync();
        }

        private void SolutionEvents_OnAfterLoadProject(Project project)
        {
            _=SolutionUtil.DefinirProjetosInicializacaoAsync();
            
        }

        #region Projects Itens

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
            var arquivosControles = ArquivoControleUtil.RetornarArquivosControle(arquivos);

            var arquivosCodigoComum = arquivos.Where(x => x.Extension.Equals(EXTENSAO_TYPESCRIPT) && !ArquivoControleUtil.IsArquivoControle(x));

            var arquivosControlesNormalizados = this.RetornarArquivosControlesNormalizados(arquivosControles);
            foreach (var arquivo in arquivosControles)
            {
                if (arquivo.Exists)
                {
                    var projetoTS = await this.AgruparArquivoAsync(arquivo);
                    projetosTS.AddIsNotNull(projetoTS);
                }
            }

            foreach (var arquivo in arquivosCodigoComum)
            {
                if (arquivo.Exists)
                {
                    var projetoTS = await this.InserirTemplateAsync(arquivo);
                    projetosTS.AddIsNotNull(projetoTS);
                }
            }
            await this.NormalizarProjetosAsync(projetosTS);
        }

        private IEnumerable<FileInfo> RetornarArquivosControlesNormalizados(IEnumerable<FileInfo> arquivosControles)
        {
            if (arquivosControles.Count() > 1)
            {
                var retorno = new List<FileInfo>();
                var grupos = arquivosControles.GroupBy(x => ArquivoControleUtil.RetornarCaminhoShtml(x));
                foreach (var grupo in grupos)
                {
                    var arquivo = grupo.OrderBy(x => ArquivoControleUtil.RetornarOrdenacao(x)).First();
                    retorno.Add(arquivo);
                }
                return retorno;
            }
            return arquivosControles;
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


        #endregion

        #region Solutions

        private void SolutionEvents_OnAfterOpenSolution(Solution obj)
        {
            _ = this.SolucaoAbertaAsync();
        }

        private void SolutionEvents_OnBeforeCloseSolution()
        {
            this.DispensarProjetos();
            DiretorioInicializarUtil.ClearDiretorioProjetoTypescriptInicializacao();
        }

        #endregion

        #region Build Events
        public void AtivarEventosBuild()
        {
            this._isEventoBuildAtivo = true;
        }
        public void DesativarEventosBuild()
        {
            this._isEventoBuildAtivo = true;
        }
        private void BuildEvents_ProjectBuildStarted(Project obj)
        {
            if (this._isEventoBuildAtivo)
            {
                _ = this.CompilacaoIniciandoAsync(obj);
            }
        }
        private void BuildEvents_ProjectBuildDone(ProjectBuildDoneEventArgs obj)
        {
            if (this._isEventoBuildAtivo)
            {
                _ = this.BuildDoneAsync();
            }
        }
        private async Task CompilacaoIniciandoAsync(Project obj)
        {
            var tempoGeral = Stopwatch.StartNew();
            this.TtempoCompilacao?.Stop();
            this.TtempoCompilacao = Stopwatch.StartNew();

            //var projetoTS = await this.RetornarProjetoTSAsync(new FileInfo(obj.FullPath));
            //if(projetoTS!= null)
            //{
            //    await projetoTS?.NormalizarReferenciasAsync();
            //}

            await this.CompilacaoIniciandoAsync(tempoGeral);
        }

        #endregion

        #region Documents
        private void DocumentEvents_Saved(string file)
        {
            _ = this.DocumentEventsSavedAsync(file);
            _ = this.DocumentEventsOpenedAsync(file);
        }

        private void DocumentEvents_Opened(string file)
        {
            _ = this.DocumentEventsOpenedAsync(file);
        }

        private Task DocumentEventsOpenedAsync(string file)
        {
            return Task.CompletedTask;
        }

        private async Task DocumentEventsSavedAsync(string file)
        {
            if (File.Exists(file))
            {
                await this.MensagemArquivoAlteradoAsync(file);
            }

        }

        private void DocumentEvents_BeforeDocumentWindowShow(DocumentView documentoView)
        {
            var encoding = documentoView.Document?.Encoding;
            if (encoding != null &&
                encoding.EncodingName != Encoding.UTF8.EncodingName)
            {
                var caminho = CaminhoUtil.NormalizarCaminho(documentoView.FilePath, EnumTipoCaminho.CaminhoWindows);
                var reg = new Regex(@"[\\github\\clones\\|\\github\\forks\\]", RegexOptions.IgnoreCase);
                if (reg.IsMatch(caminho))
                {
                    return;
                }
                 
                LogVSUtil.Alerta($"Alterando encoding do arquivo {Path.GetFileName(documentoView.FilePath)} de {encoding?.EncodingName} para UTF-8");

                documentoView.Document.Encoding = Encoding.UTF8;
                documentoView.Document.Save();
            }
        }

        #endregion

    }
}
