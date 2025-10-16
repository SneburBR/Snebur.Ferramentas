using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class OrdenacaoOpcoesAttribute : Attribute
    {
        public EnumOrdenacaoNovoRegistro OrdenacaoNovoRegistro { get; set; } = EnumOrdenacaoNovoRegistro.Fim;

        public bool IsIgnorarMigracao { get; set; }

        public string NomePropriedadeMapeada { get; set; }

        public OrdenacaoOpcoesAttribute()
        {

        }

        [IgnorarConstrutorTS]
        public OrdenacaoOpcoesAttribute(EnumOrdenacaoNovoRegistro ordenacaoNovoRegistro)
        {
            this.OrdenacaoNovoRegistro = ordenacaoNovoRegistro;
        }

    }
    [IgnorarGlobalizacao]
    public enum EnumOrdenacaoNovoRegistro
    {
        Inicio = 1,
        Fim = 2,
        Aleatorio = 3
    }
}