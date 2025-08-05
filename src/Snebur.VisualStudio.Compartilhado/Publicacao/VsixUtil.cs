using System;
using System.IO;

namespace Snebur.VisualStudio
{
    public static class VsixUtil
    {

        public static void IncrementarVersaoExtensaoVisualStudio(string caminhoProjeto)
        {
            var caminhoVsix = Path.Combine(caminhoProjeto, "source.extension.vsixmanifest");
            try
            {
                var xml = new System.Xml.XmlDocument();
                xml.Load(caminhoVsix);

                var IdentityTag = xml.GetElementsByTagName("Identity")[0];
                var versaoString = IdentityTag.Attributes["Version"].Value;
                if (Version.TryParse(versaoString, out var versao))
                {
                    var agora = DateTime.Now;

                    var ano = Int32.Parse(DateTime.Now.Year.ToString().Substring(2, 2));
                    //var versaoData = Convert.ToInt32($"{DateTime.Now.Month:00}{DateTime.Now.Day:00}");
                    var novaVersao = new Version(ano, agora.Month, agora.Day, versao.Revision + 1);
                    IdentityTag.Attributes["Version"].Value = novaVersao.ToString();
                    xml.Save(caminhoVsix);
                }

            }
            catch (Exception erro)
            {
                LogVSUtil.LogErro(erro);
            }
        }
    }
}
        
 