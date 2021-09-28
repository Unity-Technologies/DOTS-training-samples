
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial class SpawnerSystem : SystemBase
{


    protected override void OnUpdate()
    {
        var random = new Random(0x1234567);   
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        Entities
            .WithStructuralChanges()
            .ForEach((Entity entity, in Spawner spawner) =>
        {
            ecb.DestroyEntity(entity);

            SpawnCubes(spawner, ecb, random);

            SpawnBuildings(ecb, spawner, random);
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
    
    // This describes the number of buffer elements that should be reserved
    // in chunk data for each instance of a buffer. In this case, 8 integers
    // will be reserved (32 bytes) along with the size of the buffer header
    // (currently 16 bytes on 64-bit targets)
    //[InternalBufferCapacity(8)]
    //TODO: ask fabrice, could we have just a buffer of floats? Do we need a struct? Does burst optmizes this somehow?
    public struct CurrentPoint : IBufferElementData
    {
        public float3 Value;
    }
    
    public struct PreviousPoint : IBufferElementData
    {
        public float3 Value;
    }
    public struct AnchorPoint : IBufferElementData
    {
	    public bool Value;
    }

    public struct NeighborCount : IBufferElementData
    {
	    public int Value;
    }

    private static void SpawnBuildings(EntityCommandBuffer ecb, Spawner spawner, Random random)
    {
        var instance = ecb.CreateEntity();
        ecb.AddComponent<World>(instance);
        //TODO: initialize the arrays to be beamsize*2
        var bufferCurrent = ecb.AddBuffer<CurrentPoint>(instance);
        var bufferPrevious = ecb.AddBuffer<PreviousPoint>(instance);
        var anchorBuffer = ecb.AddBuffer<AnchorPoint>(instance);
        var neighborBuffer = ecb.AddBuffer<NeighborCount>(instance);


        var tempCurrent = new List<CurrentPoint>();
        var tempPrevious = new List<PreviousPoint>();
        var tempAnchor = new List<AnchorPoint>();
        
/*
        for (var i = 0; i < spawner.TowerCount; i++)
        {
            var randPos = random.NextFloat3(new float3(-50f,0f,-50f),new float3(50f,0f,50f));
            var pointA = new CurrentPoint()
            {
                Value = randPos + new float3(5f,0f,0f)
            };
            var pointB = new CurrentPoint(){
                Value = randPos
            };
            var pointC = new CurrentPoint(){
                Value = randPos + new float3(0f,0f,5f)
            };

            tempCurrent.Add(pointA);
            tempCurrent.Add(pointB);
            tempCurrent.Add(pointC);

            var beamAEntity =  ecb.Instantiate(spawner.BeamPrefab);
            var beamA = new Beam()
            {
                pointAIndex = i*3 + 0,
                pointBIndex = i*3 + 1,
                size = math.length(pointA.Value - pointB.Value)
            };
            ecb.SetComponent(beamAEntity, beamA);


            var beamBEntity =  ecb.Instantiate(spawner.BeamPrefab);
            var beamB = new Beam()
            {
                pointAIndex = i*3 +1,
                pointBIndex = i*3 +2,
                size = math.length(pointB.Value - pointC.Value)
            };
            ecb.SetComponent(beamBEntity, beamB);
        }
*/

        //var pointsList = tempCurrent;
        //Copied code:

		
		//List<Bar> barsList = new List<Bar>();
		//List<List<Matrix4x4>> matricesList = new List<List<Matrix4x4>>();
		//matricesList.Add(new List<Matrix4x4>());

		var pointCount = 0;

		// buildings
		for (int i = 0; i < 35; i++) {
			int height = random.NextInt(4,12);
			Vector3 pos = new Vector3(random.NextFloat(-45f,45f),0f,random.NextFloat(-45f,45f));
			float spacing = 2f;
			for (int j = 0; j < height; j++)
			{
				float3 currentPosition;
					
				//Point point = new Point();
				currentPosition.x = pos.x+spacing;
				currentPosition.y = j * spacing;
				currentPosition.z = pos.z-spacing;


				tempCurrent.Add(new CurrentPoint() { Value = currentPosition});
				tempPrevious.Add(new PreviousPoint() { Value = currentPosition });
				tempAnchor.Add(new AnchorPoint() { Value = j == 0 });

				currentPosition.x = pos.x-spacing;
				currentPosition.y = j * spacing;
				currentPosition.z = pos.z-spacing;
				
				tempCurrent.Add(new CurrentPoint() { Value = currentPosition});
				tempPrevious.Add(new PreviousPoint() { Value = currentPosition });
				tempAnchor.Add(new AnchorPoint() { Value = j == 0 });


				currentPosition.x = pos.x+0f;
				currentPosition.y = j * spacing;
				currentPosition.z = pos.z+spacing;

				tempCurrent.Add(new CurrentPoint() { Value = currentPosition});
				tempPrevious.Add(new PreviousPoint() { Value = currentPosition });
				tempAnchor.Add(new AnchorPoint() { Value = j == 0 });

				
				pointCount += 3;
			}
		}

		// ground details
		for (int i=0;i<600;i++) {
			float3 pos = new float3(random.NextFloat(-55f,55f),0f,random.NextFloat(-55f,55f));
			//Point point = new Point();
			float3 currentPosition;
			currentPosition.x = pos.x + random.NextFloat(-.2f,-.1f);
			currentPosition.y = pos.y + random.NextFloat(0f,3f);
			currentPosition.z = pos.z + random.NextFloat(.1f,.2f);
			
			
			tempCurrent.Add(new CurrentPoint() { Value = currentPosition});
			tempPrevious.Add(new PreviousPoint() { Value = currentPosition });
			tempAnchor.Add(new AnchorPoint() { Value = false });
			pointCount++;
			

			currentPosition.x = pos.x + random.NextFloat(.2f,.1f);
			currentPosition.y = pos.y + random.NextFloat(0f,.2f);
			currentPosition.z = pos.z + random.NextFloat(-.1f,-.2f);

			tempCurrent.Add(new CurrentPoint() { Value = currentPosition});
			tempPrevious.Add(new PreviousPoint() { Value = currentPosition });
			pointCount++;
			
			tempAnchor.Add(new AnchorPoint() { Value = (random.NextFloat()<.1f) });
		}


		var tempNeighbor = new NeighborCount[tempCurrent.Count];

		var beamCount = 0; 
		for (int i = 0; i < tempCurrent.Count; i++) {
			for (int j = i + 1; j < tempCurrent.Count; j++) {
				//Bar bar = new Bar();
				//bar.AssignPoints(pointsList[i],pointsList[j]);

				var beamSize = math.length(tempCurrent[i].Value - tempCurrent[j].Value);

				if (beamSize < 5f && beamSize>.2f) {
					
					/*var beamAEntity =  ecb.Instantiate(spawner.BeamPrefab);
					var beamA = new Beam()
					{
						pointAIndex = i,
						pointBIndex = j,
						size = beamSize
					};
					ecb.SetComponent(beamAEntity, beamA);*/
					
					tempNeighbor[i] = new NeighborCount { Value = tempNeighbor[i].Value + 1 };
					tempNeighbor[j] = new NeighborCount { Value = tempNeighbor[j].Value + 1 };
					beamCount++; 

					/*bar.point1.neighborCount++;
					bar.point2.neighborCount++;*/

					//barsList.Add(bar);
					//matricesList[batch].Add(bar.matrix);
					/*if (matricesList[batch].Count == instancesPerBatch) {
						batch++;
						matricesList.Add(new List<Matrix4x4>());
					}
					if (barsList.Count % 500 == 0) {
						yield return null;
					}*/
				}
				
				
			}
		}
		
		bufferCurrent.Capacity = beamCount * 2;
		bufferPrevious.Capacity = beamCount * 2;
		neighborBuffer.Capacity = beamCount * 2;
		anchorBuffer.Capacity = beamCount * 2;
		
		pointCount = 0;
		for (int i=0;i<tempCurrent.Count;i++) {
			if (tempNeighbor[i].Value > 0)
			{
				bufferCurrent.Add(tempCurrent[i]);
				bufferPrevious.Add(tempPrevious[i]);
				anchorBuffer.Add(tempAnchor[i]);
				neighborBuffer.Add(new NeighborCount() { Value = 0 });
				/*
				bufferCurrent[pointCount] = tempCurrent[i];
				bufferPrevious[pointCount] = tempPrevious[i];
				anchorBuffer[pointCount] = tempAnchor[i];
				//we are able to remove this since the array is initialized with 0 automatically
				//neighborBuffer[pointCount] = new NeighborCount(){ Value = 0 };
				*/
				pointCount++;
			}
		}


		var barCount = 0;
		//TODO: Profile this, if its expensive, we might be able to move this loop above, then merge this loop with the one before the loop above, and use a map to remap the beams indexes
		for (int i = 0; i < pointCount; i++) {
			for (int j = i + 1; j < pointCount; j++) {
				var delta = bufferCurrent[i].Value - bufferCurrent[j].Value;
				var beamSize = math.length(delta);

				if (beamSize < 5f && beamSize>.2f) {
					var beamAEntity =  ecb.Instantiate(spawner.BeamPrefab);
					var beamA = new Beam()
					{
						pointAIndex = i,
						pointBIndex = j,
						size = beamSize
					};
					neighborBuffer[i] = new NeighborCount { Value = neighborBuffer[i].Value + 1 };
					neighborBuffer[j] = new NeighborCount { Value = neighborBuffer[j].Value + 1 };
					ecb.SetComponent(beamAEntity, beamA);
					barCount++;
					
					float upDot = math.acos(math.abs(math.dot(new float3(0f,1f,0f), math.normalize(delta))))/math.PI;
					ecb.SetComponent<URPMaterialPropertyBaseColor>(beamAEntity, new URPMaterialPropertyBaseColor()
					{
						Value =  new float4(1f,1f,1f,1f) * upDot * random.NextFloat(.7f,1f)
					});

				}
			}
		}

		Debug.Log(pointCount + " points, extra room for " + (bufferCurrent.Length - pointCount)  + " (" +barCount + " bars)");
		
		System.GC.Collect();

    }

    private static void SpawnCubes(Spawner spawner, EntityCommandBuffer ecb, Random random)
    {
        for (int i = 0; i < spawner.CubeCount; i++)
        {
            var instance = ecb.Instantiate(spawner.CubePrefab);


            float3 pos = new float3(random.NextFloat(-50f, 50f), random.NextFloat(0f, 50f), random.NextFloat(-50f, 50f));
            ecb.SetComponent(instance, new Translation()
            {
                Value = pos
            });

            ecb.SetComponent(instance, new Cube()
            {
                radius = random.NextFloat(0, 1f)
            });

            var color = random.NextFloat(.3f, .7f);
            ecb.SetComponent(instance, new URPMaterialPropertyBaseColor()
            {
                Value = new float4(color, color, color, 1f)
            });
        }
    }
}
