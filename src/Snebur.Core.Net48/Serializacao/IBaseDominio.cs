namespace Snebur.Serializacao
{
    public interface IBaseDominio
    {

    }
    public interface IBaseDominioControladorPropriedade : IBaseDominio
    {
        void DestivarControladorPropriedadeAlterada();
        void AtivarControladorPropriedadeAlterada();
    }
}
