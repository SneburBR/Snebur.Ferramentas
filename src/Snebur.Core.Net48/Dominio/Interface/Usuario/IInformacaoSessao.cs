namespace Snebur.Dominio
{

    public interface IInformacaoSessao : IIdentificadorAplicacao
    {

        string Cultura { get; set; }

        string Idioma { get; set; }

        EnumPlataforma Plataforma { get; set; }

        EnumTipoAplicacao TipoAplicacao { get; set; }

        Dimensao Resolucao { get; set; }

        string UserAgent { get; set; }

        Navegador Navegador { get; set; }

        SistemaOperacional SistemaOperacional { get; set; }

        string VersaoAplicacao { get; set; }

        string NomeComputador { get; set; }

        //string IP { get; set; }

        //IIPInformacao IPInformacao { get; set; }
    }
}