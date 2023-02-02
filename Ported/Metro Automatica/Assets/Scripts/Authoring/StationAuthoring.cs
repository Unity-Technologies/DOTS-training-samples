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
                // QueuePoints1 = new NativeArray<float3>(authoring._queuePoints1.Count, Allocator.Temp),
                // QueuePoints2 = new NativeArray<float3>(authoring._queuePoints2.Count, Allocator.Temp),
                // BridgePathWays1 = new NativeArray<float3>(authoring._bridgePathWays1.Count, Allocator.Temp),
                // BridgePathWays2 = new NativeArray<float3>(authoring._bridgePathWays2.Count, Allocator.Temp),
            };
            AddComponent<Station>(tempStation);
            AddBuffer<QueueWaypointCollection>();
            AddBuffer<BridgeWaypointCollection>();

            DynamicBuffer<QueueWaypointCollection> tempQueueColl = SetBuffer<QueueWaypointCollection>();
            DynamicBuffer<BridgeWaypointCollection> tempBridgeColl = SetBuffer<BridgeWaypointCollection>();
            
            
            for (int i = 0; i < authoring._queuePoints1.Count; i++)
            {
                tempQueueColl.Insert(i,new QueueWaypointCollection
                    { North = authoring._queuePoints1[i].position, South = authoring._queuePoints2[i].position });

                // var tt = tempQueueColl[i];
                // tt.North = authoring._queuePoints1[i].position;
                // tt.South = authoring._queuePoints2[i].position;

            }
            for (int i = 0; i < authoring._bridgePathWays1.Count; i++)
            {
                tempBridgeColl.Insert(i,new BridgeWaypointCollection()
                    { West = authoring._bridgePathWays1[i].position, East = authoring._bridgePathWays2[i].position });
                
                // var tt = tempBridgeColl[i];
                // tt.West = authoring._bridgePathWays1[i].position;
                // tt.East = authoring._bridgePathWays2[i].position;
            }
            
           
            /*StationWaypoints tempWaypoints = new StationWaypoints()
            {
                BridgePathWays1 = authoring._bridgePathWays1,
                BridgePathWays2 = authoring._bridgePathWays2, 
                QueuePoints1 = authoring._queuePoints1,
                QueuePoints2 = authoring._queuePoints2
                
                //Convert here to use float3 instead of Transform
            };
            };*/
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
