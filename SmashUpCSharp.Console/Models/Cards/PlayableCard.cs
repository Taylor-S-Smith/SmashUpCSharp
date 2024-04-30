

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
    }
}
