Under the 'Originals' directory, you'll find small simulations/games implemented in classic (non-DOTS) Unity. Your task is to implement one of these samples in DOTS.

1. Clone this repository.
2. Make a new branch for your sample port.
3. Create a new Unity Project and place it under the 'Ported' directory, *e.g* /Ported/AutoFarmers/
4. Configure the project for DOTS.

Feel free to copy assets and code snippets from the originals into your ports. The focus of this exercise is to familiarize yourself with the DOTS API and to practice Data-oriented Design principles!

## Sample Gallery

*Expand the arrows for more details about each sample.*

<details>
  <summary><strong>Ant Pheromones</strong>: Ants look for food and spread pheromones to influence the pathing of other ants.<br><i>Click here for details</i></summary>
  
  <ul>
<li>Ants bring food from the source (green spot) to the destination (red spot).</li>
<li>Each ant spawns at the center point with a random heading.</li>
<li>Ants bounce off of walls at the inverse angle.</li>
<li>Ants will steer towards the food source (green dot) if they have line of sight.</li>
<li>Ants leave pheromone in the spots they walk over.</li>
<li>Ant steering is biased towards the nearest adjacent spot of pheromone.</li>
<li>Pheromone at a spot decays over time.</li>
<li>Ant steering is also slightly randomized, so ants don’t walk in straight lines.</li>
<li>The gaps in the ring walls randomly vary in size and placement.</li>
<li>Keyboard controls allow the user to slow down, speed up, and reset the simulation.</li>
</ul>
</details>

![Ant Pheromones](_imgs/AntPhermones.gif?raw=true)
 
---
 
<details>
  <summary><strong>Auto Farmers</strong>: Farmers gather and sell resources to expand their farm.<br><i>Click here for details</i></summary>
  
   <ul>
<li>On start, a single farmer (the green cylinders) spawns.</li>
<li>Farmers walk around to destroy rocks (the grey cylinders) to make way for tilling the soil (creating brown-striped crop cells).</li>
<li>A plant grows in each crop cell. (The color and mesh of plant randomly varies.)</li>
<li>When fully grown, a plant is ready for harvest by a farmer. Farmers deposit plants in the nearest silo (grey-blue cylinder).</li>
<li>When enough resources have been collected into a silo, a new farmer spawns from the silo.</li>
<li>Farmers look for their next task in a limited radius. If no task is found, they look in an increasingly larger radius until a task is found.</li>
<li>For every fifth farmer spawned, a drone is spawned instead.</li>
<li>Drones fly and harvest plants. They do not destroy rocks or plant crops.</li>
<li>After depositing, drones hover over the silo until they locate a plant to harvest.</li>
<li>Keyboard controls allow the user to reset the simulation.</li>
   </ul>
</details>
 
![Auto Farmers](_imgs/AutoFarmers.png?raw=true)
 
---
 
 <details>
   <summary><strong>Bucket Brigade</strong>: Firefighters pass buckets along a chain to extinguish a fire.<br><i>Click here for details</i></summary>
     
   <ul>
