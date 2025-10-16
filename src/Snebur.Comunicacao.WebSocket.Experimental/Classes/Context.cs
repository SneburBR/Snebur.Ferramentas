using Snebur.Comunicacao.WebSocket.Experimental.Handlers;
using Snebur.Comunicacao.WebSocket.Experimental.Handlers.WebSocket;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Snebur.Comunicacao.WebSocket.Experimental.Classes
{
    public class ConexaoContexto : IDisposable
    {
        public ulong MaxFrameSize = 102400; //100kb
        private int _bufferSize = 512;

        public readonly SessaoContexto SessaoContexto;

        public byte[] Buffer;

        public bool Connected { get; set; } = true;

        public TcpClient Connection;


        public Handler Handler = Handler.Instance;

        public Cabecalho Cabecalho;

        public bool IsSetup;

        public SemaphoreSlim ReceiveReady = new SemaphoreSlim(1);

        public int ReceivedByteCount;

        public SemaphoreSlim SendReady = new SemaphoreSlim(1);

        public WebSocketServer Server;

        public SocketAsyncEventArgs ReceiveEventArgs { get; set; }
        public SocketAsyncEventArgs SendEventArgs { get; set; }


        public ConexaoContexto(WebSocketServer server, TcpClient connection)
        {
            this.Server = server;
            this.Connection = connection;
            this.Buffer = new byte[this._bufferSize];
            this.SessaoContexto = new SessaoContexto(this);

            this.ReceiveEventArgs = new SocketAsyncEventArgs();
            this.SendEventArgs = new SocketAsyncEventArgs();

            this.Handler.RegisterContext(this);

            if (connection != null)
            {
                this.SessaoContexto.ClientAddress = connection.Client.RemoteEndPoint;
            }
        }

        /// <summary>
        /// Gets or sets the size of the buffer.
        /// </summary>
        /// <value>
        /// The size of the buffer.
        /// </value>
        public int BufferSize
        {
            get { return this._bufferSize; }
            set
            {
                this._bufferSize = value;
                this.Buffer = new byte[this._bufferSize];
            }
        }

        #region IDisposable Members


        public void Dispose()
        {
            this.Connected = false;
            this.SessaoContexto.OnDisconnect();

            // close client connection
            if (this.Connection != null)
            {
                try
                {
                    this.Connection.Close();
                }
                catch (Exception)
                {
                    // skip
                }
            }
            this.SendReady.Release();
            this.ReceiveReady.Release();
        }

        #endregion


        public void Disconnect()
        {
            this.Connected = false;
        }

        public void Reset()
        {
            if (this.SessaoContexto.DataFrame != null)
            {
                if (this.SessaoContexto.DataFrame.State == DataFrame.DataState.Complete)
                {
                    this.SessaoContexto.DataFrame.Reset();
                }
            }
            this.ReceivedByteCount = 0;
        }
    }
}
