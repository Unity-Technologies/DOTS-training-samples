using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class AssignTeamsSystem : SystemBase
    {
        EntityQuery m_WorkersQuery;
        EntityQuery m_WaterTagQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<TeamData>();
            RequireSingletonForUpdate<Temperature>();
            RequireSingletonForUpdate<FireSimConfigValues>();
            
            m_WorkersQuery = GetEntityQuery(ComponentType.ReadOnly<Position>(), ComponentType.ReadOnly<TeamId>());
            m_WaterTagQuery = GetEntityQuery(ComponentType.ReadOnly<LocalToWorld>(), ComponentType.ReadOnly<WaterTag>());
        }

        protected override void OnUpdate()
        {
            if (m_WaterTagQuery.IsEmpty || m_WorkersQuery.IsEmpty)
                return;
            
            var teamContainerEntity = GetSingletonEntity<TeamData>();
            var teamDatas = EntityManager.GetBuffer<TeamData>(teamContainerEntity);
            
            var temperatureEntity = GetSingletonEntity<Temperature>();
            var temperatures = EntityManager.GetBuffer<Temperature>(temperatureEntity);
            
            for (var ourTeamId = 0; ourTeamId < teamDatas.Length; ourTeamId++)
            {
                ref var teamData = ref teamDatas.ElementAt(ourTeamId);
                var noKnownFireOrNoFireAtTargetFireCell = teamData.TargetFireCell < 0 || teamData.TargetFireCell >= temperatures.Length || ! temperatures[teamData.TargetFireCell].IsOnFire;
                if (noKnownFireOrNoFireAtTargetFireCell)
                {
                    var averageTeamPos = GetTeamPos(ourTeamId);
                    var hasFoundFire = TryGetClosestFireTo(averageTeamPos, temperatures, out int closestFireCell, out var closestFireDistance);

                    if (hasFoundFire)
                    {
                        var fireSimConfigValues = GetSingleton<FireSimConfigValues>();
                        var closestFire = fireSimConfigValues.GetCellWorldPosition2D(closestFireCell);
                        
                        var closestWater = GetClosestWaterTo(averageTeamPos, out var closestWaterDistance);
                        Debug.Log($"Team {ourTeamId} (average pos: {averageTeamPos}) has found a new closest fire position {closestFire} ({closestFireDistance:0.0}m) at cell {closestFireCell}, and closest water source {closestWater} ({closestWaterDistance:0.0}m)!");
                        Debug.DrawLine(Utils.To3D(closestWater), Utils.To3D(closestFire), Color.red, 5f);
                        teamData.TargetFireCell = closestFireCell;
                        teamData.TargetWaterPos = closestWater;
                        teamData.TargetFirePos = closestFire;
                    }
                }
            }
        }
        
        float2 GetClosestWaterTo(float2 ourPos, out float closestDistance)
        {
            // NW: Hack: Just grab the middle of the water source entity. Don't care about the size of it!
            using var waterLocalToWorlds = m_WaterTagQuery.ToComponentDataArray<LocalToWorld>(Allocator.Temp);
            
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
        
        bool TryGetClosestFireTo(float2 ourPos, DynamicBuffer<Temperature> temperatures, out int closestFireCell, out float closestFireDistance)
        {
            var config = GetSingleton<FireSimConfigValues>();

            closestFireCell = -1;
            var closestFireDistanceSqr = float.PositiveInfinity;
            for (var i = 0; i < temperatures.Length; i++)
            {
                var temp = temperatures[i];
                if (! temp.IsOnFire) continue;
                
                var firePos = config.GetCellWorldPosition2D(i);
                
                var distanceSqr = math.distancesq(firePos, ourPos);
                if (distanceSqr < closestFireDistanceSqr)
                {
                    closestFireDistanceSqr = distanceSqr;
                    closestFireDistance = distanceSqr;
                    closestFireCell = i;
                }
            }

            if (closestFireCell >= 0)
            {
                closestFireDistance = math.sqrt(closestFireDistanceSqr);
                return true;
            }
            closestFireDistance = float.PositiveInfinity;
            return false;
        }

        static float2 ClosestPointInAABB2D(AABB aabb, float2 ourPos)
        {
            var min = aabb.Min.xz;
            var max = aabb.Max.xz;
            return new float2(math.clamp(ourPos.x, min.x, max.x), math.clamp(ourPos.y, min.y, max.y));
        }

        float2 GetTeamPos(int ourTeamId)
        {
            using var workerPositions = m_WorkersQuery.ToComponentDataArray<Position>(Allocator.Temp);
            using var workerTeams = m_WorkersQuery.ToComponentDataArray<TeamId>(Allocator.Temp);

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
            return teamPosition;
        }
    }
}
