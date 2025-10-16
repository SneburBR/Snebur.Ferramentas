using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoConverterAttribute : Attribute, IValorPadrao
    {
        private static object _bloqueio = new object();

        private IConverterValorPadrao _instancia;

        public bool IsValorPadraoOnUpdate { get; set; } 

        public string CaminhoTipo { get; }

        public IConverterValorPadrao InstanciaConverter
        {
            get
            {
                if (this._instancia == null)
                {
                    lock (_bloqueio)
                    {
                        if (this._instancia == null)
                        {
                            this._instancia = this.RetornarNovaInstancia();
                        }
                    }
                }
                return this._instancia;
            }
        }
        public ValorPadraoConverterAttribute(string caminhoTipo)
        {
            this.CaminhoTipo = this.NormalizarCaminhoTipo(caminhoTipo);
        }

        private string NormalizarCaminhoTipo(string caminhoTipo)
        {
            return caminhoTipo;
        }
        #region IValorPadrao 

        public bool IsTipoNullableRequerido { get; } = false;

        public object RetornarValorPadrao(object contexto, 
                                          Entidade entidadeCorrente, 
                                          object valorPropriedade)
        {
            return this.InstanciaConverter.RetornarValorPadrao(contexto,
                                                               entidadeCorrente, 
                                                               valorPropriedade);
        }
        #endregion

        private IConverterValorPadrao RetornarNovaInstancia()
        {
            var instanca = this.RetornarNovaInstanciaInterno();
            if (instanca is IConverterValorPadrao instanciaConverer)
            {
                return instanciaConverer;
            }
            throw new Erro($"A instancia do tipo {instanca.GetType().Name} não implementa a interface {nameof(IValorPadrao)}");
        }

        private object RetornarNovaInstanciaInterno()
        {
            
            var tipo = Type.GetType(this.CaminhoTipo);
            if (tipo == null)
            {
                throw new Erro($"O tipo {this.CaminhoTipo} não foi encontrado");
            }
            try
            {
                return Activator.CreateInstance(tipo);
            }
            catch (Exception ex)
            {
                throw new Erro($"Não foi possivel instanciar o tipo {tipo.Name}", ex);
            }
        }
    }
}