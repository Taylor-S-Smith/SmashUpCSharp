Other TODOs:
- Remove duplication in Page.Run() logic
- Build the Base Deck correctly
- Choose factions correctly
- "Tooth And Claw And Guns" permanantly negates the effect, it should only until the end of the turn. Probably make a list of functions that are being held off of a minion.

Misc Tests:
- Test removing Upgrade from a minion
- Test removing Upgrade from a minion when it's ApplyPower was protected
- War Raptor can survive Leprachaun
- Move a war raptor to a base with a different number of war raptors
- There are two War Raptors on a base, so they are power 4. I move one of them to a base with a Cub Scout also at power 4. Raptor Restroyed.


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
- Some card text had to change to fit, but still should work as expected
- Some names had to change to fit

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