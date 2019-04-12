using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MonoCJ
{
    public abstract class MonoGame : Game
    {
        GraphicsDeviceManager graphics;

        SpriteBatch spriteBatch;

        public Color backgroundColor = Color.Black;

        public SpriteFont defaultFont;

        public List<GameObject> gos = new List<GameObject>();

        public Camera mainCamera;


        public Random rand = new Random();

        private string fontName;
        private int fontSize;


        private int resWdth;
        private int resHght;
        private int windowWdth;
        private int windowHght;

        public MonoGame(string fontName, int fontSize, int resolutionWidth, int resolutionHeight)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


            this.fontName = fontName;
            this.fontSize = fontSize;

            resWdth = resolutionWidth;
            resHght = resolutionHeight;
            windowWdth = resWdth * 100;
            windowHght = resHght * 100;

            
            Window.AllowUserResizing = true;

        }


        protected override void Initialize()
        {
            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);

            try
            {
                defaultFont = Content.Load<SpriteFont>(@"Fonts\defaultFont");
            }
            catch
            {
                Graphics.CreateFontAsset(fontName, fontSize);
                defaultFont = Content.Load<SpriteFont>(@"Fonts\defaultFont");
            }

            mainCamera = new Camera();

            Graphics.Init(this);

            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);

            Settings.Init(resWdth, resHght, windowWdth, windowHght, this);

            Start();

        }

        protected override void Update(GameTime gameTime)
        {
            Input.Begin();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            mainCamera.Update();

            for (int i = 0; i < gos.Count; i++)
            {
                gos[i].Update(dt);
            }

            Update(dt);

            Input.End();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(backgroundColor);

            spriteBatch.Begin(
                SpriteSortMode.FrontToBack, 
                transformMatrix: mainCamera.Transform);

            for (int i = 0; i < gos.Count; i++)
            {
                if (gos[i].isActive)
                    gos[i].Draw(spriteBatch);
            }

            Draw(spriteBatch);


            spriteBatch.End();

            base.Draw(gameTime);
        }

        void OnResize(object o, EventArgs e)
        {
            Settings.PreserveResolution();

            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            graphics.ApplyChanges();
            

            OnWindowResize();
        }

      




        public void ResizeWindow(int wdth, int hght)
        {
            graphics.PreferredBackBufferWidth = wdth;
            graphics.PreferredBackBufferHeight = hght;
            GraphicsDevice.Viewport = new Viewport(0, 0, wdth, hght);
            graphics.ApplyChanges();
        }

 


        public abstract void Start();

        public abstract void Update(float dt);

        public abstract void Draw(SpriteBatch sb);

        public abstract void OnWindowResize();


    }
}
