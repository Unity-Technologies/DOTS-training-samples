using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class LineAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    // Add fields to your component here. Remember that:
    //
    // * The purpose of this class is to store data for authoring purposes - it is not for use while the game is
    //   running.
    // 
    // * Traditional Unity serialization rules apply: fields must be public or marked with [SerializeField], and
    //   must be one of the supported types.
    //
    // For example,
    //    public float scale;

    const float k_StepSize = 0.1f;

    Entity m_Entity;
    EntityManager m_EntityManager;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Call methods on 'dstManager' to create runtime components on 'entity' here. Remember that:
        //
        // * You can add more than one component to the entity. It's also OK to not add any at all.
        //
        // * If you want to create more than one entity from the data in this class, use the 'conversionSystem'
        //   to do it, instead of adding entities through 'dstManager' directly.
        //
        // For example,
        //   dstManager.AddComponentData(entity, new Unity.Transforms.Scale { Value = scale });

        var buffer = dstManager.AddBuffer<LinePositionBufferElement>(entity);
        var currentWaypointIdx = 1;
        float3 currentPosition = transform.GetChild(0).position;
        buffer.Add(currentPosition);
        while (currentWaypointIdx < transform.childCount)
        {
            var nextWaypoint = transform.GetChild(currentWaypointIdx);
            var diff = (float3)nextWaypoint.position - currentPosition;
            while(math.length(diff) < k_StepSize)
            {
                if (++currentWaypointIdx < transform.childCount)
                {
                    nextWaypoint = transform.GetChild(currentWaypointIdx);
                    diff = (float3)nextWaypoint.position - currentPosition;
                }
                else
                    return;
            }
            var step = math.normalize(diff) * k_StepSize;
            currentPosition += step;
            buffer.Add(currentPosition);
        }
        dstManager.AddComponentData(entity, new Line{ });
        m_Entity = entity;
        m_EntityManager = dstManager;
    }

    //void Update()
    //{
    //    m_EntityManager = World.Active.EntityManager;
    //    if (m_EntityManager != null)
    //    {
    //        m_Entity = m_EntityManager.CreateEntityQuery(typeof(Line)).ToEntityArray(Unity.Collections.Allocator.TempJob)[0];
    //        var buffer = m_EntityManager.GetBuffer<LinePositionBufferElement>(m_Entity);
    //        for (int i = 0; i <  buffer.Length-1; i++)
    //        {
    //            Debug.DrawLine(buffer[i].Value, buffer[i + 1].Value,Color.red);
    //        }
    //    }
    //}
}
