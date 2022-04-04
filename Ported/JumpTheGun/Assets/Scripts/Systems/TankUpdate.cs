using JumpTheGun;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
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
        var childs = this.GetBufferFromEntity<Child>(true);
        var linkedEntityGroups = this.GetBufferFromEntity<LinkedEntityGroup>(true);
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
            .WithReadOnly(childs)
            .WithReadOnly(heightBuffer)
            .WithReadOnly(linkedEntityGroups)
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

                    var boxIndex = playerBox.x + playerBox.y * terrainData.TerrainWidth;
                    float boxHeight = heightBuffer[boxIndex];
                    float3 end = TerrainUtility.LocalPositionFromBox(playerBox.x, playerBox.y, boxHeight + Constants.CANNONBALLRADIUS);
                    float distance = (new Vector2(end.z - start.z, end.x - start.x)).magnitude;
                    float duration = distance / Constants.CANNONBALL_SPEED;
                    if (duration < .0001f)
                        duration = 1;

                    // find out non-brick-colliding height
                    // search slopiest brick, and interpolate altitude at middistance
                    // From start to mid-distance, then from end to mid-distance, and same on left and right
                    localFindBestParabola.Begin();
                    float defaultHeight = math.max(start.y, end.y) + Constants.CANNONBALLRADIUS +1;
                    float midDistanceHeight = defaultHeight;
                    float slope = 0;
                    float2 orth2d = math.normalize((end-start).xz);
                    float3 offset = new float3(orth2d.y, 0, orth2d.x) *Constants.CANNONBALLRADIUS;
                    var offsets = new NativeArray<float3>(6, Allocator.Temp);
                    offsets[0] = float3.zero;
                    offsets[1] = float3.zero;
                    offsets[2] = offset;
                    offsets[3] = offset;
                    offsets[4] = -offset;
                    offsets[5] = -offset;
                    for (int i = 0 ; i < 6 ; ++i) {
                        float3 starti = (i % 2 == 0 ? start : end) + offsets[i];
                        float3 endi = (i % 2 == 0 ? end : start) + offsets[i];
                        float3 dir = endi-starti;
                        float3 norm = math.normalize(dir);
                        float3 midPoint = (endi+starti)*0.5f;
                        float midSqDist = math.lengthsq(midPoint-starti);
                        float3 candidate = starti;
                        float latestBrickHeight = 0.0f;
                        while (math.lengthsq(candidate-starti) <= midSqDist) {
                            candidate += norm * Constants.SPACING;
                            int2 candidateBox = TerrainUtility.BoxFromLocalPosition(candidate, terrainData.TerrainWidth, terrainData.TerrainLength);
                            int index = candidateBox.x + candidateBox.y * terrainData.TerrainWidth;
                            float brickHeight = heightBuffer[index];
                            if (brickHeight >= latestBrickHeight) { // no need to test lower bricks and farther away, the slope's steepness and the midDistanceHeight will necessarily be lower
                                latestBrickHeight = brickHeight;
                                float3 candidateBrickPos = TerrainUtility.LocalPositionFromBox(candidateBox.x, candidateBox.y, brickHeight);
                                float2 offset2D = candidateBrickPos.xz - norm.xz * (Constants.CANNONBALLRADIUS + Constants.SPACING*0.71f);
                                float3 hitPos = new float3(offset2D.x, candidateBrickPos.y, offset2D.y);
                                Debug.DrawLine(starti, hitPos, Color.red, 1, false);
                                float3 dirHitPos = hitPos - starti;
                                float candidateSlope = dirHitPos.y/math.length(dirHitPos.xz);
                                float k = math.length(midPoint.xz-starti.xz)/math.length(hitPos.xz - starti.xz);
                                float3 midCandidatePos = new float3(midPoint.x, starti.y+(hitPos.y - starti.y)*k, midPoint.z);
                                if (candidateSlope > slope && midCandidatePos.y > midDistanceHeight) { // slope and midDistanceHeight measure the same thing only one of the tests is normally needed
                                    Debug.DrawLine(starti, midCandidatePos, Color.magenta, 1, false);
                                    midDistanceHeight = midCandidatePos.y;
                                }
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
                    var parabolaData = Parabola.Create(start.y, midDistanceHeight, end.y);
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
                    var linkedEntityGroupBuffer = linkedEntityGroups[entity];
                    Entity canonEntity = linkedEntityGroupBuffer[1].Value; // 1 comes from the position of the cannon in the LinkedEntityGroup Buffer, do not use Child since it seems the index is not consistant
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

}
