using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoNovoHashCrc32 : Attribute, IValorPadrao
    {
        private int NumeroCaracteres { get; }
        public ValorPadraoNovoHashCrc32(int numeroCaracteres)
        {
            this.NumeroCaracteres = numeroCaracteres;
        }
        public object RetornarValorPadrao(object contexto, Entidade entidade, object valorPropriedade)
        {
            //return ChecksumUtil.RetornarChecksumCrc2(Guid.NewGuid().ToByteArray());
            return Guid.NewGuid().ToString("N").RetornarPrimeirosCaracteres(this.NumeroCaracteres);
  
        }

        #region IValorPadrao 

        public bool IsTipoNullableRequerido { get; } = false;

        public bool IsValorPadraoOnUpdate { get; } = false;

        #endregion
    }
}