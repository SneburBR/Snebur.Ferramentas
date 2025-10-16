using Newtonsoft.Json;
using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public partial class Borda : BaseMedidaTipoComplexo, IBorda
    {
        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public Cor Cor { get => new Cor(this.CorRgba); set => this.CorRgba = value.Rgba; }

        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public double AfastamentoVisualizacao { get => this.RetornarValorVisualizacao(this._afastamento); }

        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public double EspessuraVisualizacao { get => this.RetornarValorVisualizacao(this._espessura); }

        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public bool IsExisteBorda
        {
            get
            {
                return !this.Cor.IsTransparente || this.Arredondamento > 0;
            }
        }
        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public override bool IsEmpty
        {
            get
            {
                return this.Cor.IsTransparente && this.Espessura == 0 && this.Afastamento == 0;
            }
        }
    }
}