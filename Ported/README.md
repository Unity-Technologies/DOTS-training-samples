
# Game initialization
`GameInitializationSystem` looks for the presence of an entity with a `GameInitialization` component. If that entity exists, the system clears all bees, resources and particles (if any), and then spawns the initial bees & resources to start the game. This means resetting the game is just a question of recreating the `GameInitialization` entity

# Resources
`ResourceSystem` handles:
* Processing a list of "resource spawn events" that other systems can use to spawn resources easily
* Making resources fall with gravity when they have no `ResourceCarrier` and have not settled on the ground (no `ResourceSettled`)
* Detecting when a resource has reached the ground, and either handle marking the resource as `ResourceSettled` (in a stack) or making it spawn bees if it landed in a team zone
* Making resources snap their horizontal position to grid cell lanes
* Making resources follow their carrying bee if they have a `ResourceCarrier`
* Handle logic of removing a resource from a stack if the resource has a `ResourceCarrier` AND a `ResourceSettled`
* Handle logic of making a resource fall when its `ResourceCarrier` has died (is null)

# Bees
`BeeSystem` handles:
* Processing a list of "bee spawn events" that other systems can use to spawn bees easily
* Selecting the bee behaviour when there's no current behaviour (go for resources or go attack enemies)
* Handling the resource-gathering behaviour
* Handling the attack behaviour
* Handling movement/velocity
* Handling scaling/stretching relatively to velocity
* Handling what happens on death (VFX)

# Particles
`ParticleSystem` handles:
* Processing a list of "particle spawn events" that other systems can use to spawn particles easily
* Handling various particle properties depending on particle archetype (velocity, lifetime, stretch, scaling, etc...)

# Camera
* An `EntityCamera` (Entity) is created in the ECS world, and its orbit-camera logic is handled in the `EntityCameraSystem`
* A `GameObjectCamera` (GameObject) is present in the scene. This monobehaviour registers itself as the `GameObjectCameraTransform` in the `GameObjectCameraSystem`
* `GameObjectCameraSystem` handles copying the `EntityCamera` transform to the `GameObjectCamera` transform every frame

# Possible Improvements
* Spawning of bees, resources, particles would be more efficient if done by batch instead of one-by-one
* The `Bee` component currently contains a significant amount of data that never changes at runtime. This data needlessly bloats the size of bees in chunks, and would be better stored in a BlobAsset or in a singleton
* For the convenience of setting bee colors programatically at runtime through DOTS material property components, the bees are separated into two meshes (a parent and a child). This induces a significant performance cost due to transform parenting systems & rendering systems having to deal with more renderers. This was done because material property components don't seem to work for meshes with multiple materials, but could easily be avoided with texture masks instead
* I cut corners short in some places, where some parts of some jobs could have been done in parallel, but they remain single-threaded because I didn't take the time to split the work across multiple jobs. 
* The concept of a "Velocity" for entities (bees, resources, particles) could have been generalized into its own component/system, but I instead went with `Bee`, `Resource`, `Particle` each managing their own Velocity
