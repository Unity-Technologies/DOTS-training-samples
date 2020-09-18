using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using Unity.Physics;

public class CullPhysics : SystemBase
{
    List<SharedWorldBounds> boundsList = new List<SharedWorldBounds>();
    public struct CullPhysicsJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<SharedWorldBounds> InputBoundsArray;
        public NativeList<SharedWorldBounds>.ParallelWriter OutputBoundsList;
        [ReadOnly] public NativeArray<float3> TornadoPositions;
        [ReadOnly] public NativeArray<float> TornadoBreakDistances;

        public void Execute(int index)
        {
            float3 inputPosition = InputBoundsArray[index].center;

            for (int i = 0; i < TornadoPositions.Length; i++)
            {
                if (math.length(inputPosition - TornadoPositions[i]) < TornadoBreakDistances[i]) // TornadoForceData[i].tornadoBreakDist)
                {
                    OutputBoundsList.AddNoResize(InputBoundsArray[index]);
                    break;
                }
            }
        }
    }

    protected override void OnUpdate()
    {
        //Gather tornado data
        EntityQuery tornadoQuery = GetEntityQuery(typeof(TornadoForceData));
        NativeArray<Entity> tornadoEntities = tornadoQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<float3> tornadoPositions = new NativeArray<float3>(tornadoEntities.Length, Allocator.TempJob);
        NativeArray<float> tornadoBreakDistances = new NativeArray<float>(tornadoEntities.Length, Allocator.TempJob);
        for (int i = 0; i < tornadoEntities.Length; i++)
        {
            tornadoPositions[i] = EntityManager.GetComponentData<Translation>(tornadoEntities[i]).Value;
            tornadoBreakDistances[i] = EntityManager.GetComponentData<TornadoForceData>(tornadoEntities[i]).tornadoBreakDist;
        }

        //Gather all shared world bounds
        boundsList.Clear();
        EntityManager.GetAllUniqueSharedComponentData<SharedWorldBounds>(boundsList);
        NativeArray<SharedWorldBounds> boundsArray = boundsList.ToNativeArray(Allocator.TempJob);
        NativeList<SharedWorldBounds> removalArray = new NativeList<SharedWorldBounds>(boundsArray.Length, Allocator.TempJob);

        //Find shared world bounds which can be activated
        Dependency = new CullPhysicsJob
        {
            InputBoundsArray = boundsArray,
            OutputBoundsList = removalArray.AsParallelWriter(),
            TornadoPositions = tornadoPositions,
            TornadoBreakDistances = tornadoBreakDistances
        }.Schedule(boundsArray.Length, 64, Dependency);

        Dependency.Complete();

        //Activate entities
        for (int i = 0; i < removalArray.Length; i++)
        {
            EntityQuery query = GetEntityQuery(typeof(SharedWorldBounds));
            query.SetSharedComponentFilter<SharedWorldBounds>(removalArray[i]);
            EntityManager.RemoveComponent(query, new ComponentTypes(typeof(PhysicsExclude), typeof(SharedWorldBounds)));
        }

        boundsArray.Dispose();
        removalArray.Dispose();
        tornadoEntities.Dispose();
        tornadoPositions.Dispose();
        tornadoBreakDistances.Dispose();
    }
}
