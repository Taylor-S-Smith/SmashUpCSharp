I have almost finished the frontend, though I have kept most of the mess of BattlePage the same.
I have decided that BattlePage will directly get a Table object.

My current next step is to figure out a way to initialize a game after deck selection and pass the table to the battle screen.
If possible, I want to keep the game initalization code out of the deck selection screen itself. Here are a few preliminary ideas:
- Have a new page type that just performs logic. Deck selection wouldn't directly call battle page, but the GameGeneration page that will the end up calling battlePage. This would require the ability to pass in parameters into a page
- Rewrite pages so that instead of the page deciding what is next, the entire path is written down in an PagePath object. The PageManager (perhaps rename it) interprets/displays this object. When a page is done it simply yields to the next thing in line. This would allow me to pass in arbitrary parameters, handle dependencies on a per page basis, and run arbitrary code in between
- Create super object that composes a PagePath, and a Renderer. When constructed it shows the StartPage. Based on selection it Loads up a new PagePath, and uses it's renderer to render it arbitrarily like above. This object will be what implements IFrontendAPI. Think about dividing BattlePage up into multiple pages. Perhaps it will be abstract, and different versions of it will be used for different gamemodes.
- Start in Application, and write the code you want to be able to write, create objects and instances as needed.


The next steps are:
- Finish connection between BattlePageService and table, then test it.
- Allow pages to pass information to one another
- Generate a game/battle at the approprate time, pass it to battlepage
- Implement Prototype pattern for cards
- Implement repos/data storage
- Remove TempSetUpMemory
- Remove duplication in Page.Run() logic
- Build the Base Deck correctly






Why Event Approach?
At this point I have questioned the API approach, since events seems easier to implement, though the coupling will be much tighter than an API.
If I think I will make this online, or use a different UI then it seems like an API may be a better approach, but for a proof of concept
events seem so much easier and faster.


Rules Variance/TODO:
- Choosing factions doesn't follow any rules standard
- "Build the Base Deck" includes faction bases, not set bases



Event Approach Rules:
- Events should only be used for logic that:
	- Applies to an unknown number of objects
	- Will not be the same for each object