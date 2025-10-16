using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ChaveEstrangeiraExternaAttribute : BaseAtributoDominio, IChaveEstrangeiraAttribute
    {
        public string NomePropriedade { get; set; }
        public string Name => this.NomePropriedade;

        public ChaveEstrangeiraExternaAttribute(string nomePropriedade)
        {
            this.NomePropriedade = nomePropriedade;
        }
    }
}
