namespace Models.Cards
{
    public abstract class PrimitiveCard(string title, IList<string> graphic) : IIdentifiable
    {
        public int Id { get; set; }
        public string Title { get; set; } = title;
        protected IList<string> Graphic { get; set; } = graphic;

        public virtual IList<string> GetGraphic()
        {
            return Graphic;
        }
    }

}
