using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluralAttribute : Attribute
    {

        public string Nome { get; set; }

        public PluralAttribute(string nome)
        {
            this.Nome = nome;
        }
    }
}