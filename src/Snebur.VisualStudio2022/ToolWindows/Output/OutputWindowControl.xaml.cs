//------------------------------------------------------------------------------
// <copyright file="JanelaSneburControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Snebur.VisualStudio
{
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Snebur.Publicacao;
    using Snebur.Utilidade;
    using Snebur.VisualStudio.Utilidade;

    /// <summary>
    /// Interaction logic for JanelaSneburControl.
    /// </summary>
    public partial class OutputWindowControl : UserControl
    {
        public ObservableCollection<ILogMensagemViewModel> Logs => LogVSUtil.Logs;
        public int PortaDepuracao => ConfiguracaoVSUtil.PortaDepuracao;

        public static OutputWindowControl Instacia { get; private set; }

        //public static DTE2 DTE { get; private set; }

        public OutputWindowControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.Loaded += this.Janela_Loaded;

            //JanelaSneburControl.DTE = Package.GetGlobalService(typeof(DTE)) as DTE2;
            //JanelaSneburControl.DTE.Events.DocumentEvents.DocumentOpened += this.DocumentEvents_DocumentOpened;
            //JanelaSneburControl.DTE.Events.SolutionEvents.Opened += this.SolutionEvents_Opened;
            //JanelaSneburControl.DTE.Events.SolutionItemsEvents.ItemAdded += SolutionItemsEvents_ItemAdded;

            Instacia = this;
        }

        public void Janela_Loaded(object sender, RoutedEventArgs e)
        {
            //LogVSUtil.Logs = this.Logs;
            this.TxtTitulo.Text = "Snebur v." + this.GetType().Assembly.GetName().Version.ToString();
            this.AtualizarEstadoServicoDepuracao();
            LogUtil.CriarEspacoSneburVisualizadorEventos();
        }

        private void BtnNormalizar_Click(object sender, RoutedEventArgs e)
        {
            _ = this.NormalizarProjetosReferenciasAsync();
        }

        public async Task NormalizarProjetosReferenciasAsync()
        {
            if (ConfiguracaoVSUtil.IsNormalizandoTodosProjetos)
            {
                return;
            }

            try
            {
                this.Logs.Clear();
                this.BtnNormalizar.IsEnabled = false;
                ConfiguracaoVSUtil.IsNormalizandoTodosProjetos = true;
                await this.NormalizarInternoAsync(false);
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }
            finally
            {
                this.BtnNormalizar.IsEnabled = true;
                ConfiguracaoVSUtil.IsNormalizandoTodosProjetos = false;
            }
        }

      
        private async Task NormalizarInternoAsync(bool compilar = false)
        {
            AjudanteAssembly.Clear();

            var isSucesso = false;
            var tempo = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                LogVSUtil.Clear();

                LogVSUtil.Log("Normalizando");
                AjudanteAssembly.Inicializar(true);

                //throw new Exception("Apenas Teste");
                var dte = await VSEx.GetDTEAsync();
                dte.ExecuteCommand("File.SaveAll");

                ProjetoUtil.DefinirProjetosInicializacao();


                var projetos = await ProjetoUtil.RetornarProjetosAsync();
                if (projetos.Count > 0)
                {
                    LogVSUtil.Log($"Total de projetos encontrados {projetos.Count}");

                    var projetosTypeScript = projetos.OfType<ProjetoTypeScript>().ToList();


                    var projetosDominio = projetos.OfType<ProjetoDominio>().OrderBy(x => x.ConfiguracaoProjeto.PrioridadeDominio).ToList();
                    foreach (var projeto in projetosDominio)
                    {
                        await projeto.NormalizarReferenciasAsync(true);
                    }

                    var projetosContextoDados = projetos.OfType<ProjetoContextoDados>().ToList();
                    foreach (var projeto in projetosContextoDados)
                    {
                       await projeto.NormalizarReferenciasAsync(true);
                    }

                    var projetosRegrasNegocioTS = projetos.OfType<ProjetoRegrasNegocioTypeScript>().ToList();
                    foreach (var projeto in projetosRegrasNegocioTS)
                    {
                       await  projeto.NormalizarReferenciasAsync(true);
                    }

                    var projetosRegrasNegocioCSharp = projetos.OfType<ProjetoRegrasNegocioCSharp>().ToList();
                    foreach (var projeto in projetosRegrasNegocioCSharp)
                    {
                      await   projeto.NormalizarReferenciasAsync(true);
                    }

                    var projetosServicosTS = projetos.OfType<ProjetoServicosTypescript>().ToList();
                    foreach (var projeto in projetosServicosTS)
                    {
                        await projeto.NormalizarReferenciasAsync(true);
                    }

                    var projetosServicosDotNet = projetos.OfType<ProjetoServicosDotNet>().ToList();
                    foreach (var projeto in projetosServicosDotNet)
                    {
                        await projeto.NormalizarReferenciasAsync(true);
                    }

                    foreach (var projeto in projetosTypeScript)
                    {
                        await projeto.NormalizarReferenciasAsync(compilar);
                    }

                    var projetosSass = projetos.OfType<ProjetoEstilo>().ToList();
                    foreach (var projeto in projetosSass)
                    {
                        await projeto.NormalizarReferenciasAsync(compilar);
                    }

                    //GerenciadorProjetos.Reiniciar();
                    foreach (var projeto in projetos)
                    {
                        //projeto.Dispose();
                    }
                    isSucesso = true;
                    ProjetoTypeScriptUtil.AtualizarScriptsDebug(projetos.OfType<ProjetoTypeScript>().ToList());
                }

            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }
            finally
            {
                GerenciadorProjetos.Instancia.TempoCompilacao?.Stop();
                if (isSucesso)
                {
                    tempo.Stop();
                    LogVSUtil.Sucesso("Normalização finalizada.", tempo);
                }
                else
                {
                    if (!compilar)
                    {
                        await this.NormalizarInternoAsync(true);
                    }
                }
            }
        }

        private void BtnReiniciarGerenciadorProjetosTS_Click(object sender, RoutedEventArgs e)
        {
            GerenciadorProjetos.Reiniciar();
        }

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            this.LimparLogs();
        }

        public void LimparLogs()
        {
            this.Logs.Clear();
        }

        private async void BtnIniciarPararServicoDepuracao_Click(object sender, RoutedEventArgs e)
        {
            this.BtnIniciarPararServicoDepuracao.IsEnabled = false;
            if (GerenciadorProjetos.EstadoServicoDepuracao == EnumEstadoServicoDepuracao.Ativo)
            {
                GerenciadorProjetos.Instancia.PararServicoDepuracao();
            }
            else
            {
                GerenciadorProjetos.Instancia.IniciarServicoDepuracao();
            }
            await System.Threading.Tasks.Task.Delay(3000);
            this.AtualizarEstadoServicoDepuracao();
        }


        private void AtualizarEstadoServicoDepuracao()
        {
            if (GerenciadorProjetos.EstadoServicoDepuracao == EnumEstadoServicoDepuracao.Ativo)
            {
                this.TxtPortaDepuracao.Text = $"Depuracao porta: {this.PortaDepuracao}";
                this.BtnIniciarPararServicoDepuracao.Content = "Parar depuracao";
            }
            else
            {
                this.TxtPortaDepuracao.Text = String.Empty;
                this.BtnIniciarPararServicoDepuracao.Content = "Iniciar depuracao";
            }
            this.BtnIniciarPararServicoDepuracao.IsEnabled = true;
        }

        private void Link_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement el && el.DataContext is LogMensagemViewModel logVM)
            {
                if (logVM.Acao != null)
                {
                    logVM.Acao.Invoke();
                }
            }
        }


        private void BtnFormatarStringFormat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogVSUtil.Logs?.Clear();
                this.FormatarNovoStringFormat();
            }
            catch (Exception erro)
            {
                LogVSUtil.LogErro(erro);
            }
            this.Dispatcher.VerifyAccess();
        }

        private void FormatarNovoStringFormat()
        {
            this.Dispatcher.VerifyAccess();

            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            var documento = dte.ActiveDocument;

            if (documento != null)
            {
                var nomeArquivo = documento.Name;
                var fi = new FileInfo(documento.FullName);

                if (FormatarDocumentoUtil.ExtensoesSuportadas.Contains(fi.Extension))
                {
                    if (documento.Selection is TextSelection selecao)
                    {
                        var posicao = selecao.TopPoint;
                        var posicaoLinha = selecao.TopPoint.Line;
                        var posicaoColuna = selecao.TopPoint.LineCharOffset;

                        selecao.SelectAll();

                        var conteudo = selecao.Text;
                        var isCsharp = fi.Extension.ToLower() == ".cs";

                        var objSubstituir = new SubstituicaoNovoStringFormatTS(conteudo, isCsharp);
                        var conteudoFormatado = objSubstituir.RetornarConteudo();

                        //selecao.SelectAll();
                        var totalLinhas = conteudoFormatado.TotalLinhas();
                        if ((totalLinhas - 1) < posicaoLinha)
                        {
                            posicaoLinha = totalLinhas - 1;
                        }
                        selecao.Delete();
                        selecao.Insert(conteudoFormatado);
                        selecao.Collapse();

                        selecao.MoveToLineAndOffset(posicaoLinha, posicaoColuna, true);
                        selecao.Collapse();


                        if (conteudoFormatado.Contains(SubstituicaoNovoStringFormatTS.PESQUISAR))
                        {
                            selecao.FindText(SubstituicaoNovoStringFormatTS.PESQUISAR);
                            selecao.SelectLine();
                        }
                    }
                }
            }
        }

        private async void BtnHtmlIntellisense_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                this.IsEnabled = false;
                await HtmlIntelliSense.InicializarAsync();
                LogVSUtil.Sucesso("IntelliSense inicializado, Reinicie o VisualStudio e configure a extensão .shtml para 'HTML (WebForms) Editor'", stopwatch);
            }
            catch(Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }
            finally
            {
                this.IsEnabled = true;
            }
        }
    }

}