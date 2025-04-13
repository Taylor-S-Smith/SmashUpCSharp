

using SmashUp.Backend.API;

namespace SmashUp.Network;

internal class Cache
{
    public int PlayerCount { get; set; }
    public List<string> PlayerNames { get; internal set; } = [];
    public List<(string, List<FactionModel>)> FactionSelections { get; internal set; } = [];
}
