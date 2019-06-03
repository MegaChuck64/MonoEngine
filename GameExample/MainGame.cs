
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoCJ;
using System;
using PlayFab;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace GameExample
{
    enum NetworkState
    {
        IsClient, IsServer, IsNothing
    }



    public class MainGame : MonoCJ.MonoGame
    {
        public MainGame() : base("Consolas", 18, 16, 9) { }

        GameObject controlPanel;
        GameObject messagePanel;
        GameObject outputPanel;

        GameObject nameText;

        Server server;
        Client client;
  
        NetworkState netState = NetworkState.IsNothing;


        #region Frame Events

        public override void Start()
        {
            Window.Title = "CJ's Game.";

            backgroundColor = Color.SlateGray;


            controlPanel = new GameObject(this, 0, 0, 64, 64, Color.DarkGray);
            controlPanel.AddComponent(new Button(controlPanel, new Rectangle(0, 0, 64, 64), defaultFont, "Broadcast", Color.White, BroadcastButtonPressedAsync));
            controlPanel.AddComponent(new Button(controlPanel, new Rectangle(0, 0, 64, 64), defaultFont, "Connect", Color.White, ConnectButtonPressedAsync));
            controlPanel.AddComponent(new TextField(controlPanel, defaultFont, "", Color.DarkGreen, 24, 0, "url"));
          



            messagePanel = new GameObject(this, 0, 0, 64, 64, Color.DarkSlateGray);
            messagePanel.AddComponent(new TextRenderer(messagePanel, defaultFont, Color.DarkGreen));

            outputPanel = new GameObject(this, 0, 0, 64, 64, Color.Black);
            outputPanel.AddComponent(new TextField(outputPanel, defaultFont, "", Color.DarkGreen, 24, 46));


            nameText = new GameObject(this, 8, Settings.WindowHeight - 16, 64, 16, Color.LightPink, 0.3f);
            nameText.AddComponent(new TextField(nameText, defaultFont, "", Color.Green, 24, 0, "name"));
            nameText.GetComponent<TextField>().drawLayer = 0.31f;


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

            nameText.Update(dt);
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

        public override void Draw(SpriteBatch sb)
        {

            outputPanel.Draw(sb);

            controlPanel.Draw(sb);

            messagePanel.Draw(sb);

            nameText.Draw(sb);


        }

        #endregion

        #region Network Events

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

                    AddMessage(b);

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
                    var msgBody = msg.ReadString();
                    AddMessage(msgBody);
                    server.BroadcastMessage(msgBody);
                    break;

                case "PING":
                   
                    server.SendMessage(msg, msg.SenderConnection);

                    break;

                default:
                    break;
            }
        }

        #endregion

        #region Local Events

        public override void OnWindowResize()
        {

            controlPanel.rect.Position = Vector2.Zero;

            controlPanel.rect.Size = GetPanelSize(0.2f, .9f);




            var buttonList = controlPanel.GetComponents<Button>();

            if (buttonList != null)
            {
                for (int i = 0; i < buttonList.Count; i++)
                {
                    buttonList[i].relativeBounds =
                         new Rectangle(
                        (int)MathUtils.Percentage(.2f, controlPanel.rect.Size.X),
                        (int)MathUtils.Percentage(.05f, controlPanel.rect.Size.Y) + (int)(i * MathUtils.Percentage(.12f, controlPanel.rect.Size.Y)),
                        (int)MathUtils.Percentage(.6f, controlPanel.rect.Size.X),
                        (int)MathUtils.Percentage(.3f, controlPanel.rect.Size.X));
                }
            }

            //var urlText = controlPanel.GetComponent<TextField>();
            
            nameText.rect.Position = new Vector2(0, controlPanel.rect.Bounds.Height);
            nameText.rect.Size = GetPanelSize(0.2f, 0.1f);


            messagePanel.rect.Position = new Vector2(controlPanel.rect.Bounds.Width, 0);

            messagePanel.rect.Size = GetPanelSize(0.6f, 0.75f);


            outputPanel.rect.Position = new Vector2(controlPanel.rect.Bounds.Width, messagePanel.rect.Bounds.Height);

            outputPanel.rect.Size = GetPanelSize(0.6f, 0.25f);

        }

        public void HandleTextInput(object sender, TextInputEventArgs e)
        {
            var url = controlPanel.GetComponent<TextField>();
            var name = nameText.GetComponent<TextField>();
            TextField field = outputPanel.GetComponent<TextField>();

            if (url.isSelected) field = url;
            else if (name.isSelected) field = name;
            else field = outputPanel.GetComponent<TextField>();

            if (e.Character == '\b')
            {
                if (field.text.Length > 0)
                    field.text = field.text.Remove(field.text.Length - 1);
            }
            else if (e.Character == '\r')
            {

                switch (netState)
                {
                    case NetworkState.IsClient:
                        if (field.text[0] == '?' && field.text[1] == '>')
                        {
                            if (field.text.Substring(2,4).ToLower() == "ping")
                            {
                                client.Ping();
                            }

                            AddMessage(field.text);

                        }
                        else
                        {
                            client.SendMessage(nameText.GetComponent<TextField>().text + ": " + field.text);
                        }
                        break;
                    case NetworkState.IsServer:
                        if (field.text[0] == '?' && field.text[1] == '>')
                        {
                            if (field.text.Substring(2, 3).ToLower() == "brd")
                            {
                                server.BroadcastMessage(nameText.GetComponent<TextField>().text + ": " + field.text.Substring(field.text[5] == ' ' ? 6 : 5));
                            }

                            AddMessage(field.text);

                        }
                        break;
                    case NetworkState.IsNothing:
                        AddMessage(field.text);
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

        async void BroadcastButtonPressedAsync()
        {

            if (netState == NetworkState.IsServer)
            {
                server.Disconnect();

                netState = NetworkState.IsNothing;

                ChangeButtonText(controlPanel, "Disconnect", "Broadcast");

                AddMessage("Disconnected.");
            }
            else if (netState == NetworkState.IsNothing)
            {

                var reg = await Account.RegisterDomainAsync(GetLocalIPAddress(), controlPanel.GetComponents<TextField>()[0].text);
                AddMessage(reg);

                server.Connect();
                netState = NetworkState.IsServer;

                ChangeButtonText(controlPanel, "Broadcast", "Disconnect");

            }
        }

        async void ConnectButtonPressedAsync()
        {

            if (netState == NetworkState.IsNothing)
            {
                var url = controlPanel.GetComponent<TextField>().text;
                var ip = await Account.GetIPAsync(url);

                if (ip.Contains("ERROR"))
                {
                    AddMessage(ip);
                }
                else
                {
                    client.Connect(ip);
                    netState = NetworkState.IsClient;
                    ChangeButtonText(controlPanel, "Connect", "Disconnect");
                }
            }
            else if (netState == NetworkState.IsClient)
            {
                ChangeButtonText(controlPanel, "Disconnect", "Connect");
            }

        }

        #endregion

        #region Helper

        public void ChangeButtonText(GameObject parent, string textToChange, string newText)
        {
            (parent.components.Find(c => c is Button && (c as Button).text.ToLower() == textToChange.ToLower()) as Button).text = newText;
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

        public Vector2 GetPanelSize(float widthPercent, float heightPercent)
        {
            return new Vector2((int)MathUtils.Percentage(widthPercent, Settings.WindowWidth), (int)MathUtils.Percentage(heightPercent, Settings.WindowHeight));
        }

        public void ScrollMessages(bool up)
        {
            messagePanel.GetComponent<TextRenderer>().Scroll(up);
        }

        public void AddMessage(string msg)
        {
            if (msg.Trim()[0] == '?' && msg.Trim()[1] == '>')
            {
                ExecuteCommand(msg.Substring(2));
            }
            var lns = msg.Split('\n');
            foreach (var ln in lns.Where(l=>l!=""))
            {
                messagePanel.GetComponent<TextRenderer>().AddText(ln);
            }
        }

        void ExecuteCommand(string cmd)
        {
            var prts = cmd.Split('(', ')', ';').Where(x => x != "").ToArray();
            switch (prts[0].ToLower())
            {
                case "readfile":
                    AddMessage(System.IO.File.ReadAllText(prts[1]));
                    break;
                case "":
                    AddMessage(System.IO.File.ReadAllText(prts[1]));
                    break;
                default:
                    break;
            }
        }


        #endregion

    }
}
