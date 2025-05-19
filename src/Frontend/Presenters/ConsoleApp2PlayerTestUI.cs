using SmashUp.Backend.Models;

namespace SmashUp.Frontend.Presenters;

internal class ConsoleApp2PlayerTestUI : ConsoleAppBattleUI
{
    public override List<(string, List<Faction>)> ChooseFactions(List<string> playerNames, List<Faction> factionOptions)
    {
        return new([(playerNames[0], [factionOptions[1]]), (playerNames[1], [factionOptions[1]])]);
    }

    public override List<string> ChoosePlayerNames()
    {
        return ["Taylor", "Andrew"];
    }
}


