using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Snebur.Comunicacao.WebSocket.Experimental.Classes;
using Snebur.Comunicacao.WebSocket.Experimental.Handlers.WebSocket;

namespace Snebur.Comunicacao.WebSocket.Experimental.Handlers
{
  
    public class Handler : IDisposable
    {
        private static Handler _instance;

        protected static object createLock = new object();
        internal IAuthentication Authentication;

        private Thread[] ProcessSendThreads = new Thread[Environment.ProcessorCount];

        private ConcurrentQueue<HandlerMessage> MessageQueue { get; set; }

        /// <summary>
        /// Cancellation of threads if disposing
        /// </summary>
        private static CancellationTokenSource cancellation = new CancellationTokenSource();

        protected Handler() {

            this.MessageQueue = new ConcurrentQueue<HandlerMessage>();

            for (int i = 0; i < this.ProcessSendThreads.Length; i++)
            {
                this.ProcessSendThreads[i] = new Thread(this.ProcessSend)
                {
                    Name = "Snebur.Comunicacao.WebSocket.Experimental Send Handler Thread " + (i + 1)
                };
                this.ProcessSendThreads[i].Start();
            }
        }

        public static Handler Instance
        {
            get
            {
                if (_instance == null)
                {
                     lock(createLock){
                        if(_instance == null){
                            _instance = new Handler();
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Handles the initial request.
        /// Attempts to process the header that should have been sent.
        /// Otherwise, through magic and wizardry, the client gets disconnected.
        /// </summary>
        /// <param name="context">The user context.</param>
        public virtual void HandleRequest(ConexaoContexto context)
        {
            if (context.IsSetup)
            {
                context.Disconnect();
            }
            else
            {
                ProcessHeader(context);
            }
        }

        /// <summary>
        /// Processes the header.
        /// </summary>
        /// <param name="context">The user context.</param>
        public void ProcessHeader(ConexaoContexto context)
        {
            string data = Encoding.UTF8.GetString(context.Buffer, 0, context.ReceivedByteCount);
            //Check first to see if this is a flash socket XML request.
            if (data == "<policy-file-request/>\0")
            {
                //if it is, we access the Access Policy Server instance to send the appropriate response.
                context.Disconnect();
                throw new Exception("Cross domain não implementado para FLASH - quem quer saber de flash ");

                //context.Server.AccessPolicyServer.SendResponse(context.Connection);
                
            }
            else //If it isn't, process http/websocket header as normal.
            {
                context.Cabecalho = new Cabecalho(data);
                switch (context.Cabecalho.Protocol)
                {
                    case Protocol.WebSocketHybi00:
                        context.Handler.UnregisterContext(context);
                        context.Handler = WebSocket.hybi00.Handler.Instance;
                        context.SessaoContexto.DataFrame = new WebSocket.hybi00.DataFrame();
                        context.Handler.RegisterContext(context);
                        break;
                    case Protocol.WebSocketRFC6455:
                        context.Handler.UnregisterContext(context);
                        context.Handler = WebSocket.rfc6455.Handler.Instance;
                        context.SessaoContexto.DataFrame = new WebSocket.rfc6455.DataFrame();
                        context.Handler.RegisterContext(context);
                        break;
                    default:
                        context.Cabecalho.Protocol = Protocol.None;
                        break;
                }
                if (context.Cabecalho.Protocol != Protocol.None)
                {
                    context.Handler.HandleRequest(context);
                }
                else
                {
                    context.SessaoContexto.Send(Response.NotImplemented, true, true);
                }
            }
        }

        private void ProcessSend()
        {
            while (!cancellation.IsCancellationRequested)
            {
                while (this.MessageQueue.IsEmpty)
                {
                    Thread.Sleep(10);
                    if (cancellation.IsCancellationRequested) return;
                }

                HandlerMessage message;

                if (!this.MessageQueue.TryDequeue(out message))
                {
                    continue;
                }

                Send(message);
            }
        }

        private void Send(HandlerMessage message)
        {
            message.Context.SendEventArgs.UserToken = message;
            
            try
            {
              message.Context.SendReady.Wait(cancellation.Token);
            }
            catch (OperationCanceledException)
            {
              return;
            }

            try
            {
                List<ArraySegment<byte>> data = message.IsRaw ? message.DataFrame.AsRaw() : message.DataFrame.AsFrame();
                message.Context.SendEventArgs.BufferList = data;
                message.Context.Connection.Client.SendAsync(message.Context.SendEventArgs);
            }
            catch
            {
                message.Context.Disconnect();
            }
        }

        /// <summary>
        /// Sends the specified data.
        /// </summary>
        /// <param name="dataFrame">The data.</param>
        /// <param name="context">The user context.</param>
        /// <param name="raw">whether or not to send raw data</param>
        /// <param name="close">if set to <c>true</c> [close].</param>
        public void Send(DataFrame dataFrame, ConexaoContexto context, bool raw = false, bool close = false)
        {
            if (context.Connected)
            {
                HandlerMessage message = new HandlerMessage { DataFrame = dataFrame, Context = context, IsRaw = raw, DoClose = close };
                MessageQueue.Enqueue(message);
            }
        }

        void SendEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            HandlerMessage message = (HandlerMessage)e.UserToken;

            if (e.SocketError != SocketError.Success)
            {
                message.Context.Disconnect();
                return;
            }
           
            message.Context.SendReady.Release();
            message.Context.SessaoContexto.OnSend();

            if (message.DoClose)
            {
                message.Context.Disconnect();
            }
        }

        public void RegisterContext(ConexaoContexto context)
        {
            context.SendEventArgs.Completed += this.SendEventArgs_Completed;
        }

         public void UnregisterContext(ConexaoContexto context)
        {
            context.SendEventArgs.Completed -= this.SendEventArgs_Completed;
        }

        private class HandlerMessage
        {
            public DataFrame DataFrame { get; set;}
            public ConexaoContexto Context { get; set;}
            public bool IsRaw { get; set;}
            public bool DoClose { get; set;}
        }
        
        public void Dispose()
        {
          cancellation.Cancel();      
        }
        
    }
}
