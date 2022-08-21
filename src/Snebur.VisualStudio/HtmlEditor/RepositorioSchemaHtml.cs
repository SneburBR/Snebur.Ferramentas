using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using System.IO;

namespace Snebur.VisualStudio
{
    public class RepositorioSchemaHtml
    {
        // <!--inicio sn-controle-->
        // <!--fim sn-controle-->

        // name="flowContent"
        // name="commonAttributeGroup"

        //C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\Packages\Schemas\html

        //Visual Studio 2015
        //public const string REPOSITORIO_SCHEMA_HTML = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Packages\schemas\html";
        public const string REPOSITORIO_SCHEMA_HTML = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\Packages\Schemas\html";
        public const string ARQUIVO_HTML_5 = "html_5.xsd";
        public const string ARQUIVO_X_HTML_5 = "xhtml_5.xsd";

        //C:\Projetos\Snebur\Snebur.Framework\Snebur.UI.Typescript\TypeScripts\Snebur.UI\Atributo\AtributosHtml.Statica.ts
        private static string[] CaminhosAtributosSnebur = { @"C:\Projetos\Snebur\Snebur.Framework\Snebur.UI.Typescript\TypeScripts\Snebur.UI\Atributo\AtributosHtml.Statica.ts",
                                                              @"C:\Projetos\Dev\Snebur.Framework\Snebur.UI.Typescript\TypeScripts\Snebur.UI\Atributo\AtributosHtml.Statica.ts" };

        //C:\Projetos\Snebur\Snebur.Framework\Snebur.UI.Typescript\TypeScripts\Snebur.UI\Controle\ElementoControle\ElementoControle.Referencias.ts
        private static string[] CaminhosControlesSnebur = { @"E:\Projetos\TFS\Snebur\Snebur.Framework\Snebur.UI.Typescript\TypeScripts\Snebur.UI\Controle\ElementoControle\ElementoControle.Referencias.ts"
                                                              /*@"E:\Projetos\Dev\Snebur.Framework\Snebur.UI.Typescript\TypeScripts\Snebur.UI\Controle\ElementoControle\ElementoControle.Referencias.ts"*/ };

        public const string ENCONTRAR = "<xsd:complexType mixed=\"true\" name=\"simpleFlowContentElement\">";

        public static string CaminhoSchemaHTML5
        {
            get
            {
                return Path.Combine(REPOSITORIO_SCHEMA_HTML, ARQUIVO_HTML_5);
            }
        }

        public static string CaminhoSchemaXHTML5
        {
            get
            {
                return Path.Combine(REPOSITORIO_SCHEMA_HTML, ARQUIVO_X_HTML_5);
            }
        }

        public static string CaminhoAtributosSnebur
        {
            get
            {
                foreach(var caminho in RepositorioSchemaHtml.CaminhosAtributosSnebur)
                {
                    if (File.Exists(caminho))
                    {
                        return caminho;
                    }
                }
                return RepositorioSchemaHtml.CaminhosAtributosSnebur.First();
            }
        }

        public static string CaminhoControlesSnebur
        {
            get
            {
                foreach (var caminho in RepositorioSchemaHtml.CaminhosControlesSnebur)
                {
                    if (File.Exists(caminho))
                    {
                        return caminho;
                    }
                }
                return RepositorioSchemaHtml.CaminhosControlesSnebur.First();
            }
        }


    }
}
