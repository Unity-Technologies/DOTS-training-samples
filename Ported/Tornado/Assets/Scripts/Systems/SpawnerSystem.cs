
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
            .ForEach((Entity entity, in Spawner spawner) =>
        {
            ecb.DestroyEntity(entity);

            SpawnCubes(spawner, ecb, random);

            SpawnBuildings(ecb, spawner, random);
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    private static void SpawnBuildings(EntityCommandBuffer ecb, Spawner spawner, Random random)
    {
	    const int maxTowerHeight = 12;
        var instance = ecb.CreateEntity();


        ecb.AddComponent<World>(instance);


        var bufferCurrent = ecb.AddBuffer<CurrentPoint>(instance);
        var bufferPrevious = ecb.AddBuffer<PreviousPoint>(instance);
        var anchorBuffer = ecb.AddBuffer<AnchorPoint>(instance);
        var neighborBuffer = ecb.AddBuffer<NeighborCount>(instance);

        var maxPoints = spawner.TowerCount * maxTowerHeight * 3 + spawner.GroundPoints * 2;
        /*
        var tempCurrent = new List<CurrentPoint>(maxPoints);
        var tempPrevious = new List<PreviousPoint>(maxPoints);
        var tempAnchor = new List<AnchorPoint>(maxPoints);
*/
        var tempCurrent = new NativeArray<CurrentPoint>(maxPoints, Allocator.TempJob);
        var tempPrevious = new NativeArray<PreviousPoint>(maxPoints, Allocator.TempJob);
        var tempAnchor = new NativeArray<AnchorPoint>(maxPoints, Allocator.TempJob);

        var pointCount = 0;
        // buildings
		for (int i = 0; i < spawner.TowerCount; i++) {
			int height = random.NextInt(4,maxTowerHeight);
			Vector3 pos = new Vector3(random.NextFloat(-45f,45f),0f,random.NextFloat(-45f,45f));
			float spacing = 2f;
			for (int j = 0; j < height; j++)
			{
				float3 currentPosition;
				
				currentPosition.x = pos.x+spacing;
				currentPosition.y = j * spacing;
				currentPosition.z = pos.z-spacing;
				
				tempCurrent[pointCount] = new CurrentPoint() { Value = currentPosition};
				tempPrevious[pointCount] = new PreviousPoint() { Value = currentPosition };
				tempAnchor[pointCount] = new AnchorPoint() { Value = j == 0 };
				pointCount++;

				currentPosition.x = pos.x-spacing;
				currentPosition.y = j * spacing;
				currentPosition.z = pos.z-spacing;
				
				tempCurrent[pointCount] = new CurrentPoint() { Value = currentPosition};
				tempPrevious[pointCount] = new PreviousPoint() { Value = currentPosition };
				tempAnchor[pointCount] = new AnchorPoint() { Value = j == 0 };
				pointCount++;

				currentPosition.x = pos.x+0f;
				currentPosition.y = j * spacing;
				currentPosition.z = pos.z+spacing;

				tempCurrent[pointCount] = new CurrentPoint() { Value = currentPosition};
				tempPrevious[pointCount] = new PreviousPoint() { Value = currentPosition };
				tempAnchor[pointCount] = new AnchorPoint() { Value = j == 0 };
				pointCount++;

			}
		}

		// ground details
		for (int i=0;i<spawner.GroundPoints;i++) {
			float3 pos = new float3(random.NextFloat(-55f,55f),0f,random.NextFloat(-55f,55f));
			float3 currentPosition;
			currentPosition.x = pos.x + random.NextFloat(-.2f,-.1f);
			currentPosition.y = pos.y + random.NextFloat(0f,3f);
			currentPosition.z = pos.z + random.NextFloat(.1f,.2f);

			tempCurrent[pointCount] = new CurrentPoint() { Value = currentPosition};
			tempPrevious[pointCount] = new PreviousPoint() { Value = currentPosition };
			tempAnchor[pointCount] = new AnchorPoint() { Value = false };
			pointCount++;

			currentPosition.x = pos.x + random.NextFloat(.2f,.1f);
			currentPosition.y = pos.y + random.NextFloat(0f,.2f);
			currentPosition.z = pos.z + random.NextFloat(-.1f,-.2f);

			tempCurrent[pointCount] = new CurrentPoint() { Value = currentPosition};
			tempPrevious[pointCount] = new PreviousPoint() { Value = currentPosition };
			tempAnchor[pointCount] = new AnchorPoint() { Value = (random.NextFloat()<.1f) };
			pointCount++;
		}


		//var tempNeighbor = new NeighborCount[pointCount];
		var tempNeighbor = new NativeArray<NeighborCount>(pointCount, Allocator.TempJob);


		var beamCount = 0; 
		for (int i = 0; i < pointCount; i++) {
			for (int j = i + 1; j < pointCount; j++) {

				var beamSize = math.length(tempCurrent[i].Value - tempCurrent[j].Value);

				if (beamSize < 5f && beamSize>.2f) {
					tempNeighbor[i] = new NeighborCount { Value = tempNeighbor[i].Value + 1 };
					tempNeighbor[j] = new NeighborCount { Value = tempNeighbor[j].Value + 1 };
					beamCount++;
				}
			}
		}
		
		bufferCurrent.Capacity = beamCount * 2;
		bufferPrevious.Capacity = beamCount * 2;
		neighborBuffer.Capacity = beamCount * 2;
		anchorBuffer.Capacity = beamCount * 2;
		
		var fPointCount = 0;
		for (int i=0;i<pointCount;i++) {
			if (tempNeighbor[i].Value > 0)
			{
				bufferCurrent.Add(tempCurrent[i]);
				bufferPrevious.Add(tempPrevious[i]);
				anchorBuffer.Add(tempAnchor[i]);
				neighborBuffer.Add(new NeighborCount() { Value = 0 });
				fPointCount++;
			}
		}

		ecb.RemoveComponent<Translation>(spawner.BeamPrefab);
		ecb.RemoveComponent<Rotation>(spawner.BeamPrefab);
		ecb.RemoveComponent<NonUniformScale>(spawner.BeamPrefab);
		 
		var barCount = 0;
		//TODO: Profile this, if its expensive, we might be able to move this loop above, then merge this loop with the one before the loop above, and use a map to remap the beams indexes
		for (int i = 0; i < fPointCount; i++) {
			for (int j = i + 1; j < fPointCount; j++) {
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
					ecb.SetComponent(beamAEntity, new URPMaterialPropertyBaseColor()
					{
						Value =  new float4(1f,1f,1f,1f) * upDot * random.NextFloat(.7f,1f)
					});
				}
			}
		}

		//Debug.Log(fPointCount + " points, extra room for " + (bufferCurrent.Length - fPointCount)  + " (" +barCount + " bars)");
		 
		//System.GC.Collect();

		tempCurrent.Dispose();
		tempPrevious.Dispose();
		tempAnchor.Dispose();
		tempNeighbor.Dispose();

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
                spinningRadius = random.NextFloat(0, 1f)
            });

            var color = random.NextFloat(.3f, .7f);
            ecb.SetComponent(instance, new URPMaterialPropertyBaseColor()
            {
                Value = new float4(color, color, color, 1f)
            });
        }
    }
}
