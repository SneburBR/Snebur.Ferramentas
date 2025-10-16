using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropriedadeIdentificadorProprietarioAttribute : BaseAtributoDominio, IBaseValorPadrao
    {
        public bool IsTipoNullableRequerido => false;

        public bool IsValorPadraoOnUpdate => false;

        public bool IsPermitirValorGlboal { get; set; }
        public string ValorGlobal { get; set; }

        public PropriedadeIdentificadorProprietarioAttribute()
        {

        }

        [IgnorarConstrutorTS]
        public PropriedadeIdentificadorProprietarioAttribute(object valorGlobal)
        {
            this.IsPermitirValorGlboal = true;
            this.ValorGlobal = valorGlobal?.ToString() ?? "NULL";
        }
    }
}