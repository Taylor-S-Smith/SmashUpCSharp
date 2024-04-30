using SmashUp.Utilities;

namespace SmashUp.Rendering
{
    public class Presenter(PrimitivePage page)
    {
        public PrimitivePage Page = page;
        private bool needToRender = true;

        public void Present()
        {
            //Initialize
            var (consoleWidth, consoleHeight) = ConsoleUtil.GetWidthAndHeight();

            //Main Rendering Loop
            while (true)
            {
                //We only need to re-render if the console resized,
                //but we always need to check for user input
                if (needToRender)
                {
                    Console.CursorVisible = false;
                    Page.Render(consoleWidth, consoleHeight);
                    needToRender = false;
                }
                if (Console.KeyAvailable)
                {
                    UserKeyPress keyPress = keyMappings.GetValueOrDefault(Console.ReadKey(true).Key);
                    PrimitivePage? page = Page.ChangeState(keyPress, ref needToRender);

                    //Perform redirection if needed
                    if (page != null)
                    {
                        //Returning the Quit page ends the presenter
                        if(page is Quit) { break; }

                        Page = page;
                        needToRender = true;
                    }
                }
                needToRender = needToRender || ConsoleUtil.HandleConsoleResize(ref consoleWidth, ref consoleHeight);

                // prevent CPU spiking
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

        }
    }
}
