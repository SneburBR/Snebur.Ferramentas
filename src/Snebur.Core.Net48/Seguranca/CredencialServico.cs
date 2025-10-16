using Snebur.Dominio.Atributos;

namespace Snebur.Seguranca
{
    public class CredencialServico : Credencial
    {
        public CredencialServico()
        {
        }
        [IgnorarConstrutorTS]
        public CredencialServico(string identificadorUsuario, string senha) : base(identificadorUsuario, senha)
        {
        }
    }
}