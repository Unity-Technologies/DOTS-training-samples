using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(ConstraintsSystem))]
public partial class RenderPrepSystem : SystemBase
{
    private List<BeamBatch> m_BeamBatches;
    private NativeArray<Entity>[] m_WorldEntititesCache = null;
    private DynamicBuffer<CurrentPoint>[] m_CurrentPointsCache;

    protected override void OnCreate()
    {
        m_BeamBatches = new List<BeamBatch>();
    }
    
    protected override void OnUpdate()
    {
        if (m_BeamBatches.Count == 0)
        {
            EntityManager.GetAllUniqueSharedComponentData(m_BeamBatches);
        }
        var worldQuery = GetEntityQuery(typeof(World), typeof(BeamBatch));
        
        if(m_WorldEntititesCache == null && m_BeamBatches.Count > 0) {
            m_WorldEntititesCache = new NativeArray<Entity>[m_BeamBatches.Count];
            m_CurrentPointsCache = new DynamicBuffer<CurrentPoint>[m_BeamBatches.Count];
            for (var i = 0; i < m_BeamBatches.Count; i++)
            {
                worldQuery.SetSharedComponentFilter(m_BeamBatches[i]);

                m_WorldEntititesCache[i] = worldQuery.ToEntityArray(Allocator.Persistent);
				
                var worldEntity = m_WorldEntititesCache[i][0];
                m_CurrentPointsCache[i] = GetBuffer<CurrentPoint>(worldEntity);
            }
        }


        for (var i = 0; i < m_BeamBatches.Count; i++) {
			
            var currentPointBuffer = m_CurrentPointsCache[i];
            
            Entities
                .WithReadOnly(currentPointBuffer)
                .WithNativeDisableContainerSafetyRestriction(currentPointBuffer)
                .WithSharedComponentFilter(m_BeamBatches[i])
                .ForEach((Entity entity, ref Beam beam, ref LocalToWorld localToWorld) =>
                {
                    //TODO: cache positions in beams?
                    var pointA = currentPointBuffer[beam.pointAIndex];
                    var pointB = currentPointBuffer[beam.pointBIndex];

                    //TODO: separate from random access generation 
                    var direction = math.normalize(pointB.Value - pointA.Value);
                    localToWorld.Value = float4x4.TRS(pointB.Value + (pointA.Value - pointB.Value) / 2f,
                        quaternion.LookRotation(direction, new float3(0f, 1f, 0f)), new float3(.25f, .25f, beam.size));


                }).ScheduleParallel();
        }

        //TODO: complete in the optimal place
        Dependency.Complete();


    }
}
