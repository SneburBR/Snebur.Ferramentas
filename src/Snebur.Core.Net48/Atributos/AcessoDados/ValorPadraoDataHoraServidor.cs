using System;

namespace Snebur.Dominio.Atributos
{
    /// <summary>
    /// Pendente IsPermitirAtualizacao deve atualizar quando qualquer
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoDataHoraServidorAttribute : SomenteLeituraAttribute, IValorPadrao
    {
        public bool IsDataHoraUTC { get; set; } = true;
        public bool IsAceitarAtualizacao { get; set; } = false;
        public bool IsValorPadraoOnUpdate { get; set; }
        public override OpcoesSomenteLeitura OpcoesSomenteLeitura => new OpcoesSomenteLeitura(!this.IsAceitarAtualizacao, this.IsNotificarSeguranca);

        public ValorPadraoDataHoraServidorAttribute( )
        {
            //this.IsDataHoraUTC = isDataHoraUTC;
            //this.IsAceitarAtualizacao = isAceitarAtualizacao;
            this.IsNotificarSeguranca = false;
        }

        public bool IsTipoNullableRequerido { get; } = false;

        public object RetornarValorPadrao(object contexto, 
                                          Entidade entidadeCorrente, 
                                         object valorPropriedade)
        {
            if(!entidadeCorrente.__IsNewEntity)
            {
                return this.IsDataHoraUTC ? DateTime.UtcNow : DateTime.Now;
            }
            return null;
        }
    }
}
