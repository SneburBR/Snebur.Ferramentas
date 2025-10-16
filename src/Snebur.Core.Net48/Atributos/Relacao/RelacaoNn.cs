using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RelacaoNnAttribute : BaseRelacaoAttribute
    {
        [IgnorarPropriedade]
        public Type TipoEntidadeRelacao { get; set; }

        public string NomeTipoEntidadeRelacao { get; set; }

        [IgnorarConstrutorTS]
        public RelacaoNnAttribute(Type tipoEntidadeRelacao)
        {
            this.TipoEntidadeRelacao = tipoEntidadeRelacao;
            this.NomeTipoEntidadeRelacao = this.TipoEntidadeRelacao.Name;
        }

        public RelacaoNnAttribute(string nomeTipoEntidadeRelacao)
        {
            this.NomeTipoEntidadeRelacao = nomeTipoEntidadeRelacao;
            throw new ErroOperacaoInvalida("Não usar esse construtor - está aqui somente para dominio do Typescript");
        }
    }
}