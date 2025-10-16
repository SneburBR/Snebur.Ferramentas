using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Snebur.Comunicacao.WebSocket.Experimental
{
    public abstract class TcpServer : IDisposable
    {
        private const int LIMITE_BUFFER_CONEXOES_ATIVAS = 20;

        private readonly SemaphoreSlim _bloqueioUnico = new SemaphoreSlim(1);
        private readonly SemaphoreSlim _limitarConexoes = new SemaphoreSlim(LIMITE_BUFFER_CONEXOES_ATIVAS);
        private static readonly object BloqueioConstrutor = new object();

        public int BufferSize { get; protected set; } = 512;
        public int ConexoesAtivas { get; private set; }

        public IPAddress ListenAddress { get; } = IPAddress.Any;
        public int Porta { get; }

        private TcpListener TcpListenerInterno;

        protected TcpServer(IPAddress listenAddress, int porta)
        {
            this.Porta = porta;
            this.ListenAddress = listenAddress;
        }
         
        public virtual void Inicializar()
        {
            if (this.TcpListenerInterno == null)
            {
                lock (BloqueioConstrutor)
                {
                    if (this.TcpListenerInterno == null)
                    {
                        this.TcpListenerInterno = new TcpListener(this.ListenAddress, this.Porta);
                        this.TcpListenerInterno.Start();
                        ThreadPool.QueueUserWorkItem(this.Monitorar);
                    }
                }
            }
        }

        public virtual void Parar()
        {
            if (this.TcpListenerInterno != null)
            {
                this.TcpListenerInterno.Stop();
            }
            this.TcpListenerInterno = null;
        }

        private void Monitorar(object userState)
        {
            while (this.TcpListenerInterno != null)
            {
                try
                {
                    this.TcpListenerInterno.BeginAcceptTcpClient(this.ReceberConexaoTcpClienteAsync, null);
                }
                catch (SocketException)
                {
                    /* Ignore */
                }
                this._limitarConexoes.Wait();
            }
        }
 

        private void ReceberConexaoTcpClienteAsync(IAsyncResult result)
        {
            TcpClient conexaoTcpCliente = null;
            if (this.TcpListenerInterno != null)
            {
                try
                {
                    conexaoTcpCliente = this.TcpListenerInterno.EndAcceptTcpClient(result);
                }
                catch (Exception)
                {

                    conexaoTcpCliente = null;
                }
            }
            this._limitarConexoes.Release();
            if (conexaoTcpCliente != null)
            {
                this._bloqueioUnico.Wait();
                this.ConexoesAtivas++;
                this._bloqueioUnico.Release();

                ThreadPool.QueueUserWorkItem(this.ProcessarConexaoCliente, conexaoTcpCliente);

                this._bloqueioUnico.Wait();
                this.ConexoesAtivas--;
                this._bloqueioUnico.Release();
            }
        }

        protected abstract void ProcessarConexaoCliente(object connection);


        #region Dispensar

        public virtual void Reiniciar()
        {
            this.Parar();
            this.Inicializar();
        }

        public virtual void Dispose()
        {
            this.Parar();
        }

        #endregion

    }
}
