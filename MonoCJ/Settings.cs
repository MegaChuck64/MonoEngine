

using Microsoft.Xna.Framework;

namespace MonoCJ
{
    public static class Settings
    {
        public static int WindowWidth { get; private set; } = 800;
        public static int WindowHeight { get; private set; } = 450;

        public static int ResolutionWidth = 16;
        public static int ResolutionHeight = 9;

        static MonoGame game;


        public static float scale { get; private set; } = 1f;

        private static Vector2 originalWindowSize;
        private static float minScale = 0.4f;

        public static void Init(int resWidth, int resHeight, int windowWidth, int windowHeight, MonoGame gme)
        {
            ResolutionWidth = resWidth;
            ResolutionHeight = resHeight;

            game = gme;

            WindowWidth = windowWidth;
            WindowHeight = windowHeight;

            game.ResizeWindow(WindowWidth, WindowHeight);

            originalWindowSize = new Vector2(WindowWidth, WindowHeight);

            PreserveResolution();
        }

        public static Vector2 GetPanelSize(float widthPercent, float heightPercent)
        {
            return new Vector2((int)MathUtils.Percentage(widthPercent, Settings.WindowWidth), (int)MathUtils.Percentage(heightPercent, Settings.WindowHeight));
        }

        public static void PreserveResolution()
        {


            bool fromSide = false;

            if (WindowWidth < game.Window.ClientBounds.Width || WindowWidth > game.Window.ClientBounds.Width)
            {
                fromSide = true;
            }
           
            WindowWidth = game.Window.ClientBounds.Width;
            WindowHeight = game.Window.ClientBounds.Height;

            var scaleWidth = WindowWidth / ResolutionWidth;
            var scaleHeight = WindowHeight / ResolutionHeight;

            float scl = (fromSide) ? scaleWidth : scaleHeight;

            WindowWidth = (int)(ResolutionWidth * scl);
            WindowHeight = (int)(ResolutionHeight * scl);

            game.ResizeWindow(WindowWidth, WindowHeight);

            scale = originalWindowSize.X / WindowWidth;

        }
    }
}
