namespace Snebur.Seguranca
{
    public class CredencialAnonimo
    {

        public static CredencialUsuario Anonimo
        {
            get
            {
                return new CredencialUsuario
                {
                    Nome= "Anônimo",
                    IdentificadorUsuario = IDENTIFICADOR_USUARIO,
                    Senha = SENHA
                };
            }
        }

        private const string IDENTIFICADOR_USUARIO = "Anonimo";

        private const string SENHA = "c42b636c-97d1-4ea9-ba45-90ad98b42abb";

    }
    public class CredencialServicoAnonimo
    {
        public static CredencialServico Anonimo
        {
            get
            {
                return new CredencialServico
                {
                    IdentificadorUsuario = IDENTIFICADOR_USUARIO,
                    Senha = SENHA
                };
            }
        }

        private const string IDENTIFICADOR_USUARIO = "Anonimo";

        private const string SENHA = "c42b636c-97d1-4ea9-ba45-90ad98b42abb";

    }
}