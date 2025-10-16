using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexarAttribute : Attribute, IAtributoMigracao
    {
        [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
        public bool IsIgnorarMigracao { get; set; }

        public string NomeIndice { get; set; }
    }
}