using System.Text;

namespace SmashUp.Utilities
{
    public static class PageUtil
    {

        /// <summary>Generates a text selection menu</summary>
        public static StringBuilder? generateTextSelect(int consoleWidth, int consoleHeight, string[] header, string[] options, int selectedOption)
        {
            
            StringBuilder? buffer = new();

            //Ensure the current console size will fit the header
            int HeaderWidth = header.Max(line => line.Length);
            if (consoleWidth - 1 >= HeaderWidth)
            {
                //Generate Options and caluclate height/width
                List<string[]> optionGraphics = [];
                for (int i = 0; i < options.Length; i++)
                {
                    optionGraphics.Add(AsciiUtil.ToAscii((selectedOption == i ? "●" : "○") + $" {options[i]}"));
                }

                int optionsWidth = optionGraphics.Max(o => o.Max(l => l.Length));
                int renderHeight = header.Length + optionGraphics.Sum(o => o.Length) + HEADER_PADDING + OPTION_PADDING * optionGraphics.Count;

                //Make sure the console can fit the options width and entire render height
                if (consoleHeight - 1 >= renderHeight && consoleWidth - 1 >= optionsWidth)
                {
                    //If the header is longer than the options, we need to offset the options to be centered
                    int indentSize = Math.Max(0, (HeaderWidth - optionsWidth) / 2);
                    string indent = new(' ', indentSize);

                    //Generate final combined render
                    string[] render = new string[renderHeight];
                    int i = 0;
                    foreach (string line in header)
                    {
                        render[i++] = line;
                    }
                    i += HEADER_PADDING;
                    foreach (string[] option in optionGraphics)
                    {
                        i += OPTION_PADDING;
                        foreach (string line in option)
                        {
                            render[i++] = indent + line;
                        }
                    }

                    buffer = ScreenUtil.Center(render, (consoleHeight - 1, consoleWidth - 1));
                }
            }
            //If the console is too small to show the regular menu, print the tiny one instead
            if (buffer is null)
            {
                string[] render =
                [
                    "Please increase your screen size"
                    /*
                    $@"Smash Up",
                    $@"{(SelectedOption is 0 ? ">" : " ")} Start Game",
                    $@"{(SelectedOption is 1 ? ">" : " ")} View Collection",
                    $@"{(SelectedOption is 2 ? ">" : " ")} Options",
                    $@"{(SelectedOption is 3 ? ">" : " ")} Exit",
                    */
                ];
                buffer = ScreenUtil.Center(render, (consoleHeight - 1, consoleWidth - 1));
            }

            return buffer;
        }

        /// <summary>WIP - Generates a grid selection menu</summary>
        public static StringBuilder? generateGridSelect(int consoleWidth, int consoleHeight, string[] header, string[] options, int selectedOption)
        {
            const int HeaderPadding = 2;
            const int OptionPadding = 1;
            StringBuilder? buffer = new();

            //Ensure the current console size will fit the header
            int HeaderWidth = header.Max(line => line.Length);
            if (consoleWidth - 1 >= HeaderWidth)
            {
                //Generate Options and caluclate height/width
                List<string[]> optionGraphics = [];
                for (int i = 0; i < options.Length; i++)
                {
                    optionGraphics.Add(AsciiUtil.ToAscii((selectedOption == i ? "●" : "○") + $" {options[i]}"));
                }

                int optionsWidth = optionGraphics.Max(o => o.Max(l => l.Length));
                int renderHeight = header.Length + optionGraphics.Sum(o => o.Length) + HeaderPadding + OptionPadding * optionGraphics.Count;

                //Make sure the console can fit the options width and entire render height
                if (consoleHeight - 1 >= renderHeight && consoleWidth - 1 >= optionsWidth)
                {
                    //If the header is longer than the options, we need to offset the options to be centered
                    int indentSize = Math.Max(0, (HeaderWidth - optionsWidth) / 2);
                    string indent = new(' ', indentSize);

                    //Generate final combined render
                    string[] render = new string[renderHeight];
                    int i = 0;
                    foreach (string line in header)
                    {
                        render[i++] = line;
                    }
                    i += HeaderPadding;
                    foreach (string[] option in optionGraphics)
                    {
                        i += OptionPadding;
                        foreach (string line in option)
                        {
                            render[i++] = indent + line;
                        }
                    }

                    buffer = ScreenUtil.Center(render, (consoleHeight - 1, consoleWidth - 1));
                }
            }
            //If the console is too small to show the regular menu, print the tiny one instead
            if (buffer is null)
            {
                string[] render =
                [
                    "Please increase your screen size"
                    /*
                    $@"Smash Up",
                    $@"{(SelectedOption is 0 ? ">" : " ")} Start Game",
                    $@"{(SelectedOption is 1 ? ">" : " ")} View Collection",
                    $@"{(SelectedOption is 2 ? ">" : " ")} Options",
                    $@"{(SelectedOption is 3 ? ">" : " ")} Exit",
                    */
                ];
                buffer = ScreenUtil.Center(render, (consoleHeight - 1, consoleWidth - 1));
            }

            return buffer;
        }
    }
}
