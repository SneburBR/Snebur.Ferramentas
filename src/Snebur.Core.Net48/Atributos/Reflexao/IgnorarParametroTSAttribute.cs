using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class IgnorarParametroTSAttribute : Attribute
    {
    }
}