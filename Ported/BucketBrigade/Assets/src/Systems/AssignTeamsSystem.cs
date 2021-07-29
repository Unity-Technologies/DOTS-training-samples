using System;
using src.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Transforms;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(TickSystem))]
    public class AssignTeamsSystem : SystemBase
    {
        static ProfilerMarker s_FireLookupMarker = new ProfilerMarker("FireLookup");
        static ProfilerMarker s_WaterLookupMarker = new ProfilerMarker("WaterLookup");
        static ProfilerMarker s_TeamPosLookupMarker = new ProfilerMarker("TeamPos");
        EntityQuery m_WaterTagQuery;
        EntityQuery m_WorkersQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<TeamData>();
            RequireSingletonForUpdate<Temperature>();
            RequireSingletonForUpdate<FireSimConfigValues>();
            RequireSingletonForUpdate<EcsTick>();

            // NW: When we look for the closest fire and water, rather than querying the ENTIRE team (which is slow), instead we just query one of the team subgroups (as there are fewer, but still representative).
            m_WorkersQuery = GetEntityQuery(ComponentType.ReadOnly<Position>(), ComponentType.ReadOnly<TeamId>(), ComponentType.ReadOnly<FullBucketPasserTag>());
            m_WaterTagQuery = GetEntityQuery(ComponentType.ReadOnly<LocalToWorld>(), ComponentType.ReadOnly<WaterTag>());
        }

        protected override void OnUpdate()
        {
            if (m_WaterTagQuery.IsEmpty || m_WorkersQuery.IsEmpty)
                return;
            var configValues = GetSingleton<FireSimConfigValues>();
            var tick = GetSingleton<EcsTick>();

            var teamContainerEntity = GetSingletonEntity<TeamData>();
            var teamDatas = EntityManager.GetBuffer<TeamData>(teamContainerEntity);
         
            var temperatureEntity = GetSingletonEntity<Temperature>();
            var temperatures = EntityManager.GetBuffer<Temperature>(temperatureEntity).AsNativeArray();
            
            var fireSimConfigValues = GetSingleton<FireSimConfigValues>();
            using var waterLocalToWorlds = m_WaterTagQuery.ToComponentDataArray<LocalToWorld>(Allocator.Temp);

            // NW: Do not iterate over every single team every single frame.
            // Use a deterministic EcsTick to ensure we only iterate over a subset.
            for (long i = 0; i < configValues.MaxTeamAssignmentsPerFrame; i++)
            {
                var ourTeamId = (int)(tick.CurrentTick * i % teamDatas.Length);
                ref var teamData = ref teamDatas.ElementAt(ourTeamId);

                var noKnownFireOrNoFireAtTargetFireCell = ! teamData.IsValid || teamData.TargetFireCell < 0 || teamData.TargetFireCell >= temperatures.Length || !fireSimConfigValues.IsOnFire(temperatures[teamData.TargetFireCell]);
                if (noKnownFireOrNoFireAtTargetFireCell)
                {
                    // NW: Only use the EXPENSIVE GetTeamPos method if we can't already infer it using our teamData.MiddlePos.
                    var averageTeamPos = GetTeamPos(ourTeamId);

                    var hasFoundFire = TryGetClosestFireTo(averageTeamPos, in configValues, in temperatures, out var closestFireCell);
                    if (hasFoundFire)
                    {
                        var closestFire = fireSimConfigValues.GetCellWorldPosition2D(closestFireCell);
                        var closestWater = GetClosestWaterTo(averageTeamPos, out var closestWaterDistance, waterLocalToWorlds);

                        //Debug.Log($"Team {ourTeamId} (average pos: {averageTeamPos}) has found a new closest fire position {closestFire} ({closestFireDistance:0.0}m) at cell {closestFireCell}, and closest water source {closestWater} ({closestWaterDistance:0.0}m)!");
                        //Debug.DrawLine(Utils.To3D(closestWater), Utils.To3D(closestFire), Color.red, 5f);
                        teamData.TargetFireCell = closestFireCell;
                        teamData.TargetWaterPos = closestWater;
                        teamData.TargetFirePos = closestFire;
                    }
                }
            }
        }

        float2 GetClosestWaterTo(float2 ourPos, out float closestDistance, NativeArray<LocalToWorld> waterLocalToWorlds)
        {
            using (s_WaterLookupMarker.Auto())
            {
                // NW: Hack: Just grab the middle of the water source entity. Don't care about the size of it!

                // NW: Hack: BIG assumption made here that the render bounds are the same as the square water tiles.
                //using var waterAABBs = m_WaterTagQuery.ToComponentDataArray<RenderBounds>(Allocator.Temp);

                var closestWaterSqrDistance = float.PositiveInfinity;
                var closestPoint = ourPos;
                closestDistance = float.PositiveInfinity;
                for (var i = 0; i < waterLocalToWorlds.Length; i++)
                {
                    var waterLocalToWorld = waterLocalToWorlds[i].Value;
                    var waterWorldPosition = waterLocalToWorld.c3;
                    var waterPosition2D = Utils.To2D(waterWorldPosition.xyz);

                    // var aabb = waterAABB[i];
                    // var aabbPoint = ClosestPointInAABB2D(aabb.Value, ourPos);
                    var distanceSqr = math.distancesq(waterPosition2D, ourPos);
                    if (distanceSqr < closestWaterSqrDistance)
                    {
                        closestWaterSqrDistance = distanceSqr;
                        closestPoint = waterPosition2D;
                        closestDistance = distanceSqr;
                    }
                }

                closestDistance = math.sqrt(closestDistance);
                return closestPoint;
            }
        }

        bool TryGetClosestFireTo(float2 ourPos, in FireSimConfigValues config, in NativeArray<Temperature> temperatures, out int closestFireCell)
        {
            using (s_FireLookupMarker.Auto())
            {
                using var result = new NativeArray<int>(1, Allocator.TempJob);
                new GetClosestFireCell
                {
                    config = config,
                    result = result,
                    temperatures = temperatures,
                    ourPos = ourPos,
                }.Run();
                closestFireCell = result[0];
                return closestFireCell >= 0;
            }
        }

        float2 GetTeamPos(int ourTeamId)
        {
            using (s_TeamPosLookupMarker.Auto())
            {
                using var workerPositions = m_WorkersQuery.ToComponentDataArrayAsync<Position>(Allocator.TempJob, out var workerPositionsHandle);
                using var workerTeams = m_WorkersQuery.ToComponentDataArrayAsync<TeamId>(Allocator.TempJob, out var workerTeamsHandle);
                using var result = new NativeArray<float2>(1, Allocator.TempJob);
                JobHandle.CombineDependencies(workerPositionsHandle, workerTeamsHandle).Complete();
                new GetTeamPosJob
                {
                    workerPositions = workerPositions,
                    workerTeams = workerTeams,
                    result = result,
                    ourTeamId = ourTeamId
                }.Run();
                return result[0];
            }
        }

        [BurstCompile]
        [NoAlias]
        struct GetTeamPosJob : IJob
        {
            [NoAlias]
            [ReadOnly]
            public NativeArray<TeamId> workerTeams;
            [NoAlias]
            [ReadOnly]
            public NativeArray<Position> workerPositions;
            [NoAlias]
            public NativeArray<float2> result;
            [ReadOnly]
            public int ourTeamId;

            public void Execute()
            {
                var teamPosition = new float2();
                var numMembersOfTeam = 0;
                for (var i = 0; i < workerTeams.Length; i++)
                {
                    var team = workerTeams[i];
                    if (ourTeamId == team.Id)
                    {
                        numMembersOfTeam++;
                        teamPosition += workerPositions[i].Value;
                    }
                }

                teamPosition /= math.max(1, numMembersOfTeam);
                result[0] = teamPosition;
            }
        } 
        
        [BurstCompile]
        [NoAlias]
        struct GetClosestFireCell : IJob
        {
            [NoAlias]
            [ReadOnly]
            public NativeArray<Temperature> temperatures;
            [NoAlias]
            public NativeArray<int> result;
            [ReadOnly]      [NoAlias]
            public FireSimConfigValues config;
            [ReadOnly]      [NoAlias]
            public float2 ourPos;

            public void Execute()
            {
                int closestFireCell = -1;
                var closestFireDistanceSqr = float.PositiveInfinity;
                for (var i = 0; i < temperatures.Length; i++)
                {
                    var temp = temperatures[i];
                    if (!config.IsOnFire(temp)) continue;

                    var firePos = config.GetCellWorldPosition2D(i);

                    var distanceSqr = math.distancesq(firePos, ourPos);
                    if (distanceSqr < closestFireDistanceSqr)
                    {
                        closestFireDistanceSqr = distanceSqr;
                        closestFireCell = i;
                    }
                }

                result[0] = closestFireCell;
            }
        }
    }
}
