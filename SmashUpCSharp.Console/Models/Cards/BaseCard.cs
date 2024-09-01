
using SmashUp.Frontend.Utilities;
using System.Text;

namespace Models.Cards;

public class BaseCard(int factionId, string title, string[] graphic, int printedBreakpoint, int[] pointArray) : PrimitiveCard(title, graphic, factionId)
{
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
        TotalPower = attachedCards.Sum(x => x.CurrentPower);
    }

    protected override string BuildTitleLine(int width, bool useAltBorder)
    {
        char borderChar = useAltBorder ? '║' : '|';
        string power = TotalPower.ToString().PadLeft(2, '0');
        string breakpoint = CurrentBreakpoint.ToString().PadLeft(2, '0');
        string centeredTitle = RenderUtil.CenterString(Title, width - 4);

        return $"{borderChar}{power}{centeredTitle}{breakpoint}{borderChar}";
    }
}
