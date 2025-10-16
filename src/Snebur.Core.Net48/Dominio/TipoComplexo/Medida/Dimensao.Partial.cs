using Newtonsoft.Json;
using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public partial class Dimensao : BaseMedidaTipoComplexo, IDimensao
    {
        public static Dimensao Empty = new Dimensao(0, 0);

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public int LarguraVisualizacao { get => this.RetornarValorVisualizacao(this._largura); }

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public int AlturaVisualizacao { get => this.RetornarValorVisualizacao(this._altura); }

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public int LarguraPixels
        {
            get
            {
                if (this.FuncaoNormalizarDpiVisualizacao != null)
                {
                    return this.LarguraVisualizacao;
                }
                return (int)this.Largura;
            }
        }
        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public int AlturaPixels
        {
            get
            {
                if (this.FuncaoNormalizarDpiVisualizacao != null)
                {
                    return this.AlturaVisualizacao;
                }
                return (int)this.Altura;
            }
        }
        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public bool IsQuadrada => this.Orientacao == EnumOrientacao.Quadrado;

        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public override bool IsEmpty => this.Largura == 0 && this.Altura == 0;

        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public EnumOrientacao Orientacao
        {
            get
            {
                if (this.Largura == this.Altura)
                {
                    return EnumOrientacao.Quadrado;
                }
                if (this.Largura > this.Altura)
                {
                    return EnumOrientacao.Horizontal;
                }
                return EnumOrientacao.Vertical;
            }
        }
    }
}