<li>Each cell of the grid has a temperature from 0.0 to 1.0. Cells above 0.2 are on fire and transmit heat to their neighbors. The color and height of a fire cell reflects its temperature. (For visual effect, a fire cell's height randomly oscilates a small amount, but this does not reflect its actual temperature.)</li>
<li>On start, a few random cells are on fire. Buckets are randomly placed around the field.</li>
<li>Workers are grouped into teams. Each team picks the closest cell on fire and forms two lines from the fire to the nearest water (blue cells outside the field). One line passes full buckets; the other passes empty buckets.</li>
<li>Each team has a bucket fetcher, who retrieves the nearest bucket, walks it to the water at the end of the line, fills the bucket, and then drops it.</li>
<li>Workers in the 'pass full' line look for full buckets on the ground within a small radius. When found, they walk over, pick up the bucket, carry it to the next worker up the line, and drop it. The last worker in the 'pass full' line empties the bucket on the fire (lowering the temperature of that cell and its eight surrounding cells to zero). Workers move slower while carrying a full bucket.</li>
<li>Workers in the 'pass empty' line behave the same but only carry empty buckets. The last worker in the 'pass empty' line will fill the bucket in the nearest water.</li>
<li>Every frame, a team will reposition its line to the fire cell closest to its current water source.</li>
<li>When a team's water source is exhausted, it repositions the line to the next closest water.</li>
<li>When repositioning, a worker will not resume its normal behavior until it has reached its new position.</li>
<li>In the moment between a worker placing a bucket on the ground and the next picking it up, the line might reposition, in which case the bucket will be left on the ground.</li>
<li>OPTIONAL: A small configured number of 'omniworkers' (black) are not part of any team. An omniworker picks up the nearest bucket, fills it at the nearest water source, and dumps it on the nearest fire.</li>
<li>Keyboard controls allow the user to reset the simulation.</li>
   </ul>
</details>

![Bucket Brigade](_imgs/BucketBrigade.png?raw=true)

---

<details>
  <summary><strong>Combat Bees</strong>: Two bee hives battle for resources and survival.<br><i>Click here for details</i></summary>
    
   <ul>
<li>Resources (green discs) spawn in the middle. Yellow bees spawn in the yellow base. Blue bees spawn in the blue base.</li>
<li>Resources spawned at the same location will stack on top of each other.</li>
<li>Bees pick up resources and drop them in their base.</li>
<li>When a resource hits the ground of a base, it explodes, spawning several bees of that base's color.</li>
<li>Bees not carrying resources may attack and destroy enemy bees.</li>
<li>A destroyed bee emits bee fragments and blood splatters, and the resource eventually carried falls to the ground.</li>
<li>Blood splatters on surfaces shrink to nothing over time.</li>
<li>Each bee's displayed scale oscillates along all three axes, but this is just a visual effect. Collisions are not affected.</li>
<li>Keyboard controls allow the user to reset the simulation.</li>
<li>Left mouse clicks spawn additional resources at the point clicked (determined by casting a ray from the cursor).</li>
<li>The camera can be controlled with the mouse (hold down right button to rotate, use mouse wheel to zoom). </li>
   </ul>
</details>

![Combat Bees](_imgs/CombatBees.gif?raw=true)

---

<details>
  <summary><strong>Factory</strong>: Robots transport resources along lanes to crafters.<br><i>Click here for details</i></summary>
  
   <ul>
<li>Users can click to add a cluster of several additional bots. Users can also click on tiles of the grid to clear them or to add walls (grey tile), add a resource (purple tile), add a crafter (green tile), add green lines, or add purple lines.</li>
<li>Bots are purple when carrying a resource and green when not.</li>
<li>Bots pick up resources at the purple tiles and deliver them to the green tiles (crafters).</li>
<li>Bots will not collide with other bots. When bots are spawned, other bots gets pushed out of the way.</li>
<li>Bots will path around walls. If you place a wall on top of bots, they will remain stuck until you erase the tile.</li>
<li>Keyboard controls allow the user to reset the simulation.</li>
   </ul>
</details>

![Factory](_imgs/Factory.png?raw=true)

---

<details>
  <summary><strong>Highway Racers</strong>: Simulate traffic on a 4-lane highway.<br><i>Click here for details</i></summary>
  
  <ul>
<li>Each car has a random cruising speed, a random overtake speed, a random overtake distance, and a random overtake time (each within a fixed min/max range).</li>
<li>When traveling at its cruising speed, a car is grey. When going below its cruising speed, it turns red. While going above its cruising speed, it turns green.</li>
<li>Cars have three main states: cruising, looking to change lane, and overtaking.</li>
<li>While cruising, a car accelerates/brakes to its cruising speed; if it comes up behind a slower car within its overtake distance, it enters the 'looking to change lane' state.</li>
<li>A looking-to-change car will look for sufficient open space to its left or right to change lanes. While blocked from changing lanes, the car will tail the car ahead of it. When it is clear to change lane, it will enter the overtake state.</li>
<li>In the overtake state, a car first moves into another lane, then accelerates to its overtake speed. Once the car's overtake time elapses, it looks to move back into the lane it came from. If blocked from moving back, the car gives up and goes back to the cruising state. If at any time the car is impeded by a slower moving car within its overtake distance, it goes back to the cruising state.</li>
<li>Keyboard controls allow the user to reset the simulation.</li>
   </ul>
</details>

![Highway Racers](_imgs/HighwayRacers.gif?raw=true)

---

<details>
  <summary><strong>Jump The Gun</strong>: A ball jumps through a blocky landscape and avoids cannonballs.<br><i>Click here for details</i></summary>

<ul>
<li>The ball bounces from column to adjacent column towards the mouse cursor. (This requires computing the appropriate trajectory for each bounce.) The movement is clamped to the edges of the playing field.</li>
<li>On init, the cannons spawn on random columns. The cannons always turn to face the player's ball.</li>
<li>Periodically, each cannon fires a cannon ball along a trajectory that will intersect the player's ball (at its current position) but not hit any columns in between.</li>
<li>When a cannon ball hits the top of a column, the impact pushes the column down (but not below the minimum height).</li>
<li>The game is over when a cannon ball hits the player's ball.</li>
<li>Keyboard controls allow the user to reset the simulation.</li>
   </ul>  

</details>

![Jump The Gun](_imgs/JumpTheGun.gif?raw=true)

---

<details>
  <summary><strong>LabRat</strong>: Place arrows on the board to route the most rats into your home base.<br><i>Click here for details</i></summary>
    
   <ul>
<li>Mice spawn at frequent, randomized intervals in the top and bottom corners. The top mice head down. The bottom mice head up.</li>
<li>At start of play, walls are randomly placed between grid cells.</li>
<li>Cats and mice travel on the grid, changing direction when they hit a wall or travel over an arrow.</li>
<li>Cats spawn in random squares. When a cat and mouse intersect, the mouse is eaten.</li>
<li>When a mouse hits a player's 'home base' (one of the four dots placed near the center of the grid), the mouse disappears, and the player is awarded a point.</li>
<li>Players can place arrows in cells of the board. The green player places green arrows, the red player places red arrows, etc. A player cannot place their arrows in a cell occupied by an arrow of another player.</li>
<li>Once a player has three arrows on the board, their next placed arrow removes their oldest arrow on the board.</li>
<li>Only one player is human. The AI players just place their arrows randomly at random intervals.</li>
<li>At the end of 30 seconds, the player with the most points wins.</li>
<li>Keyboard controls allow the user to reset the simulation.</li>
   </ul>
</details>

![Lab Rat](_imgs/LabRat.gif?raw=true)

---

<details>
  <summary><strong>Magnetic Roads</strong>: Cars drive along 3D generated splines in all orientations.<br><i>Click here for details</i></summary>
    
   <ul>
<li>Cars drive in two lanes on both sides of the road. Cars always drive in the right lane.</li>
<li>The cars all drive at the same speed. Cars will brake before hitting the car in front of them.</li>
<li>Intersections join two or three road segments, but never four. Some intersecionts are dead ends: they connect to only one road segment.</li>
<li>At three-way intersections, each car randomly chooses whether to go left, right, or straight.</li>
<li>Cars wait to enter an intersection if their path through the intersection crosses the path of another car in the intersection.</li>
<li>Keyboard controls allow the user to reset the simulation.</li>
   </ul>
</details>

![Magnetic Roads](_imgs/MagneticRoads.gif?raw=true)   

---

<details>
  <summary><strong>Metro</strong>: Metro train transport commuters from station to station.<br><i>Click here for details</i></summary>
    
   <ul>
   <li>The trains move on a fixed schedule. Passengers should not enter or exit the train if they do not have time to get in position before the train moves.</li>
   <li>Disembarking passengers randomly pick another train to board.</li>
   <li>On init, the tracks and station placement is randomly generated.</li>
   <li>Keyboard controls allow the user to reset the simulation.</li>
   </ul>
</details>

![Metro](_imgs/Metro.gif?raw=true)

---

<details>
  <summary><strong>Stack Interchange</strong>: Cars drive through a stack interchange.<br><i>Click here for details</i></summary>
   
   <ul>
   <li>At random intervals, cars spawn at the edge of the straight roads. Cars despawn when they run off the edge of the straight roads.</li>
   <li>Cars merging onto a new road should yield for cars on the road.</li>
   <li>Each road has two lanes. Cars in both lanes of a road travel in the same direction.</li>
   <li>Cars stay in their lanes through their whole trip, <em>e.g.</em> a car in the left lane will travel in the left lane on all roads.</li>
   <li>Each car has a color matching one of the roadways. A car's path through the interchange must include the roadway maching its color.</li>
   <li>Some roadways arch upwards to pass over other roads.</li>
   <li>The simulation has multiple interchanges, but each is unconnected and independent from the others. Cars of one interchange do not travel to the other interchanges.</li>
   <li>Keyboard controls allow the user to reset the simulation.</li>
   </ul>
</details>

![Stack Interchange](_imgs/StackInterchange.png?raw=true)

---

<details>
  <summary><strong>Thrower Arms</strong>: Mechanical arms throw rocks at tin cans.<br><i>Click here for details</i></summary>
   
   <ul>
   <li>The arms and hands are animated by inverse kinematics.</li>
   <li>The cans scroll in from the sides and wrap around.</li>
   <li>Rocks scroll in from the side and get destroyed once they reach the end. Additional rocks spawn along the path to ensure all arms have sufficient rocks.</li>
   <li>Two arms should never reach for the same ball.</li>
   <li>An arm does not collide with rocks or with other arms. The fingers wrap around a rock but do not actually grip the rock. The rock is temporarily attached to the hand.</li>
   <li>Once an arm has picked up a rock, it targets a can and calculates the required throwing trajectory). (No two arms should ever attempt to pick up the same rock or target the same can.)</li>
   <li>When struck, a can is affected by gravity and the force of the hit. A single rock can hit multiple cans. Rocks are impacted by their collisions with the cans. Cans do not collide with other cans.</li>
   <li>Falling rocks and cans despawn when they fall below a certain altitude.</li>
   <li>Keyboard controls allow the user to reset the simulation.</li>
   </ul>
