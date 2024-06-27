using SmashUp.Models.Games;
using SmashUp.Utilities;
using Models.Cards;
using Repositories;
using System.Text;
using Models.Player;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace SmashUp.Rendering
{
    public class BattlePage : PrimitivePage
    {
        //Static Variables


        //Repositories
        BaseService _baseService = new();

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
        private Battle Game { get; set; } = new();

        public BattlePage(GameSetUpModel gameSetUp)
        {
            Game.Players = gameSetUp.Players;
            Game.BaseDeck.Cards = _baseService.GetBaseCards(gameSetUp.Players.SelectMany(x => x.Factions).ToList());
            Game.ActiveBases = Game.BaseDeck.DrawCards();
        }

        public override void Render(int consoleWidth, int consoleHeight)
        {
            //Initalize vars and generate fields
            StringBuilder? buffer = null;
            GenerateBaseField();
            GenerateCardField();

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

        /// <summary>
        /// Generates the base graphics
        /// </summary>
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

        /// <summary>
        /// Generates the text that lists the cards at each base for each player
        /// </summary>
        private void GenerateCardField()
        {
            //Iterate through all lines
            int lineNum = 0;
            while(true)
            {
                CardField.Append("");
                foreach(BaseCard baseCard in Game.ActiveBases)
                {
                    foreach(PrimitivePlayer player in Game.Players)
                    {
                        CardField[lineNum] += baseCard.GetCardsByPlayer(player)[lineNum];
                    }
                    CardField[lineNum] += " ";
                }
                lineNum++;
            }
                
        }

        /// <summary>
        /// Generates the field that lists the game statistics
        /// </summary>
        private void GenerateConsoleField(int consoleWidth, int consoleHeight)
        {

        }

        /// <summary>
        /// Generates the field that displays the result of user input
        /// </summary>
        private void GenerateStatField()
        {

        }


        public override PrimitivePage? ChangeState(UserKeyPress keyPress, ref bool stateChanged)
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
