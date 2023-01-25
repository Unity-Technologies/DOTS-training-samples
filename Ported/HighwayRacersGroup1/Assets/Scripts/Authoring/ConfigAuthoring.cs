using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    public int CarCount;
    public GameObject SegmentPrefab;
    public GameObject CarPrefab;
    
    class ConfigBaker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            AddComponent(new Config()
            {
                CarCount = authoring.CarCount,
                SegmentPrefab =  GetEntity(authoring.SegmentPrefab),
                CarPrefab = GetEntity(authoring.CarPrefab)
            });
        }
    }
}


struct Config : IComponentData
{
    public int CarCount;
    public Entity SegmentPrefab;
    public Entity CarPrefab;
}