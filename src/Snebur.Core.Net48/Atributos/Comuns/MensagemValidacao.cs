using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MensagemValidacaoAttribute : Attribute
    {
    }
}