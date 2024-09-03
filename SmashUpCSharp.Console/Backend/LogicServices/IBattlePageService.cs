using Models.Cards;
using SmashUp.Frontend.Utilities;

namespace SmashUp.Backend.LogicServices
{
    internal interface IBattlePageService
    {
        PlayableCard? GetSelectedCard();
        BaseCard? GetTargetedBaseCard();
        PlayableCard? GetTargetedPlayableCard();
        string? HandleKeyPress(UserKeyPress keyPress, ref bool stateChanged, PlayableCard[] interactableHandCards);
        bool IsCardSelectedOrTargeted(PrimitiveCard card);
        bool IsInCardViewMode();
        bool SelectedFromHand();
    }
}