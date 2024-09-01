using SmashUp.Frontend.Utilities;

namespace Models.Cards
{
    public abstract class PrimitiveCard(string title, string[] graphic, int factionId) : IIdentifiable
    {
        public int Id { get; set; }
        public string Title { get; set; } = title;
        protected string[] Graphic { get; set; } = graphic;
        public int FactionId { get; set; } = factionId;

        public string[] GetGraphic(bool useAltBorder = false)
        {
            int graphicWidth = Graphic.Max(line => line.Length);
            int graphicHeight = Graphic.Length;

            // We add exactly 3 additional lines through this process, two borders and a title 
            string[] returnGraphic = new string[graphicHeight + 3];

            // Top Border
            returnGraphic[0] = BuildBorder(useAltBorder ? '╔' : ' ', useAltBorder ? '═' : '_', useAltBorder ? '╗' : ' ', graphicWidth);

            // Title
            returnGraphic[1] = BuildTitleLine(graphicWidth, useAltBorder);

            // Content
            for (int i = 0; i < graphicHeight; i++)
            {
                returnGraphic[i + 2] = BuildContentLine(Graphic[i], graphicWidth, useAltBorder);
            }

            // Bottom Border
            returnGraphic[^1] = BuildBorder(useAltBorder ? '╚' : '|', useAltBorder ? '═' : '_', useAltBorder ? '╝' : '|', graphicWidth);

            return returnGraphic;
        }

        protected string BuildBorder(char leftChar, char middleChar, char rightChar, int width)
        {
            return $"{leftChar}{new string(middleChar, width)}{rightChar}";
        }
        protected string BuildContentLine(string content, int width, bool useAltBorder)
        {
            char borderChar = useAltBorder ? '║' : '|';
            string centeredContent = RenderUtil.CenterString(content, width);

            return $"{borderChar}{centeredContent}{borderChar}";
        }
        protected abstract string BuildTitleLine(int graphicWidth, bool useAltBorder);
    }

}
