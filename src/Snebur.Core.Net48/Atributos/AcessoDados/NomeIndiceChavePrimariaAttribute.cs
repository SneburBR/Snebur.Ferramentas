using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Class)]
    public class NomeIndiceChavePrimariaAttribute : Attribute
    {
        public string Nome { get; set; }
        public NomeIndiceChavePrimariaAttribute(string nome)
        {
            this.Nome = nome;
        }

    }
}
