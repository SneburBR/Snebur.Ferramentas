using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class IgnorarConstrutorTSAttribute : Attribute
    {
    }
}

