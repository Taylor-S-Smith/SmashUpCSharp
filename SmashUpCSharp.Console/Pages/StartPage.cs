using SmashUp.Models.Games;
using SmashUp.Utilities;
using System.Text;

namespace SmashUp.Rendering
{
    public class StartPage : PrimitivePage
    {
		private string[] Header  =
		[
			@"    _____ __  ______   _____ __  __   __  ______  __",
			@"   / ___//  |/  /   | / ___// / / /  / / / / __ \/ /",
			@"   \__ \/ /|_/ / /| | \__ \/ /_/ /  / / / / /_/ / /",
			@"  ___/ / /  / / ___ |___/ / __  /  / /_/ / ____/_/",
			@" /____/_/  /_/_/  |_/____/_/ /_/   \____/_/   (_)",
		];

		private int SelectedOption = 0;
        private const int HeaderPadding = 2;
        private const int OptionPadding = 1;
        private const int NumOptions = 4;


        public override void Render(int consoleWidth, int consoleHeight)
        {
            StringBuilder? buffer = null;

            //Ensure the current console size will fit the header
            int HeaderWidth = Header.Max(line => line.Length);
			if (consoleWidth - 1 >= HeaderWidth)
			{
                //Generate Options and caluclate height/width
				string[][] options =
				[
					AsciiUtil.ToAscii((SelectedOption is 0 ? "●" : "○") + " Start Game"),
                    AsciiUtil.ToAscii((SelectedOption is 1 ? "●" : "○") + " View Collection"),
                    AsciiUtil.ToAscii((SelectedOption is 2 ? "●" : "○") + " Options"),
                    AsciiUtil.ToAscii((SelectedOption is 3 ? "●" : "○") + " Exit"),
				];
				int optionsWidth = options.Max(o => o.Max(l => l.Length));
				int renderHeight = Header.Length + options.Sum(o => o.Length) + HeaderPadding + OptionPadding * options.Length;

                //Make sure the console can fit the options width and entire render height
				if (consoleHeight - 1 >= renderHeight && consoleWidth - 1 >= optionsWidth)
				{
                    //If the header is longer than the options, we need to offset the options to be centered
					int indentSize = Math.Max(0, (HeaderWidth - optionsWidth) / 2);
					string indent = new(' ', indentSize);

                    //Generate final combined render
					string[] render = new string[renderHeight];
					int i = 0;
					foreach (string line in Header)
					{
						render[i++] = line;
					}
					i += HeaderPadding;
					foreach (string[] option in options)
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
                    $@"Smash Up",
                    $@"{(SelectedOption is 0 ? ">" : " ")} Start Game",
                    $@"{(SelectedOption is 1 ? ">" : " ")} View Collection",
                    $@"{(SelectedOption is 2 ? ">" : " ")} Options",
                    $@"{(SelectedOption is 3 ? ">" : " ")} Exit",
                ];
                buffer = ScreenUtil.Center(render, (consoleHeight - 1, consoleWidth - 1));
            }

			Console.SetCursorPosition(0, 0);
			Console.Write(buffer);			
		}

        public override PrimitivePage? ChangeState(UserKeyPress keyPress, ref bool needToRender)
        {
            switch (keyPress)
            {
                case UserKeyPress.Up:
                    SelectedOption = Math.Max(0, SelectedOption - 1);
                    needToRender = true;
                    break;
                case UserKeyPress.Down:
                    SelectedOption = Math.Min(NumOptions - 1, SelectedOption + 1);
                    needToRender = true;
                    break;
                case UserKeyPress.Confirm:
                    switch (SelectedOption)
                    {
                        case 0:
                            return new BattlePage(new Battle());
                        case 1:
                            Console.WriteLine("Showing collection...");
                            break;
                        case 2:
                            Console.WriteLine("Showing options...");
                            break;
                        case 3:
                            return new Quit();
                        default:
                            throw new NotImplementedException();
                    }
					break;
                case UserKeyPress.Escape:
                    Console.WriteLine("Escaping fame...");
					break;
            }

			return null;
        }
    }
}
