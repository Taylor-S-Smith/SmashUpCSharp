using SmashUp.Backend.API;
using SmashUp.Backend.Models;

namespace SmashUp.Backend.Services;

/// <summary>
/// How the frontend talks to the backend
/// </summary>
internal class FactionService
{
    public static List<FactionModel> GetFactionModels()
    {
        List<FactionModel> factionDtos = [];
        foreach (var faction in Enum.GetValues(typeof(Faction)).Cast<Faction>())
        {
            factionDtos.Add(new((int)faction, Enum.GetName(faction) ?? throw new Exception($"Cannot find name for faction with ID:{(int)faction}")));
        }

        return factionDtos;
    }
}
