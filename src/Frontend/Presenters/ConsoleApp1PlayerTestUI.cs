using SmashUp.Backend.Models;

namespace SmashUp.Frontend.Presenters;

internal class ConsoleApp1PlayerTestUI : ConsoleAppBattleUI
{
    public override List<(string, List<Faction>)> ChooseFactions(List<string> playerNames, List<Faction> factionOptions)
    {
        return new([(playerNames[0], [factionOptions[6]])]);
    }

    public override List<string> ChoosePlayerNames()
    {
        return ["Taylor"];
    }
}


