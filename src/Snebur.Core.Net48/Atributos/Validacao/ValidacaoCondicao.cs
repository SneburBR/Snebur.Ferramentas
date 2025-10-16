using System;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    /// <summary>
    /// Criar um atributo de validação de condição, onde um propreidade da Entidade ou ViewModel precisa ser valida
    /// </summary>
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoCondicaoAttribute : BaseAtributoValidacao
    {
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "Mensagem de validação não definida.";

        public override bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            throw new NotImplementedException();
        }

        public override string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            throw new NotImplementedException();
        }
    }
}