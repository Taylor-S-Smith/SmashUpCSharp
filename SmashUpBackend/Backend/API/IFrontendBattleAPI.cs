using SmashUp.Backend.GameObjects;

namespace SmashUp.Backend.API;

/// <summary>
/// This is how the backend communicates to the frontend
/// </summary>
internal interface IFrontendBattleAPI
{
    public List<string> GetPlayerNames();
    public List<(string, List<FactionModel>)> ChooseFactions(List<string> playerNames, List<FactionModel> factionOptions);
    public bool AskMulligan();
    void StartBattle(Table table);
    public void PlayCards();
    public List<Guid> DiscardTo10(Guid playerId);
    public void EndBattle(Guid winningPlayerId);
}
