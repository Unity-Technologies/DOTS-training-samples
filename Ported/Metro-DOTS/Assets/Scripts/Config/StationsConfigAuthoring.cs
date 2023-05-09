using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;


public class StationsConfigAuthoring : MonoBehaviour
{
    public GameObject StationPrefab;
    public GameObject TrackPrefab;
    public int NumStations = 10;
    public float Spacing;
    public float3 TrackACenter;
    public float3 TrackBCenter;

    class Baker : Baker<StationsConfigAuthoring>
    {
        public override void Bake(StationsConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new StationConfig
            {
                StationEntity = GetEntity(authoring.StationPrefab, TransformUsageFlags.Dynamic),
                TrackEntity = GetEntity(authoring.TrackPrefab, TransformUsageFlags.Dynamic),
                Spacing = authoring.Spacing,
                NumStations = authoring.NumStations,
                TrackACenter = authoring.TrackACenter,
                TrackBCenter = authoring.TrackBCenter
            });
        }
    }
}

public struct StationConfig : IComponentData
{
    public Entity StationEntity;
    public Entity TrackEntity;
    public float Spacing;
    public int NumStations;
    public float3 TrackACenter;
    public float3 TrackBCenter;
}
