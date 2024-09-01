using SmashUp.Frontend.Pages.GameSetUp;
using SmashUp.Frontend.Utilities;

namespace SmashUp.Frontend.Pages
{
    /// <summary>
    /// Displays the active page. Handles the main rendering loop. Passes keyboard input to the page for state changes. 
    /// The "Active Page" is whatever is currently being presented. 
    /// "ActiveKeyMappings" is whatever the ActivePage is allowing to use as input
    /// The presenter only refereshes the page once for every time the NeedToRender variable is true. The presenter
    /// passes NeedToRender by ref whenever it calls a pages "ChangeState" method. A page should set it to true if a refresh is needed.
    /// To navigate to a new page, the ActivePage's ChangeState() should return the PageId of the page that should be presented.
    /// </summary>
    /// <param name="page">The first active page presented</param>
    public class Presenter(PrimitivePage page, Dictionary<string, Func<PrimitivePage>> availablePageDictionary)
    {
        public PrimitivePage ActivePage = page;
        public Dictionary<ConsoleKey, UserKeyPress> ActiveKeyMappings = page.KeyMappings;

        public Dictionary<string, Func<PrimitivePage>> AvailablePageDictionary = availablePageDictionary;

        private bool NeedToRender = true;

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
                    UserKeyPress keyPress = ActiveKeyMappings.GetValueOrDefault(Console.ReadKey(true).Key);
                    string? navigation = ActivePage.HandleKeyPress(keyPress, ref NeedToRender);

                    //Perform redirection if needed
                    if (navigation != null)
                    {
                        //Returning the Quit page ends the presenter
                        if (navigation == "QUIT") { break; }

                        ActivePage = AvailablePageDictionary[navigation].Invoke();
                        ActiveKeyMappings = ActivePage.KeyMappings;
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
