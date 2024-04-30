using Models.Cards;
using Repositories;
using SmashUp.Models.Games;
using SmashUp.Utilities;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;

namespace SmashUp.Rendering
{
    public class BattlePage : PrimitivePage
    {
        //Visual Elements

        //Bases in play
        private string[] BaseField = [""];
        private int BaseFieldPadding;
        //Minions and actions in play
        private string[] CardField = [""];
        private int CardFieldPadding;
        //Game statistics (Active Player, Minion/Action count , VP total, etc.)
        private string[] StatField = [""];
        private int StatFieldPadding;
        //The player's input area. Usually will be their hand, but is also how they view their decks
        private string[] ConsoleField = [""];

        //Game Objects
        private Battle Game { get; set; }

        //Temporary COnstructor to manupulate the game state for testing
        public BattlePage(Battle game)
        {
            BaseCardRepository baseRepo = new();
            game.ActiveBases.Add(baseRepo.Get(1)!);
            game.ActiveBases.Add(baseRepo.Get(2)!);
            game.ActiveBases.Add(baseRepo.Get(3)!);


            Game = game;
        }

        public override void Render(int consoleWidth, int consoleHeight)
        {
            //Initalize vars and generate fields
            StringBuilder? buffer = null;
            GenerateBaseField();

            //Ensure the current console size will fit the header
            int renderWidth = new[] {
                BaseField.Max(line => line.Length),
                CardField.Max(line => line.Length),
                StatField.Max(line => line.Length),
                ConsoleField.Max(line => line.Length)
            }.Max();

            int renderHeight = BaseField.Length + 
                               BaseFieldPadding + 
                               CardField.Length +
                               CardFieldPadding +
                               StatField.Length +
                               StatFieldPadding +
                               ConsoleField.Length;

            //Make sure the whole render can fit in the console
            if (consoleHeight - 1 >= renderHeight && consoleWidth - 1 >= renderWidth)
			{
                //Generate final combined render
				string[] render = new string[renderHeight];
				int i = 0;
				foreach (string line in BaseField)
				{
					render[i++] = line;
                }
				i += BaseFieldPadding;
                foreach (string line in CardField)
                {
                    render[i++] = line;
                }
                i += CardFieldPadding;
                foreach (string line in StatField)
                {
                    render[i++] = line;
                }
                i += StatFieldPadding;
                foreach (string line in ConsoleField)
                {
                    render[i++] = line;
                }

                buffer = ScreenUtil.Center(render, (Game.ActiveBases[0].Graphic.Count + 6, consoleWidth - 1));
			}
            //Let the user know the screen is too small
            if (buffer is null)
            {
                string[] render =
                [
                    $@"Please increase your screen size"
                ];
                buffer = ScreenUtil.Center(render, (consoleHeight - 1, consoleWidth - 1));
            }

			Console.SetCursorPosition(0, 0);
			Console.Write(buffer);			
		}

        private void GenerateBaseField()
        {
            int numBases = Game.ActiveBases.Count;
            int baseGraphicLength = Game.ActiveBases[0].Graphic.Count;

            BaseField = new string[baseGraphicLength];

            for (int i = 0; i < baseGraphicLength; i++)
            {
                for (int j = 0; j < numBases; j++)
                {
                    BaseField[i] += Game.ActiveBases[j].Graphic[i];
                    if(j < numBases - 1)
                    {
                        BaseField[i] += " ";
                    }
                }
            }
        }

        public override PrimitivePage? ChangeState(UserKeyPress keyPress, ref bool needToRender)
        {
            switch (keyPress)
            {
                case UserKeyPress.Escape:
                    return new StartPage();
            }

			return null;
        }
    }
}
