using Newtonsoft.Json;
using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    //public class Margem : BaseTipoComplexo<Margem>
    public partial class Margem : BaseMedidaTipoComplexo, IMargem
    {
        public static Margem Empty => new Margem(null, null, null, null);

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public int? EsquerdaVisualizacao { get => this.RetornarValorVisualizacao(this._esquerda); }

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public int? SuperiorVisualizacao { get => this.RetornarValorVisualizacao(this._superior); }

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public int? DireitaVisualizacao { get => this.RetornarValorVisualizacao(this._direita); }

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public int? InferiorVisualizacao { get => this.RetornarValorVisualizacao(this._inferior); }

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public bool IsExisteMargem
        {
            get
            {
                return this.Esquerda != null ||
                       this.Direita != null ||
                       this.Superior != null ||
                       this.Inferior != null;
            }
        }
        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public override bool IsEmpty
        {
            get
            {
                return this.Esquerda == null &&
                       this.Direita == null &&
                       this.Superior == null &&
                       this.Inferior == null;
            }
        }
    }
}