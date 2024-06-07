using SmashUp.Utilities;

namespace SmashUp.Rendering
{
    /// <summary>
    /// Displays the active page. Handles the main rendering loop. Passes keyboard input to the page for state changes. 
    /// The "Active Page" is whatever is currently being presented. 
    /// The presenter only refereshes the page once for every time the NeedToRender variable is true. The presenter
    /// passes NeedToRender by ref whenever it calls a pages "ChangeState" method. A page should set it to true if a refresh is needed.
    /// To navigate to a new page, the ActivePage's ChangeState() should return the Page object that should be presented.
    /// </summary>
    /// <param name="page">The first active page presented</param>
    public class Presenter(PrimitivePage page)
    {
        public PrimitivePage ActivePage = page;
        private bool NeedToRender = true;

        public Dictionary<ConsoleKey, UserKeyPress> activeKeyMappings = page.KeyMappings;

        public void Present()
        {
            //Initialize
            var (consoleWidth, consoleHeight) = ConsoleUtil.GetWidthAndHeight();

            //Main Rendering Loop
            while (true)
            {
                //We only need to re-render if the console resized,
                //but we always need to check for user input
                if (NeedToRender)
                {
                    Console.CursorVisible = false;
                    ActivePage.Render(consoleWidth, consoleHeight);
                    NeedToRender = false;
                }
                if (Console.KeyAvailable)
                {
                    UserKeyPress keyPress = activeKeyMappings.GetValueOrDefault(Console.ReadKey(true).Key);
                    PrimitivePage? page = ActivePage.ChangeState(keyPress, ref NeedToRender);

                    //Perform redirection if needed
                    if (page != null)
                    {
                        //Returning the Quit page ends the presenter
                        if(page is Quit) { break; }

                        activeKeyMappings = page.KeyMappings;
                        ActivePage = page;
                        NeedToRender = true;
                    }
                }
                NeedToRender = NeedToRender || ConsoleUtil.HandleConsoleResize(ref consoleWidth, ref consoleHeight);

                // prevent CPU spiking
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

        }
    }
}
