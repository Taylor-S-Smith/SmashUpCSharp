
using SmashUp.Frontend.Utilities;

namespace Models.Cards;

public class BaseCard(int factionId, string title, IList<string> graphic, int printedBreakpoint, int[] pointArray) : PrimitiveCard(title, graphic)
{
    public int FactionId = factionId;
    public int PrintedBreakpoint { get; set; } = printedBreakpoint;
    public int CurrentBreakpoint { get; set; } = printedBreakpoint;
    public int[] PointArray = pointArray;
    private List<PlayableCard> attachedCards = [];
    public int TotalPower { get; set; } = 0;
    

    //GET
    public List<string> GetDisplayList()
    {
        List<string> DisplayList = [];
        attachedCards = attachedCards.OrderBy(x => x.Owner).ToList();
        int? currPlayer = null;

        int cardIndex = 1;
        foreach (PlayableCard card in attachedCards)
        {
            if (currPlayer != card.Owner)
            {
                DisplayList.Add($"Player {card.Owner}'s cards:");
                currPlayer = card.Owner;
            }

            if(card.CurrentPower > 0)
            {
                DisplayList.Add($"{cardIndex}. {card.Title} ({card.CurrentPower})");
            } else
            {
                DisplayList.Add($"{cardIndex}. {card.Title}");
            }
        }

        return DisplayList;
    }

    public string GetCardsByIndex(int num)
    {
        throw new NotImplementedException();
    }

    public override IList<string> GetGraphic()
    {
        int graphicVerticleLength = Graphic[0].Length;
        string paddedTitle = RenderUtil.CenterString(Title, graphicVerticleLength - 6);
        string titleLine = $"|{TotalPower.ToString().PadLeft(2, '0')}{paddedTitle}{CurrentBreakpoint.ToString().PadLeft(2, '0')}|";

        Graphic[1] = titleLine;
        return Graphic;
    }


    //MODIFY
    public void AttachCard(PlayableCard Card)
    {
        attachedCards.Add(Card);
        UpdateTotalPower();
    }

    /*
    public abstract void AfterScores();
    public abstract void StartOfTurn();
    public abstract void CardPlayedHere();
    public abstract void CardDestroyedHere();
    */

    private void UpdateTotalPower()
    {
        TotalPower = GetAttachedCards().Sum(x => x.CurrentPower);
    }

    private List<PlayableCard> GetAttachedCards()
    {
        return attachedCards;
    }
}
