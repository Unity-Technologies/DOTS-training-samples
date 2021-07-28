using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class TankFiringystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var time = Time.ElapsedTime;

        var boxMapEntity = GetSingletonEntity<HeightBufferElement>(); // ASSUMES the singleton that has height buffer also has occupied
        var heightMap = EntityManager.GetBuffer<HeightBufferElement>(boxMapEntity);

        var player = GetSingletonEntity<Player>();
        var playerTranslation = GetComponent<Translation>(player);

        // need terrain length to calculate index to our height map array
        var refs = this.GetSingleton<GameObjectRefs>();
        var cannonballPrefab = refs.CannonballPrefab;

        var config = refs.Config.Data;
        int terrainLength = config.TerrainLength;
        int terrainWidth = config.TerrainWidth;
        var reloadTime = config.TankReloadTime;

        Entities
            .ForEach((ref Translation translation, ref Rotation rotation, ref FiringTimer firingTimer) =>
            {
                // time to shoot yet?
                if (time >= firingTimer.NextFiringTime)
                {
                    firingTimer.NextFiringTime = (float) time + reloadTime;

                    var cannonball = ecb.Instantiate(cannonballPrefab);
                    ecb.SetComponent(cannonball, new Translation
                    {
                        Value = translation.Value // TODO: do we need to stick in front of the forward of the turret?
                    }); ;
                    ecb.AddComponent(cannonball, new ParabolaTValue
                    {
                        Value = 0 // start moving right away
                    });

                    // solving parabola path
                    //start at player and move towards the box the mouse is over
                    float2 currentPos = new float2(
                        math.clamp(math.round(translation.Value.x), 0, terrainLength - 1),
                        math.clamp(math.round(translation.Value.z), 0, terrainWidth - 1)
                    );
                    int startBoxCol = (int)currentPos.x;
                    int startBoxRow = (int)currentPos.y;
                    float startY = heightMap[startBoxRow * terrainLength + startBoxCol];

                    // target box is player's current position
                    float2 playerBoxPos = new float2(
                        math.clamp(math.round(playerTranslation.Value.x), 0, terrainLength - 1),
                        math.clamp(math.round(playerTranslation.Value.z), 0, terrainWidth - 1)
                    );
                    int endBoxCol = (int)playerBoxPos.x;
                    int endBoxRow = (int)playerBoxPos.y;
                    float endY = heightMap[endBoxRow * terrainLength + endBoxCol];

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
                    float duration = dist / Cannonball.SPEED;
                    if (duration < .0001f)
                        duration = 1;

                    // determine forward movement per t
                    float3 forward = new float3(endBoxCol, 0, endBoxRow) - new float3(startBoxCol, 0, startBoxRow);

                    // construct the parabola data struct for use in the movement system
                    ecb.AddComponent(cannonball, new Parabola
                    {
                        StartY = startY,
                        Height = height,
                        EndY = endY,
                        A = a,
                        B = b,
                        C = c,
                        Duration = duration,
                        Forward = forward
                    });
                }
            }).Run(); // TODO: running as a job gives us error: InvalidOperationException: The previously scheduled job TankFiringystem:OnUpdate_LambdaJob0 writes to the Unity.Entities.EntityCommandBuffer OnUpdate_LambdaJob0.JobData.ecb. You must call JobHandle.Complete() on the job TankFiringystem:OnUpdate_LambdaJob0, before you can write to the Unity.Entities.EntityCommandBuffer safely.

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}