﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoCJ
{
    public class Camera
    {

        public float Zoom { get; set; }

        public Vector2 Position { get; set; }

        public Matrix Transform { get; protected set; }

        public Rectangle VisibleArea { get; protected set; }

        public Camera()
        {
            Zoom = 1f;
            Position = Vector2.Zero;
        }



        private void UpdateMatrix()
        {
            Transform = Matrix.CreateTranslation(new Vector3(Position.X, Position.Y, 0)) *
                        Matrix.CreateScale(Zoom);
        }



        public void MoveCamera(Vector2 movePosition)
        {
            Vector2 newPosition = Position + movePosition;
            Position = newPosition;
        }


        public void Update()
        {

            UpdateMatrix();
        }

        public void AdjustZoom(float zoomAmount)
        {
            Zoom += zoomAmount;
            if (Zoom < .35f)
            {
                Zoom = .35f;
            }
            if (Zoom > 2f)
            {
                Zoom = 2f;
            }
        }


    }
}