namespace Models.Cards
{
    public abstract class PrimitiveCard(string title, IList<string> graphic) : IIdentifiable
    {
        public int Id { get; set; }
        public string Title { get; set; } = title;
        public IList<string> Graphic { get; set; } = graphic;
    }

}
