using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.Reflection;

namespace Snebur.VisualStudio
{
    public class EstruturaClasseAtributo : BaseEstruturaClasse
    {

        public EstruturaClasseAtributo(Type tipo) : base(tipo)
        {

        }

        protected override List<EstruturaPropriedade> RetornarEstruturasPropriedades(bool ignoratTipoBase)
        {
            var estruturasPropriedade = base.RetornarEstruturasPropriedades(ignoratTipoBase);
            var propriedadesEstaticas = AjudantePropriedades.RetornarPropriedadesClasseEstaticas(this.Tipo);
            foreach (var propriedade in propriedadesEstaticas)
            {
                estruturasPropriedade.Add(new EstruturaPropriedadeEstatica(propriedade));
            }

            foreach (var propriedade in propriedadesEstaticas)
            {
                var atributos = propriedade.GetCustomAttributes();
                if (atributos.Any(x => x.GetType().Name == AjudanteAssembly.NomeTipoAtributoMensagemValidacao))
                {
                    estruturasPropriedade.Add(new EstruturaPropriedadeMensagemValidacao(propriedade));
                }
            }

            return estruturasPropriedade;
        }


    }
}
