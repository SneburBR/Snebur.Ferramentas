using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Class)]
    public class IndexarCompostoAttribute : Attribute, IAtributoMigracao
    {
        public Type TipoEntidade { get; }
        public string[] NomesPropriedade { get; }

        [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
        public bool IsIgnorarMigracao { get; set; }
        public List<PropriedadeIndexar> Propriedades { get; } = new List<PropriedadeIndexar>();

        public IndexarCompostoAttribute(Type tipoEntidade, params string[] nomesPropriedade)
        {
            this.TipoEntidade = tipoEntidade;
            this.NomesPropriedade = nomesPropriedade;

            foreach (var _nomePropriedade in nomesPropriedade)
            {
                var nomePropriedade = _nomePropriedade;
                var isIgnorarNulo = nomePropriedade.EndsWith("?");
                if (isIgnorarNulo)
                {
                    nomePropriedade = nomePropriedade.Substring(0, nomePropriedade.Length - 1);
                }

                var propriedade = ReflexaoUtil.RetornarPropriedade(tipoEntidade, nomePropriedade, true);
                if (propriedade == null)
                {
                    if (AplicacaoSnebur.Atual.TipoAplicacao == EnumTipoAplicacao.ExtensaoVisualStudio)
                    {
                        return;
                    }
                    var memsagem = $"ValidacaoUnicoCompostaAttribute(typeof({tipoEntidade.Name}), {String.Join(", ", nomesPropriedade.Select(x => $"\"{x}\""))} - A propriedade {nomePropriedade} não foi encontrada no tipo '{tipoEntidade.Name}'";
                    throw new Exception(memsagem);
                }
                this.Propriedades.Add(new PropriedadeIndexar(propriedade, isIgnorarNulo, false));
            }
        }
    }
}