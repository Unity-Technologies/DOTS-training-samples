# Port report
In this project I port the `Ant Pheromones` project using ECS and the Job system. It targets `entitites preview-0.1.1`.

## My background
I used to work as a virtual reality (VR) software developer, writing training software for airline employees using Unity. Prior to porting this project I did not have any experience with using ECS or the job system. 

## Summary
Ants begin their journey from their colony in search of the destination where resources are stored. To do so they must circumvent rings of obstacles (randomly generated each time). Along the way they leave behind pheromones to guide their fellow ants. These pheromones decay over time and eventually dissipate entirely, unless the same trails are being traversed again before long.

Upon reaching its destination, an ant must then make its way back to its colony with the resources that it can carry. Once it arrives at its colony, it heads out again in search of more resources. This cycle repeats itself insofar as the game is running. 

The user may speed up or slow down the simulation by pressing any of the buttons from 1 to 9.

Data about each ant (i.e., the direction in which it moves, its speed, its brightness, its colour, etc.) and each pheromone (i.e., how far it has strengthened or decayed) are supposed to be updated in `FixedUpdate()`. However, because these data are updated in the ported project on every update loop (i.e., when `OnUpdate()` is invoked on the systems), for the sake of comparison I decided to move the code performing these updates in the original project from `FixedUpdate()` to `Update()` instead.

In the original project, both the ants and the obstacles are rendered in batches by calling `Graphics.DrawMeshInstanced(...)` in `Update()` on the main thread. Rendering ants is much more expensive than rendering obstacles -- while every ant changes its position every frame, the obstacles remain stationary once generated. Due to this disparity in rendering costs, in the ported project I decided to use job system to render the ants, while still allowing the main thread to render the obstacles.

## The ported project
There are a lot of moving parts (literally and figuratively) in this project. Many steps are involved in calculating each ant's position, speed, velocity, orientation, and colour every frame; and these steps are often entangled together. I walked through the calculations line by line, and separated these calculations into groups, striving to ensure that each group of calculations has a coherent purpose and belongs in the same system. In total, I ended up with 16 systems, whose names are hopefully self-explanatory:

* `RandomizeFacingAngleSystem`
* `CalculateSteeringSystem`: This computes how much each ant is 1) steered away from an obstacle and 2) towards a pheromone
* `UpdateSpeedAndFacingAngleBasedOnSteeringSystem`
* `OrientTowardsDestinationSystem`
* `PickUpOrDropOffResourceSystem`
* `UpdatePositionAndVelocityAfterMovingResourceSystem`
* `AvoidObstacleSystem`
* `MoveTowardsOrAwayFromColonySystem`
* `DropPheromoneSystem`
* `DiminishPheromoneSystem`
* `UpdateLocalToWorldSystem`: This update the value in each ant's `LocalToWorld` component

I also created these components:

* `AntIndividualRendering` (Singleton): Stores colours that denote whether an ant is carrying or searching for resources
* `AntSharedRendering` (Singleton): Implements `ISharedComponentData`, and stores a common material and mesh for all ants
* `PheromoneSharedRendering` (Singleton): Implements `ISharedComponentData`, and stores a common renderer and material for all pheromones 
* `SteeringStrength` (Singleton): Stores the degree to which each feature of the landscape attracts or repels ants 
* `SteeringMovement` (Singleton): Stores the maximum speed at which each ant can move, as well as how quickly each ant accelerates 
* `Map` (Singleton): Stores data about the map width (the map is always a square), the location of resources, the colony position, as well as a `BlobAssetReference` to an `Obstacles` struct, which exposes a `Radius` property as well as a `BlobArray<float2>` of obstacle positions 
* `Brightness`, `Colour`, `Position`, `Speed`, `Velocity`, `FacingAngle`, `ResourceCarrier`: Self-explanatory
* `PheromoneColourRValueBuffer`: Implements the `IBufferElementData` interface; can be cast to and from a float
* `PheromoneSteering` and `WallSteering`: Store data about how much an ant is steered towards a pheromone or away from a wall during each frame

In addition, conversions are implemented for the `AntRenderingAuthoring`, `AntSteeringMovementAuthoring`, `AntSteeringStrengthAuthoring`, `PheromoneRendererAuthoring` and `MapAuthoring` components.

All systems make use of the `Burst` compiler as well as the job system.

## In-editor performance of the original project

### **1,000 ants**
The median frame duration was 6.44ms for simulating 1,000 ants, about 3ms of which were spent on updating information about the ants and the pheromones, so that they could be rendered correctly:

