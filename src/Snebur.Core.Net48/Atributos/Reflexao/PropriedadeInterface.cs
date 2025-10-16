using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropriedadeInterfaceAttribute : Attribute
    {
        public string NomePropriedade { get; }

        public PropriedadeInterfaceAttribute(string nomePropriedade)
        {
            this.NomePropriedade = nomePropriedade;
        }
    }
}