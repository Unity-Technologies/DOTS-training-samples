using System;

using UnityEngine;

using Unity.Entities;
using Unity.Mathematics;

public class MapAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    const int min_dimension = 64;

    [SerializeField] int2 dimensions = new int2(128, 128);

    [SerializeField] protected internal GameObject homePrefab = null;
    [Range(0.01F, 10.0F)] [SerializeField] protected internal float homeRadius = 0.5F;
    [SerializeField] protected internal Color homeColor = new Color(1, 0.5F, 0, 1);
    [SerializeField] protected internal GameObject foodPrefab = null;
    [Range(0.01F, 10.0F)] [SerializeField] protected internal float foodRadius = 0.5F;
    [SerializeField] protected internal Color foodColor = new Color(0, 0, 1, 1);
    [SerializeField] protected internal GameObject obstaclePrefab = null;
    [Range(0.01F, 10.0F)] [SerializeField] protected internal float obstacleRadius = 0.5F;
    [SerializeField] protected internal Color obstacleColor = new Color(0.707F, 0.5F, 0.707F, 1);
    [Range(1, 10)] [SerializeField] protected internal int numberOfRings = 3;
    [Range(10, 45)] [SerializeField] protected internal float openingDegrees = 30;

    public void DeclareReferencedPrefabs(System.Collections.Generic.List<GameObject> referencedPrefabs)
    {
        //Debug.Assert(null != homePrefab, $"{nameof(homePrefab)} cannot be null.", this);
        //Debug.Assert(null != foodPrefab, $"{nameof(foodPrefab)} cannot be null.", this);
        //Debug.Assert(null != obstaclePrefab, $"{nameof(obstaclePrefab)} cannot be null.", this);

        referencedPrefabs.Add(homePrefab);
        referencedPrefabs.Add(foodPrefab);
        referencedPrefabs.Add(obstaclePrefab);
    }

    public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
    {
        Debug.Assert(min_dimension <= dimensions.x && min_dimension <= dimensions.y, $"Minimum map dimensions {min_dimension}", this);

        manager.AddComponentData<Map>(entity,
                                      new Map
                                      {
                                          dimensions = dimensions,

                                          homePrefab = conversionSystem.GetPrimaryEntity(homePrefab),
                                          homeRadius = homeRadius,
                                          homeColor = new float4(homeColor.r, homeColor.g, homeColor.b, homeColor.a),

                                          foodPrefab = conversionSystem.GetPrimaryEntity(foodPrefab),
                                          foodRadius = foodRadius,
                                          foodColor = new float4(foodColor.r, foodColor.g, foodColor.b, foodColor.a),

                                          obstaclePrefab = conversionSystem.GetPrimaryEntity(obstaclePrefab),
                                          obstacleRadius = obstacleRadius,
                                          obstacleColor = new float4(obstacleColor.r, obstacleColor.g, obstacleColor.b, obstacleColor.a),
                                          
                                          numberOfRings = numberOfRings,
                                          openingDegrees = openingDegrees
                                      });

        manager.AddComponentData<MapBuilder>(entity, new MapBuilder());
    }
}
