namespace Models.Cards
{
    public class PlayableCard(int factionId, string title, string[] graphic) : PrimitiveCard(title, graphic, factionId)
    {
        public int Owner { get; set; } = 0;
        public int Controller { get; set; } = 0;
        public PlayLocation PlayLocation { get; set; } = PlayLocation.Base;
        public int? PrintedPower { get; set; }
        public int? CurrentPower { get; set; }
        public Action OnPlay { get; set; } = () => { };
        public IList<PlayableCard> AttachedCards { get; set; } = [];

        protected override string? BuildTitleLine(int width, bool useAltBorder)
        {
            return null;
        }
    }
}
