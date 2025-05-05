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
    void InitializeData(Table table);
    Guid? SelectCardFromHand(List<PlayableCard> handCards, List<List<PlayableCard>> selectableFieldCards, string displayText);
    Guid SelectBaseCard(List<Guid> validBaseIds, PlayableCard? cardToDisplay = null, string displayText = "");
    SelectFieldCardUIResult SelectFieldCard(List<List<Guid>> validCardIds, PlayableCard? cardToDisplay, string? displayText);
    Guid SelectOption(List<Option> buttons, List<PlayableCard> cardsToDisplay, string displayText);
    List<Guid> SelectCard(List<Card> validOptions, List<Card> optionsToDisplay, string displayText, int? numToReturn=null);
    void EndBattle(Player winningPlayer);
}
