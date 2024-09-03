
using SmashUp.Frontend.Utilities;
using System.Text;

namespace Models.Cards;

public class BaseCard(int factionId, string title, string[] graphic, int printedBreakpoint, int[] pointArray) : PrimitiveCard(title, graphic, factionId)
{
    public int PrintedBreakpoint { get; set; } = printedBreakpoint;
    public int CurrentBreakpoint { get; set; } = printedBreakpoint;
    public int[] PointArray = pointArray;
    public List<PlayableCard> AttachedCards { get; set; } = [];
    public int TotalPower { get; set; } = 0;
    

    //GET
    public string[] GetAttachedCardsGraphic(PlayableCard? targetedCard)
    {
        int numLines = AttachedCards.Count + AttachedCards.Select(x => x.Owner).Distinct().Count();
        string[] displayList = new string[numLines];

        int? currOwner = null;
        int cardIndex = 1;
        int displayIndex = 0;

        foreach (PlayableCard card in AttachedCards)
        {
            // Whenever the owner changes, add a new owner header line
            if (currOwner != card.Owner)
            {
                displayList[displayIndex++] = $"Player {card.Owner}'s cards:";
                currOwner = card.Owner;
            }

            StringBuilder lineBuilder = new();

            if(card == targetedCard)
            {
                lineBuilder.Append('>');
            }

            lineBuilder.Append($"{cardIndex}. {card.Title}");

            if (card.CurrentPower.HasValue)
            {   
                lineBuilder.Append($" ({card.CurrentPower})");
            }
            if (card == targetedCard)
            {
                lineBuilder.Append('<');
            }

            displayList[displayIndex++] = lineBuilder.ToString();
            cardIndex++;
        }

        return displayList;
    }

    public string GetCardsByIndex(int num)
    {
        throw new NotImplementedException();
    }

    //MODIFY
    public void AttachCard(PlayableCard Card)
    {
        AttachedCards.Add(Card);
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
        TotalPower = AttachedCards.Sum(x => x.CurrentPower ?? 0);
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
