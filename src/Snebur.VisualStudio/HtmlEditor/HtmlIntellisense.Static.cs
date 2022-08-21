using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;


namespace Snebur.VisualStudio
{
    public partial class HtmlIntellisense
    {
        public static bool Inicializado = false;

        public static void Inicializar()
        {
            if (!HtmlIntellisense.Inicializado)
            {
                HtmlIntellisense.Inicializado = true;
                AjudanteAssembly.Inicializar();
                using (var html = new HtmlIntellisense())
                {
                    
                    html.Atualizar();
                }
            }
        }

    }
}
