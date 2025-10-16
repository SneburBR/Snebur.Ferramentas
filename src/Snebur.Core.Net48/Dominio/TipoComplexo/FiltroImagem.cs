using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    [IgnorarClasseTS]
    public partial class FiltroImagem : BaseTipoComplexo, IFiltroImagem
    {
        public const double CONTRASTE_PADRAO = 100;
        public const double BRILHO_PADRAO = 100;
        public const double SATURACAO_PADRAO = 100;
        public const double SEPIA_PADRFAO = 0;
        public const double PRECO_BRANCO_PADRAO = 0;
        public const double INVERTER_PADRAO = 0;
        public const double MATRIZ_PADRAO = 0;
        public const double DESFOQUE_PADRAO = 0;

        public const double EXPOSICAO_PADRAO = 0;
        public const double CIANO_PADRAO = 0;
        public const double MAGENTA_PADRAO = 0;
        public const double AMARELO_PADRAO = 0;

        private double? _exposicao = null;
        private double? _ciano = null;
        private double? _magenta = null;
        private double? _amarelo = null;

        private double? _contraste = null;
        private double? _brilho = null;
        private double? _saturacao = null;
        private double? _sepia = null;
        private double? _pretoBranco = null;
        private double? _inverter = null;
        private double? _matriz = null;
        private double? _desfoque = null;

        [ValidacaoIntervalo(-100, 100)]
        public double? Exposicao { get => this._exposicao; set => this.NotificarValorPropriedadeAlterada(this._exposicao, this._exposicao = (value == EXPOSICAO_PADRAO) ? null : value); }

        [ValidacaoIntervalo(-100, 100)]
        public double? Magenta { get => this._magenta; set => this.NotificarValorPropriedadeAlterada(this._magenta, this._magenta = (value == MAGENTA_PADRAO) ? null : value); }

        [ValidacaoIntervalo(-100, 100)]
        public double? Ciano { get => this._ciano; set => this.NotificarValorPropriedadeAlterada(this._ciano, this._ciano = (value == CIANO_PADRAO) ? null : value); }

        [ValidacaoIntervalo(-100, 100)]
        public double? Amarelo { get => this._amarelo; set => this.NotificarValorPropriedadeAlterada(this._amarelo, this._amarelo = (value == AMARELO_PADRAO) ? null : value); }

        [ValidacaoIntervalo(0, 200)]
        public double? Contraste { get => this._contraste; set => this.NotificarValorPropriedadeAlterada(this._contraste, this._contraste = (value == CONTRASTE_PADRAO) ? null : value); }

        [ValidacaoIntervalo(0, 200)]
        public double? Brilho { get => this._brilho; set => this.NotificarValorPropriedadeAlterada(this._brilho, this._brilho = (value == BRILHO_PADRAO) ? null : value); }

        [ValidacaoIntervalo(0, 100)]
        public double? Sepia { get => this._sepia; set => this.NotificarValorPropriedadeAlterada(this._sepia, this._sepia = (value == SEPIA_PADRFAO) ? null : value); }

        [ValidacaoIntervalo(0, 200)]
        public double? Saturacao { get => this._saturacao; set => this.NotificarValorPropriedadeAlterada(this._saturacao, this._saturacao = (value == SATURACAO_PADRAO) ? null : value); }

        [ValidacaoIntervalo(0, 100)]
        public double? PretoBranco { get => this._pretoBranco; set => this.NotificarValorPropriedadeAlterada(this._pretoBranco, this._pretoBranco = (value == PRECO_BRANCO_PADRAO) ? null : value); }

        [ValidacaoIntervalo(0, 100)]
        public double? Inverter { get => this._inverter; set => this.NotificarValorPropriedadeAlterada(this._inverter, this._inverter = (value == INVERTER_PADRAO) ? null : value); }

        [ValidacaoIntervalo(0, 360)]
        public double? Matriz { get => this._matriz; set => this.NotificarValorPropriedadeAlterada(this._matriz, this._matriz = (value == MATRIZ_PADRAO) ? null : value); }

        [ValidacaoIntervalo(0, 10)]
        public double? Desfoque { get => this._desfoque; set => this.NotificarValorPropriedadeAlterada(this._desfoque, this._desfoque = (value == DESFOQUE_PADRAO) ? null : value); }

        public static FiltroImagem Empty
        {
            get
            {
                return new FiltroImagem();
            }
        }

        public FiltroImagem()
        {

        }

        protected internal override BaseTipoComplexo BaseClone()
        {
            return new FiltroImagem
            {
                Exposicao = this.Exposicao,
                Magenta = this.Magenta,
                Ciano = this.Ciano,
                Amarelo = this.Amarelo,
                Contraste = this.Contraste,
                Brilho = this.Brilho,
                Sepia = this.Sepia,
                Saturacao = this.Saturacao,
                PretoBranco = this.PretoBranco,
                Inverter = this.Inverter,
                Matriz = this.Matriz,
                Desfoque = this.Desfoque
            };
        }
    }

    public interface IFiltroImagem
    {
        double? Exposicao { get; set; }
        double? Magenta { get; set; }
        double? Ciano { get; set; }
        double? Amarelo { get; set; }
        double? Contraste { get; set; }
        double? Brilho { get; set; }
        double? Sepia { get; set; }
        double? Saturacao { get; set; }
        double? PretoBranco { get; set; }
        double? Inverter { get; set; }
        double? Matriz { get; set; }
        double? Desfoque { get; set; }
    }
}
