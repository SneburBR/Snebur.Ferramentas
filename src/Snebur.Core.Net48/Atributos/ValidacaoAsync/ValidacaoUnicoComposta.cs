using Snebur.Linq;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class ValidacaoUnicoCompostaAttribute : BaseAtributoValidacaoAsync, IAtributoMigracao
    {
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public List<PropriedadeIndexar> Propriedades { get; } = new List<PropriedadeIndexar>();

        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public List<FiltroPropriedadeIndexar> Filtros { get; } = new List<FiltroPropriedadeIndexar>();

        //[IgnorarPropriedadeTS]
        //[IgnorarPropriedadeTSReflexao]
        public Type TipoEntidade { get; }

        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "O {0} '{1}' já existe.";

        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public List<string> NomesPropriedade { get; } = new List<string>();

        public string[] ExpressoesPropriedadeFiltro { get; }

        public bool IsCriarIndicesNomeBanco { get; set; } = true;

        [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
        public bool IsIgnorarMigracao { get; set; }

        public ValidacaoUnicoCompostaAttribute(Type tipoEntidade,
                                               params string[] expressoesPropriedadeFiltro)
        {
            this.TipoEntidade = tipoEntidade;
            this.ExpressoesPropriedadeFiltro = expressoesPropriedadeFiltro;
            //this.NomesPropriedade = nomesPropriedadeOuFiltro;

#if EXTENSAO_VISUALSTUDIO
            return;
#endif

            if (expressoesPropriedadeFiltro.Count() > 0)
            {
                foreach (var expressaoPropriedadeFiltro in expressoesPropriedadeFiltro)
                {
                    this.AdicioanrFiltro(expressaoPropriedadeFiltro);
                }
            }
            

            if (ReflexaoUtil.IsTipoImplementaInterface(tipoEntidade, typeof(IDeletado)))
            {
                var propriedadeIsDeletado = ReflexaoUtil.RetornarPropriedade(tipoEntidade, nameof(IDeletado.IsDeletado), true);
                var propriedadeDataHoraDeletado = ReflexaoUtil.RetornarPropriedade(tipoEntidade, nameof(IDeletado.DataHoraDeletado), true);
                if (propriedadeIsDeletado != null)
                {
                    this.Filtros.Add(new FiltroPropriedadeIndexar(propriedadeIsDeletado,
                                                                  EnumOperadorComparacao.Igual,
                                                                  "0"));
                }

                if (propriedadeDataHoraDeletado != null)
                {
                    this.Filtros.Add(new FiltroPropriedadeIndexar(propriedadeDataHoraDeletado,
                                     EnumOperadorComparacao.Igual,
                                     "null"));
                }
            }

            if (this.Filtros.Count > 0)
            {
                var propriedadesFiltro = this.Filtros.Select(x => x.Propriedade).ToList();
                var propriedadesEmConclito = this.Propriedades.Where(x => propriedadesFiltro.Contains(x.Propriedade)).
                                                                                             Select(x => x.Propriedade);
                if (propriedadesEmConclito.Count() > 0)
                {
                    throw new Erro($"Remover as propriedades  em conflitos {String.Join(",", propriedadesEmConclito.Select(x => x.Name))}" +
                                   $" no  índice {nameof(ValidacaoUnicoCompostaAttribute)} em {tipoEntidade.Name} ");
                }

                var duplicados = propriedadesFiltro.Duplicados();
                if (duplicados.Count() > 0)
                {
                    throw new Erro($"Remover as propriedades  em conflitos {String.Join(",", duplicados.Select(x => x.Name))}" +
                                   $" no  índice {nameof(ValidacaoUnicoCompostaAttribute)} em {tipoEntidade.Name} ");
                }
            }

        }

        private void AdicioanrFiltro(string nomePropriedadeOuFiltro)
        {
            var reg = new Regex(@"\s+");
            var partes = reg.Split(nomePropriedadeOuFiltro).Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();
            if (partes.Count() > 1)
            {
                var propriedadeFiltro = FiltroPropriedadeIndexarUtil.RetornarPropriedadeFiltro(this.TipoEntidade, partes);
                if (propriedadeFiltro == null)
                {
                    throw this.RetornarException(nomePropriedadeOuFiltro);
                }
                this.Filtros.Add(propriedadeFiltro);
            }
            else
            {
                var nomePropriedade = partes.Single().Trim();
                var propriedadeIndexar = this.RetornarPropriedadeIndexar(nomePropriedade);
                if (propriedadeIndexar == null)
                {
                    throw this.RetornarException(nomePropriedadeOuFiltro);
                }
                this.Propriedades.Add(propriedadeIndexar);
                this.NomesPropriedade.Add(nomePropriedade);
            }
        }

        private PropriedadeIndexar RetornarPropriedadeIndexar(string nomePropriedade)
        {
            var tipoEntidade = this.TipoEntidade;

            var isIgnorarZero = nomePropriedade.EndsWith("?");
            if (isIgnorarZero)
            {
                nomePropriedade = nomePropriedade.Substring(0, nomePropriedade.Length - 1);
            }

            var propriedade = ReflexaoUtil.RetornarPropriedade(tipoEntidade, nomePropriedade, true);
            if (propriedade != null)
            {
                return new PropriedadeIndexar(propriedade, isIgnorarZero, false);
            }
            return null;
        }

        private Exception RetornarException(string nomePropriedade, string mensagem = null)
        {
            var tipoEntidade = this.TipoEntidade;
            var nomesPropriedade = this.NomesPropriedade;
            var memsagem = $"{mensagem} ValidacaoUnicoCompostaAttribute(typeof({tipoEntidade.Name}), {String.Join(", ", nomesPropriedade.Select(x => $"\"{x}\""))} - A propriedade ou {nomePropriedade} não foi encontrada  no tipo '{tipoEntidade.Name}'";
            return new Exception(memsagem);
        }

         

        #region IAtributoValidacao

        public bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            //validação no lado cliente;
            return true;
        }

        public string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacao, rotulo);
        }
        #endregion
    }

}