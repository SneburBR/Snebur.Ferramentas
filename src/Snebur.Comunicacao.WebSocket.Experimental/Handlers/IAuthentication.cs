using Snebur.Comunicacao.WebSocket.Experimental.Classes;

namespace Snebur.Comunicacao.WebSocket.Experimental.Handlers
{
    internal interface IAuthentication
    {
        void Authenticate(ConexaoContexto context);
    }
}