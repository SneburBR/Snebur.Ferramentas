using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ChaveEstrangeiraRelacaoUmUmAttribute : BaseAtributoDominio, IChaveEstrangeiraAttribute
    {
        public string NomePropriedade { get; set; }
        public string Name => this.NomePropriedade;

        public ChaveEstrangeiraRelacaoUmUmAttribute(string nomePropriedade)
        {
            this.NomePropriedade = nomePropriedade;
        }
    }
}
