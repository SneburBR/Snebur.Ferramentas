using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NotificarAlteracaoPropriedadeAttribute : BaseAtributoDominio, INotificarAlteracaoPropriedade
    {
        internal static HashSet<Type> TiposProprieadesAlteracaoSuportados { get; } = new HashSet<Type> { typeof(string) };

        public Type TipoEntidadeAlteracaoPropriedade { get; }

        public string NomePropriedadeAlterada { get; }
        public string NomePropriedadeRelacao { get; }
        public PropertyInfo PropriedadeRelacao { get; }

        public PropertyInfo PropriedadeValorAlterado { get; }
        public PropertyInfo PropriedadeValorAntigo { get; }
 
        public EnumOpcoesAlterarPropriedade Opcoes { get; }
         
        public NotificarAlteracaoPropriedadeAttribute(Type tipoEntidadeAlteracaoPropriedade,
                                                      string nomePropriedadeRelacao,
                                                      string nomePropriedadeAlterada,
                                                      EnumOpcoesAlterarPropriedade opcoes = EnumOpcoesAlterarPropriedade.Nenhuma)
        {
            this.TipoEntidadeAlteracaoPropriedade = tipoEntidadeAlteracaoPropriedade;
            this.Opcoes = opcoes;

            var nomePropriedadeValorAntigo = this.RetornarNomePropriedadeValorAntigo(nomePropriedadeAlterada);
            this.NomePropriedadeRelacao = nomePropriedadeRelacao;
            this.NomePropriedadeAlterada = nomePropriedadeAlterada;

            this.PropriedadeRelacao = ReflexaoUtil.RetornarPropriedade(this.TipoEntidadeAlteracaoPropriedade, nomePropriedadeRelacao);
            this.PropriedadeValorAlterado = ReflexaoUtil.RetornarPropriedade(this.TipoEntidadeAlteracaoPropriedade, nomePropriedadeAlterada);
            this.PropriedadeValorAntigo = ReflexaoUtil.RetornarPropriedade(this.TipoEntidadeAlteracaoPropriedade, nomePropriedadeValorAntigo);
            this.ValidarPropriedades();
        }

        private string RetornarNomePropriedadeValorAntigo(string nomePropriedadeAlterada)
        {
            if (nomePropriedadeAlterada.EndsWith("_Id"))
            {
                var len = nomePropriedadeAlterada.Length;
                return $"{nomePropriedadeAlterada.Substring(0, len - 3)}Antigo_Id";
            }
            return $"{nomePropriedadeAlterada}Antigo";
        }

        private void ValidarPropriedades()
        {
            if (!this.PropriedadeRelacao.PropertyType.IsSubclassOf(typeof(Entidade)))
            {
                throw new Erro($"A propriedade relacao {this.PropriedadeRelacao.Name} em {this.TipoEntidadeAlteracaoPropriedade.Name} não é do tipo relacao");
            }
            var tipoValorAlterado = ReflexaoUtil.RetornarTipoSemNullable(this.PropriedadeValorAlterado.PropertyType);
            var tipoValorAntigo = ReflexaoUtil.RetornarTipoSemNullable(this.PropriedadeValorAntigo.PropertyType);

            if (tipoValorAlterado != tipoValorAntigo)
            {
                throw new Erro($"Os tipos das propriedades {this.PropriedadeValorAlterado.Name}, {this.PropriedadeValorAntigo.Name} são diferente");
            }
            if (!this.IsTipoValorAlteradoSuportado(tipoValorAlterado))
            {
                throw new Erro($"O tipo {tipoValorAlterado.Name} da propriedade {this.PropriedadeValorAlterado.Name} em {this.TipoEntidadeAlteracaoPropriedade.Name} não é suportado");
            }
            if (!ReflexaoUtil.IsTipoImplementaInterface(this.TipoEntidadeAlteracaoPropriedade, typeof(IAlteracaoPropriedade)))
            {
                throw new Erro($"O tipo {this.TipoEntidadeAlteracaoPropriedade.Name} não implementa a interface {nameof(IAlteracaoPropriedade)}");
            }
            if (this.PropriedadeValorAntigo.PropertyType.IsValueType && !ReflexaoUtil.IsTipoNullable(this.PropriedadeValorAntigo.PropertyType))
            {
                throw new Erro($"O tipo '{this.PropriedadeValorAntigo.PropertyType}' da propriedade  {this.PropriedadeValorAntigo.Name} em '{this.TipoEntidadeAlteracaoPropriedade.Name}'  não é suportado, utilizar nullable");
            }
        }

        private bool IsTipoValorAlteradoSuportado(Type tipoValorAlterado)
        {
            return tipoValorAlterado.IsValueType ||
                   TiposProprieadesAlteracaoSuportados.Contains(tipoValorAlterado) ||
                   tipoValorAlterado.IsSubclassOf(typeof(BaseTipoComplexo));
        }
    }
}