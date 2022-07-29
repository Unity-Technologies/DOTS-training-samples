using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

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

            }).ScheduleParallel();*/

        Entities
           .ForEach((Entity entity, TransformAspect transform, ref URPMaterialPropertyBaseColor colour, ref NonUniformScale scale, ref Boxes boxes) =>
           {
               //var boxes = TerrainAreaClusters.BoxFromLocalPosition(translation.Value, config);
               transform.Position = new float3(row + 0.3f, 0, column + 0.3f); 
               if (row >= config.terrainWidth){
                    row = 0;
                    column ++;
               } else{
                    row++; 
               }
                boxes.row = row;
                boxes.column = column; 

                



               /*
               var index = boxes.x + boxes.y * terrainCluster.terrainWidth;

               if ((terrainCluster.maxTerrainHeight - terrainCluster.minTerrainHeight) < 0.5f)
               {
                   colour.Value = Config.minHeightColour;
               }

               else
               {
                   var brickHeight = heightBuffer[(int)index];

                   colour.Value = math.lerp(Config.minHeightColour, Config.maxHeightColour, (brickHeight - Config.minHeight) / (terrainCluster.maxTerrainHeight - Config.minHeight));

                   scale.Value = new float3(1, brickHeight, 1);

                   //translation.Value.y = brickHeight / 2 + Config.yOffset;
               }*/

           }).ScheduleParallel();

        
           //state.Enabled = false;
    }
}