using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoCJ
{
    public class Rect : Component, IDrawable
    {
        Texture2D texture;

        public Vector2 Position;

        public Vector2 Size;

        public Color Color;


        public Rect(GameObject owner, int x, int y, int width, int height, Color col) : base (owner)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
            Color = col;

            drawLayer = 0;
            texture = Graphics.Rect((int)Size.X, (int)Size.Y, col);
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(Position.ToPoint(), Size.ToPoint());
            }
        }


        public void Draw(SpriteBatch sb)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            sb.Draw(
                texture, 
                destinationRectangle: Bounds,                
                color: Color,
                layerDepth: drawLayer);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
