namespace Snebur.Dominio
{
    public interface IPropriedadeAlterada
    {
        string NomePropriedade { get; set; }
        object AntigoValor { get; set; }
        object NovoValor { get; set; }
    }
}
