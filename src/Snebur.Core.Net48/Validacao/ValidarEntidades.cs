using Snebur.Utilidade;
using System;
using System.Collections.Generic;

namespace Snebur.Dominio
{
    public partial class ValidarEntidades : IDisposable
    {
        internal List<Entidade> Entidades { get; set; }

        private ValidarEntidades(List<Entidade> entidades)
        {
            this.Entidades = entidades;
        }

        public List<ErroValidacao> RetornarErroValidacao(object contextoDados)
        {
            var erros = new List<ErroValidacao>();
            foreach (var entidade in this.Entidades)
            {
                if(entidade is IDeletado deletado && deletado.IsDeletado) 
                {
                    continue;
                }
                //Propriedades
                var propriedades = ReflexaoUtil.RetornarPropriedades(entidade.GetType());
                foreach (var propriedade in propriedades)
                {
                    if (entidade.Id == 0 || (entidade.__PropriedadesAlteradas != null && 
                                             entidade.__PropriedadesAlteradas.ContainsKey(propriedade.Name)))
                    {
                        var atributosValidacao = ValidacaoUtil.RetornarAtributosValidacao(propriedade);
                        if (atributosValidacao.Count > 0)
                        {
                            var valorPropriedade = propriedade.GetValue(entidade);
                            foreach (var atributoValidacao in atributosValidacao)
                            {
                                if (!atributoValidacao.IsValido(propriedade, entidade, valorPropriedade))
                                {
                                    var erroValidacao = new ErroValidacao
                                    {
                                        NomeTipoEntidade = entidade.__NomeTipoEntidade,
                                        NomePropriedade = propriedade.Name,
                                        Mensagem = String.Format("Entidade : '{0}' - {1}", entidade.__NomeTipoEntidade, 
                                                                                           atributoValidacao.RetornarMensagemValidacao(propriedade, 
                                                                                                                                       entidade, 
                                                                                                                                       valorPropriedade)),
                                        NomeTipoValidacao = atributoValidacao.GetType().Name
                                    };

                                    if (DebugUtil.IsAttached)
                                    {
                                        //throw new Erro(erroValidacao.Mensagem);
                                    }
                                    erros.Add(erroValidacao);
                                }
                            }
                        }
                    }
                }
                //Entidades
                if (entidade.__IsExisteAlteracao)
                {
                    var atributosValidacaoEntidade = ValidacaoUtil.RetornarAtributosValidacaoEntidade(entidade.__TipoEntidade);
                    foreach (var atributoEntidade in atributosValidacaoEntidade)
                    {
                        if (!atributoEntidade.IsValido(contextoDados, this.Entidades, entidade))
                        {
                            var mensagem = String.Format("Entidade : '{0}' - {1}", entidade.__NomeTipoEntidade, atributoEntidade.RetornarMensagemValidacao(entidade));
                            var erroValidacao = new ErroValidacao
                            {
                                NomeTipoEntidade = entidade.__NomeTipoEntidade,
                                NomePropriedade = "ValidacaoEntidade",
                                Mensagem = mensagem,
                                NomeTipoValidacao = atributoEntidade.GetType().Name
                            };

                            if (DebugUtil.IsAttached)
                            {
                                throw new Erro(erroValidacao.Mensagem);
                            }
                            erros.Add(erroValidacao);
                        }
                    }
                }
            }
            return erros;
        }
        #region IDisposable

        public void Dispose()
        {
            this.Entidades = null;
        }
        #endregion
    }
}