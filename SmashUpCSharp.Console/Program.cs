using System.Diagnostics;

namespace SmashUpCSharp.Console
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            List<string> smashUpLogo =
            [
                "    _____ __  ______   _____ __  __   __  ______  __",
                "   / ___//  |/  /   | / ___// / / /  / / / / __ \\/ /",
                "   \\__ \\/ /|_/ / /| | \\__ \\/ /_/ /  / / / / /_/ / /",
                "  ___/ / /  / / ___ |___/ / __  /  / /_/ / ____/_/",
                " /____/_/  /_/_/  |_/____/_/ /_/  \\____/_/   (_)",
            ];

            System.Console.WriteLine("Welcome to...");
            Print(smashUpLogo);

        }

        static void Print(List<string> strings)
        {
            foreach (string s in strings)
            {
                System.Console.WriteLine(s);
            }
        }

        
    }
}
