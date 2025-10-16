using Snebur.Utilidade;
using System;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    /// <summary>
    /// Propriedade não mapeada na entidade, 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RelacaoUmUmReversaAttribute : BaseRelacaoAttribute
    {
        public string NomePropriedadeChaveEstrangeiraReversa { get; }

        [IgnorarPropriedade]
        public PropertyInfo PropriedadeRelacaoReversa { get; }

        [IgnorarConstrutorTS]
        public RelacaoUmUmReversaAttribute(Type tipoEntidadeRelacao, string nomePropriedadeChaveEstrangeiraReversa)
        {
            this.NomePropriedadeChaveEstrangeiraReversa = nomePropriedadeChaveEstrangeiraReversa;
            this.PropriedadeRelacaoReversa = ReflexaoUtil.RetornarPropriedade(tipoEntidadeRelacao, nomePropriedadeChaveEstrangeiraReversa);
        }

        public RelacaoUmUmReversaAttribute(string nomePropriedadeChaveEstrangeiraReversa)
        {
            this.NomePropriedadeChaveEstrangeiraReversa = nomePropriedadeChaveEstrangeiraReversa;
            throw new ErroOperacaoInvalida("Não usar esse construtor - está aqui somente para dominio do Typescript");
        }
    }
}