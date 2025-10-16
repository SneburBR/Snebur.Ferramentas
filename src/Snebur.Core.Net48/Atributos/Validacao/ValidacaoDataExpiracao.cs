using Snebur.Utilidade;
using System;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    public class ValidacaoDataExpiracaoAttribute : BaseAtributoValidacao, IAtributoValidacao
    {
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "A '{0}' deve ser superior a data de publicação.";

        [MensagemValidacao]
        public static string MensagemValidacaoComposta { get; set; } = "A '{0}' deve ser superior à '{1}'.";

        public string NomePropriedadeDataPublicacao { get; set; }

        public ValidacaoDataExpiracaoAttribute(string nomePropriedadeDataPublicacao)
        {
            this.NomePropriedadeDataPublicacao = nomePropriedadeDataPublicacao;
        }

        public override bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            if (paiPropriedade != null && valorPropriedade is DateTime dataExpiracao)
            {
                var propriedadePublicacao = paiPropriedade.GetType().GetProperty(this.NomePropriedadeDataPublicacao);
                if (propriedadePublicacao == null)
                {
                    throw new Erro($"A propriedade {this.NomePropriedadeDataPublicacao} não foi encontrado na tipo {paiPropriedade.GetType().Name}");
                }

                var tipoPropriedadePublicacao = ReflexaoUtil.RetornarTipoSemNullable(propriedadePublicacao.PropertyType);
                if (tipoPropriedadePublicacao != typeof(DateTime))
                {
                    throw new Erro($" O tipo '{tipoPropriedadePublicacao.Name}' da propriedade de publicação não é suportado. Esperado '{nameof(DateTime)} ");
                }

                var dataPublicacao = ReflexaoUtil.RetornarValorPropriedade(paiPropriedade, propriedadePublicacao);
                if (dataPublicacao is DateTime dataPublicaacaoTipada)
                {
                    return dataExpiracao.AddDays(1).RetornarDataComHoraZerada() > dataPublicaacaoTipada;
                }
                return false;
            }
            return true;
        }

        public override string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacao, rotulo);
        }
    }
}