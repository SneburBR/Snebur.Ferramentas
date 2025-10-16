using System;
using System.Collections.Generic;
using Snebur.Comunicacao.WebSocket.Experimental.Classes;

namespace Snebur.Comunicacao.WebSocket.Experimental.Handlers.WebSocket
{
    internal class WebSocketHandler : Handler
    {
        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="context">The user context.</param>
        public override void HandleRequest(ConexaoContexto context)
        {
            if (context.IsSetup)
            {
                context.SessaoContexto.DataFrame.Append(context.Buffer, true);
                if (context.SessaoContexto.DataFrame.Length <= context.MaxFrameSize)
                {
                    switch (context.SessaoContexto.DataFrame.State)
                    {
                        case DataFrame.DataState.Complete:
                            context.SessaoContexto.OnReceive();
                            break;
                        case DataFrame.DataState.Closed:
                            DataFrame closeFrame = context.SessaoContexto.DataFrame.CreateInstance();
							closeFrame.State = DataFrame.DataState.Closed;
							closeFrame.Append(new byte[] { 0x8 }, true);
							context.SessaoContexto.Send(closeFrame, false, true);
                            break;
                        case DataFrame.DataState.Ping:
                            context.SessaoContexto.DataFrame.State = DataFrame.DataState.Complete;
                            DataFrame dataFrame = context.SessaoContexto.DataFrame.CreateInstance();
                            dataFrame.State = DataFrame.DataState.Pong;
                            List<ArraySegment<byte>> pingData = context.SessaoContexto.DataFrame.AsRaw();
                            foreach (var item in pingData)
                            {
                                dataFrame.Append(item.Array);
                            }
                            context.SessaoContexto.Send(dataFrame);
                            break;
                        case DataFrame.DataState.Pong:
                            context.SessaoContexto.DataFrame.State = DataFrame.DataState.Complete;
                            break;
                    }
                }
                else
                {
                    context.Disconnect(); //Disconnect if over MaxFrameSize
                }
            }
            else
            {
                Authentication.Authenticate(context);
            }
        }
    }
}