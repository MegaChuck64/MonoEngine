
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoCJ;
using System;
using PlayFab;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace GameExample
{

    public class MainGame : MonoCJ.MonoGame
    {
        public MainGame() : base("Consolas", 18, 16, 9) { }


        GameObject controlPanel;
        GameObject messagePanel;
        GameObject outputPanel;

        Server server;
        Client client;

        enum NetworkState
        {
            IsClient, IsServer, IsNothing
        }

        NetworkState netState = NetworkState.IsNothing;

        public override void Start()
        {
            Window.Title = "CJ's Game.";

            backgroundColor = Color.LightGray;


            controlPanel = new GameObject(this, 0, 0, 64, 64);
            controlPanel.AddComponent(new Button(controlPanel, new Rectangle(0, 0, 64, 64), defaultFont, "Broadcast", Color.White, BroadcastButtonPressedAsync));
            controlPanel.AddComponent(new Button(controlPanel, new Rectangle(0, 0, 64, 64), defaultFont, "Connect", Color.White, ConnectButtonPressed));
            controlPanel.AddComponent(new TextField(controlPanel, defaultFont, "", Color.Black, 0, 0));


            messagePanel = new GameObject(this, 0, 0, 64, 64);
            messagePanel.AddComponent(new TextRenderer(messagePanel, defaultFont, Color.DarkGreen));

            outputPanel = new GameObject(this, 0, 0, 64, 64);
            outputPanel.AddComponent(new TextField(outputPanel, defaultFont, "", Color.Black, 16, 46));
            OnWindowResize();

            Window.TextInput += new System.EventHandler<TextInputEventArgs>(HandleTextInput);

            AddMessage("Game started.");

            server = new Server(this, HandleServerMessages);
            client = new Client(this, HandleClientMessages);

            PlayFabSettings.staticSettings.TitleId = "2BFF6";

            var login = Task.Run(Account.LoginAsync);
            AddMessage("Attempting Login.");
            login.Wait();
            AddMessage(login.Result);
        }

        public override void Update(float dt)
        {
            var scroll = Input.GetScroll();
            if (scroll != 0)
            {
                ScrollMessages(scroll > 0);
            }

            controlPanel.Update(dt);
            messagePanel.Update(dt);
            outputPanel.Update(dt);

            switch (netState)
            {
                case NetworkState.IsClient:
                    client.CheckMessages();
                    break;
                case NetworkState.IsServer:
                    server.CheckMessages();
                    break;
                case NetworkState.IsNothing:
                    break;
            }

        }



        void HandleClientMessages(NetIncomingMessage msg)
        {
            var code = msg.ReadString();

            switch (code)
            {

                case "PING":

                    var lng = msg.ReadInt32();
                    var data = msg.ReadBytes(lng);

                    var dif = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalMilliseconds - double.Parse(System.Text.Encoding.ASCII.GetString(data));

                    AddMessage($"Ping: {dif}ms");

                    break;

                case "MSG":

                    var d = msg.ReadString();

                    AddMessage(d);

                    break;


                case "BRDC":

                    var b = msg.ReadString();

                    AddMessage($"S: {b}");

                    break;

                default:
                    break;
            }
        }

        void HandleServerMessages(NetIncomingMessage msg)
        {
            var code = msg.ReadString();

            switch (code)
            {
                case "MSG":
                    var lg = msg.ReadString();
                    AddMessage($"C: {lg}");
                    break;

                case "PING":
                   
                    server.SendMessage(msg, msg.SenderConnection);

                    break;

                default:
                    break;
            }
        }

        async void BroadcastButtonPressedAsync()
        {
            if (client != null)
            {
                client.Disconnect();
            }

            var reg = await Account.RegisterDomain(GetLocalIPAddress(), controlPanel.GetComponent<TextField>().text);           
            AddMessage(reg);


            server.Connect();
            netState = NetworkState.IsServer;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return ("ERROR -- No IPv4 found.");
        }

        async void ConnectButtonPressed()
        {
            if (server != null)
            {
                server.Disconnect();
            }
            var url = controlPanel.GetComponent<TextField>().text;
            var ip = await Account.GetIP(url);

            if (ip.Contains("ERROR"))
            {
                AddMessage(ip);
            }
            else
            {
                client.Connect(ip);
                netState = NetworkState.IsClient;
            }
        }



        public override void Draw(SpriteBatch sb)
        {
            outputPanel.Draw(sb);

            controlPanel.Draw(sb);

            messagePanel.Draw(sb);

        }


        public Vector2 GetPanelSize(float widthPercent, float heightPercent)
        {
            return new Vector2((int)MathUtils.Percentage(widthPercent, Settings.WindowWidth), (int)MathUtils.Percentage(heightPercent, Settings.WindowHeight));
        }

        public override void OnWindowResize()
        {

            controlPanel.rect.Position = Vector2.Zero;

            controlPanel.rect.Size = GetPanelSize(0.2f, 1f);


            var buttonList = controlPanel.GetComponents<Button>();

            if (buttonList != null)
            {
                for (int i = 0; i < buttonList.Count; i++)
                {
                    buttonList[i].relativeBounds =
                         new Rectangle(
                        (int)MathUtils.Percentage(.3f, controlPanel.rect.Size.X),
                        (int)MathUtils.Percentage(.05f, controlPanel.rect.Size.Y) + (int)(i * MathUtils.Percentage(.12f, controlPanel.rect.Size.Y)),
                        (int)MathUtils.Percentage(.4f, controlPanel.rect.Size.X),
                        (int)MathUtils.Percentage(.1f, controlPanel.rect.Size.Y));
                }
            }

            var urlText = controlPanel.GetComponent<TextField>();
            



            messagePanel.rect.Position = new Vector2(controlPanel.rect.Bounds.Width, 0);

            messagePanel.rect.Size = GetPanelSize(0.6f, 0.75f);


            outputPanel.rect.Position = new Vector2(controlPanel.rect.Bounds.Width, messagePanel.rect.Bounds.Height);

            outputPanel.rect.Size = GetPanelSize(0.6f, 0.25f);

        }


        public void ScrollMessages(bool up)
        {
            messagePanel.GetComponent<TextRenderer>().Scroll(up);
        }


        public void AddMessage(string msg)
        {
            messagePanel.GetComponent<TextRenderer>().AddText(msg);
        }

        public void HandleTextInput(object sender, TextInputEventArgs e)
        {
            var field = controlPanel.GetComponent<TextField>();
            
            if (!field.isSelected) field = outputPanel.GetComponent<TextField>();

            if (e.Character == '\b')
            {
                if (field.text.Length > 0)
                    field.text = field.text.Remove(field.text.Length - 1);
            }
            else if (e.Character == '\r')
            {
                AddMessage(field.text);

                switch (netState)
                {
                    case NetworkState.IsClient:
                        client.SendMessage(field.text);
                        break;
                    case NetworkState.IsServer:
                        server.BroadcastMessage(field.text);
                        break;
                    case NetworkState.IsNothing:
                        break;
                }

                field.text = "";
            }
            else
            {
                var newText = field.text + e.Character;

                if (field.font.MeasureString(newText).X + field.Owner.rect.Position.X + field.xOffset > field.Owner.rect.Size.X + field.Owner.rect.Position.X)
                {
                    field.text += "\n";
                }

                field.text += e.Character;
            }
        }

    }
}
