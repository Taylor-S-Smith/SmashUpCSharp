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

            string? title = BuildTitleLine(graphicWidth, useAltBorder);

            // We add 2 borders and sometimes a title
            string[] returnGraphic = new string[graphicHeight + (title!= null ? 3 : 2)];

            int index = 0;

            // Top Border
            returnGraphic[index++] = BuildBorder(useAltBorder ? '╔' : ' ', useAltBorder ? '═' : '_', useAltBorder ? '╗' : ' ', graphicWidth);

            // Title
            if (title != null)
            {
                returnGraphic[index++] = title;
            }

            // Content
            for (int i = 0; i < graphicHeight; i++)
            {
                returnGraphic[index++] = BuildContentLine(Graphic[i], graphicWidth, useAltBorder);
            }

            // Bottom Border
            returnGraphic[index] = BuildBorder(useAltBorder ? '╚' : '|', useAltBorder ? '═' : '_', useAltBorder ? '╝' : '|', graphicWidth);

            return returnGraphic;
        }

        protected static string BuildBorder(char leftChar, char middleChar, char rightChar, int width)
        {
            return $"{leftChar}{new string(middleChar, width)}{rightChar}";
        }
        protected static string BuildContentLine(string content, int width, bool useAltBorder)
        {
            char borderChar = useAltBorder ? '║' : '|';
            string centeredContent = RenderUtil.CenterString(content, width);

            return $"{borderChar}{centeredContent}{borderChar}";
        }
        protected abstract string? BuildTitleLine(int graphicWidth, bool useAltBorder);
    }

}
