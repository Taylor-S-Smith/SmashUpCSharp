namespace Models.Cards
{
    public class Faction(string name) : IIdentifiable
    {
        public int Id { get; set; }
        public string Name { get; set; } = name;
    }
}
