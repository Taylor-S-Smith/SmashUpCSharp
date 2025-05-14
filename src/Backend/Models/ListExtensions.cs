using SmashUp.Backend.GameObjects;

namespace SmashUp.Backend.Models;

internal static class ListExtensions
{
    public static List<Card> AsCards<T>(this IEnumerable<T> source) where T : Card
    {
        return source.Cast<Card>().ToList();
    }
    public static List<Displayable> AsDisplayable<T>(this IEnumerable<T> source) where T : Displayable
    {
        return source.Cast<Displayable>().ToList();
    }
    public static List<BaseCard> Bases(this IEnumerable<BaseSlot> source)
    {
        return source.Select(x => x.BaseCard).ToList();
    }
}
