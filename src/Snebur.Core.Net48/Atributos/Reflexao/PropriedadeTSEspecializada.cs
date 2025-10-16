using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropriedadeTSEspecializadaAttribute : BaseAtributoDominio
    {
        public string NomePropriedade { get; set; }

        public PropriedadeTSEspecializadaAttribute(string nomePropriedade)
        {
            this.NomePropriedade = nomePropriedade;
        }
    }
}