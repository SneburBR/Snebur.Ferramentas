using Newtonsoft.Json;
using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public partial class Regiao : BaseMedidaTipoComplexo, IRegiao
    {
        public static Regiao Empty => new Regiao(0, 0, 0, 0);

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public int XVisualizacao { get => this.RetornarValorVisualizacao(this._x); }

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public int YVisualizacao { get => this.RetornarValorVisualizacao(this._y); }

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

        [NaoMapear]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public Posicao Posicao
        {
            get => new Posicao(this.X, this.Y);
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }
        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public Dimensao Dimensao
        {
            get => new Dimensao(this.Largura, this.Altura);
            set
            {
                this.Largura = value.Largura;
                this.Altura = value.Altura;
            }
        }
        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public override bool IsEmpty => this.Dimensao.IsEmpty && this.Posicao.IsEmpty;

    }
}