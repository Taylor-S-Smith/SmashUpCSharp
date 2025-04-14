namespace SmashUp.Backend.Models;

internal abstract class Card(Faction faction, string name, string[] graphic, Action? onPlay = null) : Identifiable
{
    public Faction Faction { get; set; } = faction;
    public string Name { get; set; } = name;
    public string[] Graphic { get; set; } = graphic;
    public Action OnPlay { get; } = onPlay ?? (() => { });

    public abstract Card Clone();

    //This is what is injected into the event manager's events. Ex. Ongoing: This gains +1 power each time a minion is played 
    //Action internalAction = () => { };

    //This is what the event manager injects into. Ex. OnPlay: Each other player discards a random minion
    //Action externalAction = () => { };
}
