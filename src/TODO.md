The next steps are:
- Finish Turn Phase Logic
- Implement rest of Front-End API
- Wizards!!!
- Refactor input field to support a variety of display types, including: hand, single card display, list display
- Reuse Battle Page, rather than create new instance each time
- Add ability to cancel a play action
- Have actions call a method to display the action that happen to the user as they unfold, at least for events that are not obvious (e.g. "X was protected from destruction")
- Warn user before they perform an action that would not result in any meaningful changes

Other TODOs:
- Update "Tooth And Claw...And Guns" to erratta
- Remove duplication in Page.Run() logic
- Build the Base Deck correctly
- Choose factions correctly
- "Tooth And Claw And Guns" permanantly negates the effect, it should only until the end of the turn. Probably make a list of functions that are being held off of a minion.


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
- Wildlife preserve discards all other players cards currently on your minions
- Two allow you to choose


Disclaimer:
- When multiple effects are triggered by a single action, the rules say to let the active player decide the order.
  To avoid implementing a complex choosing system in an already complex UI, I instead let effects resolve in the 
  order they entered the battlefield. E.g. A bases "after a minion here is destroyed" effect will always trigger before a minion's


Card Ideas:
- After a card is destroyed, put it in YOUR discard pile.
- Transform a card into another random one.
- Generate a random card
- Choose a base. Generate a 1-power minion there
- Give the minion's in your hand +1 power
- 
- 0 Power; Animate: gain +2 power
- Play on a minion. Ongoing: This minion has +1 power. Animate: Gain control of this minion
- Your cards are animated until the end of the turn
- Grand Animator Magus: Ongoing: Your cards are animated