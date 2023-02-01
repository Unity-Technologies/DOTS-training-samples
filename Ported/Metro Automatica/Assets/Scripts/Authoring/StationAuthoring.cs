using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

class StationAuthoring : MonoBehaviour
{
    public Transform HumanSpawner;
    
    public class StationBaker : Baker<StationAuthoring>
    {
        public override void Bake(StationAuthoring authoring)
        {
            var spawnerTransform = WorldTransform.FromPosition(authoring.HumanSpawner.position);
            Station tempStation = new Station() { HumanSpawnerLocation = spawnerTransform };
            AddComponent<Station>(tempStation);
        }
    }
}

struct Station : IComponentData
{
   // public Entity StationPrefab;
   public WorldTransform HumanSpawnerLocation;
}