![alt text](https://i.imgur.com/bqswmXr.png)
![alt text](https://i.imgur.com/ifECrxK.png)

### **10,000 ants**
As a stress test, I increased the number of ants to 10,000. The median frame duration jumped markedly to 22.10ms (almost 4 times the median frame duration for simulating 1,000 ants), of which close to 18ms were spent on calculations for the next frame of the simulation:

![alt text](https://i.imgur.com/FBhOMVH.png)
![alt text](https://i.imgur.com/vX1RXUo.png)

### **100,000 ants**
To stress the system even more, I further raised the ant count to 100,000. The simulation almost came to a halt -- it was [running at a painfully slow pace](https://i.imgur.com/ArNAooP.mp4), with a median frame duration of almost 180ms, of which 170ms were spent on calculations:

![alt text](https://i.imgur.com/PKVNKUW.png)
![alt text](https://i.imgur.com/5b6r50M.png)

## In-editor performance of the ported project

### **1,000 ants**
The median frame duration was 5.72ms, of which 1.67ms was taken to update both the simulation system group and the presentation system group:
![alt text](https://i.imgur.com/ftKp0uS.png)
![alt text](https://i.imgur.com/JtYoM5P.png)

This might not seem like an impressive improvement over the original project (which took 6.44ms to simulate the same number of ants), but the ECS approach begins to pay dividends as we increase the number of ants.

### **10,000 ants**
With 10,000 ants, the median frame duration increased slightly from 5.72ms to 7.33ms, of which 3ms is spent on updating the two system groups (recall that the original project witnessed a quadruple jump in median frame rate when the ant count was increased from 1,000 to 10,000):
![alt text](https://i.imgur.com/QMCNHuD.png)
![alt text](https://i.imgur.com/ei4Iq55.png)

### **100,000 ants**
With  100,000 ants, we begin to see a significant increase in median frame duration -- it is now 28.04ms, and updating the system groups takes up close to 19ms:
![alt text](https://i.imgur.com/hD3QFo2.png)
![alt text](https://i.imgur.com/0BrOytZ.png)

Still, visually speaking it is [not at all horrible](https://imgur.com/eMdlUA9), especially when compared to the original project's glacial pace when simulating an equally large number of ants.

### **10,000,000 ants**
At 10,000,000 ants, we are finally close to reaching the same slowness that we experience when the original project simulates 100,000 ants: 

![alt text](https://i.imgur.com/dm63907.png)

## Porting experience
Once I got acquainted with the concept that 1) components are merely repositories of data that do not concern themselves with behaviour, and 2) systems govern behaviours but do not themselves persist any state/data, rewriting the original project to make use of the ECS pattern was rather intuitive, and did not pose too much of a cognitive roadblock. However, there are a few areas for improvement, which would greatly facilitate the learning experience of learners who have little to no experience with ECS and the Job system. 

### **Inadequate documentation**
One area where I did experience a lot of difficulty was finding the right data structures to use. Specifically, there is a paucity of resources elucidating `BlobAssetReference`; the only non-superficial description I have found online is in the final quarter of the talk [Converting Scene Data to DOTS](https://www.youtube.com/watch?v=TdlhTrq1oYk). 

### **Schizophrenic behaviour**

I have a method with the following signature inside a struct named `Obstacles`: 

`public (bool Exist, int? IndexOfCurrentBucket, int? DistanceToNextBucket) TryGetObstacles(float2 position)`

This method is invoked from within different jobs. Occasionally, an error message is emitted, informing me that the `ValueTuple` struct with `Auto` layout is not supported by Burst:

![alt text](https://i.imgur.com/Fak3yHx.png)

However, often it is the case that if I stop the simulation and start it again, the Burst compiler stops complaining about the `ValueTuple` struct. E.g., in the profiler window, I can see that the job inside `CollideWithObstacleSystem` (now renamed to `AvoidObstacleSystem`) is Burst-compiled:

![alt text](https://i.imgur.com/DefH9ky.png)

Here is a snippet of the code inside the `CollideWithObstacleSystem`:

   [BurstCompile]
   private struct Job : IJobForEach<Position, Velocity>
   {
	   public float ObstacleRadius;
	   public BlobAssetReference<Obstacles> Obstacles;
	   
	   public void Execute(ref Position position, ref Velocity velocity)
	   {
		   var obstacles = Obstacles.Value.TryGetObstacles(position.Value);
		   // Do more work
	   }
   }

There was also one occasion in which no error messages were emitted, even though the jobs invoking the `TryGetObstacles(float2 position)` method were not Burst-compiled. (Unfortunately I neglected to take a screenshot of my profiler window on that occasion, and I have been unable to reproduce this specific behaviour.) Suffice to say, it is schizophrenic of the compiler to sometimes claim that something is an error, and sometimes claim that it is not.

### **Unexplained garbage collection**

In a previous implementation, I noticed that calling the method `ToComponentDataArray()` causes garbage allocation:

![alt text](https://i.imgur.com/b7ukxwV.png)

Here is the offending code:

	NativeArray<Matrix4x4> localToWorlds;
	NativeArray<Vector4> colours;

	using (new ProfilerMarker("ToComponentDataArray").Auto())
	{
		localToWorlds = this._antQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob).Reinterpret<Matrix4x4>();
		colours = this._antQuery.ToComponentDataArray<Colour>(Allocator.TempJob).Reinterpret<Vector4>();
	}

I was surprised that the creation of new `NativeArray`s should allocate garbage. It is not clear to me why this should happen, especially when I dutifully called `Dispose()` on both `NativeArray`s after I no longer had any use for them. It is possible to find some information on this by reading through the Unity documentation, though you already need to know what to look for (hat tip to Scott Bilas for pointing me in the right direction):

* [The Unity.Collections.LowLevel.Unsafe.DisposeSentinel class](https://docs.unity3d.com/ScriptReference/Unity.Collections.LowLevel.Unsafe.DisposeSentinel.html)
* [Unity documentation on the NativeContainerAttribute](https://docs.unity3d.com/ScriptReference/Unity.Collections.LowLevel.Unsafe.NativeContainerAttribute.html)

 As part of the onboarding process, I think it would be great to provide a tutorial on how memory management actually operates in ECS (or in Unity more generally speaking), in order to reduce confusion, and to have an authoritative source to which one can refer when in doubt. A quick search through Confluence did not produce any results -- it is possible that my search skills are subpar.

### **Boilerplate code**

Currently, the APIs are not designed in a fashion that reduces boilerplate code. As an example, here is a method for creating ants, which runs inside the `Start()` method of a `GameObject`:

	private void CreateAnts()
	{
		EntityManager entityManager = World.Active.EntityManager;
		
		EntityArchetype antArchetype =
			entityManager.CreateArchetype(
				typeof(Position),
				typeof(Velocity),
				typeof(Speed),
				typeof(FacingAngle),
				typeof(Colour),
				typeof(Brightness),
				typeof(ResourceCarrier),
				typeof(PheromoneSteering),
				typeof(WallSteering),
				typeof(LocalToWorld));

		NativeArray<Entity> antEntities = new NativeArray<Entity>(length: this.AntCount, Allocator.Temp);
		entityManager.CreateEntity(antArchetype, antEntities);

        // Randomise ants

		antEntities.Dispose();
	}

With the appropriate API, all the code concerning ant creation can be pretty much be condensed into a single method which returns a `NativeArray<Entity>` of the specified length:

    World.Active.EntityManager.CreateEntities<T1, T2, T3, ...>(int length, Allocator allocator)

Such a method would communicate intent much more efficiently -- in one line, I can immediately understand that a specific number of entities with the specified components are to be created.

Additionally, here is the code that subsumed under `// Randomise ants` in the `CreateAnts()` method:

    foreach (Entity entity in antEntities)
    {
        entityManager.SetComponentData(
            entity,
            new Position
            {
                Value = MapWidth * 0.5f + new float2(Random.Range(-5f, 5f), Random.Range(-5f, 5f))
            });
        entityManager.SetComponentData(
            entity,
            new Brightness
            {
                Value = Random.Range(0.75f, 1.25f)
            });
        entityManager.SetComponentData(
            entity,
            new FacingAngle
            {
                Value = Random.value * 2 * math.PI
            });
    }

It would nice to be able to write more succinct code like this:

    antEntities.With(new Position { Value = MapWidth * 0.5f + new float2(Random.Range(-5f, 5f), Random.Range(-5f, 5f)) })
               .With(new Brightness { Value = Random.Range(0.75f, 1.25f) })
               .With(new FacingAngle { Value = Random.value * 2 * math.PI });

There are many other areas of the APIs which can similarly benefit from more conciseness.

## Conclusion
Porting the project is an interesting exercise, and overall it was an agreeable experience  -- it was intellectually fun and stimulating to experiment with a completely different software design paradigm; and being able to make things run much faster is rather thrilling! However, there are several points of friction which detracted from the overall enjoyability. As ECS matures, I hope that these issues will be mitigated by more complete documentation and more thoughtful design choices.