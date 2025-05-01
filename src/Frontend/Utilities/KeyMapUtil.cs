namespace SmashUp.Frontend.Utilities
{
    public static class KeyMapUtil
    {
        public static void SetDefaultKeyMappings()
        {
            FrontendGlobals.GameplayKeyMappings.Clear();
            FrontendGlobals.GameplayKeyMappings.Add(ConsoleKey.UpArrow, UserKeyPress.Up);
            FrontendGlobals.GameplayKeyMappings.Add(ConsoleKey.DownArrow, UserKeyPress.Down);
            FrontendGlobals.GameplayKeyMappings.Add(ConsoleKey.LeftArrow, UserKeyPress.Left);
            FrontendGlobals.GameplayKeyMappings.Add(ConsoleKey.RightArrow, UserKeyPress.Right);
            FrontendGlobals.GameplayKeyMappings.Add(ConsoleKey.Enter, UserKeyPress.Confirm);
            FrontendGlobals.GameplayKeyMappings.Add(ConsoleKey.Escape, UserKeyPress.Escape);
            FrontendGlobals.GameplayKeyMappings.Add(ConsoleKey.Backspace, UserKeyPress.Backspace);

            //Alphabet
            FrontendGlobals.AlphaKeyMappings.Clear();
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.A, UserKeyPress.A);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.B, UserKeyPress.B);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.C, UserKeyPress.C);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.D, UserKeyPress.D);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.E, UserKeyPress.E);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.F, UserKeyPress.F);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.G, UserKeyPress.G);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.H, UserKeyPress.H);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.I, UserKeyPress.I);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.J, UserKeyPress.J);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.K, UserKeyPress.K);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.L, UserKeyPress.L);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.M, UserKeyPress.M);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.N, UserKeyPress.N);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.O, UserKeyPress.O);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.P, UserKeyPress.P);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.Q, UserKeyPress.Q);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.R, UserKeyPress.R);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.S, UserKeyPress.S);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.T, UserKeyPress.T);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.U, UserKeyPress.U);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.V, UserKeyPress.V);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.W, UserKeyPress.W);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.X, UserKeyPress.X);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.Y, UserKeyPress.Y);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.Z, UserKeyPress.Z);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.Spacebar, UserKeyPress.Space);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.Enter, UserKeyPress.Confirm);
            FrontendGlobals.AlphaKeyMappings.Add(ConsoleKey.Backspace, UserKeyPress.Backspace);
        }
    }
}
