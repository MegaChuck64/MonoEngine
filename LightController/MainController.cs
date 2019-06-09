using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoCJ;
using ENGINE = MonoCJ;

namespace LightController
{
    public class MainController : ENGINE.MonoGame
    {
        public MainController() : base("Consolas", 24, 16, 9) { }

        GameObject terminal;
        GameObject input;

        string inputBuffer = "";

        public override void Start()
        {
            Window.TextInput += new System.EventHandler<TextInputEventArgs>(HandleTextInput);

            backgroundColor = new Color(0.1f, 0.1f, 0.1f);

            terminal = new GameObject(this, 100, 100, 100, 100, new Color(0.2f, 0.2f, 0.2f), 0.1f);
            terminal.AddComponent(new TextRenderer(terminal, defaultFont, Color.Green));

            input = new GameObject(this, 100, 100, 100, 100, new Color(0.2f, 0.2f, 0.2f), 0.1f);
            input.AddComponent(new TextField(input, defaultFont, "", Color.Green, 2, 2));


            OnWindowResize();

            DiscordController.InitController(LogMessage);

        }

        public override void Update(float dt)
        {
            var scroll = Input.GetScroll();
            if (scroll != 0)
            {
                terminal.GetComponent<TextRenderer>().Scroll(scroll > 0);
            }

            input.GetComponent<TextField>().text = inputBuffer;

            terminal.Update(dt);
        }
        public override void Draw(SpriteBatch sb)
        {
            terminal.Draw(sb);
            input.Draw(sb);
        }

        public override void OnWindowResize()
        {
            terminal.rect.Position = Settings.GetPanelSize(0.05f, 0.05f);
            terminal.rect.Size = Settings.GetPanelSize(0.9f, 0.75f);


            input.rect.Position = Settings.GetPanelSize(0.05f,0.05f) + new Vector2(0, terminal.rect.Bounds.Bottom);
            input.rect.Size = Settings.GetPanelSize(0.9f, 0.10f);
        }

        public void HandleTextInput(object sender, TextInputEventArgs e)
        {
            var trm = terminal.GetComponent<TextRenderer>();

            //backspace
            if (e.Character == '\b')
            {
                if (!string.IsNullOrEmpty(inputBuffer))
                {
                    inputBuffer = inputBuffer.Remove(inputBuffer.Length - 1);
                }
            }
            //enter
            else if (e.Character == '\r')
            {
                if (!string.IsNullOrEmpty(inputBuffer))
                {
                    LogMessage(inputBuffer);
                    inputBuffer = string.Empty;
                }
            }
            else
            {
                inputBuffer += e.Character;
            }

        }

        public void LogMessage(string msg)
        {
            var trm = terminal.GetComponent<TextRenderer>();
            trm.AddText(msg);

        }
    }
}