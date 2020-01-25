# Design Doc
[Game Jam](https://itch.io/jam/weekly-game-jam-133)

## Basics
1. Set up a mono game repository.
2. Consider sprites for items, power ups, and player.

## Game Loop
Very basic, you jump on the wall. There is a floor. 
Your score goes up if you go up and down if you go down, 
and you have to jump on walls. 
You slide on the walls if you are on them while pressing 
the key towards them. You fall straight down otherwise. 
Power ups/ score modifiers are put in the middle of 
the walls to encourage wall jumping back and forth, 
there are also hazards that come down.
The walls are basic squares that go up almost infinitely. 
Level ups occur that make the game more challenging as you go up. 
If you die you get a score that tells you how far you've gone.

## Game Loop Code
1. When player goes above 0 in Y it will reset their Y position to 790.
If they drop below 790 they will get a game over with their score.
If they get hit or touch spikes they will lose a life.
2. When they jump up over 0, 2 new walls will be created. The first time this happens
the floor just won't be reproduced so it should go away.