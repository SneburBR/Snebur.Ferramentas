using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;
using System.Collections;
using Snebur.Utilidade;
using Snebur.Dominio.Atributos;

namespace Snebur.Serializacao
{
    internal class PrepararSerializacao : IDisposable
    {
        internal Dictionary<Guid, object> ObjetosAnalisados { get; set; }
        internal Dictionary<Guid, object> BasesDominio { get; set; }

        internal PrepararSerializacao()
        {
            this.ObjetosAnalisados = new Dictionary<Guid, object>();
            this.BasesDominio = new Dictionary<Guid, object>();
        }

        internal void Preparar(object objeto)
        {
            this.PrepararObjeto(objeto);
        }

        private BaseDominio RetornarBaseDominioReferencia(BaseDominio entidade)
        {
            var baseDominioReferencia = (BaseDominio)Activator.CreateInstance(entidade.GetType());
            baseDominioReferencia.__BaseDominioReferencia = true;
            baseDominioReferencia.__IdentificadorReferencia = entidade.RetornarIdentificadorReferencia();
            return baseDominioReferencia;
        }

        private void PrepararObjeto(object objeto)
        {
            if (objeto is BaseDominio)
            {
                var baseDominio = (BaseDominio)objeto;
                baseDominio.IsSerializando = true;

                if (!this.ObjetosAnalisados.ContainsKey(baseDominio.__IdentificadorUnico))
                {
                    if ((baseDominio.__IdentificadorUnico == Guid.Empty))
                    {
                        throw new Exception("O Identificador do Objeto unico está vazio ");

                        //baseDominio.IdentificadorUnicoObjeto = Guid.NewGuid()

                        //If TypeOf baseDominio Is Entidade
                        //   DirectCast(baseDominio, Entidade).IdentificadorReferencia = baseDominio.IdentificadorUnicoObjeto
                        //End If
                    }
                    this.ObjetosAnalisados.Add(baseDominio.__IdentificadorUnico, objeto);

                    if (baseDominio.__BaseDominioReferencia)
                    {
                        throw new Exception("Entidade referencia, não suportado para serializacao. ");
                    }
                    if (!this.BasesDominio.ContainsKey(baseDominio.__IdentificadorUnico))
                    {
                        this.BasesDominio.Add(baseDominio.__IdentificadorUnico, objeto);
                    }
                    if (objeto.GetType().IsSubclassOf(typeof(BaseDominio)))
                    {
                        var propriedades = SerializacaoUtil.RetornarPropriedades(objeto.GetType());
                        foreach (var propriedade in propriedades)
                        {
                            if (!ReflexaoUtil.PropriedadePossuiAtributo(propriedade, typeof(NaoSerializarAttribute)))
                            {
                                var valorPropriedade = ReflexaoUtil.RetornarValorPropriedade(objeto, propriedade);
                                if (valorPropriedade != null)
                                {
                                    if (valorPropriedade is BaseDominio baseDomnio)
                                    {
                                        if (this.IsExisteBaseDominio(baseDomnio))
                                        {
                                            var baseDominioReferencia = this.RetornarBaseDominioReferencia((BaseDominio)valorPropriedade);
                                            this.ObjetosAnalisados.Add(baseDominioReferencia.__IdentificadorUnico, baseDominioReferencia);
                                            ReflexaoUtil.AtribuirValorPropriedade(objeto, propriedade, baseDominioReferencia);
                                        }
                                        this.PrepararObjeto(valorPropriedade);
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
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SubtiturirBaseDominioReferenciaColecaoLista(IList lista)
        {
            for (int i = 0; i <= lista.Count - 1; i++)
            {
                var item = lista[i];
                if (item is BaseDominio baseDominio && this.IsExisteBaseDominio(baseDominio))
                {
                    lista[i] = this.RetornarBaseDominioReferencia((BaseDominio)item);
                }
                else
                {
                    this.PrepararObjeto(item);
                }
            }
        }

        private void SubtiturirBaseDominioReferenciaColecaoDictionario(IDictionary dicionario)
        {
            foreach (var chave in dicionario.Keys)
            {
                var item = dicionario[chave];
                if (item is BaseDominio baseDominio)
                {
                    if (item is BaseDominio && this.IsExisteBaseDominio(baseDominio))
                    {
                        dicionario[chave] = this.RetornarBaseDominioReferencia((BaseDominio)item); ;
                    }
                    else
                    {
                        this.PrepararObjeto(item);
                    }
                }
            }
        }
        private void SubtiturirBaseDominioReferenciaColecaoHashSet(IEnumerable colecaoHashSet)
        {
            foreach (var item in colecaoHashSet)
            {
                if (item is BaseDominio baseDominio && this.IsExisteBaseDominio(baseDominio))
                {
                    (colecaoHashSet as dynamic).Remove(item);
                    (colecaoHashSet as dynamic).Add(this.RetornarBaseDominioReferencia((BaseDominio)item));
                }
                else
                {
                    this.PrepararObjeto(item);
                }
            }
        }
        //private IList RetornarLista(ICollection colecao)
        //{
        //    var tipoColecao = colecao.GetType();
        //    if (tipoColecao.IsGenericType)
        //    {
        //        if (tipoColecao.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        //        {
        //            var dicionario = (IDictionary)colecao;
        //            var lista = new List<object>();
        //            foreach (var valor in dicionario.Values)
        //            {
        //                lista.Add(valor);
        //            }
        //            return lista;
        //        }
        //    }
        //    return (IList)colecao;

        //}

        public bool IsExisteBaseDominio(BaseDominio baseDominio)
        {
            if (baseDominio.__BaseDominioReferencia)
            {
                return this.BasesDominio.ContainsKey(baseDominio.__IdentificadorReferencia.Value);
            }
            return this.BasesDominio.ContainsKey(baseDominio.__IdentificadorUnico);
        }
        #region IDisposable

        private bool disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.ObjetosAnalisados.Clear();
                    this.BasesDominio.Clear();
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