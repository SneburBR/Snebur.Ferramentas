using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    public class InstalarItensTemplate
    {
        private const string CONTROLE_USUARIO_ITEM_TEMPLATE = "ControleUsuarioItemTemplate";
        private const string DOCUMENTO_PRINCIPAL_ITEM_TEMPLATE = "DocumentoPrincipalItemTemplate";
        private const string JANELA_ITEM_TEMPLATE = "JanelaItemTemplate";
        private const string PAGINA_ITEM_TEMPLATE = "PaginaItemTemplate";
        private const string CAMINHO_PARCIAL_ITENS_TEMPLATE = "Visual Studio 2019/Templates/ItemTemplates/Visual C#";

        public static void Instalar()
        {
            InstalarItensTemplate.SalvarItemTemplate(CONTROLE_USUARIO_ITEM_TEMPLATE);
            InstalarItensTemplate.SalvarItemTemplate(DOCUMENTO_PRINCIPAL_ITEM_TEMPLATE);
            InstalarItensTemplate.SalvarItemTemplate(JANELA_ITEM_TEMPLATE);
            InstalarItensTemplate.SalvarItemTemplate(PAGINA_ITEM_TEMPLATE);
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
            var caminhoDestino = Path.Combine(caminhoMeusDocumentos, CAMINHO_PARCIAL_ITENS_TEMPLATE, nomeArquivo);

            if (!File.Exists(caminhoDestino))
            {
                var caminhoCompletoRecurso = $"Snebur.VisualStudio.Recursos.ItemTemplate.{nomeArquivo}";
                var streamResurco = assemblyVisualStudio.GetManifestResourceStream(caminhoCompletoRecurso);
                if (streamResurco != null)
                {
                    using (var fsDestino = StreamUtil.CreateWrite(caminhoDestino))
                    {
                        StreamUtil.SalvarStreamBufferizada(streamResurco, fsDestino);
                    }
                }
            }
        }
    }
}
