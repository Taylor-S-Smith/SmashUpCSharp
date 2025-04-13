using SmashUp.Backend.API;

namespace SmashUp.Backend.Models;

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
    public Player? Owner { get; set; } = null;
    public PlayableCardType CardType { get; } = cardType;
    public int? PrintedPower { get; set; } = power;
    public int? CurrentPower { get; set; } = power;
}
