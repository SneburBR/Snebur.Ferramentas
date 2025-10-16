using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class RelacaoChaveEstrangeiraAttribute : BaseRelacaoAttribute
    {
    }
}