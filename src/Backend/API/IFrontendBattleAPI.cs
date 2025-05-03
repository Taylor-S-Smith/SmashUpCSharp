using SmashUp.Backend.GameObjects;
using SmashUp.Backend.Models;

namespace SmashUp.Backend.API;

/// <summary>
/// This is how the backend communicates to the frontend
/// </summary>
public record SelectFieldCardUIResult(Guid? SelectedCardId, bool ActionCanceled);
internal interface IFrontendBattleAPI
{
    List<(string, List<FactionModel>)> ChooseFactions(List<string> playerNames, List<FactionModel> factionOptions);
    List<string> ChoosePlayerNames();
    bool AskMulligan();
    void InitializeData(Table table);
    Guid? SelectHandCard(List<PlayableCard> handCards, List<List<PlayableCard>> selectableFieldCards, string displayText);
    Guid SelectBaseCard(List<Guid> validBaseIds, PlayableCard? cardToDisplay = null, string displayText = "");
    SelectFieldCardUIResult SelectFieldCard(List<List<Guid>> validCardIds, PlayableCard? cardToDisplay, string? displayText);
    List<Guid> SelectPlayableCard(List<PlayableCard> options, int numToReturn, string displayText);
    void EndBattle(Player winningPlayer);
}
