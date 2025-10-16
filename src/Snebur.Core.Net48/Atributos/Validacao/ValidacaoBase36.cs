using Snebur.Utilidade;
using System;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoBase36Attribute : BaseAtributoValidacao
    {
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "O campo {0} deve conter apenas letras de A a Z e números de 0 a 9.";

        public bool IsPermitirEspaco { get; set; }
        public bool IsIgnorarCase { get; set; }
        public string CaracteresExtra { get; set; }

        [IgnorarConstrutorTS]
        public ValidacaoBase36Attribute() : this(false, true, null)
        {
        }

        [IgnorarConstrutorTS]
        public ValidacaoBase36Attribute(string caracteresExtra) : this(false, true, caracteresExtra)
        {
        }

        public ValidacaoBase36Attribute(bool isPermitirEspaco,
                                        bool isIgnorarCase,
                                        string caracteresExtra)
        {
            this.IsPermitirEspaco = isPermitirEspaco;
            this.IsIgnorarCase = isIgnorarCase;
            this.CaracteresExtra = caracteresExtra;
        }

        public override bool IsValido(PropertyInfo propriedade,
                                      object paiPropriedade,
                                      object valorPropriedade)
        {
            if (!ValidacaoUtil.IsDefinido(valorPropriedade))
            {
                return !ValidacaoUtil.IsPropriedadeRequerida(propriedade);
            }

            return Base36Util.IsBase36(valorPropriedade.ToString(), 
                                       this.IsPermitirEspaco,
                                       this.IsIgnorarCase,
                                       this.CaracteresExtra);
        }

        public override string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacao, rotulo);
        }
    }
}