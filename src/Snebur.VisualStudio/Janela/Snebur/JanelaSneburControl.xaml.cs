using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Snebur.VisualStudio.Projeto;
using Snebur.VisualStudio.Projeto.TypeScript;
using Snebur.VisualStudio.Utilidade;

namespace Snebur.VisualStudio
{
    
    public partial class JanelaSneburControl : UserControl
    {
        public ObservableCollection<LogMensagemViewModel> Logs { get; set; } = new ObservableCollection<LogMensagemViewModel>();

        public JanelaSneburControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.Loaded += this.Janela_Loaded;
        }
        
        public void Janela_Loaded(object sender, RoutedEventArgs e)
        {
            Repositorio.BrushWindowText = CorUtil.RetornarBrushWindowsText();

            LogMensagemUtil.Logs = this.Logs;
            LogMensagemUtil.ScrollLog = this.ScrollLog;
            TxtTitulo.Text = "Snebur.Framework - por Rubens Cordeiro v." + this.GetType().Assembly.GetName().Version.ToString();

            try
            {

                HtmlIntellisense.Inicializar();
            }
            catch (Exception ex)
            {
                LogMensagemUtil.LogErro(ex);
            }
        }

        //[SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void BtnAtualizar_Click(object sender, RoutedEventArgs e)
        {

            this.Logs.Clear();
            this.BtnAtualizar.IsEnabled = false;

            System.Threading.Tasks.Task.Factory.StartNew(Atualizar).ContinueWith((x) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.BtnAtualizar.IsEnabled = true;
                });
            });
        }

        private void Atualizar()
        {
            try
            {
                AjudanteAssembly.Inicializar();

                //throw new Exception("Apenas Teste");

                var projetos = ProjetoUtil.RetornarProjetos();
                if (projetos.Count > 0)
                {

                    var projetosDominio = projetos.OfType<ProjetoDominio>().OrderBy(x => x.ConfiguracaoDominio.PrioridadeDominio).ToList();
                    foreach (var projeto in projetosDominio)
                    {
                        projeto.Configurar();
                    }

                    var projetosTypeScript = projetos.OfType<ProjetoTypeScript>().ToList();
                    foreach (var projeto in projetosTypeScript)
                    {
                        projeto.Configurar();
                    }

                    var projetosContextoDados = projetos.OfType<ProjetoContextoDados>().ToList();
                    foreach (var projeto in projetosContextoDados)
                    {
                        projeto.Configurar();
                    }

                    var projetosSass = projetos.OfType<ProjetoSass>().ToList();
                    foreach (var projeto in projetosSass)
                    {
                        projeto.Configurar();
                    }

                    LogMensagemUtil.Log("Atualização realizada com sucesso.");
                }

            }
            catch (Exception ex)
            {
                LogMensagemUtil.LogErro(ex);
            }
        }

    }
}