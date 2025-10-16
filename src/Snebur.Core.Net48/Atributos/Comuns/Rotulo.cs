using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property |
                    AttributeTargets.Class |
                    AttributeTargets.Field |
                    AttributeTargets.Enum, AllowMultiple = false)]
    public class RotuloAttribute : BaseAtributoDominio
    {
        private string _rotuloPlural;

        public string Rotulo { get; set; }

        public string RotuloPlural
        {
            get
            {
                if (String.IsNullOrEmpty(this._rotuloPlural))
                {
                    return this.Rotulo;
                }
                return this._rotuloPlural;
            }
            set
            {
                this._rotuloPlural = value;
            }
        }
        [IgnorarConstrutorTS]
        public RotuloAttribute(string rotulo)
        {
            this.Rotulo = rotulo;
        }

        public RotuloAttribute(string rotulo,
                               [ParametroOpcionalTS] string rotuloPlural)
        {
            this.Rotulo = rotulo;
            this.RotuloPlural = rotuloPlural;
        }
    }
}