</details>

![Thrower Arms](_imgs/ThrowerArms.gif?raw=true)

---

<details>
  <summary><strong>Tornado</strong>: A tornado devastates a construction site.<br><i>Click here for details</i></summary>
  
   <ul>
   <li>A tornado travels along the ground in a figure 8 pattern.</li>
   <li>A fixed set of cubes swirl around in the tornado.</li>
   <li>The force of the tornado breaks apart the joints of the randomly spawned towers.</li>
   <li>The cubes and beams are affected by the force of the tornado, but they collide only with the ground, not with each other.</li>
   <li>Keyboard controls allow the user to reset the simulation.</li>
   </ul>
</details>

![Tornado](_imgs/Tornado.gif?raw=true)

---

<details>
  <summary><strong>Zombie Maze</strong>: A circle picks up capsules in a maze infested with zombies.<br><i>Click here for details</i></summary>
  
   <ul>
   <li>A player (yellow cube) and zombies (green cubes) move along the grid of the board.</li>
   <li>The interior maze walls are randomly generated at init time.</li>
   <li>The magenta walls move back and forth at a regular interval.</li>
   <li>The player wins by picking up all of the capsules.</li>
   <li>Most zombies path towards a randoly chosen pill. When a zombie reaches its destination pill, it selects another pill target.</li>
   <li>A fixed number of zombies path towards the player.</li>
   <li>Keyboard controls allow the user to reset the simulation.</li>
   </ul>
</details>

![Zombie Maze](_imgs/ZombieMaze.png?raw=true)

---

[Deprecated samples](deprecated.md)
