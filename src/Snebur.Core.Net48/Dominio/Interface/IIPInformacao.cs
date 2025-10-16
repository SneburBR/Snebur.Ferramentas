namespace Snebur.Dominio
{
    public interface IIPInformacao
    {
        string IP { get; set; }

        /// <summary>
        /// Representa o numero da rede, as 3 primeiras partes do IP4
        /// </summary>
        string MascaraIp4 { get; set; }

        string Hostname { get; set; }

        string Cidade { get; set; }

        string Regiao { get; set; }

        string Pais { get; set; }

        string CodigoPostal { get; set; }

        Localizacao Localizacao { get; set; }

        string ProvedorInternet { get; set; }
    }
}
