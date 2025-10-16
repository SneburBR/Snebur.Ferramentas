using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Snebur.Serializacao
{
    public class PrapararSerializacao : IDisposable
    {
        private const int MAXIMO = 100000000;
        private object Objeto;
        private int Contador = 1;

        private HashSet<Guid> ObjetosBaseDominioAnalisados = new HashSet<Guid>();
        private HashSet<int> ObjetosAnalisados = new HashSet<int>();

        private Dictionary<Guid, BaseDominioOrigem> BasesDominioOrigem = new Dictionary<Guid, BaseDominioOrigem>();
        private Dictionary<Guid, List<BaseDominioRefenciada>> BasesDominioReferenciadas = new Dictionary<Guid, List<BaseDominioRefenciada>>();

        private EnumTipoSerializacao TipoSerializacao;
        public PrapararSerializacao(object objeto, 
                                   EnumTipoSerializacao tipoSerializacao)
        {
            this.Objeto = objeto;
            this.TipoSerializacao = tipoSerializacao;
        }

        internal void Preparar()
        {
            this.PopularReferencias();
            this.ReferenciarObjetos();
        }

        internal void Normalizar()
        {
            foreach (var (identificadorReferencia, basesNegocioRefernciada) in this.BasesDominioReferenciadas.Select(x => (x.Key, x.Value)))
            {
                var baseOrigem = this.BasesDominioOrigem[identificadorReferencia].BaseDominio;
                foreach (var baseNegocioRefernciada in basesNegocioRefernciada)
                {
                    this.SubstiuirReferencia(baseNegocioRefernciada.Referencia, baseOrigem);
                }
            }
            foreach (var baseOrigem in this.BasesDominioOrigem.Values)
            {
                baseOrigem.BaseDominio.IsSerializando = false;
            }
        }

        private void ReferenciarObjetos()
        {
            foreach (var basesDominioOrigem in this.BasesDominioOrigem.Values)
            {
                var baseDominio = basesDominioOrigem.BaseDominio;
                var referencias = basesDominioOrigem.Referencias;

                if (referencias.Count > 1)
                {
                    var refenciaOrigem = referencias.OfType<ReferenciaRaiz>().FirstOrDefault() ??
                                                     referencias.First();

                    basesDominioOrigem.ReferenciaOrigem = refenciaOrigem;
                    referencias.Remove(refenciaOrigem);

                    foreach (var referencia in referencias)
                    {
                        this.Referenciar(baseDominio, referencia);
                    }
                }
                else
                {
                    //baseDominio.LimparRefencia();
                    //baseDominio.__IdentificadorUnico = null;
                }
            }
        }

        private void Referenciar(IBaseDominioReferencia baseDominio, Referencia referencia)
        {
            var baseDominioReferencia = this.RetornarNovaBaseDominioReferencia(baseDominio, referencia);
            var identificadorReferencia = baseDominioReferencia.IdentificadorReferencia;
            if (!this.BasesDominioReferenciadas.ContainsKey(identificadorReferencia))
            {
                this.BasesDominioReferenciadas.Add(identificadorReferencia, new List<BaseDominioRefenciada>());
            }
            this.BasesDominioReferenciadas[identificadorReferencia].Add(baseDominioReferencia);
            this.SubstiuirReferencia(referencia, baseDominioReferencia.BaseDominio);
        }

        private BaseDominioRefenciada RetornarNovaBaseDominioReferencia(IBaseDominioReferencia baseDominio, Referencia referencia)
        {
            var identificadorReferencia = baseDominio.RetornarIdentificadorReferencia();

            var baseDominioRerefencia = Activator.CreateInstance(baseDominio.GetType()) as IBaseDominioReferencia;
            baseDominioRerefencia.__IsBaseDominioReferencia = true;
            baseDominioRerefencia.__IdentificadorReferencia = identificadorReferencia;

            return new BaseDominioRefenciada
            {
                BaseDominio = baseDominioRerefencia,
                Referencia = referencia,
                IdentificadorReferencia = identificadorReferencia
            };
        }

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

        private void AdicioanrReferenciaRaiz(BaseDominio baseDominio)
        {
            this.AdicionarReferenciarBaseOrigem(baseDominio, new ReferenciaRaiz());
        }

        private void AdicioanrReferenciaPropriedade(BaseDominio baseDominio, object objeto, PropertyInfo proprieade)
        {
            var referenciaPropriedade = new ReferenciaPropriedade
            {
                ObjetoPai = objeto,
                Propriedade = proprieade,
            };
            this.AdicionarReferenciarBaseOrigem(baseDominio, referenciaPropriedade);
        }

        private void AdicioanrReferenciaDicionario(BaseDominio baseDominio, IDictionary dicionario, object chave)
        {
            var referenciaColecao = new ReferenciaDicionario
            {
                Dicionario = dicionario,
                Chave = chave
            };
            this.AdicionarReferenciarBaseOrigem(baseDominio, referenciaColecao);
        }

        private void AdicioanrReferenciaColecao(BaseDominio baseDominio, IList colecao, int posicao)
        {
            var referenciaColecao = new ReferenciaColecao
            {
                Colecao = colecao,
                Posicao = posicao
            };
            this.AdicionarReferenciarBaseOrigem(baseDominio, referenciaColecao);
        }

        private void AdicionarReferenciarBaseOrigem(BaseDominio baseDominio, Referencia referencia)
        {
            baseDominio.IsSerializando = true;

            var identificadorReferencia = baseDominio.RetornarIdentificadorReferencia();
            if (!this.BasesDominioOrigem.ContainsKey(identificadorReferencia))
            {
                this.BasesDominioOrigem.Add(identificadorReferencia, new BaseDominioOrigem(baseDominio));
            }
            this.BasesDominioOrigem[identificadorReferencia].Referencias.Add(referencia);
        }
        private void SubstiuirReferencia(Referencia referencia, IBaseDominioReferencia baseDominio)
        {
            switch (referencia)
            {
                case ReferenciaColecao referenciaColecao:

                    referenciaColecao.Colecao[referenciaColecao.Posicao] = baseDominio;
                    break;

                case ReferenciaPropriedade referenciaPropriedade:

                    //var objetoPai = this.NormalizarObjetoPai(referenciaPropriedade.ObjetoPai);
                    var objetoPai = referenciaPropriedade.ObjetoPai;
                    referenciaPropriedade.Propriedade.SetValue(objetoPai, baseDominio);
                    break;

                case ReferenciaDicionario referenciaDicionario:

                    referenciaDicionario.Dicionario[referenciaDicionario.Chave] = baseDominio;
                    break;

                case ReferenciaRaiz referenciaRaiz:

                    
                    throw new Erro("Referencia do tipo Raiz não pode ser referenciada");

                default:

                    throw new Erro("Referencia não suportad");
            }
        }
        //private object NormalizarObjetoPai(object objeto)
        //{
        //    if (objeto is IBaseDominioReferencia baseReferencia)
        //    {
        //        var identificadorReferencia = baseReferencia.RetornarIdentificadorReferencia();
        //        if (this.BasesDominioOrigem.ContainsKey(identificadorReferencia))
        //        {
        //            return this.BasesDominioOrigem[identificadorReferencia].BaseDominio;
        //        }
        //    }
        //    return objeto;
        //}

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
            this.ObjetosAnalisados = null;
            this.BasesDominioOrigem = null;
            this.BasesDominioReferenciadas = null;
        }
    }
}