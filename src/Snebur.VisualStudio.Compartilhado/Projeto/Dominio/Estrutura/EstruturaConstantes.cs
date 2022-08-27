using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using System.Reflection;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public class EstruturaConstantes : BaseEstrutura
    {
        public EstruturaConstantes(Type tipo) : base(tipo)
        {
            
        }

        public List<string> RetornarLinhasTypeScriptInterface()
        {

            var linhas = new List<string>();
            linhas.Add(String.Format("export class {0}", this.NomeTipo));
            linhas.Add("{");

            var constantes = ReflexaoUtil.RetornarConstantes(this.Tipo);
            constantes = constantes.Where(x => !x.GetCustomAttributes().Any(k => k.GetType().Name == AjudanteAssembly.NomeTipoIgnorarConstanteTS)).ToList();
            foreach (var constante in constantes)
            {
                var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(constante.FieldType);
                var valor = this.RetornarValor(constante);
                if(valor!= null)
                {
                    var valorTS = this.RetornarValorTypeScript(valor);
                    linhas.Add(String.Format("{0}public static readonly {1} : {2} = {3};", TAB, constante.Name, caminhoTipo, valorTS));
                }
                
            }

            linhas.Add("}");
            return linhas;
        }

        private object RetornarValor(FieldInfo constante)
        {
            try
            {
                if(constante.IsLiteral && !constante.IsInitOnly)
                {
                    return constante.GetRawConstantValue();
                }
                return constante.GetValue(null);
            }
            catch
            {
                return null;
            }
        }
    }
}
