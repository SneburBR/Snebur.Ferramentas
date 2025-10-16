namespace Snebur.Seguranca
{
    public class CredencialServicoIpInformacao
    {

        public static CredencialServico ServicoIpInformacao
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

        private const string IDENTIFICADOR_USUARIO = "ServicoIpInformacao";

        private const string SENHA = "1d243b64-1cff-4c7c-a152-3ae53b027d48";

    }
}
