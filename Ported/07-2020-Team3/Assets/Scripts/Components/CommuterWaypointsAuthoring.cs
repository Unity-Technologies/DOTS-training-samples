using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public struct CommuterWaypoint : IBufferElementData
{
    public Entity Value;
}

public class CommuterWaypointsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject[] WaypointGameObjects;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        conversionSystem.DeclareDependency(this, GetComponent<CommuterAuthoring>());
        for (var i = 0; i < WaypointGameObjects.Length; ++i)
        {
            var waypointGameObject = WaypointGameObjects[i];
            conversionSystem.DeclareDependency(gameObject, waypointGameObject);
            if (!conversionSystem.HasPrimaryEntity(waypointGameObject))
                return;
        }

        var firstWaypoint = WaypointGameObjects[0];
        dstManager.SetComponentData(entity, new Commuter { Direction = (firstWaypoint.transform.position - transform.position).normalized });
        var waypointsBuffer = dstManager.AddBuffer<CommuterWaypoint>(entity);
        waypointsBuffer.Capacity = WaypointGameObjects.Length;
        for (var i = 0; i < WaypointGameObjects.Length; ++i)
        {
            var waypointGameObject = WaypointGameObjects[i];
            waypointsBuffer.Add(new CommuterWaypoint { Value = conversionSystem.GetPrimaryEntity(waypointGameObject) });
        }
    }
}
