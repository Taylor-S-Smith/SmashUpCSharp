I have finished the minions for dinosaurs.
I am now taking a break to fix the UI to make it more extendable/easier to use.
Now, the logic services are targeters and only deal with targeting, nothing else, 
but the page may need more logic somewhere else. We target using Guids and will ultimatly return Guids to backend.
Now, we need the ConsoleUI to construct the desired BattlePage, with whichever targeters should be allowed,
based on the request. Assembly will require getring data from frontend, and I am a little torn on
how stateless I want the UI implementation to be. Should it be able to constantly request from backend?

The next steps are:
- Finish dinosaur faction.

Other TODOs:
- Remove duplication in Page.Run() logic
- Build the Base Deck correctly
- Choose factions correctly


Event Approach Rules:
- Events should only be used for logic that:
	- Applies to an unknown number of objects
	- Will not be the same for each object
- When calling class events you should call the most specific one applicable. 
  It is the responsability of the more general class events to call all dependent events.
- When Listening, listen to the most specific one. If you only need to know when a card is added to a specific base, don't listen to the event manager
- Create as specific event as you can. No need to track global deaths if per base works just as well
- When performing any operation, check the events of all effected objects, and global events
  