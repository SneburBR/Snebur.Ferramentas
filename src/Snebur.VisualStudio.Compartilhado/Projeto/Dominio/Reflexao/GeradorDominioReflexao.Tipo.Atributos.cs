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
        private string RetornarConteudoAtributosTipo(Type tipo)
        {
            var declaracaoTipo = AjudanteReflexao.RetornarDeclaracaoTipo(tipo);
            var sb = new StringBuilder();
            var atributos = this.RetornarAtributosTipo(tipo);
            foreach (var atributo in atributos)
            {
                var caminhoAtributo = this.RetornarCaminhoAtributo(atributo);
                var parametrosAtributo = this.RetornarValoresParametrosAtributo(atributo);
                sb.AppendLine($"{declaracaoTipo}.Atributos.Add(new {caminhoAtributo}({parametrosAtributo}));");
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