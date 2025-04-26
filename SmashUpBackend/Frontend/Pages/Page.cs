using System.Text;
using SmashUp.Frontend.Utilities;

namespace SmashUp.Frontend.Pages;

public abstract class Page<T>
{
    //GRAPHICS
    protected const int HEADER_PADDING = 2;
    protected const int OPTION_PADDING = 1;

    /// <summary>
    /// Handles the rendering of the page. This will physically write to the console
    /// </summary>
    /// <param name="consoleWidth">Full console width</param>
    /// <param name="consoleHeight">Full console height</param>
    protected void Render(int consoleWidth, int consoleHeight)
    {
        StringBuilder renderBuffer = GenerateRender(consoleWidth, consoleHeight);
        Console.SetCursorPosition(0, 0);
        Console.Write(renderBuffer);
    }

    /// <summary>
    /// Creates a string to render to render
    /// </summary>
    protected abstract StringBuilder GenerateRender(int consoleWidth, int consoleHeight);

    //INPUT HANDLING

    /// <summary>
    /// The keymappings this page expects to be available. 
    /// This is primarily used to switch between the default GameplayKeyMappings
    /// and the alternative AlphaKeyMappings used for text.
    /// </summary>
    public virtual Dictionary<ConsoleKey, UserKeyPress> KeyMappings { get; set; } = FrontendGlobals.GameplayKeyMappings;


    //HANDLE CONSOLE

    protected bool _needToRender = true;
}

internal abstract class ValuePage<T> : Page<T> where T : struct
{
    /// <summary>
    /// Allows a page to recieve and handle various keypresses. 
    /// The return value is the primary way to communicate with other pages.
    /// Set the protected _needToRender value to true if you want to rerender the page.
    /// </summary>
    /// <param name="keyPress">The UserKeyPress that just occured</param>
    /// <returns>Null to continue loop. Returning anything but null will exit the page loop</returns>
    public abstract T? HandleKeyPress(UserKeyPress keyPress);

    public T Run()
    {
        var (consoleWidth, consoleHeight) = ConsoleUtil.GetWidthAndHeight();
        _needToRender = true;

        //Main Rendering Loop
        while (true)
        {
            if (_needToRender)
            {
                Console.CursorVisible = false;
                Render(consoleWidth, consoleHeight);
                _needToRender = false;
            }
            if (Console.KeyAvailable) //Using ReadKey will halt execution, this check other things to process, (e.i. ongoing animations) if no key is available
            {
                UserKeyPress keyPress = KeyMappings.GetValueOrDefault(Console.ReadKey(true).Key);
                T? pageOutput = HandleKeyPress(keyPress);

                if (pageOutput != null)
                {
                    return (T)pageOutput;
                }
                _needToRender = true;
            }
            _needToRender = _needToRender || ConsoleUtil.CheckAndHandleConsoleResize(ref consoleWidth, ref consoleHeight);

            // Apparently prevents CPU spiking
            Thread.Sleep(TimeSpan.FromMilliseconds(1));
        }
    }
}

internal abstract class ReferencePage<T> : Page<T> where T : class
{
    /// <summary>
    /// Allows a page to recieve and handle various keypresses. 
    /// The return value is the primary way to communicate with other pages.
    /// Set the protected _needToRender value to true if you want to rerender the page.
    /// </summary>
    /// <param name="keyPress">The UserKeyPress that just occured</param>
    /// <returns>Null to continue loop. Returning anything but null will exit the page loop</returns>
    public abstract T? HandleKeyPress(UserKeyPress keyPress);

    public T Run()
    {
        var (consoleWidth, consoleHeight) = ConsoleUtil.GetWidthAndHeight();

        //Main Rendering Loop
        while (true)
        {
            if (_needToRender)
            {
                Console.CursorVisible = false;
                Render(consoleWidth, consoleHeight);
                _needToRender = false;
            }
            if (Console.KeyAvailable) //Using ReadKey will halt execution, this check other things to process, (e.i. ongoing animations) if no key is available
            {
                UserKeyPress keyPress = KeyMappings.GetValueOrDefault(Console.ReadKey(true).Key);
                T? pageOutput = HandleKeyPress(keyPress);

                if (pageOutput != null)
                {
                    return pageOutput;
                }
            }
            _needToRender = _needToRender || ConsoleUtil.CheckAndHandleConsoleResize(ref consoleWidth, ref consoleHeight);

            // Apparently prevents CPU spiking
            Thread.Sleep(TimeSpan.FromMilliseconds(1));
        }
    }
}
