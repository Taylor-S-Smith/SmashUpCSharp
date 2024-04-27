namespace SmashUpCSharp.Models
{
    public abstract class Card
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public int Text { get; set; }
        public string Graphic {  get; set; }

        public virtual IList<Card> AttachedCards { get; set;}
    }
    
}
