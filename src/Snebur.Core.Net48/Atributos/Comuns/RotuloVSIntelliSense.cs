using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class RotuloVSIntelliSenseAttribute : Attribute
    {
        public string Rotulo { get; set; }

        public RotuloVSIntelliSenseAttribute(string rotulo)
        {
            this.Rotulo = rotulo;
        }
    }
}