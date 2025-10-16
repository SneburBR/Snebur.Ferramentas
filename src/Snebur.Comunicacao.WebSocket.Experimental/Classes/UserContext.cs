using Snebur.Comunicacao.WebSocket.Experimental.Handlers.WebSocket;
using System;
using System.Linq;
using System.Net;

namespace Snebur.Comunicacao.WebSocket.Experimental.Classes
{

    public class SessaoContexto
    {

        protected readonly ConexaoContexto Contexto;
        public EndPoint ClientAddress;
        public object Data;

        public DataFrame DataFrame;

        protected OnEventDelegate OnConnectDelegate = x => { };
        protected OnEventDelegate OnConnectedDelegate = x => { };
        protected OnEventDelegate OnDisconnectDelegate = x => { };
        protected OnEventDelegate OnReceiveDelegate = x => { };
        protected OnEventDelegate OnSendDelegate = x => { };


        public Protocol Protocol = Protocol.None;

        public string RequestPath = "/";


        public SessaoContexto(ConexaoContexto contexo)
        {
            this.Contexto = contexo;
        }

        public Cabecalho Cabecalho
        {
            get { return this.Contexto.Cabecalho; }
        }


        public ulong MaxFrameSize
        {
            get { return this.Contexto.MaxFrameSize; }
            set { this.Contexto.MaxFrameSize = value; }
        }

        public void OnConnect()
        {
            this.OnConnectDelegate(this);
        }

        public void OnConnected()
        {
            this.OnConnectedDelegate(this);
        }

        public void OnDisconnect()
        {
            this.Contexto.Connected = false;
            this.OnDisconnectDelegate(this);
        }

        public void OnSend()
        {
            this.OnSendDelegate(this);
        }

        public void OnReceive()
        {
            this.OnReceiveDelegate(this);
        }


        public void SetOnConnect(OnEventDelegate aDelegate)
        {
            this.OnConnectDelegate = aDelegate;
        }


        public void SetOnConnected(OnEventDelegate aDelegate)
        {
            this.OnConnectedDelegate = aDelegate;
        }


        public void SetOnDisconnect(OnEventDelegate aDelegate)
        {
            this.OnDisconnectDelegate = aDelegate;
        }


        public void SetOnSend(OnEventDelegate aDelegate)
        {
            this.OnSendDelegate = aDelegate;
        }

        public void SetOnReceive(OnEventDelegate aDelegate)
        {
            this.OnReceiveDelegate = aDelegate;
        }

        public void Send(DataFrame dataFrame, bool raw = false, bool close = false)
        {
            this.Contexto.Handler.Send(dataFrame, this.Contexto, raw, close);
        }

        public void Send(string aString, bool raw = false, bool close = false)
        {
            DataFrame dataFrame = this.DataFrame.CreateInstance();
            dataFrame.Append(aString);
            this.Contexto.Handler.Send(dataFrame, this.Contexto, raw, close);
        }


        public void Send(byte[] someBytes, bool raw = false, bool close = false)
        {
            DataFrame dataFrame = this.DataFrame.CreateInstance();
            dataFrame.IsByte = true;
            dataFrame.Append(someBytes);
            this.Contexto.Handler.Send(dataFrame, this.Contexto, raw, close);
        }

        private string _identificador = null;
        public string Identificador
        {
            get
            {
                if(this._identificador == null)
                {
                    this._identificador = new string(this.ClientAddress.ToString().Where(x => Char.IsLetterOrDigit(x)).ToArray()); 
                }
                return this._identificador;
            }
        }
    }
} 