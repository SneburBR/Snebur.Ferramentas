using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class PropriedadeDescricaoAttribute : BaseAtributoDominio
    {
        public string NomePropriedade { get; set; }

        public PropriedadeDescricaoAttribute(string nomePropriedade)
        {
            this.NomePropriedade = nomePropriedade;
        }
        [IgnorarConstrutorTS]
        public PropriedadeDescricaoAttribute()
        {
            this.NomePropriedade = String.Empty;
        }
    }
}