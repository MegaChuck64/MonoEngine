using Microsoft.Xna.Framework.Input;

namespace MonoCJ
{
 

    public static class Input
    {
        public enum MouseButton
        {
            left, middle, right
        }

        public delegate void ScrollEvent(bool up);


        public static KeyboardState keys { get; private set; }
        public static KeyboardState lastKeys { get; private set; }
        public static MouseState mouse { get; private set; }
        public static MouseState lastMouse { get; private set; }

        public static int scrollValue { get; private set; } = 0;
        public static int lastScrollValue { get; private set; } = 0;


        public static void Begin()
        {
            keys = Keyboard.GetState();
            mouse = Mouse.GetState();
            scrollValue = mouse.ScrollWheelValue;
        }

        public static void End()
        {
            lastKeys = keys;
            lastMouse = mouse;
            lastScrollValue = scrollValue;
        }

        
        public static float GetScroll()
        {
            if (lastScrollValue > scrollValue)
            {
                return -1f;
            }
            else if (lastScrollValue < scrollValue)
            {
                return 1f;
            }

            return 0f;
        }


        public static bool WasPressed(Keys key)
        {
            return (lastKeys.IsKeyUp(key) && keys.IsKeyDown(key));
        }

        public static bool WasPressed(MouseButton mb)
        {
            switch (mb)
            {
                case MouseButton.left:
                    return (lastMouse.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed);

                case MouseButton.middle:
                    return (lastMouse.MiddleButton == ButtonState.Released && mouse.MiddleButton == ButtonState.Pressed);

                case MouseButton.right:
                    return (lastMouse.RightButton == ButtonState.Released && mouse.RightButton == ButtonState.Pressed);

                default:
                    return false;
            }
        }

        public static float GetAxis(string axis)
        {
            float val = 0f;

            switch (axis.ToLower())
            {
                case "horizontal":
                    if (keys.IsKeyDown(Keys.A))
                    {
                        val -= 1f;
                    }

                    if (keys.IsKeyDown(Keys.D))
                    {
                        val += 1f;
                    }
                    break;

                case "vertical":
                    if (keys.IsKeyDown(Keys.W))
                    {
                        val -= 1f;
                    }

                    if (keys.IsKeyDown(Keys.S))
                    {
                        val += 1f;
                    }
                    break;
            }

            return val;
        }
    }
}
