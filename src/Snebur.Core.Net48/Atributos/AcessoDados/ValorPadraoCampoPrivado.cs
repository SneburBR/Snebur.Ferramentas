using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoCampoPrivadoAttribute : Attribute/*, IValorPadrao*/
    {
        public object ValorPadrao { get; set; }
        public Type TipoEnum { get; }

        //public ValorPadraoCampoPrivadoAttribute(Enum valorPadrao)
        //{
        //    this.ValorPadrao = valorPadrao;
        //}

        public ValorPadraoCampoPrivadoAttribute(string valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoCampoPrivadoAttribute(int valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoCampoPrivadoAttribute(bool valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoCampoPrivadoAttribute(DateTime valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoCampoPrivadoAttribute(decimal valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoCampoPrivadoAttribute(double valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoCampoPrivadoAttribute(object valorPadrao)
        {
            if (valorPadrao != null && valorPadrao.GetType().IsEnum)
            {
                this.TipoEnum = valorPadrao.GetType();
                this.ValorPadrao = valorPadrao;
            }
            else
            {
                throw new Exception("o valor ValorPadraoCampoPrivadoAttribute não é suportado");
            }
        }

        public ValorPadraoCampoPrivadoAttribute(Type tipoEnum, int valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
            this.TipoEnum = tipoEnum;
        }

        public ValorPadraoCampoPrivadoAttribute(Type tipoEnum, Enum valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
            this.TipoEnum = tipoEnum;
        }
        //public ValorPadraoCampoPrivadoAttribute<TEnum>(TEnum valorPadrao) where T:struc
        //{
        //    this.ValorPadrao = valorPadrao;
        //}

        //public ValorPadraoCampoPrivadoAttribute(Enum valorPadrao)
        //{
        //    this.ValorPadrao = valorPadrao;
        //}

        public object RetornarValorPadrao()
        {
            return this.ValorPadrao;
        }

        #region IValorPadrao 

        public bool TipoNullableRequerido { get { return false; } }

        #endregion
    }
}