using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.Reflection;
using Snebur.VisualStudio.Utilidade;

namespace Snebur.VisualStudio.Projeto.Dominio.Estrutura
{
    public class EstruturaParametroConstrutor  :BaseEstrutura
    {

        public string NomeParametro { get; set; }

        public string Opcional { get; set; }

        public EstruturaParametroConstrutor(ParameterInfo parametro):base(parametro.ParameterType)
        {
            this.NomeParametro = parametro.Name;

            if (parametro.GetCustomAttributes().Any(x=> x.GetType().Name== AjudanteAssembly.NomeTipoParametroOpcionalTS))
            {
                this.Opcional = "?";
            }
        }

        public string RetornarTypeScript()
        {
            return String.Format(" {0}{1} : {2} ", this.NomeParametro, this.Opcional, this.CaminhoTipo);
        }

    }
}
