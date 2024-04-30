namespace Renderer
{
    public class ConsoleHelper
    {
        /// <summary>
        /// Gets the current width and height of the console. This was copied from "Console Monsters"
        /// If I didn't understand why a piece of code was there, I commented it out. If errors occur 
        /// I can always come back and uncomment parts. I just want to understand why the things that 
        /// are here are here;
        /// </summary>
        /// <returns>The width and height of the console</returns>
        public static (int Width, int Height) GetWidthAndHeight()
        {
            //while (true) {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            
            if (OperatingSystem.IsWindows())
            {
                if (Console.BufferHeight != height) Console.BufferHeight = height;
                if (Console.BufferWidth != width) Console.BufferWidth = width;

                /*
                try
                {
                    if (Console.BufferHeight != height) Console.BufferHeight = height;
                    if (Console.BufferWidth != width) Console.BufferWidth = width;
                }
                catch (Exception)
                {
                    try
                    {
                        Console.Clear();
                    }
                    catch
                    {
                        // intentionally left blank
                    }
                    continue;
                }
                */

            }
            return (width, height);
        }

        /// <summary>
        /// Updates the height and width and clears the console when it is resized in preparation of new graphics
        /// </summary>
        /// <param name="previousWidth"></param>
        /// <param name="previousHeight"></param>
        /// <returns></returns>
        public static bool HandleConsoleResize(ref int previousWidth, ref int previousHeight)
        {
            var (width, height) = GetWidthAndHeight();
            if ((previousWidth, previousHeight) != (width, height))
            {
                (previousWidth, previousHeight) = (width, height);
                Console.Clear();
                return true;
            }
            return false;
        }
    }
}
