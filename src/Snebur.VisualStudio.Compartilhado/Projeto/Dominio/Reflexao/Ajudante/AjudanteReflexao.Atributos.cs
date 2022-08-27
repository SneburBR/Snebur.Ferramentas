using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Snebur.Dominio;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
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

            if (construtores.Count > 1 &&
                !TipoUtil.TipoIgualOuSubTipo(tipo, typeof(BaseTipoComplexo)))
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

            if (!tipo.IsAbstract  && TipoUtil.TipoImplementaInterface(tipo, AjudanteAssembly.TipoInterfaceIImagem))
            {
                parametrosConstrutor.Add(new EstruturaParametroConstrutor("arquivo", typeof(FileInfo), false, false));
                parametrosConstrutor.Add(new EstruturaParametroConstrutor("informacaoImagem", "IInformacaoImagem", false, false));
            }
            return parametrosConstrutor;
        }
    }
}
