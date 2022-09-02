using Snebur.Utilidade;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace Snebur.VisualStudio
{
    public class InstalarItensTemplate
    {
        private const string CONTROLE_USUARIO = "ControleUsuario";
        private const string DOCUMENTO_PRINCIPAL = "DocumentoPrincipal";
        private const string PAGINA = "Pagina";
        private const string JANELA = "Janela";
        private const string JANELA_CADASTRO = "JanelaCadastro";

        private const string CAMINHO_PARCIAL_ITENS_TEMPLATE = "Visual Studio 2022/Templates/ItemTemplates/Snebur/";

        public static Task InstalarAsync()
        {
            return Task.Factory.StartNew(Instalar,
                                        CancellationToken.None,
                                        TaskCreationOptions.None,
                                        TaskScheduler.Default);
        }

        private static void Instalar()
        {
            InstalarItensTemplate.SalvarItemTemplate(CONTROLE_USUARIO);
            InstalarItensTemplate.SalvarItemTemplate(CONTROLE_USUARIO + "-Scss");
            InstalarItensTemplate.SalvarItemTemplate(DOCUMENTO_PRINCIPAL);
            InstalarItensTemplate.SalvarItemTemplate(DOCUMENTO_PRINCIPAL + "-Scss");
            InstalarItensTemplate.SalvarItemTemplate(JANELA);
            InstalarItensTemplate.SalvarItemTemplate(JANELA + "-Scss");
            InstalarItensTemplate.SalvarItemTemplate(PAGINA);
            InstalarItensTemplate.SalvarItemTemplate(PAGINA + "-Scss");
            InstalarItensTemplate.SalvarItemTemplate(JANELA_CADASTRO);
            //InstalarItensTemplate.SalvarItemTemplate(JANELA_CADASTRO + "-Scss");
        }

        public static void SalvarItemTemplate(string caminhoRecurso)
        {
            var assemblyVisualStudio = typeof(InstalarItensTemplate).Assembly;
            var nomeArquivo = $"{caminhoRecurso}.zip";
            var caminhoMeusDocumentos = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (String.IsNullOrEmpty(caminhoMeusDocumentos))
            {
                LogVSUtil.LogErro("Não foi possivel acessar a pasta meus documentos, acesso não autoizado");
                return;
            }

            var caminhoCompletoRecurso = $"Snebur.VisualStudio.Resources.ItemTemplate.{nomeArquivo}";
            var streamResurco = assemblyVisualStudio.GetManifestResourceStream(caminhoCompletoRecurso);
            if (streamResurco == null)
            {

                LogVSUtil.LogErro($"O recurso embutido no assembly: '{caminhoCompletoRecurso}' não foi encontrado");
                return;
            }
            var caminhoDestino = Path.Combine(caminhoMeusDocumentos, CAMINHO_PARCIAL_ITENS_TEMPLATE, nomeArquivo);
            if (!File.Exists(caminhoDestino) ||
                streamResurco.Length != new FileInfo(caminhoDestino).Length)
            {
                using (var fsDestino = StreamUtil.CreateWrite(caminhoDestino))
                {
                    StreamUtil.SalvarStreamBufferizada(streamResurco, fsDestino);
                }
            }

        }
    }
}
