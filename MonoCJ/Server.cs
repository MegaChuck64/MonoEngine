using Lidgren.Network;
using System.Collections.Generic;

namespace MonoCJ
{
    public class Server
    {
        NetServer server;
        NetPeerConfiguration config;

        MonoGame game;

        public delegate void HandleMessage(NetIncomingMessage msg);

        public HandleMessage messageHandler;

        public string name;

        public List<NetConnection> Connections
        {
            get
            {
                if (server == null)
                    return new List<NetConnection>();

                return server.Connections;
            }
        }
        public Server(MonoGame gm, HandleMessage msgHandler)
        {
            game = gm;
            messageHandler = msgHandler;
        }

        public void Connect()
        {
            config = new NetPeerConfiguration("Client")
            { Port = 6666 };


            server = new NetServer(config);

            Disconnect();

            server.Start();

            //game.Debug.Log("Server Connected.");

        }

        public void Disconnect()
        {
            if (server != null)
            server.Shutdown("Server Disconnected[Exit Message].");
        }


        public void CheckMessages()
        {
            NetIncomingMessage message;
            while ((message = server.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        messageHandler(message);
                        break;

                    case NetIncomingMessageType.StatusChanged:


                        switch (message.SenderConnection.Status)
                        {
                            case NetConnectionStatus.None:
                                break;
                            case NetConnectionStatus.InitiatedConnect:
                                break;
                            case NetConnectionStatus.ReceivedInitiation:
                                break;
                            case NetConnectionStatus.RespondedAwaitingApproval:
                                break;
                            case NetConnectionStatus.RespondedConnect:
                                break;
                            case NetConnectionStatus.Connected:
                               // game.Debug.Log("Someone connected.");

                                var ot = server.CreateMessage();
                                ot.Write("MSG");
                                ot.Write($"You have connected to server.");
                                server.SendMessage(ot, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                break;
                            case NetConnectionStatus.Disconnecting:
                                break;
                            case NetConnectionStatus.Disconnected:
                                break;
                            default:
                                break;
                        }
                        break;

                    case NetIncomingMessageType.DebugMessage:
                        //game.Debug.Log(message.ReadString());
                        break;


                    default:
                        //game.Debug.Log("unhandled message with type: "
                            //+ message.MessageType);
                        break;
                }
                server.Recycle(message);

            }

        }

        public void SendMessage(string msg, NetConnection connection)
        {
            var ms = server.CreateMessage();

            ms.Write("MSG");

            ms.Write(msg);

            server.SendMessage(ms, connection, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendMessage(NetBuffer msg, NetConnection connection)
        {
            var ms = server.CreateMessage();
            ms.Write(msg);
            server.SendMessage(ms, connection, NetDeliveryMethod.ReliableOrdered);
        }

        public void BroadcastMessage(string msg)
        {

            if (server.Connections == null || server.Connections.Count == 0) return;

            var bro = server.CreateMessage();

            bro.Write("BRDC");

            bro.Write(msg);

            server.SendMessage(bro, server.Connections, NetDeliveryMethod.ReliableOrdered, 8);
        }


        //public void HandleData(NetIncomingMessage msg)
        //{
        //    var code = msg.ReadString();

        //    switch (code)
        //    {
        //        case "MSG":
        //            var lg = msg.ReadString();
        //           // game.Debug.Log(lg);
        //            break;

        //        case "PING":

        //            var pong = server.CreateMessage();

        //            pong.Write(msg);

        //            server.SendMessage(pong, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);

        //            break;

        //        default:
        //            break;
        //    }
        //}


    }
}
