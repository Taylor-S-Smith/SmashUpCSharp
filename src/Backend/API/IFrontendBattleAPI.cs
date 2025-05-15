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
    List<(string, List<Faction>)> ChooseFactions(List<string> playerNames, List<Faction> factionOptions);
    List<string> ChoosePlayerNames();
    void InitializeData(Table table);
    Guid? SelectCardOrInvokable(List<PlayableCard> validCardsToDisplay, List<List<PlayableCard>> selectableFieldCards, string displayText);
    Guid SelectBaseCard(List<Guid> validBaseIds, Displayable? cardToDisplay = null, string displayText = "");
    SelectResult SelectFieldCard(List<List<Guid>> validCardIds, Displayable? displayable, string? displayText, bool interuptable = false);
    /// <summary>
    /// Allows user to select one of several text options
    /// </summary>
    /// <returns>Id of the selected option</returns>
    Guid SelectOption(List<Option> buttons, List<PlayableCard> cardsToDisplay, string displayText);
    List<Guid> Select(List<Displayable> display, List<Displayable> validOptions, string displayText, int? numToReturn=null);
    void ViewCards(List<Card> cardsToDisplay, string displayText="", string buttonText = "RETURN");
    void EndBattle(Player winningPlayer);

}
