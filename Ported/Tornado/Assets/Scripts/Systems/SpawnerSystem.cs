
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

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

            SpawnBuildings(ecb, spawner);
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

    private static void SpawnBuildings(EntityCommandBuffer ecb, Spawner spawner)
    {
        var instance = ecb.CreateEntity();
        ecb.AddComponent<World>(instance);
        var bufferCurrent = ecb.AddBuffer<CurrentPoint>(instance);
        ecb.AddBuffer<PreviousPoint>(instance);

        var random = new Random(0x123467);

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

            bufferCurrent.Add(pointA);
            bufferCurrent.Add(pointB);
            bufferCurrent.Add(pointC);

            var beamAEntity =  ecb.Instantiate(spawner.BeamPrefab);
            var beamA = new Beam()
            {
                pointAIndex = 0,
                pointBIndex = 1,
                size = math.length(pointA.Value - pointB.Value)
            };
            ecb.SetComponent(beamAEntity, beamA);
            var translationA = new Translation()
            {
                Value = pointA.Value + (pointA.Value - pointB.Value)/2f
            };
            ecb.SetComponent(beamAEntity, translationA);
            var ScaleA = new NonUniformScale()
            {
                Value = new float3(beamA.size, 1f, 1f)
            };
            ecb.SetComponent(beamAEntity, ScaleA);


            var beamBEntity =  ecb.Instantiate(spawner.BeamPrefab);
            var beamB = new Beam()
            {
                pointAIndex = 1,
                pointBIndex = 2,
                size = math.length(pointB.Value - pointC.Value)
            };
            ecb.SetComponent(beamBEntity, beamB);
            var translationB = new Translation()
            {
                Value = pointA.Value + (pointB.Value - pointC.Value)/2f

            };
            ecb.SetComponent(beamBEntity, translationB);
            var ScaleB = new NonUniformScale()
            {
                Value = new float3(beamB.size, 1f, 1f)
            };
            ecb.SetComponent(beamBEntity, ScaleB);
        }
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
