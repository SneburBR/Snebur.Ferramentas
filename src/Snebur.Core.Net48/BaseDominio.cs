using Newtonsoft.Json;
using Snebur.Dominio.Atributos;
using Snebur.Serializacao;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Snebur.Dominio
{
    [Plural("BasesDominio")]
    public abstract class BaseDominio : IBaseDominio, IBaseDominioReferencia, INotifyPropertyChanged, IBaseDominioControladorPropriedade
    {
        //private Guid? __identificadorReferenciaInterno;
        private bool _isSerializando;

        //flag exclusiva do serializador
        //private bool __isControleProprieadesDestativado;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [IgnorarGlobalizacao]
        [NaoMapear]
        [IgnorarPropriedadeTSReflexao]
        [PropriedadeProtegida]
        //[JsonProperty]
        public string __CaminhoTipo { get; internal protected set; }

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[IgnorarGlobalizacao]
        //[NaoMapear]
        //[IgnorarPropriedadeTS]
        //[IgnorarPropriedadeTSReflexao]
        //[PropriedadeProtegida]
        //public string __AssemblyQualifiedName { get; internal protected set; }

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[IgnorarGlobalizacao]
        //[NaoMapear]
        //[OcultarColuna]
        //[PropriedadeProtegida]
        //public bool __BaseDominioReferencia { get;  set; }

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[IgnorarGlobalizacao]
        //[NaoMapear]
        //[IgnorarPropriedadeTSReflexao]
        //[PropriedadeProtegida]
        //public Guid __IdentificadorUnico { get; set; }

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[IgnorarGlobalizacao]
        //[NaoMapear]
        //[IgnorarPropriedadeTSReflexao]
        //[PropriedadeProtegida]
        //public Guid? __IdentificadorReferencia
        //{
        //    get
        //    {
        //        if (__BaseDominioReferencia)
        //        {
        //            return this.__identificadorReferenciaInterno;
        //        }
        //        return null;
        //    }
        //    set
        //    {
        //        this.__identificadorReferenciaInterno = value;
        //    }
        //}

        //private bool IsEntidade { get; }

        //Usar AssemblyQualifiedName para obeter o tipo

        #region Obsoleto

        //[IgnorarGlobalizacao]
        //[NaoMapear]
        //[IgnorarPropriedadeTS]
        //[IgnorarPropriedadeTSReflexao]
        //public string NamespaceJS { get; set; }

        //[IgnorarGlobalizacao]
        //[NaoMapear]
        //[IgnorarPropriedadeTS]
        //[IgnorarPropriedadeTSReflexao]
        //public string NamespaceServidor { get; set; }

        #endregion

        #region Propriedade Alterada

        [NaoMapear]
        [IgnorarGlobalizacao]
        [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
        [PropriedadeProtegida]
        [XmlIgnore]
        [JsonIgnore]
        public virtual bool __IsExisteAlteracao
        {
            get
            {
                return (this.__PropriedadesAlteradas?.Count ?? 0) > 0;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [OcultarColuna]
        [IgnorarGlobalizacao]
        [NaoMapear]
        [PropriedadeProtegida]
        [SomenteLeitura]
        public Dictionary<string, PropriedadeAlterada> __PropriedadesAlteradas { get; set; } = null;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [NaoMapear]
        [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
        [JsonIgnore]
        public bool __IsControladorPropriedadesAlteradaAtivo { get; private set; } = false;

        [XmlIgnore]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        [JsonIgnore]
        internal protected bool IsSerializando
        {
            get => this._isSerializando;
            internal set
            {
                if (value)
                {
                    this.DestivarControladorPropriedadeAlterada();
                }
                else
                {
                    //if (this.__isControleProprieadesDestativado)
                    //{
                    //    this.AtivarControladorPropriedadeAlterada();
                    //}
                    this.AtivarControladorPropriedadeAlterada();
                }
                this._isSerializando = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore]
        [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
        [JsonIgnore]
        internal protected bool __IsClonado { get; set; }
        #endregion

        public BaseDominio()
        {
            var tipo = this.GetType();
            this.__CaminhoTipo = String.Format("{0}.{1}", tipo.Namespace, tipo.Name);
            //this.__AssemblyQualifiedName = $"{tipo.FullName}, {tipo.Assembly.GetName().Name}";
            this.__IdentificadorUnico = Guid.NewGuid();
            //this.__IdentificadorReferencia = this.__IdentificadorUnico;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AtivarControladorPropriedadeAlterada()
        {
            if (this.__PropriedadesAlteradas == null)
            {
                this.__PropriedadesAlteradas = new Dictionary<string, PropriedadeAlterada>();
            }
            this.__IsControladorPropriedadesAlteradaAtivo = true;
            //this.__isControleProprieadesDestativado = false;
        }

        private void DestivarControladorPropriedadeAlterada()
        {
            if (this.__IsControladorPropriedadesAlteradaAtivo)
            {
                //if (this.__PropriedadesAlteradas?.Count == 0)
                //{
                //    this.__PropriedadesAlteradas = null;
                //}
                //this.__isControleProprieadesDestativado = true;
                this.__IsControladorPropriedadesAlteradaAtivo = false;
            }
        }

        internal protected virtual void NotificarValorPropriedadeAlterada<T>(T antigoValor,
                                                                             T novoValor,
                                                                             [CallerMemberName] string nomePropriedade = "",
                                                                             string nomePropriedadeEntidade = null,
                                                                             string nomePropriedadeTipoComplexo = null)
        {
            if (this.IsSerializando)
            {
                return;
            }

            if (this.__IsControladorPropriedadesAlteradaAtivo)
            {
                if (this.__PropriedadesAlteradas.TryGetValue(nomePropriedade,
                                                            out PropriedadeAlterada propriedadeAlterada))
                {
                    if (!this.__IsClonado &&
                        Util.SaoIgual(propriedadeAlterada.AntigoValor, novoValor))
                    {
                        this.__PropriedadesAlteradas.Remove(nomePropriedade);
                    }
                    else
                    {
                        propriedadeAlterada.NovoValor = novoValor;
                    }
                }
                else
                {
                    if (!Util.SaoIgual(antigoValor, novoValor) || this.__IsClonado)
                    {
                        lock (this.__PropriedadesAlteradas.SyncLock())
                        {
                            if (!this.__PropriedadesAlteradas.ContainsKey(nomePropriedade))
                            {
                                var novaPropriedadeAlterada = PropriedadeAlterada.Create(nomePropriedade,
                                                                                         antigoValor,
                                                                                         novoValor,
                                                                                         nomePropriedadeEntidade,
                                                                                         nomePropriedadeTipoComplexo);

                                this.__PropriedadesAlteradas.Add(nomePropriedade, novaPropriedadeAlterada);
                            }
                        }
                    }
                }
            }
            this.NotificarPropriedadeAlterada(nomePropriedade);
        }

        internal protected virtual void NotificarValorPropriedadeAlteradaTipoCompleto(BaseTipoComplexo antigoValor, BaseTipoComplexo novoValor, [CallerMemberName] string nomePropriedade = "")
        {
            if (this.IsSerializando)
            {
                return;
            }
            this.NotificarValorPropriedadeAlterada(antigoValor, novoValor, nomePropriedade);
        }

        internal protected virtual T RetornarValorPropriedade<T>(T valor, [CallerMemberName] string nomePropriedade = "")
        {
            return valor;
        }

        #region INotifyPropertyChanged

        [NaoMapear]
        [IgnorarGlobalizacao]
        [XmlIgnore]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        [OcultarColuna]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool __AtivarNotificacaoPropriedadeAlterada { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        protected virtual void NotificarPropriedadeAlterada([CallerMemberName] string nomePropriedade = "")
        {
            if (this.__AtivarNotificacaoPropriedadeAlterada)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nomePropriedade));
            }
        }
        #endregion

        #region IBaseDominioReferencia

        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty]
        private Guid __IdentificadorUnico;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty]
        private Guid? __IdentificadorReferencia;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty]
        private bool? __IsBaseDominioReferencia;

        public Guid RetornarIdentificadorReferencia()
        {
            if (this.__IsBaseDominioReferencia.GetValueOrDefault())
            {
                return this.__IdentificadorReferencia.Value;
            }
            return this.__IdentificadorUnico;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [IgnorarGlobalizacao]
        [NaoMapear]
        [IgnorarPropriedadeTSReflexao]
        [PropriedadeProtegida]
        Guid IBaseDominioReferencia.__IdentificadorUnico => this.__IdentificadorUnico;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [IgnorarGlobalizacao]
        [NaoMapear]
        [IgnorarPropriedadeTSReflexao]
        [PropriedadeProtegida]
        Guid? IBaseDominioReferencia.__IdentificadorReferencia { get => this.__IdentificadorReferencia; set => this.__IdentificadorReferencia = value; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [IgnorarGlobalizacao]
        [NaoMapear]
        [IgnorarPropriedadeTSReflexao]
        [PropriedadeProtegida]
        bool? IBaseDominioReferencia.__IsBaseDominioReferencia { get => this.__IsBaseDominioReferencia; set => this.__IsBaseDominioReferencia = value; }
        bool IBaseDominioReferencia.IsSerializando { get => this.IsSerializando; set => this.IsSerializando = value; }
        //void IBaseDominioReferencia.LimparRefencia()
        //{
        //    this.__IdentificadorUnico = null;
        //}

        #endregion

        internal protected virtual void SetValue(object olbValue, object newValue, [CallerMemberName] string nomePropriedade = "")
        {
            this.NotificarValorPropriedadeAlterada(newValue, newValue, nomePropriedade);
        }

        #region IBaseDominioControlorPropriedade
        void IBaseDominioControladorPropriedade.DestivarControladorPropriedadeAlterada()
        {
            this.DestivarControladorPropriedadeAlterada();
        }

        void IBaseDominioControladorPropriedade.AtivarControladorPropriedadeAlterada()
        {
            this.AtivarControladorPropriedadeAlterada();
        }

        #endregion

        #region Serialização

        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            this.DestivarControladorPropriedadeAlterada();
            this.IsSerializando = true;
        }

        [OnSerialized]
        internal void OnSerializedMethod(StreamingContext context)
        {
            this.IsSerializando = false;
        }

        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            this.IsSerializando = true;
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            this.IsSerializando = false;
        }

        #endregion
    }
}