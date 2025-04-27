using SmashUp.Backend.Repositories;
using SmashUp.Backend.GameObjects;
using SmashUp.Frontend.Utilities;
using SmashUp.Backend.Services;
using SmashUp.Backend.Models;
using SmashUp.Frontend.Pages;
using SmashUp.Backend.API;
using SmashUp.Frontend.Pages.Target;
using System.Linq;

namespace SmashUp;

internal partial class Application()
{
    private class ConsoleAppBattleUI() : IFrontendBattleAPI
    {
        private Table _table = null!;
        private readonly Guid _endTurnButtonId = Guid.NewGuid();

        public bool AskMulligan()
        {
            throw new NotImplementedException();
        }

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

        public void InitializeData(Table table)
        {
            _table = table;
        }

        public Guid? SelectHandCard(List<PlayableCard> handCards, List<List<PlayableCard>> selectableFieldCards)
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
            var endButtonTargeter = new SelectOption([_endTurnButtonId]);
            targetLogics.Add(endButtonTargeter);

            //Don't forget to handle card displaying
            PlayableCard? cardToDisplay = null;
            while (true)
            {
                var chosenId = new BattlePage(_table.GetBaseSlots(), _table.ActivePlayer.Player, _endTurnButtonId, new Targeter(targetLogics, handTargeter), cardToDisplay).Run();

                if(chosenId == _endTurnButtonId)
                {
                    return null;
                } 
                else if (handCards.Select(x => x.Id).Contains(chosenId)) 
                { 
                    return chosenId; 
                }
                else
                {
                    cardToDisplay = selectableFieldCards.SelectMany(x => x).Where(x => x.Id == chosenId).Single();
                }
            }
            
        }

        public Guid SelectFieldCard(List<List<Guid>> validCardIds)
        {
            var fieldCardTargeter = new Select2DOption(validCardIds);

            return new BattlePage(_table.GetBaseSlots(), _table.ActivePlayer.Player, _endTurnButtonId, new Targeter([fieldCardTargeter])).Run();
        }

        public Guid SelectBaseCard(List<Guid> validBaseIds)
        {
            return new BattlePage(_table.GetBaseSlots(), _table.ActivePlayer.Player, _endTurnButtonId, new Targeter([new SelectOption(validBaseIds)])).Run();
        }

        public List<PlayableCard> DiscardTo10(Player player)
        {
            throw new NotImplementedException();
        }

        public void EndBattle(Player winningPlayer)
        {
            throw new NotImplementedException();
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
            Battle battle = new(new ConsoleAppTestUI(), new GlobalEventManager(), new Random(), new BaseCardService(new BaseCardRepository()), new PlayableCardService(new PlayableCardRepository()));
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


