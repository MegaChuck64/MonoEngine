using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoCJ
{

    

    public class Button : Component, IUpdatable, IDrawable
    {
        public enum ButtonState
        {
            None, Pressed, Released, Hover
        }

        public ButtonState state = ButtonState.None;

        public Texture2D texture;



        public Color idleColor;
        public Color hoverColor = Color.LightGray;
        public Color pressColor = Color.LightSlateGray;
        public Color releaseColor = Color.DimGray;

        public Rectangle relativeBounds;

        SpriteFont font;
        private Vector2 startWindowSize;
        public string text;

        public delegate void OnRelease();

        OnRelease onRelease;

        Rectangle destination
        {
            get
            {
                return new Rectangle(
                    relativeBounds.Location + Owner.rect.Position.ToPoint(),
                    relativeBounds.Size);
                   
                    
                    //new Point((int)(Settings.scale * relativeBounds.Size.X),(int)(Settings.scale * relativeBounds.Size.Y)));
            }
        }
        public Button(GameObject owner, Rectangle bounds, SpriteFont fnt, string label, Color clr, OnRelease releaseAction) : base(owner)
        {
            relativeBounds = bounds;
            idleColor = clr;
            texture = Graphics.TextureFromURL(@"https://wiki.industrial-craft.net/images/4/49/MachineGUI_Background.png");
            text = label;
            drawLayer = 0.1f;
            font = fnt;
            startWindowSize = owner.game.Window.ClientBounds.Size.ToVector2();
            onRelease = releaseAction;
        }


        void Test()
        {

        }
        public void Update(float dt)
        {


            if (destination.Contains(Input.mouse.Position))
            {
                if (Input.mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    state = ButtonState.Pressed;
                }
                else
                {
                    if (state == ButtonState.Pressed)
                    {
                        state = ButtonState.Released;
                        onRelease();
                    }
                    else
                    {
                        state = ButtonState.Hover;
                    }

                }
            }
            else
            {
                state = ButtonState.None;
            }



        }

        public void Draw(SpriteBatch sb)
        {

            var col = Color.White;

            switch (state)
            {
                case ButtonState.None:
                    col = idleColor;
                    break;
                case ButtonState.Pressed:
                    col = pressColor;
                    break;
                case ButtonState.Released:
                    col = releaseColor;
                    break;
                case ButtonState.Hover:
                    col = hoverColor;
                    break;
            }



            var scale = Owner.game.Window.ClientBounds.Size.ToVector2() / startWindowSize;

            var textPos = destination.Location.ToVector2();


            sb.Draw(
                texture, 
                destinationRectangle: destination, 
                color: col,
                layerDepth: drawLayer );

            sb.DrawString(
    font,
    text: text,
    position: textPos + new Vector2(destination.Width, destination.Height) / 2 - new Vector2(font.MeasureString(text).X * scale.X, font.MeasureString(text).Y * scale.Y) / 2,
    color: Color.Black,
    rotation: 0f,
    origin: Vector2.Zero,
    scale: scale,
    effects: SpriteEffects.None,
    layerDepth: drawLayer + 0.1f);


        }
    }
}
