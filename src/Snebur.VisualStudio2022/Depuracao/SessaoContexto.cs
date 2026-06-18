using Fleck;
using System.Linq;
using System.Threading.Tasks;

//using Snebur.Comunicacao.WebSocket.Experimental;
//using Snebur.Comunicacao.WebSocket.Experimental.Classes;

namespace Snebur.VisualStudio
{
    public class SessaoContexto
    {
        private readonly IWebSocketConnection _socker;
        public string Identificador { get; }
        public bool Connected
            => this._socker.IsAvailable;

        public SessaoContexto(IWebSocketConnection socker)
        {
            this._socker = socker;
            this.Identificador = new string(socker.ConnectionInfo.ClientIpAddress.ToString()
                   .Where(x => Char.IsLetterOrDigit(x)).ToArray());
        }

        public void Dispose()
        {
            if (this._socker.IsAvailable)
                this._socker?.Close();
        }

        public void Send(string message)
        {
            try
            {
                _ = this._socker.Send(message);
            }
            catch
            {

            }
        }
    }
}
