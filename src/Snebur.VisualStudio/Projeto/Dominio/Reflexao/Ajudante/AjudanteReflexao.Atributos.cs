using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.VisualStudio.Reflexao;
using Snebur.VisualStudio.Utilidade;
using Snebur.Utilidade;
using Snebur.VisualStudio.Projeto.Dominio.Estrutura;

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public partial class AjudanteReflexao
    {

        public static List<EstruturaParametroConstrutor> RetornarParametrosConstrutor(Type tipo)
        {
            var parametrosConstrutor = new List<EstruturaParametroConstrutor>();
            var construtores = tipo.GetConstructors().Where(x =>
            {
                return !x.GetCustomAttributes(true).Any(k => k.GetType().Name == AjudanteAssembly.NomeTipoIgnorarConstrutorTS);

            }).ToList();

            if (construtores.Count > 1)
            {
                throw new Exception(String.Format("Não é suportado mais de um construtor para Atributo {0}, Utilizar o IgnorarConstrutorJSAttribute", tipo.Name));
            }

            if (construtores.Count == 1)
            {
                var parametros = construtores.Single().GetParameters();
                if (parametros.Count() > 0)
                {

                    foreach (var parametro in parametros)
                    {
                        parametrosConstrutor.Add(new EstruturaParametroConstrutor(parametro));
                    }

                }
            }
            return parametrosConstrutor;
        }
    }
}
