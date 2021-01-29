using System;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

public class ObstacleBuilderSystem : SystemBase
{ 
    protected override void OnUpdate()
    {
        Unity.Mathematics.Random rand = new Unity.Mathematics.Random((uint)math.max(DateTime.Now.Millisecond, 1));

        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp); 

        Entities
            .WithAll<ObstacleBuilder>()
            .WithNone<Initialized>()
            .ForEach((Entity entity,in ObstacleBuilder obstacleBuilder) =>
        {
            ecb.AddComponent<Initialized>(entity);

            Entity ringEntity = ecb.CreateEntity();
            DynamicBuffer<RingElement> rings = ecb.AddBuffer<RingElement>(ringEntity);
            rings.Length = obstacleBuilder.numberOfRings;

            for (int n = 1; n <= obstacleBuilder.numberOfRings; ++n) // start:1 because center (0) is home
            {
                RingElement ring = rings[n - 1];

                ring.offsets.x = (float)n / (obstacleBuilder.numberOfRings + 1.0F) * obstacleBuilder.dimensions.x * 0.5F;
                ring.offsets.y = (float)n / (obstacleBuilder.numberOfRings + 1.0F) * obstacleBuilder.dimensions.y * 0.5F;

                ring.halfThickness = obstacleBuilder.obstacleRadius;

                float startingAngle = rand.NextFloat() * math.PI * 2.0F;
                float openingRadians = math.radians(obstacleBuilder.openingDegrees);

                ring.numberOfOpenings = rand.NextInt(2) + 1;
                switch (ring.numberOfOpenings)
                {
                    case 1:
                        {
                            float endAngle = startingAngle + openingRadians;
                            endAngle = endAngle % (math.PI * 2.0F);
                            ring.opening0.angles = new float2(startingAngle, endAngle);
                        }
                        break;
                    case 2:
                        {
                            float endAngle = startingAngle + openingRadians;
                            endAngle = endAngle % (math.PI * 2.0F);
                            ring.opening0.angles = new float2(startingAngle, endAngle);
                            startingAngle = (startingAngle + math.PI) % (math.PI * 2.0F);
                            endAngle = startingAngle + openingRadians;
                            endAngle = endAngle % (math.PI * 2.0F);
                            ring.opening1.angles = new float2(startingAngle, endAngle);
                        }
                        break;
                }
                rings[n - 1] = ring;

                float C = math.length(ring.offsets) * math.PI * 2.0F;
                int count = (int)math.ceil(C / (2.0F * obstacleBuilder.obstacleRadius) * 2.0F);

                for (int i = 0; i < count; ++i)
                {
                    float t = (float)i / (float)count;
                    float cir = math.PI * 2.0F;
                    float yOffset = math.PI / 2.0F;
                    float yaw = ((t * cir) + yOffset) % cir;

                    float2 xy = new float2(math.cos(yaw), math.sin(yaw));

                    bool spawn = true;
                    switch (ring.numberOfOpenings)
                    {
                        case 1:
                            {
                                float rot = WorldResetSystem.GetAngleFromorigin(xy);
                                spawn = !WorldResetSystem.IsBetween(ring.opening0.angles, rot);
                            }
                            break;
                        case 2:
                            {
                                spawn = !WorldResetSystem.IsBetween(ring.opening0.angles, yaw);
                                if (spawn)
                                    spawn = !WorldResetSystem.IsBetween(ring.opening1.angles, yaw);
                            }
                            break;
                    }

                    if (spawn)
                    {
                        Entity obstacle = ecb.Instantiate(obstacleBuilder.obstaclePrefab);
                        Translation translation = new Translation { Value = new float3(xy.x * ring.offsets.x, xy.y * ring.offsets.y, 0) };

                        ecb.SetComponent<Translation>(obstacle, translation);
                        ecb.AddComponent<Obstacle>(obstacle, new Obstacle());
                        ecb.AddComponent<URPMaterialPropertyBaseColor>(obstacle, new URPMaterialPropertyBaseColor { Value = obstacleBuilder.obstacleColor });
                    }
                }
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
