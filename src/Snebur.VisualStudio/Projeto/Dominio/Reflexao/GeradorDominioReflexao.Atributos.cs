using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.VisualStudio.Reflexao;
using Snebur.Reflexao;
using System.ComponentModel.DataAnnotations;

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public partial class GeradorDominioReflexao 
    {

        private string RetornarAssociacaoAtributosCaminhoTipo()
        {
            var sb = new System.Text.StringBuilder();
            var gruposNamespace = this.TiposAtributos.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();
            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var nomePropriedadeCaminhoTipo = ReflexaoUtil.RetornarNomePropriedade<Snebur.Dominio.ICaminhoTipo>(x => x.__CaminhoTipo);

                foreach (var tipoAtributo in grupoNamespace)
                {
                    var caminhoTipo = TipoUtil.RetornarCaminhoTipo(tipoAtributo);
                    sb.AppendLine(String.Format("{0}.{1} = \"{0}\";", caminhoTipo, nomePropriedadeCaminhoTipo)); ;
                }
            }
            return sb.ToString();
        }

        private string RetornarCaminhoAtributo(Attribute atributo)
        {
            var tipoAtributo = atributo.GetType();
            if (tipoAtributo.Namespace.StartsWith("System"))
            {
                if(TipoUtil.TipoIgualOuSubTipo(tipoAtributo, typeof(ValidationAttribute)) ||
                   TipoUtil.TipoSubTipo(tipoAtributo, typeof(ScaffoldColumnAttribute)))
                {
                    return String.Format("Snebur.Dominio.Atributos.{0}", tipoAtributo.Name);
                }

                if(TipoUtil.TipoIgual(tipoAtributo, typeof(KeyAttribute)))
                {
                    return "Snebur.Dominio.Atributos.ChavePrimaria";
                }

                throw new ErroOperacaoInvalida("Atributo invalido");
                
            }
            else
            {
                return String.Format("{0}.{1}", tipoAtributo.Namespace, tipoAtributo.Name);
            }
        }

        private string RetornarValoresParametrosAtributo(Attribute atributo)
        {
            var tipoAtributo = atributo.GetType();
            var propriedades = ReflexaoUtil.RetornarPropriedades(tipoAtributo, false);
            var parametrosConstrutor = AjudanteReflexao.RetornarParametrosConstrutor(tipoAtributo);
            var valores = new List<string>();

            foreach (var parametro in parametrosConstrutor)
            {
                var propriedadeReferenteParametro = propriedades.Where(x => x.Name.Equals(parametro.NomeParametro, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                if (propriedadeReferenteParametro == null)
                {
                    //valores.Add("null");
                    throw new Exception(String.Format("Não foi encontrado uma propriedade para parametro do atributo {0} - {1}", tipoAtributo.Name, parametro.NomeParametro));
                }
                else
                {
                    var valorPropriedade = propriedadeReferenteParametro.GetValue(atributo);
                    valores.Add(this.RetornarValorParametroAtributoPropriedade(valorPropriedade));
                }
            }
            return String.Join(",", valores);
        }

        private string RetornarValorParametroAtributoPropriedade(object valor)
        {
            if (valor == null)
            {
                return "null";
            }

            var tipo = valor.GetType();

            if (ReflexaoUtil.TipoRetornaTipoPrimario(tipo))
            {
                var tipoPrimarioEnum = ReflexaoUtil.RetornarTipoPrimarioEnum(tipo);

                switch (tipoPrimarioEnum)
                {
                    case EnumTipoPrimario.Boolean:
                        return Convert.ToBoolean(valor).ToString().ToLower();
                    case EnumTipoPrimario.DateTime:
                        return " new Date()";
                    case EnumTipoPrimario.Decimal:
                    case EnumTipoPrimario.Double:
                    case EnumTipoPrimario.Integer:
                    case EnumTipoPrimario.Long:
                    case EnumTipoPrimario.EnumValor:
                        return Convert.ToInt64(valor).ToString();
                    case EnumTipoPrimario.String:
                        return String.Format("\"{0}\"", valor.ToString());
                    case EnumTipoPrimario.Guid:
                        return String.Format("\"{0}\"", Guid.Parse(valor.ToString()).ToString());
                    case EnumTipoPrimario.TimeSpan:
                        //return string.Format(" new TimeSpan(0,0,0,0,0);");
                        throw new ErroNaoImplementado("Nunca testado");
                    default:
                        throw new NotSupportedException(String.Format("Tipo primário não suportado {0} ", tipoPrimarioEnum.ToString()));
                }
            }
            else
            {
                if (tipo == typeof(string[]))
                {
                    var valores = ((ICollection)valor).Cast<string>().Select(x => String.Format("\"{0}\"", x)).ToList();
                    return String.Format("[ {0} ] ", String.Join(",", valores));
                }


                throw new ErroNaoSuportado("O tipo do parametro atributo propriedade não é suportado");
            }
        }

    }
}
