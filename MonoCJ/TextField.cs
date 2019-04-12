

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoCJ
{
    public class TextField : Component, IDrawable, IUpdatable
    {
        public string text;

        public SpriteFont font;

        public Color color;

        public int xOffset;
        public int yOffset;

        Vector2 startWindowSize;

        public bool isSelected;

        public TextField (GameObject owner, SpriteFont fnt, string msg, Color clr, int x, int y) : base(owner)
        {
            text = msg;
            font = fnt;
            color = clr;
            xOffset = x;
            yOffset = y;

            drawLayer = 0.2f;
            startWindowSize = owner.game.Window.ClientBounds.Size.ToVector2();
        }

        public void Update(float dt)
        {
            if (Input.mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                isSelected = (Owner.rect.Bounds.Contains(Input.mouse.Position));
                
            }
        }

        public void Draw(SpriteBatch sb)
        {
            var scale = Owner.game.Window.ClientBounds.Size.ToVector2() / startWindowSize;

            sb.DrawString(font, text, Owner.rect.Position + new Vector2(xOffset * scale.X, yOffset * scale.Y), color, 0, Vector2.Zero, scale, SpriteEffects.None, drawLayer);    
        }
    }
}
