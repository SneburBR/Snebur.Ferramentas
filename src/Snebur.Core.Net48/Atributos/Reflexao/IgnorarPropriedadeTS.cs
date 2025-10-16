using System;

namespace Snebur.Dominio.Atributos
{

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnorarPropriedadeTSAttribute : Attribute
    {
    }

}