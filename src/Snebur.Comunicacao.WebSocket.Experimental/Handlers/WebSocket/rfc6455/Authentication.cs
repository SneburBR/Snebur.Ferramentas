using System;
using System.Security.Cryptography;
using System.Text;
using Snebur.Comunicacao.WebSocket.Experimental.Classes;

namespace Snebur.Comunicacao.WebSocket.Experimental.Handlers.WebSocket.rfc6455
{
    /// <summary>
    /// Handles the handshaking between the client and the host, when a new connection is created
    /// </summary>
    internal class Authentication : Handlers.Authentication
    {
        protected override bool CheckAuthentication(ConexaoContexto context)
        {
            if (context.ReceivedByteCount > 8)
            {
                var handshake = new ClientHandshake(context.Cabecalho);
                // See if our header had the required information
                if (handshake.IsValid())
                {
                    // Optionally check Origin and Location if they're set.
                    if (!String.IsNullOrEmpty(Origin))
                    {
                        var expectedOrigin = Origin;
                        if (!Origin.Contains("://"))
                        {
                            expectedOrigin = "http://" + Origin;
                        }

                        if (!handshake.Origin.Equals(expectedOrigin, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return false;
                        }
                    }
                    if (!String.IsNullOrEmpty(Destination))
                    {
                        if (handshake.Host != Destination + ":" + context.Server.Porta)
                        {
                            return false;
                        }
                    }
                    // Generate response handshake for the client
                    var serverShake = GenerateResponseHandshake(handshake, context.Server);
                    // Send the response handshake
                    SendServerHandshake(serverShake, context);
                    return true;
                }
            }
            return false;
        }

        private static ServerHandshake GenerateResponseHandshake(ClientHandshake handshake, WebSocketServer server)
        {
            var responseHandshake = new ServerHandshake {Accept = GenerateAccept(handshake.Key), Server = server, RequestProtocols = handshake.SubProtocols};
            return responseHandshake;
        }

        private static void SendServerHandshake(ServerHandshake handshake, ConexaoContexto context)
        {
            // generate a byte array representation of the handshake including the answer to the challenge
            string temp = handshake.ToString();
            byte[] handshakeBytes = Encoding.UTF8.GetBytes(temp);
            context.SessaoContexto.Send(handshakeBytes, true);
        }

        public static string GenerateAccept(string key)
        {
            if (!String.IsNullOrEmpty(key))
            {
                //var rawAnswer = key + "c6f406c2-0677-48b7-a1c9-80c457b18baa";
                var rawAnswer = key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

                // Create a hash of the rawAnswer and return it
                var hasher = SHA1.Create();
                return Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(rawAnswer)));
            }
            return String.Empty;
        }
    }
}