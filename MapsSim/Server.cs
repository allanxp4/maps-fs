using EmbedIO;
using EmbedIO.WebSockets;
using Swan.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapsSim
{
    class Server
    {
        private WebServer server;
        private WebSocket socketServer = new WebSocket("/socket");
        public Server(int port) {
            server = new WebServer(port)
                .WithCors()
                .WithModule(socketServer)
                .WithStaticFolder("/", @"./htdocs", false);

            Logger.UnregisterLogger<ConsoleLogger>();
            server.Start();
        }

        public void BroadcastOnSocket(string data)
        {
            socketServer.Broadcast(data);
        }
    }
}
