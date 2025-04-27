using SmashUp.Backend.Repositories;
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

        public Guid? SelectCardFromList(List<PlayableCard> cards)
        {
            //Use SelectHandCardTargeter class
            //SelectHandCardTargeter class will construct it's master targeter using the base targeters we have constructed

            var handTargeter = new SelectOption(cards.Select(x => x.Id).ToList());
            var endButtonTargeter = new SelectOption([_endTurnButtonId]);       
            
            var chosenId = new BattlePage(_table.GetBaseSlots(), _table.ActivePlayer.Player, _endTurnButtonId, new Targeter([handTargeter, endButtonTargeter])).Run();

            return chosenId == _endTurnButtonId ? null : chosenId;
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


