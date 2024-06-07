using SmashUp.Utilities;

namespace SmashUp
{
    public static class Globals
    {
        public readonly static Dictionary<ConsoleKey, UserKeyPress> GameplayKeyMappings = new();
        public readonly static Dictionary<ConsoleKey, UserKeyPress> AlphaKeyMappings = new();

        //public readonly static Dictionary<UserKeyPress, (ConsoleKey Main, ConsoleKey? Alternate)> reverseKeyMappings = new();

        public readonly static int HEADER_PADDING = 2;
        public readonly static int OPTION_PADDING = 1;
    }
}
