using SmashUp.Models.Games;
using SmashUp.Utilities;
using Repositories;
using Models.Cards;
using System.Text;
using Services;

namespace SmashUp.Rendering
{
    internal class BattlePage : PrimitivePage
    {
        //Static Variables
        readonly string HORIZONTAL_PADDING = " ";
        readonly int MIN_CARD_FIELD_SIZE = 15;

        //SERVICES
        readonly IBaseService _baseService;
        readonly IPlayableCardService _playableCardService;
        readonly IPlayerService _playerService;

        //Bases in play
        private string[] BaseField = [""];
        private readonly int BaseFieldPadding = 0;
        private int BaseGraphicLength = 0;

        //Minions and actions in play
        private string[] CardField = [""];
        private readonly int CardFieldPadding = 0;

        //Game statistics (Active Player, Minion/Action count , VP total, etc.)
        private string[] StatField = [""];
        private readonly int StatFieldPadding = 0;

        //The player's input area. Usually will be their hand, but is also how they view their decks
        private string[] ConsoleField = [""];

        //Game Objects
        private Battle Game { get; set; } = new();

        //Navigation
        public BattlePage(IBaseService baseService, IPlayableCardService playableCardService, IPlayerService playerService)
        {
            _baseService = baseService;
            _playableCardService = playableCardService;
            _playerService = playerService;

            Game.Players = _playerService.GetAll();
            Game.BaseDeck.Cards = _baseService.GetBaseCards(Game.Players.SelectMany(x => x.Factions).ToList());
            Game.ActiveBases = Game.BaseDeck.DrawCards(Game.Players.Count + 1);

            //JUST TO TEST
            Game.ActiveBases[0].AttachCard(_playableCardService.Get(0));
            Game.ActiveBases[1].AttachCard(_playableCardService.Get(1));
            Game.ActiveBases[2].AttachCard(_playableCardService.Get(2));
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

                buffer = RenderUtil.Center(render, (Game.ActiveBases[0].Graphic.Count + 6, consoleWidth - 1));
			}
            //Let the user know the screen is too small
            if (buffer is null)
            {
                string[] render =
                [
                    $@"Please increase your screen size"
                ];
                buffer = RenderUtil.Center(render, (consoleHeight - 1, consoleWidth - 1));
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
            BaseGraphicLength = Game.ActiveBases[0].Graphic.Count;

            BaseField = new string[BaseGraphicLength];

            for (int i = 0; i < BaseGraphicLength; i++)
            {
                for (int j = 0; j < numBases; j++)
                {
                    BaseField[i] += Game.ActiveBases[j].Graphic[i];
                    if(j < numBases - 1)
                    {
                        BaseField[i] += HORIZONTAL_PADDING;
                    }
                }
            }
        }

        /// <summary>
        /// Generates the text that lists the cards at each base for each player
        /// </summary>
        private void GenerateCardField()
        {
            //Gather the base lists
            List<List<String>> baseLists = [];
            foreach (BaseCard baseCard in Game.ActiveBases)
            {
                baseLists.Add(baseCard.GetDisplayList());
            }

            //Iterate through all lines in cardfield
            CardField = new string[Math.Max(baseLists.Max(x => x.Count), MIN_CARD_FIELD_SIZE)];
            for(int i = 0; i < CardField.Length; i++)
            {
                foreach (List<String> baseList in baseLists)
                {
                    string currString = "";
                    if(i < baseList.Count)
                    {
                        currString = baseList[i];
                    }
                    CardField[i] += RenderUtil.CenterString(currString, BaseGraphicLength);
                    CardField[i] += HORIZONTAL_PADDING;
                }
            }                
        }

        /// <summary>
        /// Generates the field that lists the game statistics
        /// </summary>
        private void GenerateConsoleField()
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
                    return null;
            }

			return null;
        }
    }
}
