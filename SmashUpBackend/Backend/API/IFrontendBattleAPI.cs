using SmashUp.Backend.GameObjects;
using SmashUp.Backend.Models;

namespace SmashUp.Backend.API;

/// <summary>
/// This is how the backend communicates to the frontend
/// </summary>
internal interface IFrontendBattleAPI
{
    List<(string, List<FactionModel>)> ChooseFactions(List<string> playerNames, List<FactionModel> factionOptions);
    List<string> ChoosePlayerNames();
    bool AskMulligan();
    List<PlayableCard> DiscardTo10(Player player);
    void InitializeData(Table table);
    Guid? SelectHandCard(List<PlayableCard> cards, List<List<PlayableCard>> selectableFieldCards);
    Guid SelectBaseCard(List<Guid> validBaseIds);
    Guid SelectFieldCard(List<List<Guid>> validCardIds);
    void EndBattle(Player winningPlayer);
}
