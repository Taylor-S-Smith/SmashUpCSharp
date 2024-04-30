using SmashUp.Utilities;

namespace SmashUp
{
    public static class Globals
    {
        public readonly static Dictionary<ConsoleKey, UserKeyPress> keyMappings = new();
        public readonly static Dictionary<UserKeyPress, (ConsoleKey Main, ConsoleKey? Alternate)> reverseKeyMappings = new();
    }
}
