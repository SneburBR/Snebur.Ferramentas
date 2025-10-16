using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class RelacaoNnEspecializadaAttribute : RelacaoNnAttribute
    {
        [IgnorarPropriedade]
        public Type TipoEntidadeRelacaoEspecializada { get; set; }

        public RelacaoNnEspecializadaAttribute(Type tipoEntidadeRelacao, Type tipoEntidadeEspecializada) : base(tipoEntidadeRelacao)
        {
            this.TipoEntidadeRelacaoEspecializada = tipoEntidadeEspecializada;
        }
    }
}