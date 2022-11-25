## Deprecated Samples

The following samples have proven to be poor exercises because they are too simple, too complicated, or too confusing. We list them here for completeness.

* Cliff Divers (Difficulty 1): Dramatic shots of people diving down a cliff.

    ![Cliff Divers](_imgs/CliffDivers.gif?raw=true)

* CpuCarManufacturer (Difficulty 2): Car manufacturing used as a metaphor for the memory stack.

    ![Cpu Car Manufacturer](_imgs/CpuCarManufacturer.png?raw=true)

* Distance Field Attractors (Difficulty 1): Colorful particles move towards the surface of an invisible mesh.

    ![Distance Field Attractors](_imgs/DistanceFieldAttractors.gif?raw=true)
    
* Future HUD (Difficulty 3): Futuristic HUDs drawn with OpenGL primitives.
 
    ![Future HUD](_imgs/FutureHUD.png?raw=true)
    
* JobCloth (Difficulty 2): Simulate clothing using the Job system.

    ![Job Cloth](_imgs/JobCloth.gif?raw=true)

* Surgeon Master (Difficulty 2): Cut and stitch a triangle-mesh.

    ![Surgeon Master](_imgs/SurgeonMaster.png?raw=true)

* Parade (Difficulty 1): Take part in a parade and let people cheer for you.

    ![Parade](_imgs/Parade.gif?raw=true)
    
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
  <summary><strong>Magnetic Roads</strong>: Cars drive along 3D generated splines in all orientations.<br><i>Click here for details</i></summary>
    
   <ul>
<li>Cars drive in two lanes on both sides of the road. Cars always drive in the right lane.</li>
<li>The cars all drive at the same speed. Cars will brake before hitting the car in front of them.</li>
<li>Intersections join two or three road segments, but never four. Some intersecionts are dead ends: they connect to only one road segment.</li>
<li>At three-way intersections, each car randomly chooses whether to go left, right, or straight.</li>
<li>Cars wait to enter an intersection if their path through the intersection crosses the path of another car in the intersection.</li>
   </ul>
</details>

![Magnetic Roads](_imgs/MagneticRoads.gif?raw=true)   

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


