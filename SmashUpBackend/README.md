I have finished the frondend<->backend connnection. 


The next steps are:
- Implement Prototype pattern for cards
- Implement repos/data storage
- Create one full faction
- Test entire game, fill in cracks

Other TODOs:
- Remove duplication in Page.Run() logic
- Build the Base Deck correctly
- Choose factions correctly


Event Approach Rules:
- Events should only be used for logic that:
	- Applies to an unknown number of objects
	- Will not be the same for each object
