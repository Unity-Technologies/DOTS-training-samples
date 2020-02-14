using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[DisallowMultipleComponent]
[RequiresEntityConversion]
public class PlatformAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Transform nav_FRONT_UP;
    public Transform nav_FRONT_DOWN;
    public Transform nav_BACK_UP;
    public Transform nav_BACK_DOWN;

    public Transform[] queuePoints;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PlatformNavPoints
        {
            backDown = nav_BACK_DOWN.position,
            backUp = nav_BACK_UP.position,
            frontDown = nav_FRONT_DOWN.position,
            frontUp = nav_FRONT_UP.position
        });
        
        var queueBuffer = dstManager.AddBuffer<QueuePosition>(entity);
        for (int i = 0; i < queuePoints.Length; i++)
        {
            var p = queuePoints[i].position;
            queueBuffer.Add(new float3(p.x, p.y, p.z));
        }
    }
}
