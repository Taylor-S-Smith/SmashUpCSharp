using SmashUp.Frontend.Utilities;

namespace SmashUp.Frontend;

public static class FrontendGlobals
{
    internal readonly static Dictionary<ConsoleKey, UserKeyPress> GameplayKeyMappings = [];
    internal readonly static Dictionary<ConsoleKey, UserKeyPress> AlphaKeyMappings = [];
}
