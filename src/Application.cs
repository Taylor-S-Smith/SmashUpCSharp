using SmashUp.Backend.GameObjects;
using SmashUp.Frontend.Utilities;
using SmashUp.Backend.Services;
using SmashUp.Backend.Models;
using SmashUp.Frontend.Pages;
using SmashUp.Backend.API;
using SmashUp.Frontend.Pages.Target;

namespace SmashUp;

internal partial class Application()
{
    private class ConsoleAppBattleUI() : IFrontendBattleAPI
    {
        private Table _table = null!;
        private static readonly Button _showDiscardButton = new(Guid.NewGuid(), "DISCARD");
        private static readonly Button _endTurnButton = new(Guid.NewGuid(), "END TURN");
        private static readonly Button _showDeckButton = new(Guid.NewGuid(), "DECK");
        private static readonly List<Button> _buttons = [_showDiscardButton, _endTurnButton, _showDeckButton];
        private static readonly List<Guid> _buttonIds = _buttons.Select(x => x.Id).ToList();

        public virtual List<(string, List<FactionModel>)> ChooseFactions(List<string> playerNames, List<FactionModel> factionOptions)
        {
            return new DeckSelectionPage(playerNames, factionOptions).Run();
        }

        public virtual List<string> ChoosePlayerNames()
        {
            int numPlayers = new PlayerNumPage().Run();
            List<string> names = new PlayerNamePage(numPlayers).Run();
            return names;
        }

        public bool SelectBool(List<PlayableCard> cardsToDisplay, string displayText)
        {
            // Create Targeter
            Button yes = new(Guid.NewGuid(), "Yes");
            Button no = new(Guid.NewGuid(), "No");

            var buttonTargeter = new SelectOption([yes.Id, no.Id]);

            var optionId = new BattlePage(_table != null ? _table.GetBaseSlots() : [], new("Mulligan", []), cardsToDisplay, [yes, no], new Targeter([buttonTargeter]), displayText).Run();

            return optionId == yes.Id;
        }

        public void InitializeData(Table table)
        {
            _table = table;
        }

        public Guid? SelectHandCard(List<PlayableCard> handCards, List<List<PlayableCard>> selectableFieldCards, string displayText = "")
        {
            // Create Targeter
            List<TargetLogic> targetLogics = [];
            if(selectableFieldCards.Sum(x => x.Count) != 0)
            {
                var PlayFieldTargeter = new Select2DOption(selectableFieldCards.Select(x => x.Select(y => y.Id).ToList()).ToList());
                targetLogics.Add(PlayFieldTargeter);
            }
            var handTargeter = new SelectOption(handCards.Select(x => x.Id).ToList());
            targetLogics.Add(handTargeter);
            var endButtonTargeter = new SelectOption(_buttonIds);
            targetLogics.Add(endButtonTargeter);

            
            BattlePage page = new(_table.GetBaseSlots(), _table.ActivePlayer.Player, _table.ActivePlayer.Player.Hand.ToList(), _buttons, new Targeter(targetLogics, handTargeter), displayText);
            while (true)
            {
                var chosenId = page.Run();
                displayText = "";

                if (chosenId == _endTurnButton.Id)
                {
                    return null;
                }
                else if (chosenId == _showDiscardButton.Id)
                {
                    Console.WriteLine(string.Join(", ", _table.ActivePlayer.Player.DiscardPile.Select(x => x.Name)));
                    Console.ReadLine();
                }
                else if (chosenId == _showDeckButton.Id)
                {
                    Console.WriteLine(string.Join(", ", _table.ActivePlayer.Player.Deck.GetCards()));
                    Console.ReadLine();
                }
                else if (handCards.Select(x => x.Id).Contains(chosenId)) 
                { 
                    return chosenId; 
                }
                else
                {
                    //Switch to "Display Single Card Info" mode
                    var cardToDisplay = selectableFieldCards.SelectMany(x => x).Where(x => x.Id == chosenId).Single();
                    new BattlePage(_table.GetBaseSlots(), _table.ActivePlayer.Player, [cardToDisplay], _buttons, new Targeter([])).Run();
                    page.Run();
                }
            }            
        }

        public SelectFieldCardUIResult SelectFieldCard(List<List<Guid>> validCardIds, PlayableCard? cardToDisplay, string? displayText)
        {
            var fieldCardTargeter = new Select2DOption(validCardIds);

            Guid? selectedCardId = new BattlePage(_table.GetBaseSlots(), _table.ActivePlayer.Player, [cardToDisplay], _buttons, new Targeter([fieldCardTargeter]), displayText ?? "").Run();

            return new(selectedCardId, selectedCardId == null);
        }

        public Guid SelectBaseCard(List<Guid> validBaseIds, PlayableCard? cardToDisplay=null, string displayText="")
        {
            List<PlayableCard> cardsToDisplay = [];
            if (cardToDisplay != null) cardsToDisplay.Add(cardToDisplay);
            return new BattlePage(_table.GetBaseSlots(), _table.ActivePlayer.Player, cardsToDisplay, _buttons, new Targeter([new SelectOption(validBaseIds)]), displayText).Run();
        }

        public List<Guid> SelectPlayableCard(List<PlayableCard> options, int numToReturn, string displayText)
        {
            List<Guid> selectedOptions = [];
            var page = new BattlePage(_table.GetBaseSlots(), _table.ActivePlayer.Player, options, _buttons, new Targeter([new SelectOption(options.Select(x => x.Id).ToList())]), displayText);

            while(selectedOptions.Count < numToReturn)
            {
                Guid chosenOptionId = page.Run();
                selectedOptions.Add(chosenOptionId);
                options.Remove(options.Where(x => x.Id == chosenOptionId).Single());
            }
            return selectedOptions;
        }

        public void EndBattle(Player winningPlayer)
        {
            Console.WriteLine($"{winningPlayer.Name} won the game!!!");
            Console.ReadLine();
        }

    }

    private class ConsoleAppTestUI: ConsoleAppBattleUI
    {
        public override List<(string, List<FactionModel>)> ChooseFactions(List<string> playerNames, List<FactionModel> factionOptions)
        {
            return new([(playerNames[0], [factionOptions[1]]), (playerNames[1], [factionOptions[1]]), (playerNames[2], [factionOptions[1]])]);
        }

        public override List<string> ChoosePlayerNames()
        {
            return ["Taylor", "Andrew", "Caden"];
        }
    }

    internal void Run()
    {
        // Initialize Data
        KeyMapUtil.SetDefaultKeyMappings();

        //Run Start Page
        var result = new StartPage().Run();

        if(result == StartPageResult.Exit)
        {
            return;
        }
        else if (result == StartPageResult.StartGame)
        {
            Battle battle = new(new ConsoleAppTestUI(), new GlobalEventManager(), new Random());
            battle.StartBattle();
        }
        else if (result == StartPageResult.ShowCollection)
        {
            Console.WriteLine("Showing Collection...");
        }
        else if (result == StartPageResult.Options)
        {
            Console.WriteLine("Showing Options...");
        }

    }
}


