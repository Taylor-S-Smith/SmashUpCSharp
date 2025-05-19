Should decreasing power from ongoing effect be considerd affecting oneself? Or just increasing?

alpha and archive should conflict (but we should change it so they don't)
If alpha played first, no card drawn
If archive played first, drawn


Keep in mind:
- Madness interacts strangely with mass enchant
- If you try to play a special card with mass enchant, it will discard without effect if it's condition is not met

Misc Tests to one day write:
- Test removing Upgrade from a minion
- Test removing Upgrade from a minion when it's ApplyPower was protected
- War Raptor can survive Leprachaun
- Move a war raptor to a base with a different number of war raptors
- There are two War Raptors on a base, so they are power 4. I move one of them to a base with a Cub Scout also at power 4. Raptor Restroyed.
- Tooth and Claw And Guns stops a minions own effect (Oponents war raptor won't gain power)
- Wildlife preserve discards all other players cards currently on your minions
- Two allow you to choose
- Wizard base puts cards on top of base deck in correct order

Event Approach Rules: 
- Only functions marked by  the 'event' keyword can be listened to. Otherwise we only add functionality through an instantiated object
- Each event should only ever be triggered in ONE place
- When calling class events you should call the most specific one applicable.
- We tigger events in the order of PlayableCard, Base, Global. (NOTE: Should this order be reversed for exiting base/battlefield?)
- When Listening, listen to the most specific one. No need to recieve global death info if per base works just as well
- "BeforeAttempt" suggests that an action will be attempted, but it is unclear if it will actually happen or not
- "Before" suggests that the action specified has not happened yet, but is assured to happen. Other related events (such as removing from hand in the case of discard) could have already occured, but not the specified one
- "On" suggests that it is triggered in the in-between state. E.g. removed from battlefield, but not in discard pile
- "After" suggests that the action specified has been fully resolved up to this point. E.g. the card exists in the battlefield (However, there is nuance to this. See above for order on timing resolutions for multiple event types)
 
Disclaimer:
- When multiple effects are triggered by a single action, the rules say to let the active player decide the order.
  To avoid implementing a complex choosing system in an already complex UI, I instead let effects resolve in the 
  order they entered the battlefield. E.g. A bases "after a minion here is destroyed" effect will always trigger before a minion's
- Some card text had to change to fit, but still should work as expected
- Some names had to change to fit

Faction(s) with digital-only Mechanics:
- Generate random cards (Conjurers+)
- Transform cards (Alchemists+)
- Generate tokens ((Necomancer/Cultists)+Alchemists)
- Add effects (power, onplay) to cards in hand (Enchanters+)
- Steal card text
- Retrigger onplay actions

- Keyword: Animate
- Cultists

Card Ideas:
- After a card is destroyed, put it in YOUR discard pile.
- Transform a card into another random one.
- Generate a random card. Generate a random ___ card.
- Choose a base. Generate a 1-power minion there
- Give the minion's in your hand +1 power
- Master Alchemist: When you transform a minion, it always be 5 or more power
- 
- 0 Power; Animate: gain +2 power
- Play on a minion. Ongoing: This minion has +1 power. Animate: Gain control of this minion
- Your cards are animated until the end of the turn
- Grand Animator Magus: Ongoing: Your cards are animated