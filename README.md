# Project-Orbital

Online 2D Android game created by Unity.

Source code is located under Assets/Scripts.

I. Motivation
 
People idle a lot: breaking time between classes, waiting for MRT, queuing in a dentist’s office, etc. That’s when they might want some forms of fast, simple recreation and it is our motivation to develop the game Aiken shoot it - an easy-to-play but interesting online mobile game.
 
II. Project scope
        	
          A casual Android game created by Unity.
 
Target users: casual players
Target devices: Android devices
IDE and back-end library: Unity and Unity Engine, Unity High Level API
Game resources: Unity’s Multiplayer Service, Unity asset store, online free image (Ex: Flatiron, etc.), online free sound effects.
 
II. Game descriptions

        	This is an online Android 2D top-view shooting game where players would  achieve victory by eliminating opponents' spaceships. Utilizing your strategy and your shooting skills to taking down the opponents with the help of various items in the game. There game comes with 2 different play modes: Free fire and Turn-based. Get ready to control your spaceship and have fun with your friends.

2 game modes:

  Free-fire :  2 - 4 players
    Players are able to move and shoot opponents in real time
    Players can see the whole map
    Pick-up items are available to enhance players abilities
    Objective: eliminate the opponents
    
  Turn based: 2 players
    Players are able to move a limited distance in their turns
    Players’ turn ends after either passing time limit and finishing their shoot.
    Players are only able to see themselves and a small area around themselves and the bullets.
    Players could light up the map by using bouncing bullets that light up along its’ projection.
    Pick-up items are available to enhance players abilities
    Objective: eliminate the opponents
    
   
Items and map:
  Random-connected map - All maps are randomly spawned and connected.
  Items are spawned randomly across the map.
  
  Free fire items list:
    Boost fire rate: x5 attack speed (x5 the number of spawnable bullets / time unit)
    Boost damage: x2 damage for the next shoot
    Bullet penetration: Straight laser bullet unaffected by environments, x1.5 damage
    Silence bullet: hit player and disable their shooting ability for 5 seconds
    Stun bullet: Stun player for 4 seconds (disable moving/ shooting abilities)
    Healing area: player standing on this area is healed over time
    Health potion: Immediately restore a small amount of health
    Invisible: Make player invisible for 5 seconds
    
  Turn-based items list:
    Increase vision: x3 vision area for 1 second
    Boost energy: reset player’s energy in that turn to maximum (Energy bar is below health bar)
    Boost damage: x2 damage for the next shoot (can be saved until next turn)
    Health potion: Immediately restore a small amount of health
    Invisible: Make player invisible for 30 seconds (2 turns)
