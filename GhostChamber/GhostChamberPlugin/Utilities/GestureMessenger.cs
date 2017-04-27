using System;
using System.Net;
using System.Net.Sockets;

using Autodesk.AutoCAD.ApplicationServices;

namespace GhostChamberPlugin.Utilities
{
    /**
     * GestureMesenger class messages the Ghost Streamer application about the current gesture being used. This is then used to display the corresponding UI element in the stream.
     */
    class GestureMessenger
    {
        const string LISTENER_ADDRESS = "127.0.0.1";    /**< Localhost address being used to message to GhostStreamer. */
        const int LISTENER_PORT = 1313;                 /**< The port used for messaging. */
        private Socket mSocket;                         /**< The socket used for messaging */
        private bool mConnected;                        /**< Boolean that is true if a connection is established. */

        /**
         * Default constructor of class.
         */
        public GestureMessenger()
        {
        }

        /**
         * Connects to the port that GhostStreamer is listening on.
         */
        public void ConnectToListener()
        {
            IPAddress address;
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Attempting to connect");
            address = IPAddress.Parse(LISTENER_ADDRESS);
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mSocket.BeginConnect(address, LISTENER_PORT, new AsyncCallback(ConnectCallback), this);
        }

        /**
         * Disconnects from port.
         */
        public void DisconnectFromListener()
        {
            if (mSocket != null)
            {
                mSocket.Disconnect(false);
                mConnected = false;
            }
        }

        /** 
         * @static
         * CallBack function once the connection is set up.
         * @param ar The IAsyncresult state containing the object that was connected.
         */
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
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

        /**
         * Sends the message with the gesture Type being called.
         * @param gestureType the enum value of the gesture currently in use.
         */
        public void SendGestureMessage(GestureType gestureType)
        {
            if (mConnected)
            {
                byte[] message = new byte[1];
                message[0] = (byte)gestureType;

                mSocket.Send(message);
            }

            //Commented out because it often fires a race condition due to the method being called before the connection sequence is complete.
            //else
            //{
            //    //ConnectToListener();
            //}
        }
    }
}
