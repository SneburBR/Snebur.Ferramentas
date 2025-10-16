using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ChavePrimariaAttribute : BaseAtributoDominio
    {
        public bool IsIdentity { get; }

        public ChavePrimariaAttribute(bool isIdentity = true )
        {
            this.IsIdentity = isIdentity;
        }
    }
}