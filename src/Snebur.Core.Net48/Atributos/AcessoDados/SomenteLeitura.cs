using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class )]
    public class SomenteLeituraAttribute : BaseAtributoDominio
    {
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public bool IsNotificarSeguranca { get; protected set; }

        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]        
        public virtual OpcoesSomenteLeitura OpcoesSomenteLeitura => new OpcoesSomenteLeitura(true, this.IsNotificarSeguranca);
    }

    public class OpcoesSomenteLeitura
    {
        public bool IsSomenteLeitura { get; } = true;
        public bool IsNotificarSeguranca { get; private set; }
        public OpcoesSomenteLeitura(bool isSomenteLeitura, bool isNotificarSeguranca = true)
        {
            this.IsSomenteLeitura = isSomenteLeitura;
            this.IsNotificarSeguranca = isNotificarSeguranca;
        }
    }
}