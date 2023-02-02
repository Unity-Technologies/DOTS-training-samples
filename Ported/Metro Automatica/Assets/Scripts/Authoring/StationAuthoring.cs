using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

class StationAuthoring : MonoBehaviour
{
    public Transform HumanSpawner;
    public List<Transform> _queuePoints1;
    public List<Transform> _queuePoints2;
    public List<Transform> _bridgePathWays1;
    public List<Transform> _bridgePathWays2;

    public class StationBaker : Baker<StationAuthoring>
    {
        public override void Bake(StationAuthoring authoring)
        {
            var spawnerTransform = WorldTransform.FromPosition(authoring.HumanSpawner.position);
            Station tempStation = new Station() { HumanSpawnerLocation = spawnerTransform
            };
            AddComponent<Station>(tempStation);
            
            DynamicBuffer<QueueWaypointCollection> tempQueueColl = AddBuffer<QueueWaypointCollection>();
            DynamicBuffer<BridgeWaypointCollection> tempBridgeColl = AddBuffer<BridgeWaypointCollection>();

            tempQueueColl.Length = authoring._queuePoints1.Count;
            for (int i = 0; i < authoring._queuePoints1.Count; i++)
            {
                tempQueueColl[i] = new QueueWaypointCollection
                    { North = authoring._queuePoints1[i].position, South = authoring._queuePoints2[i].position};
            }

            tempBridgeColl.Length = authoring._bridgePathWays1.Count;
            for (int i = 0; i < authoring._bridgePathWays1.Count; i++)
            {
                tempBridgeColl[i] = new BridgeWaypointCollection()
                    { West = authoring._bridgePathWays1[i].position, East = authoring._bridgePathWays2[i].position };
            }
        }
    }
}

struct Station : IComponentData
{
   // public Entity StationPrefab;
   public WorldTransform HumanSpawnerLocation;
   //public StationWaypoints StationWaypoints;

   // public NativeArray<float3> QueuePoints1;
   // public NativeArray<float3> QueuePoints2;
   // public NativeArray<float3> BridgePathWays1;
   // public NativeArray<float3> BridgePathWays2;
}

[InternalBufferCapacity(5)]
struct QueueWaypointCollection : IBufferElementData
{
    public float3 North;
    public float3 South;
}

[InternalBufferCapacity(4)]
struct BridgeWaypointCollection : IBufferElementData
{
    public float3 East;
    public float3 West;
}
