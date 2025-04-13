using SmashUp.Frontend.Utilities;
using SmashUp.Frontend.Pages;
using SmashUp.Network;
using SmashUp.Backend.GameObjects;
using SmashUp.Backend.API;
using SmashUp.Backend.Services;
using SmashUp.Backend.Repositories;
using System.Runtime.CompilerServices;

namespace SmashUp;

internal partial class Application()
{
    private class ConsoleAppBattleUI() : IFrontendBattleAPI
    {
        private BattlePage _battlePage = null!;
        public bool AskMulligan()
        {
            throw new NotImplementedException();
        }

        public List<(string, List<FactionModel>)> ChooseFactions(List<string> playerNames, List<FactionModel> factionOptions)
        {
            return new DeckSelectionPage(playerNames, factionOptions).Run();
        }

        public void StartBattle(Table table)
        {
            _battlePage = new(new(table));
        }

        public List<string> GetPlayerNames()
        {
            int numPlayers = new PlayerNumPage().Run();
            List<string> names = new PlayerNamePage(numPlayers).Run();
            return names;
        }

        public void PlayCards()
        {
            _battlePage.Run();
        }

        public List<Guid> DiscardTo10(Guid playerId)
        {
            throw new NotImplementedException();
        }

        public void EndBattle(Guid winningPlayerId)
        {
            throw new NotImplementedException();
        }
    }

    private class ConsoleAppTestUI() : IFrontendBattleAPI
    {
        private BattlePage _battlePage = null!;
        public bool AskMulligan()
        {
            throw new NotImplementedException();
        }

        public List<(string, List<FactionModel>)> ChooseFactions(List<string> playerNames, List<FactionModel> factionOptions)
        {
            return new([(playerNames[0], [factionOptions[0]]), (playerNames[1], [factionOptions[0]])]);
        }

        public void StartBattle(Table table)
        {
            _battlePage = new(new(table));
        }

        public List<string> GetPlayerNames()
        {
            return ["Taylor", "Andrew"];
        }

        public void PlayCards()
        {
            _battlePage.Run();
        }

        public List<Guid> DiscardTo10(Guid playerId)
        {
            throw new NotImplementedException();
        }

        public void EndBattle(Guid winningPlayerId)
        {
            throw new NotImplementedException();
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
            Battle battle = new(new ConsoleAppTestUI(), new Random(), new BaseCardService(new BaseCardRepository()), new PlayableCardService(new PlayableCardRepository()));
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


