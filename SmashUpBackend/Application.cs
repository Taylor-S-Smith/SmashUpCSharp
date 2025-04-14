using SmashUp.Frontend.Utilities;
using SmashUp.Frontend.Pages;
using SmashUp.Backend.GameObjects;
using SmashUp.Backend.API;
using SmashUp.Backend.Services;
using SmashUp.Backend.Repositories;
using SmashUp.Backend.Models;
using SmashUp.Backend.LogicServices;

namespace SmashUp;

internal partial class Application()
{
    private class ConsoleAppBattleUI() : IFrontendBattleAPI
    {
        public bool AskMulligan()
        {
            throw new NotImplementedException();
        }

        public virtual List<(string, List<FactionModel>)> ChooseFactions(List<string> playerNames, List<FactionModel> factionOptions)
        {
            return new DeckSelectionPage(playerNames, factionOptions).Run();
        }

        public virtual List<string> GetPlayerNames()
        {
            int numPlayers = new PlayerNumPage().Run();
            List<string> names = new PlayerNamePage(numPlayers).Run();
            return names;
        }

        public void PlayCards(Table table, IBackendBattleAPI backendBattleAPI)
        {
            new BattlePage(new BattlePageTargeter(table, backendBattleAPI)).Run();
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
            return new([(playerNames[0], [factionOptions[1]]), (playerNames[1], [factionOptions[1]])]);
        }

        public override List<string> GetPlayerNames()
        {
            return ["Taylor", "Andrew"];
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
            Battle battle = new(new ConsoleAppTestUI(), new EventManager(), new Random(), new BaseCardService(new BaseCardRepository()), new PlayableCardService(new PlayableCardRepository()));
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


