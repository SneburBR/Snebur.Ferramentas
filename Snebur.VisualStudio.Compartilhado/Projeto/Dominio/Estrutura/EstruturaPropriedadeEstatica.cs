using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.Reflection;
using Snebur.VisualStudio.Utilidade;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    public class EstruturaPropriedadeEstatica : EstruturaPropriedade
    {
        public EstruturaPropriedadeEstatica(PropertyInfo propriedade) : base(propriedade)
        {

        }

        public override List<string> RetornarLinhasTypeScript(string tabInicial)
        {
            var linha = String.Format("{0}public static {1} : {2} = {3}; ", tabInicial, this.Propriedade.Name, this.CaminhoTipo, this.RetornarValorPropriedadeEstatica());
            return new List<string> { linha };
        }

        private string RetornarValorPropriedadeEstatica()
        {
            var valor = this.Propriedade.GetValue(null);
            if (valor == null)
            {
                return "null";
            }
            if (ReflexaoUtil.TipoRetornaTipoPrimario(valor.GetType()))
            {
                return this.RetornarValorTypeScript(valor);
            }
            throw new NotSupportedException("RetornarValorPropriedadeEstatica");
        }
    }
}
