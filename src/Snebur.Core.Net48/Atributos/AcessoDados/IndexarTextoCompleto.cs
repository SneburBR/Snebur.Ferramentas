using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexarTextoCompletoAttribute : Attribute, IAtributoMigracao
    {
        [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
        public bool IsIgnorarMigracao { get; set; }
    }
}
