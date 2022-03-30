using JumpTheGun;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;


public partial class TankUpdate : SystemBase
{
    protected override void OnUpdate()
    {
        var entityElement = GetSingletonEntity<EntityElement>();
        var buffer = GetBuffer<EntityElement>(entityElement);
        var terrainData = this.GetSingleton<TerrainData>();
        float deltaTime = Time.DeltaTime;
        var player = this.GetSingletonEntity<PlayerTag>();
        var translation = GetComponent<Translation>(player);
        Entities
            .ForEach((Entity entity, ref Tank tank, in Translation tankTranslation) =>
            {
                tank.cooldownTime += deltaTime;
                if (tank.cooldownTime >= Constants.tankLaunchPeriod)
				{

                    // launching cannonball
                    tank.cooldownTime -= Constants.tankLaunchPeriod;

                    // start and end positions
                    float3 start = tankTranslation.Value;
                    int2 playerBox = BoxFromLocalPosition(translation.Value);

                    var boxEntity = buffer[playerBox.x + playerBox.y * terrainData.TerrainWidth]; // Correct?
                    var box = GetComponent<Brick>(boxEntity);
                    Vector3 end = LocalPositionFromBox(playerBox.x, playerBox.y, box.height + Constants.CANNONBALLRADIUS);
                    float distance = (new Vector2(end.z - start.z, end.x - start.x)).magnitude;
                    float duration = distance / Constants.CANNONBALL_SPEED;
                    if (duration < .0001f)
                        duration = 1;

                    // binary searching to determine height of cannonball arc
                    float low = math.max(start.y, end.y);
                    float high = low * 2;

                    // look for height of arc that won't hit boxes
                    while (true)
                    {
                        var parabolaData = Parabola.Create(start.y, high, end.y);
                        if (!CheckBoxCollision(start, end, parabolaData, terrainData, buffer, translation))
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
                    while (high - low > Constants.playerParabolaPrecision)
                    {
                        float mid = (low + high) / 2;
                        var parabolaData = Parabola.Create(start.y, mid, end.y);
                        if (CheckBoxCollision(start, end, parabolaData, terrainData, buffer, translation))
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

                    //// launch with calculated height
                    //float height = (low + high) / 2;
                    //Cannonball cannonball = Cannonball.Create(transform.parent, start);
                    //cannonball.Launch(end, height, duration);

                    //// set cannon rotation
                    //SetCannonRotation(Mathf.Atan2(Parabola.Solve(paraA, paraB, paraC, .1f) - Parabola.Solve(paraA, paraB, paraC, 0), .1f) * Mathf.Rad2Deg);

                }

			}).Schedule();
    }
    /// <summary>
    /// Simulates firing a cannonball with the given trajectory.
    /// Returns true if the cannonball would hit a box on the way there.
    /// </summary>
    public static bool CheckBoxCollision(Vector3 start, Vector3 end, ParabolaData parabola, TerrainData terrainData,
        DynamicBuffer<EntityElement> entityElements, Translation translation)
    {

        float3 diff = end - start;
        float distance = math.length(new float2(diff.x, diff.z));

        int steps = math.max(2, (int)math.ceil(distance / Box.SPACING) + 1);

        steps = (int)math.ceil(steps * Constants.collisionStepMultiplier);

        for (int i = 0; i < steps; i++)
        {
            float t = i / (steps - 1f);

            float3 pos = GetSimulatedPosition(start, end, parabola, t);

            if (HitsCube(pos, Constants.CANNONBALLRADIUS, terrainData, entityElements, translation))
            {
                return true;
            }

        }

        return false;
    }

    /// <summary>
    /// Checks if the given cube intersects nearby boxes or tanks.
    /// </summary>
    public static bool HitsCube(float3 center, float width, TerrainData terrainData,
        DynamicBuffer<EntityElement> entityElements, Translation translation)
    {

        // check nearby boxes
        int colMin = (int)math.floor((center.x - width / 2) / Box.SPACING);
        int colMax = (int)math.ceil((center.x + width / 2) / Box.SPACING);
        int rowMin = (int)math.floor((center.z - width / 2) / Box.SPACING);
        int rowMax = (int)math.ceil((center.z + width / 2) / Box.SPACING);

        colMin = math.max(0, colMin);
        colMax = math.min(terrainData.TerrainWidth - 1, colMax);
        rowMin = math.max(0, rowMin);
        rowMax = math.min(terrainData.TerrainLength - 1, rowMax);

        for (int c = colMin; c <= colMax; c++)
        {
            for (int r = rowMin; r <= rowMax; r++)
            {
                var boxEntity = entityElements[c + r * terrainData.TerrainWidth]; // Correct?
                var hits = HitsCube(boxEntity, center, width, translation);
                if (hits)
                    return true;
            }
        }

        // TODO: check tanks

        return false;

    }
    public static bool HitsCube(EntityElement entityElement, float3 center, float width, Translation translation)
    {
        return false;
        //int2 playerBox = BoxFromLocalPosition(translation.Value);

        //Bounds boxBounds = new Bounds(transform.localPosition, new Vector3(SPACING, height, SPACING));
        //Bounds cubeBounds = new Bounds(center, new Vector3(width, width, width));

        //return boxBounds.Intersects(cubeBounds);
    }

    public static float3 GetSimulatedPosition(Vector3 start, Vector3 end, ParabolaData parabola, float t)
    {
        return new float3(
            math.lerp(start.x, end.x, t),
            Parabola.Solve(parabola, t),
            math.lerp(start.z, end.z, t)
        );
    }

    public static int2 BoxFromLocalPosition(float3 localPos)
    {
        return new int2((int)math.round(localPos.x / Box.SPACING), (int)math.round(localPos.z / Constants.SPACING));
    }
    public static float3 LocalPositionFromBox(int col, int row, float yPosition = 0)
    {
        return new float3(col * Constants.SPACING, yPosition, row * Constants.SPACING);
    }
}
