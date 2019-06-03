
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonoCJ
{
    public abstract class Component
    {
        public GameObject Owner { get; private set; }

        public string name = "";

        public float drawLayer = 0;

        public Component(GameObject owner, string name = null)
        {
            Owner = owner;
            if (name == null)
            {
                var tp = GetType();

                name = tp.ToString() + owner.components.FindAll(c=>c.GetType() == GetType()).Count;
            }

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
