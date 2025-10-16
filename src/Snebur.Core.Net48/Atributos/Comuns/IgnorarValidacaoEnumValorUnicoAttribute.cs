using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class IgnorarValidacaoEnumValorUnicoAttribute : Attribute
    {
    }
}
