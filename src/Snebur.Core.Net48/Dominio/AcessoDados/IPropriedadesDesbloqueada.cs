namespace Snebur.Dominio
{
    public interface IPropriedadesDesbloqueada
    {
        string NomeEntidade { get; set; }

        string NomeProprieade { get; set; }

        EnumTipoDesbloqueio EnumTipoDesbloqueio { get; set; }
    }
}
