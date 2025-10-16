using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class TipoTSAttribute : Attribute
    {
        public string CaminhoTipoTS { get; set; }

        public TipoTSAttribute(string caminhoTipoTS)
        {
            this.CaminhoTipoTS = caminhoTipoTS;
        }
    }
}