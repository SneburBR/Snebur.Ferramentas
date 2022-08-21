using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.VisualStudio.Utilidade;
using Snebur.VisualStudio.Reflexao;
using System.Reflection;
using Snebur.Utilidade;
using Snebur.Reflexao;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public partial class GeradorDominioReflexao : BaseGeradorDominio
    {

        private string RetornarConteudoAtributosTipo(Type tipo)
        {
            var declaracaoTipo = AjudanteReflexao.RetornarDeclaracaoTipo(tipo);
            var sb = new StringBuilder();
            var atributos = this.RetornarAtributosTipo(tipo);
            foreach (var atributo in atributos)
            {
                var caminhoAtributo = this.RetornarCaminhoAtributo(atributo);
                var parametrosAtributo = this.RetornarValoresParametrosAtributo(atributo);
                sb.AppendLine(String.Format("{0}.Atributos.Add(new {1}({2}));", declaracaoTipo, caminhoAtributo, parametrosAtributo));
            }

            return sb.ToString();
        }
 
        private List<Attribute> RetornarAtributosTipo(Type tipo)
        {
            var atributos = tipo.GetCustomAttributes().Where(x => x.GetType().Name != AjudanteAssembly.NomeTipoIgnorarAtributoTS).ToList();
            atributos = atributos.Where(x => TipoUtil.TipoIgualOuSubTipo(x.GetType(), typeof(ValidationAttribute)) ||
                                             TipoUtil.TipoIgualOuSubTipo(x.GetType(), typeof(ForeignKeyAttribute)) ||
                                             TipoUtil.TipoIgualOuSubTipo(x.GetType(), AjudanteAssembly.TipoBaseAtributoDominio)).ToList();
            return atributos;
        }


    }
}