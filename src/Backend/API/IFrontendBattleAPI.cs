using SmashUp.Backend.GameObjects;
using SmashUp.Backend.Models;

namespace SmashUp.Backend.API;

/// <summary>
/// This is how the backend communicates to the frontend
/// </summary>
public enum ResultType
{
    Success,
    NoValidTargets,
    Finished
}
public record SelectResult(Guid? SelectedId, ResultType Type);
internal interface IFrontendBattleAPI
{
    List<(string, List<FactionModel>)> ChooseFactions(List<string> playerNames, List<FactionModel> factionOptions);
    List<string> ChoosePlayerNames();
    void InitializeData(Table table);
    Guid? SelectCardOrInvokable(List<PlayableCard> validCardsToDisplay, List<List<PlayableCard>> selectableFieldCards, string displayText);
    Guid SelectBaseCard(List<Guid> validBaseIds, PlayableCard? cardToDisplay = null, string displayText = "");
    SelectResult SelectFieldCard(List<List<Guid>> validCardIds, PlayableCard? cardToDisplay, string? displayText, bool interuptable = false);
    Guid SelectOption(List<Option> buttons, List<PlayableCard> cardsToDisplay, string displayText);
    List<Guid> SelectCard(List<Card> optionsToDisplay, List<Card> validOptions, string displayText, int? numToReturn=null);
    void ViewCards(List<Card> cardsToDisplay, string displayText="", string buttonText = "RETURN");
    void EndBattle(Player winningPlayer);

}
