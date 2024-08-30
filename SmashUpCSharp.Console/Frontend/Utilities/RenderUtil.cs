using System.Text;

namespace SmashUp.Frontend.Utilities
{
    public static class RenderUtil
    {

        /// <summary>Generates a text selection menu</summary>
        public static StringBuilder? GenerateTextSelect(int consoleWidth, int consoleHeight, string[] header, string[] options, int selectedOption)
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

                    buffer = Center(render, (consoleHeight - 1, consoleWidth - 1));
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
                buffer = Center(render, (consoleHeight - 1, consoleWidth - 1));
            }

            return buffer;
        }

        /// <summary>WIP - Generates a grid selection menu</summary>
        public static StringBuilder? GenerateGridSelect(int consoleWidth, int consoleHeight, string[] header, string[] options, int selectedOption)
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

                    buffer = Center(render, (consoleHeight - 1, consoleWidth - 1));
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
                buffer = Center(render, (consoleHeight - 1, consoleWidth - 1));
            }

            return buffer;
        }

        public static StringBuilder Center(string[] render, (int Height, int Width) bufferSize, (int J, int I)? renderCenterPoint = null)
        {
            int renderWidth = render.Max(line => line is null ? 0 : line.Length);
            renderCenterPoint ??= (render.Length / 2, renderWidth / 2);
            (int J, int I) offset = ((bufferSize.Height - render.Length) / 2, (bufferSize.Width - renderWidth) / 2);
            offset = (offset.J + renderCenterPoint.Value.J - render.Length / 2, offset.I + renderCenterPoint.Value.I - renderWidth / 2);
            StringBuilder sb = new(bufferSize.Height * bufferSize.Width);
            for (int j = 0; j < bufferSize.Height; j++)
            {
                for (int i = 0; i < bufferSize.Width; i++)
                {
                    var (dj, di) = (j - offset.J, i - offset.I);
                    if (dj >= 0 && dj < render.Length && di >= 0 && render[dj] is not null && di < render[dj].Length)
                    {
                        char c = render[dj][di];
                        sb.Append(c);
                    }
                    else
                    {
                        sb.Append(' ');
                    }
                }
                sb.AppendLine();
            }
            return sb;
        }

        public static string CenterString(string stringToCenter, int totalLength)
        {
            if (stringToCenter.Length <= totalLength)
            {
                return stringToCenter.PadLeft((totalLength - stringToCenter.Length) / 2
                                + stringToCenter.Length)
                       .PadRight(totalLength);
            }
            else
            {
                return stringToCenter.Substring(0, totalLength);
            }
        }

        public static string LeftJustifyString(string stringToJustify, int totalLength)
        {
            if (stringToJustify.Length <= totalLength)
            {
                return stringToJustify.PadRight(totalLength);
            }
            else
            {
                return stringToJustify[..totalLength];
            }
        }
    }
}
