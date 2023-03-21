﻿using EnvDTE;
using System.Diagnostics;
using System.IO;

namespace Snebur.VisualStudio
{
    public partial class GerenciadorProjetos
    {
        private void InicializarEventosDTE()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            this.SolutionEventsInterno.Opened += this.SolutionEvents_Opened;
            this.BuildEventsInterno.OnBuildBegin += this.BuildEvents_OnBuildBegin;
            this.BuildEventsInterno.OnBuildDone += this.BuildEvents_OnBuildDone;

            this.DocumentEventsInterno.DocumentOpened += this.DocumentEvents_DocumentOpened;
            this.DocumentEventsInterno.DocumentSaved += this.DocumentEvents_DocumentSaved;
            this.SolutionEventsInterno.BeforeClosing += this.SolutionEvents_BeforeClosing;
        }
   
        private void SolutionEvents_Opened()
        {
            _ = this.ExecutarAsync(this.SolucaoAbertaAsync);
        }

        private void BuildEvents_OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            //_ = this.ExecutarAsync(this.CompilacaoIniciandoAsync);
        }

        private void BuildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            if (this._isEventoBuildAtivo)
            {
                LogVSUtil.Log("Executando tarefas depois da compilação");
                _ = this.BuildDoneAsync();
            }
        }

        private void DocumentEvents_DocumentOpened(Document documento)
        {
            //_ = this.ExecutarAsync(this.DocumentoAbertoAsync, documento);
        }

        private void DocumentEvents_DocumentSaved(Document documento)
        {
            _ = DocumentSavedAsync(documento) ;
        }

        private async Task DocumentSavedAsync(Document documento)
        {
            //await this.ExecutarAsync(this.DocumentoSalvoAsync, documento);
            await this.ExecutarAsync(this.MensagemArquivoAlteradoAsync, documento);
        }
        private async Task DocumentoSalvoAsync(Document documento)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            this.InicializarGerenciadorSeNecessario();
            var nome = documento.Name;
            if (!String.IsNullOrWhiteSpace(nome))
            {
                var arquivo = new FileInfo(documento.FullName);
                if (arquivo.Exists)
                {
                    await this.ArquivoRenomeadoAsync(arquivo);
                }
            }
        }

        private async Task BuildDoneAsync()
        {
            var t = Stopwatch.StartNew();
            if (this.DiretorioProjetoTypescriptInicializacao == null)
            {
                ProjetoUtil.DefinirProjetosInicializacao();
            }
            var tempo = DateTime.Now - this.DataHoraUltimaVerificacao;
            if (tempo.TotalSeconds > 15)
            {
                if (await this.ExecutarAsync(this.CompilacaoConcluidaAsync))
                {
                    this.DataHoraUltimaVerificacao = DateTime.Now;
                }
            }
            else
            {
                this.NormalizarScripts();
            }
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            LogVSUtil.Sucesso("Tarefas depois da compilação", t);
        }

        private async Task SolucaoAbertaAsync()
        {
            await this.AnalisarNecessidadeServicoDepuracaoAsync();
        }

        public async Task AnalisarNecessidadeServicoDepuracaoAsync()
        {
            await this.AtualizarProjetosAsync();
            if (this.ProjetosTS.Count > 0)
            {
                await this.IniciarServicoDepuracaoAsync();
            }
            this.SoluacaoAbertaInterno?.Invoke(this, EventArgs.Empty);
        }

        private async Task CompilacaoIniciandoAsync(Stopwatch tempoAntesBuild)
        {
            if (ConfiguracaoVSUtil.IsNormalizandoTodosProjetos)
            {
                return;
            }

            this.IsCompilando = true;

            

            if (this.IsLimparLogCompilandoInterno)
            {
                LogVSUtil.Clear();
            }

            try
            {
           
                await this.ServicoDepuracao.SalvarPortaAsync();

                tempoAntesBuild.Stop();
                LogVSUtil.Sucesso($"Processos antes de compilar", tempoAntesBuild);
            }
            catch (FileNotFoundException ex)
            {
                LogVSUtil.LogErro(ex);
                await this.ReiniciarAsync();
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }
        }

        private async Task CompilacaoConcluidaAsync()
        {
            this.NormalizarScripts();
            //this.GerarScriptsMinificados();
            //this.GerarScriptsProtegidos();


            if (this.TempoCompilacao != null)
            {
                //this.TempoCompilacao.Stop();
                LogVSUtil.Sucesso("Compilação finalizada", this.TempoCompilacao);
            }

            this.IsCompilando = false;
            //this.NotificarArquivosAlteradoPendentes();
        }

        private void NormalizarScripts()
        {
            foreach (var projetoTS in this.ProjetosTS.Values)
            {
                using (var normalizarCompilacao = new NormalizarCompilacaoJavascript(projetoTS))
                {
                    normalizarCompilacao.Normalizar();
                }
            }
        }
    }
}
