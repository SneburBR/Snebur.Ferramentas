using Newtonsoft.Json;
using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public partial class Posicao : BaseMedidaTipoComplexo, IPosicao
    {
        public static Posicao Empty => new Posicao(0, 0);

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
        public override bool IsEmpty => this.X == 0 && this.Y == 0;
    }
}
