using SmashUp.Backend.GameObjects;
using SmashUp.Backend.Models;

namespace SmashUp.Backend.API;

/// <summary>
/// This is how the backend communicates to the frontend
/// </summary>
internal interface IFrontendBattleAPI
{
    public List<string> GetPlayerNames();
    public List<(string, List<FactionModel>)> ChooseFactions(List<string> playerNames, List<FactionModel> factionOptions);
    public bool AskMulligan();
    public void PlayCards(Table table, IBackendBattleAPI backendBattleAPI);
    public List<PlayableCard> DiscardTo10(Player player);
    public void EndBattle(Player player);
}
