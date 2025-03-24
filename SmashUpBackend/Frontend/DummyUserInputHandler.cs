using SmashUp.Backend.GameObjects;
using SmashUp.Backend.Models;

namespace SmashUp;

internal partial class Application
{
    class DummyUserInputHandler() : IUserInputHandler
    {
        public bool AskMulligan()
        {
            Console.WriteLine("In AskMulligan");
            Console.ReadLine();
            return true;
        }

        public List<(string, List<Faction>)> ChooseFactions()
        {
            Console.WriteLine("In ChooseFactions");
            Console.ReadLine();

            return [("Taylor", []), ("Andrew", []), ("Caden", [])];
        }

        public List<Guid> DiscardTo10(Guid playerId)
        {
            Console.WriteLine("In DiscardTo10");
            Console.ReadLine();

            return new();
        }

        public void EndBattle(Guid winningPlayerId)
        {
            Console.WriteLine("In EndBattle");
            Console.ReadLine();
        }

        public List<string> GetPlayers()
        {
            Console.WriteLine("In GetPlayers");
            Console.ReadLine();

            return ["Taylor", "Andrew", "Caden"];
        }

        public void PlayCards()
        {
            Console.WriteLine("In PlayCards");
            Console.ReadLine();
        }
    }
}
