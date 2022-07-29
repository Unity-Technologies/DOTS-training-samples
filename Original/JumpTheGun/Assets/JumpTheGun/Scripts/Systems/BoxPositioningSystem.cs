using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Burst;

[BurstCompile]
partial struct BoxPosJob : IJobEntity
{
    public Config config; 
    public EntityCommandBuffer.ParallelWriter ECB;
    public int row;
    public int col;

    void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, TransformAspect transform, Boxes boxes, ref URPMaterialPropertyBaseColor colour, ref NonUniformScale scale)
    {

        Random random;
        random = Random.CreateFromIndex((uint)entity.Index);

        // Notice that this is a lambda being passed as parameter to ForEach.
        //var pos = transform.Position;

        //pos.x = random.NextFloat(0, 10);
        //pos.z = random.NextFloat(0, 10);

        transform.Position = new float3(row * 1.2f, 0, col * 1.2f);

        if (row >= config.terrainWidth)
        {
            row = 0;
            col++;
        }
        else
        {
            row++;
        }
        boxes.row = row;
        boxes.column = col;


        UnityEngine.Debug.Log("This row is: " + row + " " + "This column is: " + col);



        scale.Value = new float3(1, random.NextFloat(0, 5), 1);

        //transform.Position = pos;

        if ((config.maxTerrainHeight - config.minTerrainHeight) < 0.5f)
        {
            colour.Value = Config.minHeightColour;
        }
        else
        {
            colour.Value = Config.maxHeightColour;
        }
    }
}

[BurstCompile]
partial struct BoxPositioningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();



        int row = 0;
        int column = 0;



        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var BoxJob = new BoxPosJob
        {
            config = config,
            row = row,
            col = column,
            ECB = ecb.AsParallelWriter(),
        };

        BoxJob.Run(); 
        //state.Enabled = false; 
    }
}

/*

partial class BoxPositioningSystem : SystemBase
{

    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        //state.RequireForUpdate<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var config = SystemAPI.GetSingleton<Config>();
        //var anEntity = GetSingletonEntity<HeightElement>();
        //var heightBuffer = this.GetBuffer<HeightElement>(anEntity, true);
        //var terrainCluster = this.GetSingleton<Config>();
        int row = 0;
        int column = 0; 

        Random random;
        random = Random.CreateFromIndex(1234);

        var deltaTime = Time.DeltaTime;

        /*Entities
            .WithAll<Boxes>()
            .ForEach((Entity entity, TransformAspect transform, ref NonUniformScale scale) =>
            {
                // Notice that this is a lambda being passed as parameter to ForEach.
                var pos = transform.Position;

                pos.x = random.NextFloat(0, 10);
                pos.z = random.NextFloat(0, 10);

                scale.Value = new float3(1, random.NextFloat(0, 5), 1);

                transform.Position = pos;

            }).ScheduleParallel();

        Entities
           .ForEach((Entity entity, TransformAspect transform, ref URPMaterialPropertyBaseColor colour, ref NonUniformScale scale, ref Boxes boxes) =>
           {
               //var boxes = TerrainAreaClusters.BoxFromLocalPosition(translation.Value, config);
               transform.Position = new float3(row * 1.2f, 0, column * 1.2f); 
               if (row >= config.terrainWidth){
                    row = 0;
                    column ++;
               } else{
                    row++; 
               }
                boxes.row = row;
                boxes.column = column; 

                

               var boxLocalPos = TerrainAreaClusters.BoxFromLocalPosition(transform.Position, config);

               
               var index = boxLocalPos.x + boxLocalPos.y * config.terrainWidth;

               if ((config.maxTerrainHeight - config.minTerrainHeight) < 0.5f)
               {
                   colour.Value = Config.minHeightColour;
               }
            /*
               else
               {
                   var brickHeight = heightBuffer[(int)index];

                   colour.Value = math.lerp(Config.minHeightColour, Config.maxHeightColour, (brickHeight - Config.minHeight) / (terrainCluster.maxTerrainHeight - Config.minHeight));

                   scale.Value = new float3(1, brickHeight, 1);

                   //translation.Value.y = brickHeight / 2 + Config.yOffset;
               }

           }).Schedule();

        
           //state.Enabled = false;
    }
}*/