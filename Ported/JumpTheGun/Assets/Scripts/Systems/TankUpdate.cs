using JumpTheGun;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Color = UnityEngine.Color;
using ProfilerMarker = Unity.Profiling.ProfilerMarker;


[UpdateAfter(typeof(TerrainAreaSystem))]
public partial class TankUpdate : SystemBase
{
    static ProfilerMarker s_TankRotate = new ProfilerMarker("TankRotate");
    static ProfilerMarker s_FindBestParabola = new ProfilerMarker("FindBestParabola");

    static ProfilerMarker s_AlignCannonToParabola = new ProfilerMarker("AlignCannonToParabola");
    EntityCommandBufferSystem _ecbSystem;
    protected override void OnCreate()
    {
        _ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<EntityElement>();
        RequireSingletonForUpdate<TerrainData>();
        RequireSingletonForUpdate<EntityPrefabHolder>();
    }

    protected override void OnUpdate()
    {
        var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
        var entityElement = GetSingletonEntity<EntityElement>();
        var buffer = this.GetBuffer<EntityElement>(entityElement, true);
        var childs = this.GetBufferFromEntity<Child>(true);
        var heightBuffer = this.GetBuffer<HeightElement>(entityElement, true);
        
        var terrainData = this.GetSingleton<TerrainData>();
        var prefabHolder = this.GetSingleton<EntityPrefabHolder>();
        float deltaTime = Time.DeltaTime;
        var player = this.GetSingletonEntity<PlayerTag>();
        var translation = GetComponent<Translation>(player);

        ProfilerMarker localTankRotate = s_TankRotate;
        ProfilerMarker localFindBestParabola = s_FindBestParabola;
        ProfilerMarker localAlignCannonToParabola = s_AlignCannonToParabola;
        Entities
            .WithNativeDisableParallelForRestriction(childs)
            .WithReadOnly(buffer)
            .WithReadOnly(childs)
            .WithReadOnly(heightBuffer)
            .ForEach((int entityInQueryIndex, Entity entity, ref Tank tank, ref Rotation tankRotation, in Translation tankTranslation) =>
            {
                
                // rotate toward player
                localTankRotate.Begin();
                float3 diff = translation.Value - tankTranslation.Value;
                float angle = math.atan2(diff.x, diff.z);
                float rotation = angle;
                tankRotation.Value = quaternion.EulerXYZ(0,rotation,0);
                localTankRotate.End();

                tank.cooldownTime += deltaTime;
                if (tank.cooldownTime >= Constants.tankLaunchPeriod)
				{
                    // launching cannonball
                    tank.cooldownTime -= Constants.tankLaunchPeriod;

                    // start and end positions
                    float3 start = tankTranslation.Value;
                    int2 playerBox = TerrainUtility.BoxFromLocalPosition(translation.Value, terrainData.TerrainWidth, terrainData.TerrainLength);

                    var boxEntity = buffer[playerBox.x + playerBox.y * terrainData.TerrainWidth];
                    var box = GetComponent<Brick>(boxEntity);
                    float3 end = TerrainUtility.LocalPositionFromBox(playerBox.x, playerBox.y, box.height + Constants.CANNONBALLRADIUS);
                    float distance = (new Vector2(end.z - start.z, end.x - start.x)).magnitude;
                    float duration = distance / Constants.CANNONBALL_SPEED;
                    if (duration < .0001f)
                        duration = 1;

                    // find out non-brick-colliding height
                    // search slopiest brick, and interpolate altitude at middistance
                    // From start to mid-distance, then from end to mid-distance, and same on left and right
                    localFindBestParabola.Begin();
                    float defaultHeight = math.max(start.y, end.y) + Constants.CANNONBALLRADIUS +1;
                    float height = defaultHeight;
                    float slope = 0;
                    float2 orth2d = math.normalize((end-start).xz);
                    float3 offset = new float3(orth2d.y, 0, orth2d.x) *Constants.CANNONBALLRADIUS;
                    var starts = new NativeArray<float3>(6, Allocator.Temp);
                    starts[0] = start;
                    starts[1] = end;
                    starts[2] = start+offset;
                    starts[3] = end+offset;
                    starts[4] = start-offset;
                    starts[5] = end-offset;
                    var ends = new NativeArray<float3>(6, Allocator.Temp);
                    ends[0] = end;
                    ends[1] = start;
                    ends[2] = end+offset;
                    ends[3] = start+offset;
                    ends[4] = end-offset;
                    ends[5] = start-offset;
                    for (int i = 0 ; i < 6 ; ++i) {
                        float3 starti = starts[i];
                        float3 endi = ends[i];
                        float3 dir = endi-starti;
                        float3 norm = math.normalize(dir);
                        float3 midPoint = (endi+starti)*0.5f;
                        float midSqDist = math.lengthsq(midPoint-starti);
                        float3 candidate = starti;
                        while (math.lengthsq(candidate-starti) <= midSqDist) {
                            candidate += norm * Constants.SPACING;
                            int2 candidateBox = TerrainUtility.BoxFromLocalPosition(candidate, terrainData.TerrainWidth, terrainData.TerrainLength);
                            int index = candidateBox.x + candidateBox.y * terrainData.TerrainWidth;
                            float brickHeight = heightBuffer[index];
                            float3 candidateBrickPos = TerrainUtility.LocalPositionFromBox(candidateBox.x, candidateBox.y, brickHeight);
                            float2 offset2D = candidateBrickPos.xz - norm.xz * (Constants.CANNONBALLRADIUS + Constants.SPACING*0.71f);
                            float3 hitPos = new float3(offset2D.x, candidateBrickPos.y, offset2D.y);
                            //Debug.DrawLine(starti, hitPos, Color.red, 1, false);
                            float3 dirHitPos = hitPos - starti;
                            float candidateSlope = dirHitPos.y/math.length(dirHitPos.xz);
                            float k = math.length(midPoint.xz-starti.xz)/math.length(hitPos.xz - starti.xz);
                            float3 midCandidatePos = new float3(midPoint.x, starti.y+(hitPos.y - starti.y)*k, midPoint.z);
                            if (candidateSlope > slope && midCandidatePos.y > height) {
                                //Debug.DrawLine(starti, midCandidatePos, Color.magenta, 1, false);
                                height = midCandidatePos.y;
                            }
                        }
                    }
                    localFindBestParabola.End();

                    // Spawn canonBall
                    var instance = ecb.Instantiate(entityInQueryIndex, prefabHolder.CannonBallEntityPrefab);
                    float3 position = tankTranslation.Value;
                    ecb.SetComponent(entityInQueryIndex, instance, new Translation
                    {
                        Value = position
                    });
                    var parabolaData = Parabola.Create(start.y, height, end.y);
                    parabolaData.duration = duration;
                    parabolaData.startPoint = start.xz;
                    parabolaData.endPoint = end.xz;
                    ecb.SetComponent(entityInQueryIndex, instance, parabolaData);

                    // align cannon rotation with parabola
                    localAlignCannonToParabola.Begin();
                    const float canonLength = 0.5f;
                    const float canonSqLength = canonLength*canonLength;
                    float tHigh = 1.0f;
                    float tLow = 0.0f;
                    float tMid = (tHigh - tLow) / 2;
                    float sqLength = math.lengthsq(parabolaData.endPoint-parabolaData.startPoint);
                    while (tLow == 0.0f && tHigh != 0.0f)
                    {
                        tMid = (tHigh - tLow) / 2;
                        var hMid = Parabola.Solve(parabolaData, tMid) - start.y;
                        var sqA = math.lengthsq(hMid);
                        var sqB = sqLength * math.lengthsq(tMid);
                        var sum = sqA + sqB;
                        if (sum <= canonSqLength)
                        {
                            tLow = tMid;
                        }
                        else
                        {
                            tHigh = tMid;
                        }
                    }
                    if (tHigh == 0.0f) {
                        Debug.Log("There's a bug!");
                        tMid = 0.1f;
                    }
                    float length = math.sqrt(sqLength);
                    var cannonRotation = math.atan2(Parabola.Solve(parabolaData, tMid) - Parabola.Solve(parabolaData, 0.0f), tMid*length);
        			var localRotation = quaternion.EulerXYZ(-cannonRotation, 0, 0);
                    localAlignCannonToParabola.End();

                    // Get canon child entity
                    var childBuffer = childs[entity];
                    var canonEntity = childBuffer[1].Value;
                    ecb.SetComponent(entityInQueryIndex, canonEntity, new Rotation
                    {
                        Value = localRotation
                    });
                }

			//}).Schedule(); // 200 ms in PlayerLoop
            }).ScheduleParallel(); // 8 ms in PlayerLoop
            //}).Run();

            _ecbSystem.AddJobHandleForProducer(Dependency);
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
                var hits = HitsCube(boxEntity, center, width, translation, terrainData);
                if (hits)
                    return true;
            }
        }

        // TODO: check tanks

        return false;

    }
    public static bool HitsCube(EntityElement boxEntity, float3 center, float width, Translation translation, TerrainData terrainData)
    {
        return false;
        //var brick = GetComponent<Brick>(boxEntity);
        //Bounds boxBounds = new Bounds(translation.Value, new Vector3(Constants.SPACING, brick.height, Constants.SPACING));
        //Bounds cubeBounds = new Bounds(center, new Vector3(width, width, width));
//
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

}
