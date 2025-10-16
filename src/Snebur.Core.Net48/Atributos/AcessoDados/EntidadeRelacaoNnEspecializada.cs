using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Class)]
    public class EntidadeRelacaoNnEspecializadaAttribute : Attribute
    {
    }
}