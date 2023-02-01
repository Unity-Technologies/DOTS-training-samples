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
            Station tempStation = new Station() { HumanSpawnerLocation = spawnerTransform };
            AddComponent<Station>(tempStation);
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
   public StationWaypoints StationWaypoints;
   
}