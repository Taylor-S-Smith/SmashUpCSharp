

using SmashUp.Frontend.Utilities;
using System.Text;

namespace Models.Cards
{
    public class PlayableCard(int factionId, string title, string[] graphic) : PrimitiveCard(title, graphic, factionId)
    {
        public int Owner { get; set; } = 0;
        public int Controller { get; set; } = 0;
        public PlayLocation PlayLocation { get; set; } = PlayLocation.Base;
        public int PrintedPower { get; set; } = 0;
        public int CurrentPower { get; set; } = 0;
        public Action OnPlay { get; set; } = () => { };
        public IList<PlayableCard> AttachedCards { get; set; } = [];

        protected override string BuildTitleLine(int width, bool useAltBorder)
        {
            char borderChar = useAltBorder ? '║' : '|';
            string centeredTitle = RenderUtil.CenterString(Title, width - 2);

            return $"{borderChar}{PrintedPower}{centeredTitle}{PrintedPower}{borderChar}";
        }
    }
}
