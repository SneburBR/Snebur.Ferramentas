using Snebur.Dominio;

namespace Snebur.Imagens
{
    public class EfeitoImagem
    {
        public EnumEfeitoImagem EfeitoImagemEnum { get; }
        public FiltroImagem Filtro { get; }
        public SobrePosicao SobrePosicao { get; }

        public EfeitoImagem(EnumEfeitoImagem filtroImagemConhecidoEnum,
                                     FiltroImagem filtro)
        {
            this.EfeitoImagemEnum = filtroImagemConhecidoEnum;
            this.Filtro = filtro;
        }

        public EfeitoImagem(EnumEfeitoImagem filtroImagemConhecidoEnum,
                                     FiltroImagem filtro,
                                     SobrePosicao sobrePosicao) :
                                     this(filtroImagemConhecidoEnum, filtro)
        {
            this.SobrePosicao = sobrePosicao;
        }
    }
}
