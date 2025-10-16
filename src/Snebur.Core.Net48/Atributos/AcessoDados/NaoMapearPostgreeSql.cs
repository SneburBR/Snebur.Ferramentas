using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class NaoMapearPostgreeSql : Attribute
    {
    }
}
