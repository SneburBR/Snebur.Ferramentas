using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum)]
    public class IgnorarTSReflexaoAttribute : Attribute
    {
    }
}

