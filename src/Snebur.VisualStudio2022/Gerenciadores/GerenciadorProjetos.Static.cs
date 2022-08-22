using EnvDTE;
using EnvDTE80;
using Snebur.Utilidade;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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

        public static bool IsLimparLogCompilando { get; set; } = true;

        public static Dictionary<string, ObservadorArquivoProjeto> DicionarioObservadoresArquivo { get; } = new Dictionary<string, ObservadorArquivoProjeto>();
        public static event EventHandler SoluacaoAberta;


        public static EnumEstadoServicoDepuracao EstadoServicoDepuracao => GerenciadorProjetos.Instancia?.ServicoDepuracao?.Estado ?? EnumEstadoServicoDepuracao.Parado;


        private static readonly object _bloqueio = new();
        public static GerenciadorProjetos _instancia;
        public static GerenciadorProjetos Instancia => ThreadUtil.RetornarValorComBloqueio(ref _instancia, () => new GerenciadorProjetos());
        //public static void Inicializar(SneburVisualStudio2022Package package)
        //{
        //    if (Instancia == null)
        //    {
        //        lock (_bloqueio)
        //        {
        //            if (Instancia == null)
        //            {
        //                Instancia = new GerenciadorProjetos(package);
        //                Instancia.
        //                Instancia.Inicializar();
        //                _ = Instancia.InializarServidoDepuracaoAsync();
        //            }
        //        }
        //    }
        //}

      

    }
}
