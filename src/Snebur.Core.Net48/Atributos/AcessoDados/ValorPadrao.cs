using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoAttribute : Attribute, IValorPadrao
    {
        private bool _isValorPadraoOnUpdate;
        private EnumTipoValorPadrao _tipoValorPadrao = EnumTipoValorPadrao.Comum;

        public object ValorPadrao { get; set; }
        public bool IsValorPadraoQuandoNullOrDefault { get; set; } = true;
        public bool IsValorPadraoQuandoNullOrWhiteSpace { get; set; }
     
        public EnumTipoValorPadrao TipoValorPadrao
        {
            get 
            {
             return  this.IsValorPadraoQuandoNullOrDefault 
                    ?  EnumTipoValorPadrao.ValorPropriedadeNullOrDefault 
                    :   this.IsValorPadraoQuandoNullOrWhiteSpace  ? EnumTipoValorPadrao.ValorPropriedadeNullOrWhiteSpace
                                                                  : this._tipoValorPadrao;

            }
            set => this._tipoValorPadrao = value;
        }
         
        public bool IsValorPadraoOnUpdate
        {
            get => this.IsValorPadraoQuandoNullOrDefault || this._isValorPadraoOnUpdate;
            set => this._isValorPadraoOnUpdate = value;
        }
   

        public ValorPadraoAttribute(Enum valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoAttribute(string valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoAttribute(int valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoAttribute(bool valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoAttribute(DateTime valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoAttribute(decimal valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoAttribute(double valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public object RetornarValorPadrao(object contexto,
                                          Entidade entidade,
                                          object valorPropriedade)
        {
            return this.ValorPadrao;
        }

        #region IValorPadrao 

        public bool IsTipoNullableRequerido { get { return true; } }

        #endregion
    }
}