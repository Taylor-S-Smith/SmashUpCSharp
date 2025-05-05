namespace SmashUp.Backend.Models;

internal static class CardExtensions
{
    public static List<Card> AsCards<T>(this IEnumerable<T> source) where T : Card
    {
        return source.Cast<Card>().ToList();
    }
}
