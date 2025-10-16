using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Snebur.Serializacao
{
    public class NormalizarDeserializacao : IDisposable
    {
        private const int MAXIMO = 100000000;
        private int Contador = 1;

        private string Json;
        public object Objeto;

        private HashSet<Guid> ObjetosBaseDominioAnalisados = new HashSet<Guid>();
        private HashSet<int> ObjetosAnalisados = new HashSet<int>();

        private Dictionary<Guid, IBaseDominioReferencia> BasesDominioOrigem = new Dictionary<Guid, IBaseDominioReferencia>();
        private List<BaseDominioRefenciada> BasesDominioReferenciadas = new List<BaseDominioRefenciada>();

        private EnumTipoSerializacao TipoSerializacao;
        public NormalizarDeserializacao(string json, 
                                        object objeto,
                                        EnumTipoSerializacao tipoSerializacao)
        {
            this.Json = json;
            this.Objeto = objeto;
            this.TipoSerializacao = tipoSerializacao;
        }

        internal void Normalizar()
        {
            this.PopularReferencias();
            foreach (var baseNegocioRefernciada in this.BasesDominioReferenciadas)
            {
                if (!this.BasesDominioOrigem.ContainsKey(baseNegocioRefernciada.IdentificadorReferencia))
                {
                    throw new ErroSerializacao(this.Json, new Erro("A base de origem não foi encontrada na deserilizacao"));
                }
                var baseOrigem = this.BasesDominioOrigem[baseNegocioRefernciada.IdentificadorReferencia]; ;
                this.SubstiuirReferencia(baseNegocioRefernciada.Referencia, baseOrigem);
            }
        }

        private void SubstiuirReferencia(Referencia referencia, IBaseDominioReferencia baseDominio)
        {
            switch (referencia)
            {
                case ReferenciaColecao referenciaColecao:

                    referenciaColecao.Colecao[referenciaColecao.Posicao] = baseDominio;
                    break;

                case ReferenciaDicionario referenciaDicionario:

                    throw new NotImplementedException();

                case ReferenciaPropriedade referenciaPropriedade:

                    //var objetoPai = this.NormalizarObjetoPai(referenciaPropriedade.ObjetoPai);
                    var objetoPai = referenciaPropriedade.ObjetoPai;
                    referenciaPropriedade.Propriedade.SetValue(objetoPai, baseDominio);
                    break;

                case ReferenciaRaiz referenciaRaiz:

                    throw new Erro("Referencia do tipo Raiz não pode ser referenciada");

                default:

                    throw new Erro("Referencia não suportada");
            }
        }
        //private object NormalizarObjetoPai(object objeto)
        //{
        //    if (objeto is IBaseDominioReferencia baseReferencia)
        //    {
        //        var identificadorReferencia = baseReferencia.RetornarIdentificadorReferencia();
        //        if (this.BasesDominioOrigem.ContainsKey(identificadorReferencia))
        //        {
        //            return this.BasesDominioOrigem[identificadorReferencia];
        //        }
        //    }
        //    return objeto;
        //}

        private void PopularReferencias()
        {
            if (this.Objeto is BaseDominio baseDominio)
            {
                this.AdicioanrReferenciaRaiz(baseDominio);
            }
            this.VarrerObjeto(this.Objeto);
        }

        private void VarrerObjeto(object objeto)
        {
            this.Contador += 1;

            if (this.Contador > MAXIMO)
            {
                throw new Exception("Falha ao preparar objeto para serialização, o numero maximo de analise foi atingido");
            }
            if (objeto == null)
            {
                return;
            }
            var tipo = objeto.GetType();
            if (tipo.IsValueType || this.IsObjetoAnalisado(objeto) || tipo == typeof(string))
            {
                return;
            }
            this.AdicionarObjetoAnalizado(objeto);
            if (objeto is IList colecao)
            {
                if (colecao.Count > 0)
                {
                    var tipoItem = ReflexaoUtil.RetornarTipoGenericoColecao(colecao.GetType());
                    if (tipoItem.IsValueType || tipoItem == typeof(string))
                    {
                        return;
                    }
                    for (var posicao = 0; posicao < colecao.Count; posicao++)
                    {
                        var item = colecao[posicao];
                        if (item is BaseDominio baseDominio)
                        {
                            this.AdicioanrReferenciaColecao(baseDominio, colecao, posicao);
                        }
                        this.VarrerObjeto(item);
                    }
                }
            }
            else if (objeto is IDictionary dicionario)
            {
                foreach (var chave in dicionario.Keys)
                {
                    var item = dicionario[chave];
                    if (item is BaseDominio baseDominio)
                    {
                        this.AdicioanrReferenciaDicionario(baseDominio, dicionario, chave);
                    }
                    else
                    {
                        break;
                    }
                    this.VarrerObjeto(item);
                }
            }
            else
            {
                var propriedades = JsonUtil.RetornarPropriedadesSerializavel(tipo, false, this.TipoSerializacao);
                foreach (PropertyInfo proprieade in propriedades)
                {
                    var item = proprieade.TryGetValueOrDefault(objeto);
                    if (item is BaseDominio baseDominio)
                    {
                        this.AdicioanrReferenciaPropriedade(baseDominio, objeto, proprieade);
                    }
                    this.VarrerObjeto(item);
                }
            }
        }
        private void AdicioanrReferenciaRaiz(IBaseDominioReferencia baseDominio)
        {
            this.AdicioanrReferencia(baseDominio, new ReferenciaRaiz());
        }

        private void AdicioanrReferenciaPropriedade(IBaseDominioReferencia baseDominio, object objeto, PropertyInfo proprieade)
        {
            var referenciaPropriedade = new ReferenciaPropriedade
            {
                ObjetoPai = objeto,
                Propriedade = proprieade,
            };
            this.AdicioanrReferencia(baseDominio, referenciaPropriedade);
        }

        private void AdicioanrReferenciaDicionario(IBaseDominioReferencia baseDominio, IDictionary dicionario, object chave)
        {
            var referenciaColecao = new ReferenciaDicionario
            {
                Dicionario = dicionario,
                Chave = chave
            };
            this.AdicioanrReferencia(baseDominio, referenciaColecao);
        }
        private void AdicioanrReferenciaColecao(IBaseDominioReferencia baseDominio, IList colecao, int posicao)
        {
            var referenciaColecao = new ReferenciaColecao
            {
                Colecao = colecao,
                Posicao = posicao
            };
            this.AdicioanrReferencia(baseDominio, referenciaColecao);
        }
        private void AdicioanrReferencia(IBaseDominioReferencia baseDominio,
                                         Referencia referencia)
        {
            if (baseDominio.__IsBaseDominioReferencia.GetValueOrDefault())
            {
                this.BasesDominioReferenciadas.Add(new BaseDominioRefenciada
                {
                    BaseDominio = baseDominio,
                    IdentificadorReferencia = baseDominio.__IdentificadorReferencia.Value,
                    Referencia = referencia
                });
            }
            else
            {
                if (!this.BasesDominioOrigem.ContainsKey(baseDominio.__IdentificadorUnico))
                {
                    this.BasesDominioOrigem.Add(baseDominio.__IdentificadorUnico, baseDominio);
                }
            }
        }

        private bool IsObjetoAnalisado(object objeto)
        {
            if (objeto is IBaseDominioReferencia baseDominio)
            {
                return this.ObjetosBaseDominioAnalisados.Contains(baseDominio.__IdentificadorUnico);
            }
            return this.ObjetosAnalisados.Contains(objeto.GetHashCode());
        }

        private void AdicionarObjetoAnalizado(object objeto)
        {
            if (objeto is IBaseDominioReferencia baseDominio)
            {
                this.ObjetosBaseDominioAnalisados.Add(baseDominio.__IdentificadorUnico);
            }
            this.ObjetosAnalisados.Add(objeto.GetHashCode());
        }

        public void Dispose()
        {
            this.ObjetosAnalisados.Clear();
            this.BasesDominioOrigem.Clear();
            this.BasesDominioReferenciadas.Clear();

            this.Objeto = null;
            this.Json = null;
            this.ObjetosAnalisados = null;
            this.BasesDominioOrigem = null;
            this.BasesDominioReferenciadas = null;
        }
    }
}