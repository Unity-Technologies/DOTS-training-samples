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
        var playerParabolaPrecision = config.PlayerParabolaPrecision;
        var collisionStepMultiplier = config.CollisionStepMultiplier;

        Entities
            .ForEach((Entity entity, ref Translation translation, ref Rotation rotation, ref FiringTimer firingTimer) =>
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


                    float height = CalculateHeight(startY, endY);

                    JumpTheGun.Parabola.Create(startY, height, endY, out float a, out float b, out float c);

                    float2 startPos = new float2(startBoxCol, startBoxRow);
                    float2 endPos = new float2(endBoxCol, endBoxRow);
                    float dist = math.distance(startPos, endPos);
                    float duration = dist / Cannonball.SPEED;
                    if (duration < 1f)
                        duration = 1; // no less than 1 sec duration to reach target

                    // determine forward vector for the full parabola
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

                    // TODO: set cannon rotation values into AimDirection
                    //SetCannonRotation(Mathf.Atan2(Parabola.Solve(paraA, paraB, paraC, .1f) - Parabola.Solve(paraA, paraB, paraC, 0), .1f) * Mathf.Rad2Deg);
                    ecb.SetComponent(entity, new AimDirection
                    {
                        Pitch = 0,
                        Yaw = 0
                    });
                }
            }).Schedule();
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    /// <summary>
    /// Binary searching to determine height of cannonball arc
    /// </summary>
    /// <param name="startY"></param>
    /// <param name="endY"></param>
    /// <returns></returns>
    private static float CalculateHeight(float startY, float endY)
    {
        // TODO: implement the original arc checks to avoid collision with boxes
        /*float low = math.max(startY, endY);
        float high = low * 2;
        float paraA, paraB, paraC;

        // look for height of arc that won't hit boxes
        while (true)
        {
            Parabola.Create(startY, high, endY, out paraA, out paraB, out paraC);
            if (!Cannonball.CheckBoxCollision(start, end, paraA, paraB, paraC))
            {
                // high enough
                break;
            }
            // not high enough.  Double value
            low = high;
            high *= 2;
            // failsafe
            if (high > 9999)
            {
                return; // skip launch
            }
        }

        // do binary searches to narrow down
        while (high - low > playerParabolaPrecision)
        {
            float mid = (low + high) / 2;
            Parabola.Create(start.y, mid, end.y, out paraA, out paraB, out paraC);
            if (Cannonball.CheckBoxCollision(start, end, paraA, paraB, paraC))
            {
                // not high enough
                low = mid;
            }
            else
            {
                // too high
                high = mid;
            }
        }

        // launch with calculated height
        float height = (low + high) / 2;*/
        return math.max(startY, endY) + Player.BOUNCE_HEIGHT; // TODO: temp hack
    }
}