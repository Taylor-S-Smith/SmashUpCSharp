using SmashUp.Frontend.Pages.Battle.LogicModules;
using SmashUp.Frontend.Pages.Battle;
using SmashUp.Backend.Repositories;
using SmashUp.Backend.GameObjects;
using SmashUp.Frontend.Utilities;
using SmashUp.Backend.Services;
using SmashUp.Backend.Models;
using SmashUp.Frontend.Pages;
using SmashUp.Backend.API;

namespace SmashUp;

internal partial class Application()
{
    private class ConsoleAppBattleUI() : IFrontendBattleAPI
    {
        private Table _table = null!;
        private IBackendBattleAPI _backendBattleAPI = null!;
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

        public void InitializeData(Table table, IBackendBattleAPI backendBattleAPI)
        {
            _table = table;
            _backendBattleAPI = backendBattleAPI;
        }

        public Guid SelectCardFromList(List<PlayableCard> cards)
        {
            //Use SelectHandCardTargeter class
            //SelectHandCardTargeter class will construct it's master targeter using the base targeters we have constructed
            return new BattlePage(_table.GetBaseSlots(), _table.ActivePlayer.Player, new SelectOption(cards.Select(x => x.Id).ToList())).Run();
        }

        public Guid SelectFieldCard(List<List<Guid>> validCardIds)
        {
            return new BattlePage(_table.GetBaseSlots(), _table.ActivePlayer.Player, new Select2DOption(validCardIds)).Run();
        }

        public Guid SelectBaseCard(List<Guid> validBaseIds)
        {
            return new BattlePage(_table.GetBaseSlots(), _table.ActivePlayer.Player, new SelectOption(validBaseIds)).Run();
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


