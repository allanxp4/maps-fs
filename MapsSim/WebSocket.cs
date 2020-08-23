using EmbedIO.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapsSim
{
    class WebSocket : WebSocketModule
    {
        public WebSocket(string urlPath): base(urlPath, true){}

        protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
        {
            throw new NotImplementedException();
        }

        protected override Task OnClientConnectedAsync(IWebSocketContext context)
        {
            return base.OnClientConnectedAsync(context);
        }

        public void Broadcast(string data)
        {
            BroadcastAsync(data);
        }
    }
}
