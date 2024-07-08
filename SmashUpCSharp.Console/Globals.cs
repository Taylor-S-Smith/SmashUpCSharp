using SmashUp.Utilities;

namespace SmashUp
{
    public static class Globals
    {
        internal readonly static Dictionary<ConsoleKey, UserKeyPress> GameplayKeyMappings = [];
        internal readonly static Dictionary<ConsoleKey, UserKeyPress> AlphaKeyMappings = [];

        //public readonly static Dictionary<UserKeyPress, (ConsoleKey Main, ConsoleKey? Alternate)> reverseKeyMappings = new();

        internal readonly static int HEADER_PADDING = 2;
        public readonly static int OPTION_PADDING = 1;
    }
}
