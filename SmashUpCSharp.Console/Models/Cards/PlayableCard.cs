

using SmashUp.Frontend.Utilities;
using System.Text;

namespace Models.Cards
{
    public class PlayableCard(string title, int printedPower, IList<string> graphic, PlayLocation playLocation, Action onPlay) : PrimitiveCard(title, graphic)
    {
        public int Owner { get; set; } = 0;
        public int Controller { get; set; } = 0;
        public PlayLocation PlayLocation { get; set; } = playLocation;
        public int PrintedPower { get; set; } = printedPower;
        public int CurrentPower { get; set; } = printedPower;
        public Action OnPlay { get; set; } = onPlay;
        public IList<PlayableCard> AttachedCards { get; set; } = [];

        public IList<string> GetGraphic(bool useAltBorder=false)
        {
            int graphicWidth = Graphic.Max(line => line.Length);
            int graphicHeight = Graphic.Count;

            string[] returnGraphic = new string[graphicHeight + 2];

            // Top Border
            StringBuilder topBorder = new();
            topBorder.Append(useAltBorder ? '╔': ' ');
            topBorder.Append(useAltBorder ? '═' : '_', graphicWidth);
            topBorder.Append(useAltBorder ? '╗' : ' ');

            returnGraphic[0] = topBorder.ToString();

            // Card Content
            for (int i = 0; i < Graphic.Count; i++)
            {
                StringBuilder lineBuilder = new();
                lineBuilder.Append(useAltBorder ? '║' : '|');
                lineBuilder.Append(RenderUtil.CenterString(Graphic[i], graphicWidth));
                lineBuilder.Append(useAltBorder ? '║' : '|');

                returnGraphic[i + 1] = lineBuilder.ToString();
            }

            // Bottom Border
            StringBuilder bottomBorder = new();
            bottomBorder.Append(useAltBorder ? '╚' : '|');
            bottomBorder.Append(useAltBorder ? '═' : '_', graphicWidth);
            bottomBorder.Append(useAltBorder ? '╝' : '|');

            returnGraphic[^1] = bottomBorder.ToString();

            return returnGraphic;
        }
    }
}
