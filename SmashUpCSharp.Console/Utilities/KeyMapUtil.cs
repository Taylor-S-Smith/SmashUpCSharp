namespace SmashUp.Utilities
{
    public static class KeyMapUtil
    {
        public static void SetDefaultKeyMappings()
        {
            GameplayKeyMappings.Clear();
            GameplayKeyMappings.Add(ConsoleKey.UpArrow, UserKeyPress.Up);
            GameplayKeyMappings.Add(ConsoleKey.DownArrow, UserKeyPress.Down);
            GameplayKeyMappings.Add(ConsoleKey.LeftArrow, UserKeyPress.Left);
            GameplayKeyMappings.Add(ConsoleKey.RightArrow, UserKeyPress.Right);
            GameplayKeyMappings.Add(ConsoleKey.Enter, UserKeyPress.Confirm);
            GameplayKeyMappings.Add(ConsoleKey.Escape, UserKeyPress.Escape);
            GameplayKeyMappings.Add(ConsoleKey.Backspace, UserKeyPress.Backspace);

            //Alphabet
            AlphaKeyMappings.Clear();
            AlphaKeyMappings.Add(ConsoleKey.A, UserKeyPress.A);
            AlphaKeyMappings.Add(ConsoleKey.B, UserKeyPress.B);
            AlphaKeyMappings.Add(ConsoleKey.C, UserKeyPress.C);
            AlphaKeyMappings.Add(ConsoleKey.D, UserKeyPress.D);
            AlphaKeyMappings.Add(ConsoleKey.E, UserKeyPress.E);
            AlphaKeyMappings.Add(ConsoleKey.F, UserKeyPress.F);
            AlphaKeyMappings.Add(ConsoleKey.G, UserKeyPress.G);
            AlphaKeyMappings.Add(ConsoleKey.H, UserKeyPress.H);
            AlphaKeyMappings.Add(ConsoleKey.I, UserKeyPress.I);
            AlphaKeyMappings.Add(ConsoleKey.J, UserKeyPress.J);
            AlphaKeyMappings.Add(ConsoleKey.K, UserKeyPress.K);
            AlphaKeyMappings.Add(ConsoleKey.L, UserKeyPress.L);
            AlphaKeyMappings.Add(ConsoleKey.M, UserKeyPress.M);
            AlphaKeyMappings.Add(ConsoleKey.N, UserKeyPress.N);
            AlphaKeyMappings.Add(ConsoleKey.O, UserKeyPress.O);
            AlphaKeyMappings.Add(ConsoleKey.P, UserKeyPress.P);
            AlphaKeyMappings.Add(ConsoleKey.Q, UserKeyPress.Q);
            AlphaKeyMappings.Add(ConsoleKey.R, UserKeyPress.R);
            AlphaKeyMappings.Add(ConsoleKey.S, UserKeyPress.S);
            AlphaKeyMappings.Add(ConsoleKey.T, UserKeyPress.T);
            AlphaKeyMappings.Add(ConsoleKey.U, UserKeyPress.U);
            AlphaKeyMappings.Add(ConsoleKey.V, UserKeyPress.V);
            AlphaKeyMappings.Add(ConsoleKey.W, UserKeyPress.W);
            AlphaKeyMappings.Add(ConsoleKey.X, UserKeyPress.X);
            AlphaKeyMappings.Add(ConsoleKey.Y, UserKeyPress.Y);
            AlphaKeyMappings.Add(ConsoleKey.Z, UserKeyPress.Z);
            AlphaKeyMappings.Add(ConsoleKey.Spacebar, UserKeyPress.Space);
            AlphaKeyMappings.Add(ConsoleKey.Enter, UserKeyPress.Confirm);
        }
    }
}
