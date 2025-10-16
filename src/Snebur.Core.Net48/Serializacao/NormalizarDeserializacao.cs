using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Snebur.Dominio;
using Snebur.Utilidade;

namespace Snebur.Serializacao
{

    internal class NormalizarDeserializacao : IDisposable
    {

        internal object Objeto { get; set; }

        internal Dictionary<Guid, BaseDominio> ObjetosAnalisados { get; set; }
        internal Dictionary<Guid, BaseDominio> BasesDominios { get; set; }

        internal NormalizarDeserializacao(object objeto)
        {
            this.Objeto = objeto;

            this.ObjetosAnalisados = new Dictionary<Guid, BaseDominio>();
            this.BasesDominios = new Dictionary<Guid, BaseDominio>();
        }

        internal void Normalizar()
        {
            this.AnalisarBaseDominio(this.Objeto);
            this.ObjetosAnalisados.Clear();
            this.SubstituirBasesDominioReferencia(this.Objeto);
            this.ObjetosAnalisados.Clear();
        }

        internal void AnalisarBaseDominio(object objeto)
        {
            if (objeto != null)
            {
                if (objeto.GetType().IsSubclassOf(typeof(BaseDominio)))
                {
                    var baseDominio = (BaseDominio)objeto;

                    if (!this.ObjetosAnalisados.ContainsKey(baseDominio.__IdentificadorUnico))
                    {
                        this.ObjetosAnalisados.Add(baseDominio.__IdentificadorUnico, baseDominio);

                        if (!baseDominio.__BaseDominioReferencia)
                        {
                            if (this.BasesDominios.ContainsKey(baseDominio.__IdentificadorUnico))
                            {
                                throw new Erro(" Possivel erro do algoritmo, a bases negocio soh pode existe um vez, as demais precisão ser referencias ");
                            }
                            this.BasesDominios.Add(baseDominio.__IdentificadorUnico, baseDominio);
                        }
                        var propriedades = SerializacaoUtil.RetornarPropriedades(objeto.GetType()).ToList();

                        foreach (var propriedade in propriedades)
                        {
                            var valorPropriedade = ReflexaoUtil.RetornarValorPropriedade(objeto, propriedade);

                            if (valorPropriedade != null)
                            {
                                if (ReflexaoUtil.TipoRetornaColecao(valorPropriedade.GetType()))
                                {
                                    var colecao = valorPropriedade as IEnumerable;
                                    foreach (var item in colecao)
                                    {
                                        this.AnalisarBaseDominio(item);
                                    }
                                }
                                else
                                {
                                    this.AnalisarBaseDominio(valorPropriedade);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal void SubstituirBasesDominioReferencia(object objeto)
        {
            if (objeto != null && objeto is BaseDominio baseDominio)
            {
                baseDominio.IsSerializando = false;
                if (!this.ObjetosAnalisados.ContainsKey(baseDominio.__IdentificadorUnico))
                {
                    this.ObjetosAnalisados.Add(baseDominio.__IdentificadorUnico, baseDominio);
                    baseDominio.IsSerializando = true;

                    var propriedades = SerializacaoUtil.RetornarPropriedades(objeto.GetType());

                    foreach (var propriedade in propriedades)
                    {
                        var valorPropriedade = ReflexaoUtil.RetornarValorPropriedade(objeto, propriedade);

                        if (valorPropriedade != null)
                        {
                            if (valorPropriedade is BaseDominio bodeDominio)
                            {
                                if (bodeDominio.__BaseDominioReferencia)
                                {
                                    if (!this.BasesDominios.ContainsKey(bodeDominio.__IdentificadorReferencia.Value))
                                    {
                                        throw new Erro("Não foi encontrada uma BaseDominio para a Referencia da mesma. Isso se deve uma referencia circular na arvore.");
                                    }
                                    ReflexaoUtil.AtribuirValorPropriedade(objeto, propriedade, this.BasesDominios[bodeDominio.__IdentificadorReferencia.Value]);
                                }
                                this.SubstituirBasesDominioReferencia(valorPropriedade);
                            }
                            else if (ReflexaoUtil.TipoRetornaColecao(valorPropriedade.GetType()))
                            {
                                if (valorPropriedade is IList colecaoLista)
                                {
                                    this.SubtiturirBaseDominioReferenciaColecaoLista(colecaoLista);
                                    continue;
                                }
                                if (valorPropriedade is IDictionary colecaoDicionario)
                                {
                                    this.SubtiturirBaseDominioReferenciaColecaoDictionario(colecaoDicionario);
                                    continue;
                                }
                                if (propriedade.PropertyType.Name.StartsWith("HashSet") && valorPropriedade is IEnumerable colecaoHashSet)
                                {
                                    this.SubtiturirBaseDominioReferenciaColecaoHashSet(colecaoHashSet);
                                    continue;
                                }
                                throw new Erro($"A colecao {propriedade.Name} em  {objeto.GetType().Name} não é suportada, suportados {nameof(IList)}, {nameof(IDictionary)},{nameof(HashSet<object>)}");
                                //foreach (var item in colecao)
                                //{
                                //    this.SubstituirBasesDominioReferencia(item);
                                //}
                            }
                        }
                    }
                }
                baseDominio.IsSerializando = false;
            }
        }

        internal void SubtiturirBaseDominioReferenciaColecaoLista(IList colecaoLista)
        {
            for (var i = 0; i < colecaoLista.Count; i++)
            {
                var item = colecaoLista[i];
                if (item is BaseDominio baseDominio)
                {
                    if (baseDominio.__BaseDominioReferencia)
                    {
                        if (!this.BasesDominios.ContainsKey(baseDominio.__IdentificadorReferencia.Value))
                        {
                            throw new Erro("Não foi encontrada uma BaseDominio para a Referencia da mesma. Isso se deve uma referencia circular na arvore.");
                        }
                        item = this.BasesDominios[baseDominio.__IdentificadorReferencia.Value];
                        colecaoLista[i] = item;
                    }
                }
                this.SubstituirBasesDominioReferencia(item);
            }
        }

        internal void SubtiturirBaseDominioReferenciaColecaoDictionario(IDictionary dicionario)
        {
            foreach (var chave in dicionario.Keys)
            {
                var item = dicionario[chave];
                if (item is BaseDominio baseDominio)
                {
                    if (baseDominio.__BaseDominioReferencia)
                    {
                        if (!this.BasesDominios.ContainsKey(baseDominio.__IdentificadorReferencia.Value))
                        {
                            throw new Erro("Não foi encontrada uma BaseDominio para a Referencia da mesma. Isso se deve uma referencia circular na arvore.");
                        }
                        item = this.BasesDominios[baseDominio.__IdentificadorReferencia.Value];
                        dicionario[chave] = item;
                    }
                }
                this.SubstituirBasesDominioReferencia(item);
            }
        }

        internal void SubtiturirBaseDominioReferenciaColecaoHashSet(IEnumerable colecaoHashSet)
        {
            foreach (var item in colecaoHashSet)
            {
                var itemReferencia = item;
                if (item is BaseDominio baseDominio)
                {
                    if (baseDominio.__BaseDominioReferencia)
                    {
                        if (!this.BasesDominios.ContainsKey(baseDominio.__IdentificadorReferencia.Value))
                        {
                            throw new Erro("Não foi encontrada uma BaseDominio para a Referencia da mesma. Isso se deve uma referencia circular na arvore.");
                        }
                        itemReferencia = this.BasesDominios[baseDominio.__IdentificadorReferencia.Value];

                        (colecaoHashSet as dynamic).Remove(item);
                        (colecaoHashSet as dynamic).Add(itemReferencia);
                    }
                }
                this.SubstituirBasesDominioReferencia(itemReferencia);
            }
        }
        #region IDisposable

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.BasesDominios.Clear();
                    this.ObjetosAnalisados.Clear();

                    this.BasesDominios = null;
                    this.ObjetosAnalisados = null;
                }
            }
            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}