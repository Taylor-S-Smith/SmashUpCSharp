namespace Backend.Models;

internal enum PlayableCardType
{
    minion,
    action
}

/// <summary>
/// Any card that can by played by a Player
/// </summary>
internal class PlayableCard(Faction faction, PlayableCardType cardType, string name, string[] graphic, int power, Action? onPlay = null) : Card(faction, name, graphic, onPlay)
{
    public Guid? Owner { get; private set; } = null;
    public PlayableCardType CardType { get; } = cardType;
    public int? Power { get; set; } = power;

    public void SetOwner(Guid playerId)
    {
        Owner = playerId;
    }
}
