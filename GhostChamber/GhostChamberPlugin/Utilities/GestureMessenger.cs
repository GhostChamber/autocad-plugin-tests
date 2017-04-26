using System;
using System.Net;
using System.Net.Sockets;

using Autodesk.AutoCAD.ApplicationServices;

namespace GhostChamberPlugin.Utilities
{
    class GestureMessenger
    {
        const string LISTENER_ADDRESS = "127.0.0.1";
        const int LISTENER_PORT = 1313;

        private IPAddress mAddress;
        private Socket mSocket;
        private bool mConnected;

        private const int MSG_NONE = 0;
        private const int MSG_GRAB = 1;
        private const int MSG_ZOOM = 2;
        private const int MSG_ORBIT = 3;

        public GestureMessenger()
        {

        }

        public void ConnectToListener()
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Attempting to connect");
            mAddress = IPAddress.Parse(LISTENER_ADDRESS);
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mSocket.BeginConnect(mAddress, LISTENER_PORT, new AsyncCallback(ConnectCallback), this);
            //mSocket.Connect(mAddress, LISTENER_PORT);
            //mConnected = true;
        }

        public void DisconnectFromListener()
        {
            if (mSocket != null)
            {
                mSocket.Disconnect(false);
                mConnected = false;
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Connected!!!!!");

                GestureMessenger self = (GestureMessenger)ar.AsyncState;
                Socket socket = self.mSocket;

                socket.EndConnect(ar);

                self.mConnected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void SendGestureMessage(GestureType gestureType)
        {
            if (!mConnected)
            {
                //ConnectToListener();
            }
            else
            {
                byte[] message = new byte[1];
                message[0] = (byte)gestureType;

                mSocket.Send(message);
            }
        }
    }
}
