using Lidgren.Network;
using System;

namespace MonoCJ
{
    public class Client
    {
        NetClient client;
        NetPeerConfiguration config;
        MonoGame game;

        public delegate void HandleMessage(NetIncomingMessage msg);

        private HandleMessage messageHandler;

        public bool isConnected { get; private set; }

        public Client(MonoGame gm, HandleMessage msgHandler)
        {
            game = gm;
            config = new NetPeerConfiguration("Client");
            client = new NetClient(config);
            messageHandler = msgHandler;
        }
        public void Connect(string ip)
        {
            client.Start();
            client.Connect(host: ip, port: 6666);
            //game.Debug.Log("Client Connected");
        }
        public void Disconnect()
        {
            if (client != null)
            client.Disconnect("Client Disconnected[Exit Message].");

            //game.Debug.Log("Client Disconnected");

        }
        public void SendMessage(string msg)
        {
            var message = client.CreateMessage();

            message.Write("MSG");

            message.Write(msg);

            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }


        public void Ping()
        {
            var message = client.CreateMessage();

            message.Write("PING");

            byte[] data;

            data = System.Text.Encoding.ASCII.GetBytes(TimeSpan.FromTicks(DateTime.Now.Ticks).TotalMilliseconds.ToString());

            message.Write(data.Length);
            message.Write(data);

            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }


        //handle data from the server
        //public void HandleData(NetIncomingMessage msg)
        //{
        //    var code = msg.ReadString();

        //    switch (code)
        //    {

        //        case "PING":

        //            var lng = msg.ReadInt32();
        //            var data = msg.ReadBytes(lng);

        //            var dif = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalMilliseconds - double.Parse(System.Text.Encoding.ASCII.GetString(data));

        //          //  game.Debug.Log($"Ping: {dif}ms");

        //            break;

        //        case "MSG":

        //            var d = msg.ReadString();

        //           // game.Debug.Log(d);

        //            break;


        //        case "BRDC":

        //            var b = msg.ReadString();

        //            //game.Debug.Log($"SERVER: {b}");

        //            break;

        //        default:
        //            break;
        //    }
        //}

        public void CheckMessages()
        {
            if (client.ServerConnection == null)
            {
                isConnected = false;
                return;
            }

            isConnected = true;
            NetIncomingMessage message;
            while ((message = client.ReadMessage()) != null)
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
                             //   game.Debug.Log("Connected to server.");
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
            }
        }
    }
}
