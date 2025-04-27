using System.Text;
using SmashUp.Backend.API;
using SmashUp.Backend.GameObjects;
using SmashUp.Backend.Models;

namespace SmashUp.Frontend.Utilities;
internal class CardGraphicUtil
{
    public static string[] GenerateBaseCardGraphic(string[] graphic, string title, int totalPower, int currentBreakpoint, bool useAltBorder = false)
    {
        return GenerateCardGraphic(graphic, useAltBorder, (width, useAltBorder) => TitleLineBuilder(width, useAltBorder, title, totalPower.ToString(), currentBreakpoint.ToString()));
    }
    public static string[] GeneratePlayableCardGraphic(string[] graphic, string title, int? power, bool useAltBorder = false)
    {
        string leftText;
        string rightText;
        if (power == null)
        {
            leftText = "A ";
            rightText = " A";
        }
        else
        {
            leftText = ((int)power).ToString();
            rightText = ((int)power).ToString();
        }
        return GenerateCardGraphic(graphic, useAltBorder, (width, useAltBorder) => TitleLineBuilder(width, useAltBorder, title, leftText, rightText));
    }

    private static string[] GenerateCardGraphic(string[] graphic, bool useAltBorder = false, Func<int, bool, string?>? titleLineBuilder = null)
    {
        int graphicWidth = graphic.Max(line => line.Length);
        int graphicHeight = graphic.Length;
        string? title = null;

        if (titleLineBuilder != null) title = titleLineBuilder(graphicWidth, useAltBorder);

        // We add 2 borders and sometimes a title
        string[] returnGraphic = new string[graphicHeight + (title != null ? 3 : 2)];

        int index = 0;

        // Top Border
        returnGraphic[index++] = BuildBorder(useAltBorder ? '╔' : '┌', useAltBorder ? '═' : '─', useAltBorder ? '╗' : '┐', graphicWidth);

        // Title
        if (title != null)
        {
            returnGraphic[index++] = title;
        }

        // Content
        for (int i = 0; i < graphicHeight; i++)
        {
            returnGraphic[index++] = BuildContentLine(graphic[i], graphicWidth, useAltBorder);
        }

        // Bottom Border
        returnGraphic[index] = BuildBorder(useAltBorder ? '╚' : '└', useAltBorder ? '═' : '─', useAltBorder ? '╝' : '┘', graphicWidth);

        return returnGraphic;
    }
    private static string BuildBorder(char leftChar, char middleChar, char rightChar, int width)
    {
        return $"{leftChar}{new string(middleChar, width)}{rightChar}";
    }
    private static string BuildContentLine(string content, int width, bool useAltBorder)
    {
        char borderChar = useAltBorder ? '║' : '│';
        string centeredContent = RenderUtil.CenterString(content, width);

        return $"{borderChar}{centeredContent}{borderChar}";
    }
    private static string TitleLineBuilder(int width, bool useAltBorder, string title, string left, string right)
    {
        char borderChar = useAltBorder ? '║' : '│';
        string leftText = left.PadLeft(2, '0');
        string rightText = right.PadLeft(2, '0');
        string centeredTitle = RenderUtil.CenterString(title, width - 4);

        return $"{borderChar}{leftText}{centeredTitle}{rightText}{borderChar}";
    }

    public static string[] GetAttachedCardsGraphic(BaseSlot baseCardSlot, Guid? targetedCardId)
    {
        // Count number of filled territories (Each will have it's own heading), and number of cards
        int numLines = baseCardSlot.Territories.Where(x => x.Cards.Count > 0).ToList().Count + baseCardSlot.Territories.SelectMany(x => x.Cards).Count();
        string[] displayList = new string[numLines];

        int cardIndex = 1;
        int displayIndex = 0;

        foreach (PlayerTerritory territory in baseCardSlot.Territories)
        {
            if(territory.Cards.Count > 0)
            {
                displayList[displayIndex++] = $"{territory.player.Name}'s cards ({territory.Cards.Sum(x => x.CurrentPower)}):";
                foreach (PlayableCard card in territory.Cards)
                {
                    StringBuilder lineBuilder = new();

                    if (card.Id == targetedCardId)
                    {
                        lineBuilder.Append('>');
                    }

                    lineBuilder.Append($"{cardIndex}. {card.Name}");

                    if (card.CurrentPower.HasValue)
                    {
                        lineBuilder.Append($" ({card.CurrentPower})");
                    }
                    if (card.Id == targetedCardId)
                    {
                        lineBuilder.Append('<');
                    }

                    displayList[displayIndex++] = lineBuilder.ToString();
                    cardIndex++;
                }
            }            
        }

        

        return displayList;
    }
}
