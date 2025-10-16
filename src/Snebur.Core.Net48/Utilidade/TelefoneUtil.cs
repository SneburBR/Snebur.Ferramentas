using System.Collections.Generic;
using System.Linq;

namespace Snebur.Utilidade
{

    public static partial class TelefoneUtil
    {
        private static readonly List<DDD> Ddds = new List<DDD> {

                new DDD(11,  "São Paulo ", "São Paulo/Jundiaí/Bragança Paulista"),
                new DDD(12,  "São Paulo ", "São José dos Campos/Vale do Paraíba"),
                new DDD(13,  "São Paulo ", "Santos/Baixada Santista/Vale do Ribeira"),
                new DDD(14,  "São Paulo ", "Bauru/Marília/Jaú/Botucatu"),
                new DDD(15,  "São Paulo ", "Sorocaba/Itapetininga"),
                new DDD(16,  "São Paulo ", "Ribeirão Preto/Araraquara/São Carlos"),
                new DDD(17,  "São Paulo ", "São José do Rio Preto/Barretos"),
                new DDD(18,  "São Paulo ", "Presidente Prudente/Araçatuba/Assis"),
                new DDD(19,  "São Paulo ", "Campinas/Região Metropolitana/Piracicaba"),
                new DDD(21,  "Rio de Janeiro",      "Rio de Janeiro, Região Metropolitana e Teresópolis"),
                new DDD(22,  "Rio de Janeiro",      "Campos dos Goytacazes/Nova Friburgo/Macaé/Cabo Frio"),
                new DDD(24,  "Rio de Janeiro",      "Petrópolis/Volta Redonda/Angra dos Reis"),
                new DDD(27,  "Espírito Santo",      "Vitória e Região Metropolitana"),
                new DDD(28,  "Espírito Santo",      "Mesoreregião Sul do Espírito Santo"),
                new DDD(31,  "Minas Gerais",        "Belo Horizonte, Região Metropolitana e Vale do Aço"),
                new DDD(32,  "Minas Gerais",        "Juiz de Fora/São João Del Rei"),
                new DDD(33,  "Minas Gerais",        "Governador Valadares/Teófilo Otoni/Caratinga/Manhuaçu"),
                new DDD(34,  "Minas Gerais",        "Uberlândia e Triângulo Mineiro"),
                new DDD(35,  "Minas Gerais",        "Poços de Caldas/Pouso Alegre/Varginha"),
                new DDD(37,  "Minas Gerais",        "Divinópolis/Itaúna"),
                new DDD(38,  "Minas Gerais",        "Montes Claros/Diamantina/Noroeste de Minas"),
                new DDD(41,  "Paraná" ,              "Curitiba e Região Metropolitana"),
                new DDD(42,  "Paraná" ,              "Ponta Grossa/Guarapuava"),
                new DDD(43,  "Paraná" ,              "Londrina/Apucarana"),
                new DDD(44,  "Paraná" ,              "Maringá/Campo Mourão/Umuarama"),
                new DDD(45,  "Paraná" ,              "Cascavel/Foz do Iguaçu"),
                new DDD(46,  "Paraná" ,             "Francisco Beltrão/Pato Branco"),
                new DDD(47,  "Santa Catarina", "Joinville/Blumenau/Itajaí/Balneário Camboriú"),
                new DDD(48,  "Santa Catarina", "Florianópolis e Região Metropolitana/Criciúma/Tubarão"),
                new DDD(49,  "Santa Catarina", "Chapecó/Lages/Caçador"),
                new DDD(51,  "Rio Grande do Sul", "Porto Alegre e Região Metropolitana/Santa Cruz do Sul/Litoral Norte"),
                new DDD(53,  "Rio Grande do Sul", "Pelotas/Rio Grande"),
                new DDD(54,  "Rio Grande do Sul", "Caxias do Sul/Passo Fundo"),
                new DDD(55,  "Rio Grande do Sul", "Santa Maria/Uruguaiana/Santana do Livramento/Santo Ângelo"),
                new DDD(61,  "Distrito Federal" , "Goiás Abrangência em todo o Distrito Federal e municípios goianos da Região Integrada de Desenvolvimento do Distrito Federal e Entorno"),
                new DDD(62,  "Goiás", "Goiânia e Região Metropolitana/Anápolis/Niquelândia/Porangatu"),
                new DDD(63,  "Tocantins", "Abrangência em todo o estado"),
                new DDD(64,  "Goiás", "Rio Verde/Itumbiara/Caldas Novas/Catalão"),
                new DDD(65,  "Mato Grosso", "Cuiabá e Região Metropolitana"),
                new DDD(66,  "Mato Grosso", "Rondonópolis/Sinop"),
                new DDD(67,  "Mato Grosso do Sul", "Abrangência em todo o estado "),
                new DDD(68,  "Acre", "Abrangência em todo o estado"),
                new DDD(69,  "Rondônia", "Abrangência em todo o estado"),
                new DDD(71,  "Bahia", "Salvador e Região Metropolitana"),
                new DDD(73,  "Bahia", "Itabuna/Ilhéus"),
                new DDD(74,  "Bahia", "Juazeiro"),
                new DDD(75,  "Bahia", "Feira de Santana/Alagoinhas"),
                new DDD(77,  "Bahia", "Vitória da Conquista/Barreiras"),
                new DDD(79,  "Sergipe", "Abrangência em todo o estado"),
                new DDD(81,  "Pernambuco", "Recife e Região Metropolitana/Caruaru"),
                new DDD(82,  "Alagoas", "Abrangência em todo o estado"),
                new DDD(83,  "Paraíba", "Abrangência em todo o estado"),
                new DDD(84,  "Rio Grande do Norte", "Abrangência em todo o estado"),
                new DDD(85,  "Ceará", "Fortaleza e Região Metropolitana"),
                new DDD(86,  "Piauí", "Teresina e Região Metropolitana/Parnaíba"),
                new DDD(87,  "Pernambuco", "Petrolina/Garanhuns/Serra Talhada/Salgueiro"),
                new DDD(88,  "Ceará", "Juazeiro, do Norte/Sobral"),
                new DDD(89,  "Piauí", "Picos/Floriano"),
                new DDD(91,  "Pará", "Belém/Região Metropolitana"),
                new DDD(92,  "Amazonas", "Manaus/Região Metropolitana/Parintins"),
                new DDD(93,  "Pará", "Santarém/Altamira"),
                new DDD(94,  "Pará", "Marabá"),
                new DDD(95,  "Roraima", "Abrangência em todo o estado"),
                new DDD(96,  "Amapá", "Abrangência em todo o estado"),
                new DDD(97,  "Amazonas", "Abrangência no interior do estado"),
                new DDD(98,  "Maranhão", "São Luís e Região Metropolitana"),
                new DDD(99,  "Maranhão", "Imperatriz/Caxias/Codó")

             };

        public static Dictionary<int, DDD> DicionariosDDD { get; } = Ddds.ToDictionary(x => x.Ddd);
        public static Dictionary<int, DDD> DicionariosDDI { get; } = Ddds.ToDictionary(x => x.Ddd);
    }

    public class DDD
    {
        public int Ddd { get; }
        public string Estado { get; }

        public List<string> Cidades { get; } = new List<string>();

        public DDD(int ddd, string estado, string cidades)
        {
            this.Ddd = ddd;
            this.Estado = estado;
            this.Cidades.AddRange(cidades.Split('/'));
        }
    }
}