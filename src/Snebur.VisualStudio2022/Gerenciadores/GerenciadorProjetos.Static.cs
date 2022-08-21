using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Snebur.VisualStudio.Utilidade;

namespace Snebur.VisualStudio
{
    public class ObservadorArquivoProjeto : IDisposable
    {
        public string IdentificadorProjeto { get; }

        private HashSet<string> CaminhosObservando { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private List<FileSystemWatcher> Observadores { get; } = new List<FileSystemWatcher>();

        public ObservadorArquivoProjeto(string identificadorProjeto)
        {
            this.IdentificadorProjeto = identificadorProjeto;
        }

        public bool IsObservandoDiretorioFiltro(string caminho, string filtro)
        {
            var caminhoFiltro = String.Concat(caminho, filtro);
            return this.CaminhosObservando.Contains(caminhoFiltro);
        }
        public void AdicionarNovoObservador(FileSystemWatcher observador)
        {
            if (this.IsObservandoDiretorioFiltro(observador.Path, observador.Filter))
            {
                LogVSUtil.LogErro($"O arquivo já está sendo observado {observador.Path}");
            }
            else
            {
                this.Observadores.Add(observador);
                this.CaminhosObservando.Add(String.Concat(observador.Path, observador.Filter));
            }
        }

        public void Dispose()
        {
            LogVSUtil.Log($"Dispensando obserdador de arquivos do projeto {this.IdentificadorProjeto}");
            foreach (var observador in this.Observadores)
            {
                observador.Dispose();
            }
            this.Observadores.Clear();
        }
    }
    public partial class GerenciadorProjetos
    {
        public static DTE2 DTE_GLOBAL { get; private set; }
        public static SolutionEvents SolutionEvents { get; private set; }
        public static DocumentEvents DocumentEvents { get; private set; }
        public static BuildEvents BuildEvents { get; private set; }

        //public static ProjectItemsEvents SolutionItemsEvents { get; private set; }
        //public static DebuggerEvents DebuggerEvents { get; private set; }
        //public static CommandEvents CommandEvents { get; set; }
        //public static FindEvents FindEvents { get; private set; }
        //public static ProjectItemsEvents MiscFilesEvents { get; private set; }
        //public static DTEEvents DTEEvents { get; private set; }

        public static bool IsLimparLogCompilando { get; set; } = true;

        public static Dictionary<string, ObservadorArquivoProjeto> DicionarioObservadoresArquivo { get; } = new Dictionary<string, ObservadorArquivoProjeto>();
        public static event EventHandler SoluacaoAberta;
        public static GerenciadorProjetos Instancia { get; private set; }

        public static EnumEstadoServicoDepuracao EstadoServicoDepuracao => GerenciadorProjetos.Instancia?.ServicoDepuracao?.Estado ?? EnumEstadoServicoDepuracao.Parado;

        //public static string DiretorioSolucacao { get; set; }
        public static string DiretorioProjetoTypescriptInicializacao { get; set; }
        public static ConfiguracaoProjetoTypeScript ConfiguracaoProjetoTypesriptInicializacao { get; set; }

        public static void InicializarAsync(SneburVisualStudio2022Package package)
        {
            if (Instancia == null)
            {
                Instancia = new GerenciadorProjetos(package);
                Instancia.SoluacaoAbertaInterno += GerenciadorProjetos.GerenciadorProjetos_SoluacaoAberta;
                Instancia.Inicializar();
            }
            Instancia.Inivializar();
        }

        private void Inivializar()
        {
            _ = Task.Factory.StartNew(() =>
              {
                  this.ServicoDepuracao = new ServicoDepuracao();
                  this.ServicoDepuracao.EventoLog += this.ServicoDepuracao_Log;
              },
             CancellationToken.None,
             TaskCreationOptions.None,
             TaskScheduler.Default);
        }

        private static void GerenciadorProjetos_SoluacaoAberta(object sender, EventArgs e)
        {
            GerenciadorProjetos.SoluacaoAberta?.Invoke(sender, e);
        }

        private static void InicializarPropriedadesGlobal()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            GerenciadorProjetos.DTE_GLOBAL = dte;
            GerenciadorProjetos.SolutionEvents = dte.Events.SolutionEvents;
            GerenciadorProjetos.DocumentEvents = dte.Events.DocumentEvents;
            GerenciadorProjetos.BuildEvents = dte.Events.BuildEvents;

            //GerenciadorProjetos.SolutionItemsEvents = dte.Events.SolutionItemsEvents;
            //GerenciadorProjetos.DebuggerEvents = dte.Events.DebuggerEvents;
            //GerenciadorProjetos.CommandEvents = dte.Events.CommandEvents;
            //GerenciadorProjetos.FindEvents = dte.Events.FindEvents;
            //GerenciadorProjetos.MiscFilesEvents = dte.Events.MiscFilesEvents;
            //GerenciadorProjetos.DTEEvents = dte.Events.DTEEvents;
        }


        public static void ReiniciarAsync()
        {
            GerenciadorProjetos.Instancia.ReiniciarServidorReiniciarInterno();
        }



        //internal static void IniciarServicoDepuracao()
        //{
        //    GerenciadorProjetos.Instancia.IniciarServicoDepuracaoInterno();
        //}

        //internal static void PararServicoDepuracao()
        //{
        //    GerenciadorProjetos.Instancia.PararServicoDepuracaoInterno();
        //}


    }
}
