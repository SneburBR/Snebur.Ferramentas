using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoUnicoAttribute : BaseAtributoValidacaoAsync, IAtributoValidacao, IAtributoMigracao
    {
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "O {0} '{1}' já existe.";
        public bool IsIgnorarNulo { get; set; } = true;
        public bool IsIgnorarZero { get; set; }
        public Type TipoEntidade { get; }
        public string NomePropriedadeFiltro { get; }
        public object ValorPropriedadeFiltro { get; }
        public EnumOperadorComparacao OperadorFiltro { get; }

        [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
        public bool IsIgnorarMigracao { get; set; }

        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public List<FiltroPropriedadeIndexar> Filtros { get; } = new List<FiltroPropriedadeIndexar>();

        [IgnorarConstrutorTS]
        public ValidacaoUnicoAttribute()
        {

        }

        [IgnorarConstrutorTS]
        public ValidacaoUnicoAttribute(Type tipoEntidade,
                                       string nomePropriedadeFiltro,
                                       object valorPropriedadeFiltro,
                                       EnumOperadorComparacao operadorFiltro = EnumOperadorComparacao.Igual) : this(tipoEntidade, false, false, nomePropriedadeFiltro, valorPropriedadeFiltro, operadorFiltro)
        {

        }

        public ValidacaoUnicoAttribute(Type tipoEntidade,
                                       bool isIgnorarNulo,
                                       bool isIgnorarZero,
                                       string nomePropriedadeFiltro,
                                       object valorPropriedadeFiltro,
                                       EnumOperadorComparacao operadorFiltro)
        {
            this.TipoEntidade = tipoEntidade;
            this.IsIgnorarNulo = isIgnorarNulo;
            this.IsIgnorarZero = isIgnorarZero;
            this.NomePropriedadeFiltro = nomePropriedadeFiltro;
            this.ValorPropriedadeFiltro = valorPropriedadeFiltro;
            this.OperadorFiltro = operadorFiltro;
             
            var propriedade = ReflexaoUtil.RetornarPropriedade(tipoEntidade, nomePropriedadeFiltro, true);
            if (propriedade == null)
            {
                throw new ErroNaoImplementado($"ValidacaoUnicoAttribute -> Propriedade '{nomePropriedadeFiltro}' não encontrada na entidade '{tipoEntidade.Name}'");
            }

            var valorSqlString = SqlUtil.SqlValueString(valorPropriedadeFiltro);
            var filtro = new FiltroPropriedadeIndexar(propriedade, operadorFiltro, valorSqlString);
            this.Filtros.Add(filtro);
        }

        #region IAtributoValidacao

        public bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            if (!ValidacaoUtil.IsDefinido(valorPropriedade))
            {
                return !ValidacaoUtil.IsPropriedadeRequerida(propriedade);
            }
            // throw new ErroNaoImplementado("Validacao unico não implementado");
            return true;
        }

        public string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacao, rotulo, valorPropriedade?.ToString() ?? "null");
        }
        #endregion
    }
}