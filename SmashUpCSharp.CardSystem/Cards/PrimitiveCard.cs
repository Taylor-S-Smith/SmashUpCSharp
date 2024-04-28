namespace Models.Cards
{
    public abstract class PrimitiveCard(string title, string text, IList<string> graphic) : IIdentifiable
    {
        public int Id { get; set; }

        public string Title { get; set; } = title;
        public string Text { get; set; } = text;
        public IList<string> Graphic { get; set; } = graphic;
    }

}
