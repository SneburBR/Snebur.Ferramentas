using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public partial class GeradorDominioReflexao : BaseGeradorDominio
    {
        private string RetornarConteudoAtributosPropriedade(PropertyInfo propriedade)
        {
            var declaracaoProprieade = AjudanteReflexao.RetornarDeclaracaoPropriedade(propriedade);
            var sb = new StringBuilder();
            var atributos = this.RetornarAtributosPropriedade(propriedade);
            foreach (var atributo in atributos)
            {
                var caminhoAtributo = this.RetornarCaminhoAtributo(atributo);
                var parametrosAtributo = this.RetornarValoresParametrosAtributo(atributo);
                sb.AppendLine($"{declaracaoProprieade}.Atributos.Add(new {caminhoAtributo}({parametrosAtributo}));");
            }
            return sb.ToString();
        }

        private List<Attribute> RetornarAtributosPropriedade(PropertyInfo propriedade)
        {
            var atributos = propriedade.GetCustomAttributes().Where(x => x.GetType().Name != AjudanteAssembly.NomeTipoIgnorarAtributoTS).ToList();
            atributos = atributos.Where(x => TipoUtil.TipoIgualOuSubTipo(x.GetType(), typeof(ValidationAttribute)) ||
                                             TipoUtil.TipoIgualOuSubTipo(x.GetType(), typeof(ForeignKeyAttribute)) ||
                                             TipoUtil.TipoIgualOuSubTipo(x.GetType(), typeof(KeyAttribute)) ||
                                             TipoUtil.TipoIgualOuSubTipo(x.GetType(), typeof(ScaffoldColumnAttribute)) ||
                                             TipoUtil.TipoIgualOuSubTipo(x.GetType(), AjudanteAssembly.TipoBaseAtributoDominio)).ToList();
            return atributos;
        }
    }
}