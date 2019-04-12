
using Microsoft.Xna.Framework.Graphics;

namespace MonoCJ
{
    public abstract class Component
    {
        public GameObject Owner { get; private set; }

        public float drawLayer = 0;

        public Component(GameObject owner)
        {
            Owner = owner;
        }
    }

    public interface IUpdatable
    {
        void Update(float dt);
    }

    public interface IDrawable
    {
        void Draw(SpriteBatch sb);
    }

    //public abstract class CameraComponent : Component, IDrawable
    //{


    //    public Camera camera;
    //    public abstract void Draw(SpriteBatch sb);

    //    public CameraComponent(GameObject owner, Camera cam): base(owner)
    //    {
    //        camera = cam;
    //    }

    //}

}
