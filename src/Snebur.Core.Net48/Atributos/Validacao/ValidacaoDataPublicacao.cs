using Snebur.Utilidade;
using System;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    public class ValidacaoDataPublicacaoAttribute : BaseAtributoValidacao, IAtributoValidacao
    {
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "A '{0}' deve ser superior ou igual à data de hoje.";

        public ValidacaoDataPublicacaoAttribute()
        {
        }

        public override bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            if (valorPropriedade is DateTime dataPublicacao && 
                paiPropriedade is Entidade entidadePai &&
                entidadePai.Id == 0)
            {
                return dataPublicacao.RetornarDataComHoraZerada() >= DateTime.Now.RetornarDataComHoraZerada();
            }
            return true;
        }

        public override string RetornarMensagemValidacao(PropertyInfo propriedade, 
                                                         object paiPropriedade,
                                                         object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            if (valorPropriedade is DateTime dataPublicacao)
            {
              return  $" A {rotulo}: {dataPublicacao.RetornarDataComHoraZerada():dd/MM/yyyy} deve ser superior à data de hoje:  {DateTime.Now.RetornarDataComHoraZerada():dd/MM/yyyy}";
            }

            return String.Format(MensagemValidacao, rotulo); ;
        }
    }
}