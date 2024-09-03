using SmashUp.Frontend.Utilities;

namespace SmashUp.Frontend.Pages
{
    public abstract class PrimitivePage
    {
        /// <summary>
        /// Handles the rendering of the page. This will physically write to the console
        /// </summary>
        /// <param name="consoleWidth">Full console width</param>
        /// <param name="consoleHeight">Full console height</param>
        public abstract void Render(int consoleWidth, int consoleHeight);

        /// <summary>
        /// Allows a page to recieve and handle various keypresses. 
        /// The return value is the primary way to communicate with the presenter.
        /// Returning a PageCode will navigate the user to the associated page.
        /// Returning NULL will indicate that no navigation should occur.
        /// </summary>
        /// <param name="keyPress">The UserKeyPress that just occured</param>
        /// <param name="stateChanged">
        /// A secondary way to communicate with the presenter. 
        /// Setting this variable to true will refresh the page.
        /// By default this is false and will not result in the page refreshing.
        /// </param>
        /// <returns></returns>
        public abstract string? HandleKeyPress(UserKeyPress keyPress, ref bool stateChanged);

        /// <summary>
        /// The keymappings this page expects to be available. 
        /// This is primarily used to switch between the default GameplayKeyMappings
        /// and the alternative AlphaKeyMappings used for text.
        /// </summary>
        public virtual Dictionary<ConsoleKey, UserKeyPress> KeyMappings { get; set; } = GameplayKeyMappings;
    }
}
