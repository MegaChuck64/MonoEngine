using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace MonoCJ
{
    public class GameObject
    {
        public MonoGame game;

        public bool isActive = true;

        public List<Component> components { get; protected set; } = new List<Component>();

        public Rect rect;

        public Color tint;

        public GameObject(MonoGame gme, int x, int y, int width, int height, Color? tnt = null)
        {
            game = gme;

            tint = tnt == null ? Color.Transparent : tnt.Value;

            rect = new Rect(this, x, y, width, height, Graphics.RandomColor());


        }

        public void Update(float dt)
        {
            if (isActive)
            {
                foreach (var cmp in components.FindAll(c => c is IUpdatable))
                {
                    (cmp as IUpdatable).Update(dt);
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (isActive)
            {

                rect.Draw(sb);

                foreach (var cmp in components.FindAll(c => c is IDrawable))
                {
                    (cmp as IDrawable).Draw(sb);
                }

            }
        }

        public void AddComponent<T>(T component) where T : Component
        {
            components.Add(component);
        }

        public T GetComponent<T>() where T : Component
        {
            return components.Find(c => c is T) as T;
        }

        public List<T> GetComponents<T>() where T : Component
        {
            var comps = components.FindAll(c => c.GetType() == typeof(T));

            return comps.Cast<T>().ToList();
        }
    }
}
