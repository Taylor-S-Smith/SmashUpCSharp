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
        private static readonly Option _showDiscardButton = new("VIEW DISCARD");
        private static readonly Option _endTurnButton = new("END TURN");
        private static readonly Option _showDeckButton = new("VIEW DECK");
        private static readonly List<Option> _buttons = [_showDiscardButton, _endTurnButton, _showDeckButton];
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

        public Guid SelectOption(List<Option> buttons, List<PlayableCard> cardsToDisplay, string displayText)
        {
            // Create Targeter

            var buttonTargeter = new SelectOption(buttons.Select(x => x.Id).ToList());

            var optionId = new BattlePage(_table != null ? _table.GetBaseSlots() : [], new("Mulligan", []), cardsToDisplay.ConvertAll(x => (Card)x), buttons, new Targeter([buttonTargeter]), displayText).Run();

            return optionId;
        }

        public void InitializeData(Table table)
        {
            _table = table;
        }

        public Guid? SelectCardFromHand(List<PlayableCard> handCards, List<List<PlayableCard>> selectableFieldCards, string displayText = "")
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

            
            BattlePage page = new(_table.GetBaseSlots(), _table.ActivePlayer.Player, _table.ActivePlayer.Player.Hand.ToList().ConvertAll(x => (Card)x), _buttons, new Targeter(targetLogics, handTargeter), displayText);
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
                    ViewCards(_table.ActivePlayer.Player.DiscardPile.AsCards());
                }
                else if (chosenId == _showDeckButton.Id)
                {
                    Console.WriteLine(string.Join(", ", _table.ActivePlayer.Player.Deck.GetCardNames()));
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

        public SelectResult SelectFieldCard(List<List<Guid>> validCardIds, PlayableCard? cardToDisplay, string? displayText, bool interuptable=false)
        {
            List<TargetLogic> logics = [];
            logics.Add(new Select2DOption(validCardIds));

            Option finishButton = new("I AM DONE");
            if (interuptable) logics.Add(new SelectOption([finishButton.Id]));

            Guid? selectedId = new BattlePage(_table.GetBaseSlots(), _table.ActivePlayer.Player, [cardToDisplay], [finishButton], new Targeter(logics), displayText ?? "").Run();

            if(selectedId == finishButton.Id) return new(null, ResultType.Finished);

            return new(selectedId, ResultType.Success);
        }

        public Guid SelectBaseCard(List<Guid> validBaseIds, PlayableCard? cardToDisplay=null, string displayText="")
        {
            List<PlayableCard> cardsToDisplay = [];
            if (cardToDisplay != null) cardsToDisplay.Add(cardToDisplay);
            return new BattlePage(_table.GetBaseSlots(), _table.ActivePlayer.Player, cardsToDisplay.ConvertAll(x => (Card)x), _buttons, new Targeter([new SelectOption(validBaseIds)]), displayText).Run();
        }

        public List<Guid> SelectCard(List<Card> optionsToDisplay, List<Card> validOptions, string displayText, int? numToReturn)
        {
            List<Guid> selectedOptions = [];
            Option done = new("I AM DONE");
            List<Option> buttons = [];
            SelectOption optionsTargeter = new(optionsToDisplay.Select(x => x.Id).ToList());
            List<TargetLogic> targetLogics = [optionsTargeter];
                        
            if (numToReturn == null) // User can click on any number of valid targets, they break the loop on button
            {
                buttons.Add(done);
                var endButtonTargeter = new SelectOption([done.Id]);
                targetLogics.Add(endButtonTargeter);
            }

            var page = new BattlePage(_table.GetBaseSlots(), _table.ActivePlayer.Player, optionsToDisplay, buttons, new Targeter(targetLogics), displayText);

            while(optionsToDisplay.Count > 0 && (numToReturn == null || selectedOptions.Count < numToReturn))
            {
                Guid chosenOptionId = page.Run();
                if (chosenOptionId == done.Id) break;
                if(validOptions.Where(x => x.Id == chosenOptionId).Count() != 0)
                {
                    selectedOptions.Add(chosenOptionId);

                    // Remove from list so graphic doesn't appear
                    optionsToDisplay.Remove(optionsToDisplay.Where(x => x.Id == chosenOptionId).Single());
                    // Remove from targeter so the targetIndex doesn't overflow
                    optionsTargeter.RemoveOption(chosenOptionId);
                }
            }

            return selectedOptions;
        }
        
        public void ViewCards(List<Card> cardsToDisplay, string displayText = "", string buttonText="RETURN")
        {
            Option returnButton = new(buttonText);
            SelectOption optionsTargeter = new (cardsToDisplay.Select(x => x.Id).ToList());
            SelectOption returnButtonTargeter = new ([returnButton.Id]);
            List<TargetLogic> targetLogics = [optionsTargeter, returnButtonTargeter];

            var page = new BattlePage(_table.GetBaseSlots(), _table.ActivePlayer.Player, cardsToDisplay, [returnButton], new Targeter(targetLogics, returnButtonTargeter), displayText);

            Guid? chosenId = null;
            while (chosenId != returnButton.Id)
            {
                chosenId = page.Run();
            }
        }

        public void EndBattle(Player winningPlayer)
        {
            Console.WriteLine($"{winningPlayer.Name} won the game!!!");
            Console.ReadLine();
        }

    }

    private class ConsoleApp3PlayerTestUI : ConsoleAppBattleUI
    {
        public override List<(string, List<FactionModel>)> ChooseFactions(List<string> playerNames, List<FactionModel> factionOptions)
        {
            return new([(playerNames[0], [factionOptions[6]]), (playerNames[1], [factionOptions[6]]), (playerNames[2], [factionOptions[6]])]);
        }

        public override List<string> ChoosePlayerNames()
        {
            return ["Taylor", "Andrew", "Caden"];
        }
    }
    private class ConsoleApp1PlayerTestUI : ConsoleAppBattleUI
    {
        public override List<(string, List<FactionModel>)> ChooseFactions(List<string> playerNames, List<FactionModel> factionOptions)
        {
            return new([(playerNames[0], [factionOptions[6]])]);
        }

        public override List<string> ChoosePlayerNames()
        {
            return ["Taylor"];
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
            Battle battle = new(new ConsoleApp1PlayerTestUI(), new GlobalEventManager(), new Random());
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


