using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoIDUsuarioLogadoAttribute : Attribute, IBaseValorPadrao
    {
        public bool IsTipoNullableRequerido => false;

        public bool IsPermitirUsuarioAnonimo { get; }

        public bool IsFiltrarUsuario { get; set; } = false;

        public bool IsFiltrarUsuarioAdministrador { get; set; } = false;
   
        public bool IsValorPadraoOnUpdate { get; set; }

        public ValorPadraoIDUsuarioLogadoAttribute(bool isPermitirUsuarioAnonimo = false)
        {
            this.IsPermitirUsuarioAnonimo = isPermitirUsuarioAnonimo;
        }
    }
}