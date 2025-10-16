using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Field)]
    public class IgnorarConstanteTSAttribute : Attribute
    {
    }
}