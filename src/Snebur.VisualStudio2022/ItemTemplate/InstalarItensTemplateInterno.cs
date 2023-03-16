using Snebur.Utilidade;
using System.IO;

namespace Snebur.VisualStudio
{
    public partial class InstalarItensTemplate
    {
        private class InstalarItensTemplateInterno
        {
            private const string CONTROLE_USUARIO = "ControleUsuario";
            private const string DOCUMENTO_PRINCIPAL = "DocumentoPrincipal";
            private const string PAGINA = "Pagina";
            private const string JANELA = "Janela";
            private const string JANELA_CADASTRO = "JanelaCadastro";

            private const string CAMINHO_PARCIAL_ITENS_TEMPLATE = "Visual Studio 2022/Templates/ItemTemplates/Snebur/";

            private string DiretorioItensTemplate { get; }

            public InstalarItensTemplateInterno()
            {
                this.DiretorioItensTemplate = this.RetornarDiretorioItensTemplate();
            }

            public void Instalar()
            {
                if (this.DiretorioItensTemplate == null ||
                  !Directory.Exists(this.DiretorioItensTemplate))
                {
                    return;
                }

                this.SalvarItemTemplate(CONTROLE_USUARIO);
                this.SalvarItemTemplate(CONTROLE_USUARIO + "-Scss");
                this.SalvarItemTemplate(DOCUMENTO_PRINCIPAL);
                this.SalvarItemTemplate(DOCUMENTO_PRINCIPAL + "-Scss");
                this.SalvarItemTemplate(JANELA);
                this.SalvarItemTemplate(JANELA + "-Scss");
                this.SalvarItemTemplate(PAGINA);
                this.SalvarItemTemplate(PAGINA + "-Scss");
                this.SalvarItemTemplate(JANELA_CADASTRO);
            }

            private void SalvarItemTemplate(string caminhoRecurso)
            {
                var assemblyVisualStudio = typeof(InstalarItensTemplateInterno).Assembly;
                var nomeArquivo = $"{caminhoRecurso}.zip";

                var caminhoCompletoRecurso = $"Snebur.VisualStudio.Resources.ItemTemplate.{nomeArquivo}";
                var streamResurco = assemblyVisualStudio.GetManifestResourceStream(caminhoCompletoRecurso);
                if (streamResurco == null)
                {

                    LogVSUtil.LogErro($"O recurso embutido no assembly: '{caminhoCompletoRecurso}' não foi encontrado");
                    return;
                }

                var caminhoDestino = Path.Combine(this.DiretorioItensTemplate, nomeArquivo);
                if (!File.Exists(caminhoDestino) ||
                    streamResurco.Length != new FileInfo(caminhoDestino).Length)
                {
                    using (var fsDestino = StreamUtil.CreateWrite(caminhoDestino))
                    {
                        StreamUtil.SalvarStreamBufferizada(streamResurco, fsDestino);
                    }
                }

            }

            private string RetornarDiretorioItensTemplate()
            {
                var diretorio = this.RetornarDiretorioItensTemplateInterno();
                if (Directory.Exists(diretorio))
                {
                    diretorio = Path.Combine(diretorio, "Snebur");
                    DiretorioUtil.CriarDiretorio(diretorio);
                    return diretorio;
                }
                return null;
            }
            private string RetornarDiretorioItensTemplateInterno()
            {
                var diretorioItensTemplate = ConfiguracaoGeral.Instance.DiretorioItensTemplate;
                if (!String.IsNullOrWhiteSpace(diretorioItensTemplate))
                {
                    if (Directory.Exists(diretorioItensTemplate))
                    {
                        return diretorioItensTemplate;
                    }
                    LogVSUtil.LogErro($"Diretorio dos itens template não encontrado: {diretorioItensTemplate}");
                }


                var caminhoMeusDocumentos = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (String.IsNullOrEmpty(caminhoMeusDocumentos))
                {
                    LogVSUtil.LogErro("Não foi possivel acessar a pasta meus documentos, acesso não autoizado");
                    return null;
                }

                diretorioItensTemplate = Path.Combine(caminhoMeusDocumentos, CAMINHO_PARCIAL_ITENS_TEMPLATE);
                if (Directory.Exists(diretorioItensTemplate))
                {
                    return diretorioItensTemplate;
                }
                LogVSUtil.LogErro($"Diretorio dos itens template não encontrado: {diretorioItensTemplate}");
                return null;
            }
        }
    }

}
