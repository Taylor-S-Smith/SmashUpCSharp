using SmashUpCSharp.Models;

namespace Models
{
    public abstract class PlayableCard : Card
    {
        public int Owner { get; set; }
        public int Controller { get; set; }
        public PlayLocation PlayLocation { get; set; }
        public abstract void OnPlay();
    }
}
