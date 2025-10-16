using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoIDSessaoUsuarioAttribute : Attribute, IBaseValorPadrao
    {
        public bool IsSomenteCadastro { get; }
        public bool IsTipoNullableRequerido => false;
        public bool IsValorPadraoOnUpdate { get; set; } = false;

        public ValorPadraoIDSessaoUsuarioAttribute(bool isSomenteCadastro)
        {
            this.IsSomenteCadastro = isSomenteCadastro;
        }

    }
}