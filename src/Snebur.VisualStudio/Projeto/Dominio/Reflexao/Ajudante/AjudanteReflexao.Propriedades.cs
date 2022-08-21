using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.VisualStudio.Reflexao;
using System.Reflection;

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public partial class AjudanteReflexao
    {
        public static string RetornarDeclaracaoPropriedade(PropertyInfo propriedade)
        {
            var caminhoTipo = TipoUtil.RetornarCaminhoTipo(propriedade.DeclaringType);
            caminhoTipo = caminhoTipo.Replace(".", "_");
            return String.Format("__$propriedade_{0}_{1}", caminhoTipo, propriedade.Name);
        }
    }
}
