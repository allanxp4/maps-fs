using Microsoft.FlightSimulator.SimConnect;
using Swan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MapsSim
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct SimData
    {
        public double altitude;
        public double latitude;
        public double longitude;
        public double heading;

    };

    class SimBridge
    {
        private Action<SimData> simDataCallback;
        SimConnect connection;

        enum DEFINITIONS
        {
            SimData,
            SimStart
        }

        public SimBridge(Action<SimData> callback)
        {
            simDataCallback = callback;
            connect();
        }

        void connect()
        {
            const int WM_USER_SIMCONNECT = 0x0402;

            connection = new SimConnect("simmaps", IntPtr.Zero, WM_USER_SIMCONNECT, null, 0);

            connection.AddToDataDefinition(DEFINITIONS.SimData, "Plane Altitude", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            connection.AddToDataDefinition(DEFINITIONS.SimData, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            connection.AddToDataDefinition(DEFINITIONS.SimData, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            connection.AddToDataDefinition(DEFINITIONS.SimData, "Plane Heading Degrees True", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0, SimConnect.SIMCONNECT_UNUSED);
            connection.RegisterDataDefineStruct<SimData>(DEFINITIONS.SimData);

            connection.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(simconnect_OnRecvSimobjectData);

            connection.OnRecvEvent += new SimConnect.RecvEventEventHandler(simconnect_OnRecvEvent);

            connection.SubscribeToSystemEvent(DEFINITIONS.SimStart, "SimStart");

            requestData();

            Terminal.WriteLine("Connected to Flight Simulator", ConsoleColor.Green);

            while (true)
            {
                connection.ReceiveMessage();
                Thread.Sleep(10);
            }
        }

        void requestData()
        {
            connection.RequestDataOnSimObject(DEFINITIONS.SimData, DEFINITIONS.SimData,
            SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SIM_FRAME,
            SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }

        void simconnect_OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data)
        {
            switch((DEFINITIONS)data.dwID)
            {
                case DEFINITIONS.SimStart:
                    requestData();
                    break;
            }
        }

        void simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            switch ((DEFINITIONS)data.dwRequestID)
            {
                case DEFINITIONS.SimData:
                    try
                    {
                        handleSimData((SimData)data.dwData[0]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    break;

            }
        }

        void handleSimData(SimData simData)
        {
            simDataCallback.Invoke(simData);
        }

    }
}
