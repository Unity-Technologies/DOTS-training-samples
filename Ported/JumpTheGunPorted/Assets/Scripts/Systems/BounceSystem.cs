using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BounceSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var heightMapEntity = GetSingletonEntity<HeightBufferElement>();
        var heightMap = EntityManager.GetBuffer<HeightBufferElement>(heightMapEntity);

        // need terrain length to calculate index to our height map array
        var refs = this.GetSingleton<GameObjectRefs>();
        var config = refs.Config.Data;
        int terrainLength = config.TerrainLength;

        Entities
            .ForEach((Entity entity, ref ParabolaTValue tValue, in Translation translation, in Player playerTag) =>
            {
                if (tValue.Value < 0)
                {
                    // solving parabola path
                    int startBoxCol = (int)translation.Value.x;
                    int startBoxRow = (int)translation.Value.z;
                    float startY = heightMap[startBoxRow * terrainLength + startBoxCol] + Player.Y_OFFSET;

                    int endBoxCol = startBoxCol; // TODO: needs to be the tile that mouse is over
                    int endBoxRow = startBoxRow; // TODO: needs to be the tile that mouse is over
                    float endY = heightMap[endBoxRow * terrainLength + endBoxCol] + Player.Y_OFFSET;
                    float height = math.max(startY, endY);

                    // make height max of adjacent boxes when moving diagonally
                    if (startBoxCol != endBoxCol && startBoxRow != endBoxRow)
                    {
                        height = math.max(height, math.max(heightMap[endBoxRow * terrainLength + startBoxCol], heightMap[startBoxRow * terrainLength + endBoxCol]));
                    }
                    height += Player.BOUNCE_HEIGHT;

                    JumpTheGun.Parabola.Create(startY, height, endY, out float a, out float b, out float c);

                    float2 startPos = new float2(startBoxCol, startBoxRow);
                    float2 endPos = new float2(endBoxCol, endBoxRow);
                    float dist = math.distance(startPos, endPos);
                    float duration = math.max(1, dist) * Player.BOUNCE_BASE_DURATION;

                    ecb.AddComponent(entity, new Parabola
                    {
                        StartY = startY,
                        Height = height,
                        EndY = endY,
                        A = a,
                        B = b,
                        C = c,
                        Duration = duration
                    });

                    tValue.Value = 0;
                }
            }).Run(); // TODO: can I make this a job or parallel? errors right now doing that, figure out later

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}