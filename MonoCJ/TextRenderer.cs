using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoCJ
{
    public class TextRenderer : Component, IDrawable
    {
        SpriteFont font;
        Color color;
        public List<string> textList = new List<string>();
        public float yStep = 0f;

        //public int lowTextListNdx { get; protected set; } = 0;
        public int listNdx { get; protected set; } = 0;

        public int xOffset = 0;
        public int yOffset = 0;

        Vector2 startWindowSize;

        public TextRenderer(GameObject owner, SpriteFont fnt, Color clr) : base (owner)
        {
            font = fnt;
            color = clr;
            xOffset = 12;
            yOffset = 12;

            drawLayer = 0.2f;
            startWindowSize = owner.game.Window.ClientBounds.Size.ToVector2();

        }

        public void Scroll(bool up)
        {

            if (up)
            {
                if (listNdx > 0)
                listNdx--;
            }
            else
            {
                
                if (listNdx < textList.Count - 1)
                listNdx++;
            }

        }


        public void AddText(string txt)
        {
            textList.Add(txt);

            listNdx = textList.Count - 1;
            
        }
        public void Draw(SpriteBatch sb)
        {
            var scale = Owner.game.Window.ClientBounds.Size.ToVector2() / startWindowSize;

            int spotIncr = 0;
            for (int i = listNdx; i >= 0; i--)
            {
                Vector2 sz = new Vector2(1, 1);
                //I dont 
                try
                {
                    sz = font.MeasureString(textList[i]);
                }
                catch(System.Exception e)
                {

                }
                yStep = sz.Y;

                var pos = Owner.rect.Position + new Vector2(xOffset * scale.X, Owner.rect.Size.Y - (yStep*scale.Y) - (yOffset * scale.Y) - (spotIncr * yStep * scale.Y));
                if (Owner.rect.Bounds.Contains(pos))
                {
                    sb.DrawString(
                        font,
                        text: textList[i],
                        position: pos,
                        color: color,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: scale,
                        effects: SpriteEffects.None,
                        layerDepth: drawLayer);
                }

                spotIncr++;
            }
        }
    }
}
