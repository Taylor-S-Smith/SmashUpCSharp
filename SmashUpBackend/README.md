The next steps are:
- Change Battle Play methods to be more modular, PlayMinion and PlayAction should be able to play to just about any location
- Finish wildlifePreserve
- Add dinosaur bases.
- Change trigger functions to be included in the setters when possible, we shouldn't have to call a dedicated trigger if we are also calling a fucntion to preform the action (unless very specific timing is required)
- Update display when navigating hand
- Add ability to cancel a play action
- Have actions call a method to display the action that happen to the user as they unfold, at least for events that are not obvious (e.g. "X was protected from destruction")
- Warm user before they perform an action that would not result in any meaningful changes

Other TODOs:
- Update "Tooth And Claw...And Guns" to erratta
- Remove duplication in Page.Run() logic
- Build the Base Deck correctly
- Choose factions correctly


Misc Tests:
- Test removing Upgrade from a minion
- Test removing Upgrade from a minion when it's ApplyPower was protected


Event Approach Rules:
- Events should only be used for logic that:
	- Applies to an unknown number of objects
	- Will not be the same for each object
- When calling class events you should call the most specific one applicable. 
  It is the responsability of the more general class events to call all dependent events.
- When Listening, listen to the most specific one. If you only need to know when a card is added to a specific base, don't listen to the event manager
- Create as specific event as you can. No need to track global deaths if per base works just as well
- When performing any operation, check the events of all effected objects, and global events
  


Faction(s) with computer specific Mechanics:
- Generate random cards (Conjurers+)
- Transform cards (Alchemists+)
- Generate tokens ((Necomancer/Cultists)+Alchemists)
- Add effects (power, onplay) to cards in hand (Enchanters+)
- Steal card text

- Keyword: Animate
- Cultists


Test:
- Tooth protects against detah, discards
- Two allow you to choose


Disclaimer:
- I tried to implement the most latest version of the rules, except I did not implement erattaed functionality