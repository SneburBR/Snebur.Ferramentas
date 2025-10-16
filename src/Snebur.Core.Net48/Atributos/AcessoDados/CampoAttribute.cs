using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class CampoAttribute : ColumnAttribute
    {

        public string NomeCampo { get; set; }

        public CampoAttribute() : base()
        {
        }

        public CampoAttribute(string nomeCampo) : base(nomeCampo)
        {
            this.NomeCampo = nomeCampo;
        }
    }

    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class TipoBancoAttribute : ColumnAttribute 
    {
        public Type Tipo { get; }

        public TipoBancoAttribute(Type tipo)
        {
            this.Tipo = tipo;
        }
    }
}