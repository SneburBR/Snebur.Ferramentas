using Snebur.Utilidade;
using System;

namespace Snebur.Dominio.Atributos
{
    public class FormatarRotaAttribute : Attribute, IValorPadrao
    {
        public bool IsTipoNullableRequerido => false;
        public bool IsValorPadraoOnUpdate => true;
        public object RetornarValorPadrao(object contexto, 
                                          Entidade entidadeCorrente,
                                          object valorPropriedade)
        {
            if(valorPropriedade is string rota)
            {
                return FormatacaoUtil.FormatarRota(rota);
            }
            return null;
        }
         
    }
}
