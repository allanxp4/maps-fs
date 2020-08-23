using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using Swan;

namespace MapsSim
{

    class Program
    {
        static Server server = new Server(6789);

        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Terminal.WriteLine("Connecting to flight simulator", ConsoleColor.Cyan);
                    Terminal.WriteLine($"Go to http://{GetLocalIP()}:6789 on any device inside this network", ConsoleColor.Green);
                    var bridge = new SimBridge(simDataCallback);
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Couldn't connect to flight simulator: " + e.Message);
                    Console.WriteLine("Check if it's open with an active flight.");
                    Console.WriteLine("Trying again in 10 seconds...");
                    Thread.Sleep(10000);
                }
            }
        }
        private static void simDataCallback(SimData simData)
        {
            var data = JsonConvert.SerializeObject(simData);
            server.BroadcastOnSocket(data);
        }

        private static string GetLocalIP()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }
    }
}

