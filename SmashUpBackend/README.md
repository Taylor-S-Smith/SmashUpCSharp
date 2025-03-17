I have essentially finished the basic data structures of the game. 

The next steps are:
- Go Through SetUp (See "Setup" https://smashup.fandom.com/wiki/Rules)
- Implement way to sort through cards, but also create them dynamically
- implement basic console commands, everything a player could do in the physical world (e.g. show hand/discard, show active bases, etc) 
  below is the list of commands your SmashUp shell could do. Some of them will be on this level, but most will be commands that will
  be implemented on further steps

show hand - shows your current hand
show discard - shows your current discard pile
show deck - shows your current deck
show deck 'int' - shows the top 'int' card(s) in your deck
shuffle deck - shuffles your deck
draw 'int' - draws the top 'int' card(s) from your deck
discard 'index'- discards the specified card from your hand
discard random - discards a random card from your hand
discard base 'index' card 'index' - discards the specified card from the specified base
discard base 'index' - discards all cards from the specified base
play 'index' - plays a card from your hand
play discard 'index' - plays the specified card from the discard pile - ADD DOUBLE
return base 'index' card 'index' - places the specified card into your hand from the specified base
move base 'index' card 'index' - moves the speficied card at the specified base to a new one of your choosing
recard 'index' - places the specified card into your hand from your discard pile
recard deck 'index' - places the specified card into your deck from your discard pile
retrieve 'index' - places the specified card into your hand from your deck
distrieve 'index' - places the specified card into your deck from your hand
List of Card Commands:
read 'card name' - shows you the card text from 'card name'
victory 'int' - gives you 'int' victory points
show victory - shows you your current victory point total

Afterwards, here are the big idea next steps:

- Start Turn
- Play Cards
- Score Bases
- Draw Cards

After this, you should be able to play a functional game of SmashUp through your API. It may be a good idea at this point to
Create another, even simpler shell app for testing purposes. Consider writing some tests here (or as you write cards).
Next, create a frontend. We will start with the same ASCII graphics as before, but since the abckend is complete, we could
consider others as well.