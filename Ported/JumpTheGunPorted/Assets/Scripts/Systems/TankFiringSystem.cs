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
                    float endY = heightMap[endBoxRow * terrainLength + endBoxCol] + Cannonball.RADIUS;

                    float3 start = new float3(startBoxCol, startY, startBoxRow) + Cannonball.RADIUS;
                    float3 end = new float3(endBoxCol, endY, endBoxRow);
                    
                    float height = CalculateHeight(start, end, playerParabolaPrecision, collisionStepMultiplier, terrainWidth, terrainLength, heightMap);
                    
                    if (height > 0)
                    {
                        var cannonball = ecb.Instantiate(cannonballPrefab);
                        ecb.SetComponent(cannonball, new Translation
                        {
                            Value = translation.Value // TODO: do we need to stick in front of the forward of the turret?
                        }); ;
                        ecb.AddComponent(cannonball, new ParabolaTValue
                        {
                            Value = 0 // start moving right away
                        });

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

                        // TODO: set cannon pitch (maybe also yaw too at this point)
                        //SetCannonRotation(math.Atan2(Parabola.Solve(paraA, paraB, paraC, .1f) - Parabola.Solve(paraA, paraB, paraC, 0), .1f) * math.Rad2Deg);
                        /*ecb.SetComponent(entity, new Rotation
                        {
                            Pitch = 0,
                            Yaw = 0
                        });*/
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning("Cannonball height could not be determined, skipping launch");
                    }
                }
            }).Schedule();
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }


    //Binary searching to determine height of cannonball arc (avoiding boxes in between)
    private static float CalculateHeight(float3 start, float3 end, float playerParabolaPrecision, float collisionStepMultiplier, int terrainWidth, int terrainLength, DynamicBuffer<HeightBufferElement> heightMap)
    {
        float low = math.max(start.y, end.y);
        float high = low * 2;
        float paraA, paraB, paraC;

        // look for height of arc that won't hit boxes
        while (true)
        {
            JumpTheGun.Parabola.Create(start.y, high, end.y, out paraA, out paraB, out paraC);
            if (!CheckBoxCollision(start, end, paraA, paraB, paraC, collisionStepMultiplier, terrainWidth, terrainLength, heightMap))
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
                return -1; // skip launch
            }
        }

        // do binary searches to narrow down
        while (high - low > playerParabolaPrecision)
        {
            float mid = (low + high) / 2;
            JumpTheGun.Parabola.Create(start.y, mid, end.y, out paraA, out paraB, out paraC);
            if (CheckBoxCollision(start, end, paraA, paraB, paraC, collisionStepMultiplier, terrainWidth, terrainLength, heightMap))
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
        float height = (low + high) / 2;
        return height;
    }

    public static bool CheckBoxCollision(float3 start, float3 end, float paraA, float paraB, float paraC, float collisionStepMultiplier, int terrainWidth, int terrainLength, DynamicBuffer<HeightBufferElement> heightMap)
    {

        float3 diff = end - start;
        float distance = (float) math.sqrt(diff.x* diff.x + diff.z* diff.z);

        int steps = math.max(2, (int) math.ceil(distance / Box.SPACING) + 1);

        steps = (int) math.ceil(steps * collisionStepMultiplier);

        for (int i = 0; i < steps; i++)
        {
            float t = i / (steps - 1f);

            float3 pos = GetSimulatedPosition(start, end, paraA, paraB, paraC, t);

            if (HitsAnyCube(pos, Cannonball.RADIUS, terrainWidth, terrainLength, heightMap))
            {
                return true;
            }

        }

        return false;
    }

    public static float3 GetSimulatedPosition(float3 start, float3 end, float paraA, float paraB, float paraC, float t)
    {
        return new float3(
            math.lerp(start.x, end.x, t),
            JumpTheGun.Parabola.Solve(paraA, paraB, paraC, t),
            math.lerp(start.z, end.z, t)
        );
    }

    public static bool HitsAnyCube(float3 center, float width, int terrainWidth, int terrainLength, DynamicBuffer<HeightBufferElement> heightMap)
    {
        // check nearby boxes
        int colMin = (int) math.floor((center.x - width / 2) / Box.SPACING);
        int colMax = (int)math.ceil((center.x + width / 2) / Box.SPACING);
        int rowMin = (int)math.floor((center.z - width / 2) / Box.SPACING);
        int rowMax = (int)math.ceil((center.z + width / 2) / Box.SPACING);

        colMin = math.max(0, colMin);
        colMax = math.min(terrainLength - 1, colMax);
        rowMin = math.max(0, rowMin);
        rowMax = math.min(terrainWidth - 1, rowMax);

        for (int c = colMin; c <= colMax; c++)
        {
            for (int r = rowMin; r <= rowMax; r++)
            {
                var boxHeight = heightMap[r * terrainLength + c];
                var boxLocalPos = new float3(c, boxHeight / 2, r);

                if (HitsCube(center, width, boxLocalPos, boxHeight))
                    return true;
            }
        }

        // TODO: check tanks

        return false;

    }

    public static bool HitsCube(float3 center, float width, float3 boxLocalPos, float height)
    {
        UnityEngine.Bounds boxBounds = new UnityEngine.Bounds(boxLocalPos, new float3(Box.SPACING, height, Box.SPACING));
        UnityEngine.Bounds cubeBounds = new UnityEngine.Bounds(center, new float3(width, width, width));
        return boxBounds.Intersects(cubeBounds);
    }
}