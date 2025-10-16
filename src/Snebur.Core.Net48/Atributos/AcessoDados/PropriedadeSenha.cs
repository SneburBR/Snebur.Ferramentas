using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarGlobalizacao]
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropriedadeSenhaAttribute : Attribute
    {
    }
}