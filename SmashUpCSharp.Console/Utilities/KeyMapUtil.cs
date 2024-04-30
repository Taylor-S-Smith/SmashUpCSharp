namespace SmashUp.Utilities
{
    public static class KeyMapUtil
    {
        public static void SetDefaultKeyMappings()
        {
            reverseKeyMappings.Clear();
            reverseKeyMappings.Add(UserKeyPress.Up, (ConsoleKey.UpArrow, ConsoleKey.W));
            reverseKeyMappings.Add(UserKeyPress.Down, (ConsoleKey.DownArrow, ConsoleKey.S));
            reverseKeyMappings.Add(UserKeyPress.Left, (ConsoleKey.LeftArrow, ConsoleKey.A));
            reverseKeyMappings.Add(UserKeyPress.Right, (ConsoleKey.RightArrow, ConsoleKey.D));
            reverseKeyMappings.Add(UserKeyPress.Confirm, (ConsoleKey.Enter, null));
            reverseKeyMappings.Add(UserKeyPress.Action, (ConsoleKey.E, null));
            reverseKeyMappings.Add(UserKeyPress.Status, (ConsoleKey.B, null));
            reverseKeyMappings.Add(UserKeyPress.Escape, (ConsoleKey.Escape, null));
            ApplyKeyMappings();
        }

        public static void ApplyKeyMappings()
        {
            keyMappings.Clear();
            foreach (var pair in reverseKeyMappings)
            {
                keyMappings.Add(pair.Value.Main, pair.Key);
                if (pair.Value.Alternate is not null)
                {
                    keyMappings.Add(pair.Value.Alternate.Value, pair.Key);
                }
            }
        }
    }
}
