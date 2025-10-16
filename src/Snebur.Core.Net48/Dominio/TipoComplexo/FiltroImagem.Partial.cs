using System;

namespace Snebur.Dominio
{
    public partial class FiltroImagem : BaseTipoComplexo
    {
        public bool IsExistExposicao => !this.IsNullOuPadrao(this.Exposicao, EXPOSICAO_PADRAO);

        public bool IsExisteBrilho => !this.IsNullOuPadrao(this.Brilho, BRILHO_PADRAO);

        public bool IsExisteContraste => !this.IsNullOuPadrao(this.Contraste, CONTRASTE_PADRAO);

        public bool IsExisteSaturacao => !this.IsNullOuPadrao(this.Saturacao, SATURACAO_PADRAO);

        public bool IsExistePetroBranco => !this.IsNullOuPadrao(this.PretoBranco, PRECO_BRANCO_PADRAO);

        public bool IsExisteSepia => !this.IsNullOuPadrao(this.Sepia, SEPIA_PADRFAO);

        public bool IsExisteMatrix => !this.IsNullOuPadrao(this.Matriz, MATRIZ_PADRAO);

        public bool IsExisteInverter => !this.IsNullOuPadrao(this.Inverter, INVERTER_PADRAO);

        public bool IsExisteDesfoque => !this.IsNullOuPadrao(this.Desfoque, DESFOQUE_PADRAO);

        public bool IsExisteCiano => !this.IsNullOuPadrao(this.Ciano, CIANO_PADRAO);

        public bool IsExisteMagenta => !this.IsNullOuPadrao(this.Magenta, MAGENTA_PADRAO);

        public bool IsExisteAmarelo => !this.IsNullOuPadrao(this.Amarelo, AMARELO_PADRAO);

        public bool IsExisteExposicao => !this.IsNullOuPadrao(this.Exposicao, EXPOSICAO_PADRAO);

        public bool IsExisteExposicaoExtra => this.IsExisteExposicao && Math.Abs(this.Exposicao.Value) > 50;

        public bool IsExisteAjuste => this.IsExisteAjusteCor || this.IsExisteAjusteComum;

        public bool IsExisteAjusteCor
        {
            get
            {
                return this.IsExisteCiano ||
                       this.IsExisteMagenta ||
                       this.IsExisteAmarelo ||
                       this.IsExistExposicao;
            }
        }

        public bool IsExisteAjusteComum
        {
            get
            {
                return this.IsExisteBrilho ||
                       this.IsExisteContraste ||
                       this.IsExisteSaturacao ||
                       this.IsExistePetroBranco ||
                       this.IsExisteSepia ||
                       this.IsExisteMatrix ||
                       this.IsExisteInverter ||
                       this.IsExisteDesfoque;
            }
        }

        private bool IsNullOuPadrao<T>(T? valor, T padrao) where T : struct
        {
            return valor == null || valor.Value.Equals(padrao);
        }
    }
}
