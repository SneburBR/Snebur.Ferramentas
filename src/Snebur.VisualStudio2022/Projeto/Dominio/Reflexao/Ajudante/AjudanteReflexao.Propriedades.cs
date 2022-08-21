using System.Reflection;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public partial class AjudanteReflexao
    {
        public static string RetornarDeclaracaoPropriedade(PropertyInfo propriedade)
        {
            var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(propriedade.DeclaringType);
            caminhoTipo = caminhoTipo.Replace(".", "_");
            return $"__$propriedade_{caminhoTipo}_{propriedade.Name}";
        }
    }
}