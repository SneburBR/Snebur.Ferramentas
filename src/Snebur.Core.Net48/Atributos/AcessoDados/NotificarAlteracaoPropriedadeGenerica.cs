using Snebur.UI;
using System;

namespace Snebur.Dominio.Atributos
{
    //[IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class NotificarAlteracaoPropriedadeGenericaAttribute : BaseAtributoDominio, INotificarAlteracaoPropriedade
    {
        public EnumOpcoesAlterarPropriedade Opcoes { get; }
        public string FormatacaoPersonalizada { get; set; } 
        public string Formatacao { get; set; } = EnumFormatacao.Nenhuma.ToString().ToLower();
        public Type TipoPropriedadeRelacao { get; set; }
        public string CaminhoTipoPropriedadeRelacao { get; set; }
        public bool IsEnum { get; set; }
        
        public NotificarAlteracaoPropriedadeGenericaAttribute([TipoTS("string")] EnumFormatacao formatacao,
                                                              Type tipoPropriedadeRelacao,
                                                              string caminhoTipoPropriedadeRelacao,
                                                              EnumOpcoesAlterarPropriedade opcoes)
        {
            this.TipoPropriedadeRelacao = tipoPropriedadeRelacao;
            this.CaminhoTipoPropriedadeRelacao = caminhoTipoPropriedadeRelacao;
            this.Formatacao = formatacao.ToString().ToLower();
            this.Opcoes = opcoes;
        }

        /// <summary>
        /// Notifica a alteração de uma propriedade genérica
        /// </summary>
        /// <param name="formatacaoPersonalizada"> Formatação personalizada para valores da propriedade Ex: 'Total {0}' </param>
        [IgnorarConstrutorTS]
        public NotificarAlteracaoPropriedadeGenericaAttribute(string formatacaoPersonalizada = null)
        {
            this.FormatacaoPersonalizada = formatacaoPersonalizada;
        }

        [IgnorarConstrutorTS]
        public NotificarAlteracaoPropriedadeGenericaAttribute(EnumFormatacao formatacao)
        {
            this.Formatacao = formatacao.ToString().ToLower();
        }

        [IgnorarConstrutorTS]
        public NotificarAlteracaoPropriedadeGenericaAttribute(EnumOpcoesAlterarPropriedade opcoes)
        {
            this.Opcoes = opcoes;   
        }

        [IgnorarConstrutorTS]
        public NotificarAlteracaoPropriedadeGenericaAttribute(Type tipoPropriedadeRelacao,
                                                              EnumOpcoesAlterarPropriedade opcoes = EnumOpcoesAlterarPropriedade.Nenhuma)
        {
            this.TipoPropriedadeRelacao = tipoPropriedadeRelacao;
            this.CaminhoTipoPropriedadeRelacao = $"{tipoPropriedadeRelacao.Namespace}.{tipoPropriedadeRelacao.Name}";
            this.Opcoes = opcoes;
            this.IsEnum = tipoPropriedadeRelacao.IsEnum;
        }

        [IgnorarConstrutorTS]
        public NotificarAlteracaoPropriedadeGenericaAttribute(string caminhoTipoPropriedadeRelacao,
                                                             EnumOpcoesAlterarPropriedade opcoes = EnumOpcoesAlterarPropriedade.Nenhuma)
        {
            this.CaminhoTipoPropriedadeRelacao = caminhoTipoPropriedadeRelacao;
            this.Formatacao = EnumFormatacao.Nenhuma.ToString().ToLower();
            this.Opcoes = opcoes;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class NotificarTodasAlteracoesPropriedadeGenericaAttribute : BaseAtributoDominio, INotificarAlteracaoPropriedade
    {
        public EnumOpcoesAlterarPropriedade Opcoes { get; }
        public NotificarTodasAlteracoesPropriedadeGenericaAttribute()
        {

        }
    }

    [IgnorarInterfaceTS]
    public interface INotificarAlteracaoPropriedade
    {
        EnumOpcoesAlterarPropriedade Opcoes { get; }
    }
}