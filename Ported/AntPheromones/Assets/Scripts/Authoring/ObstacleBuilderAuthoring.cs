using System;

using UnityEngine;

using Unity.Entities;
using Unity.Mathematics;

public class ObstacleBuilderAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    const int min_dimension = 64;

    [SerializeField] int2 dimensions = new int2(128, 128);

    [SerializeField] protected internal GameObject obstaclePrefab = null;
    [Range(0.01F, 10.0F)] [SerializeField] protected internal float obstacleRadius = 0.5F;
    [SerializeField] protected internal Color obstacleColor = new Color(0.707F, 0.5F, 0.707F, 1);
    [Range(1, 10)] [SerializeField] protected internal int numberOfRings = 3;
    [Range(10, 45)] [SerializeField] protected internal float openingDegrees = 30;

    public void DeclareReferencedPrefabs(System.Collections.Generic.List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(obstaclePrefab);
    }

    public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
    {
        Debug.Assert(min_dimension <= dimensions.x && min_dimension <= dimensions.y, $"Minimum map dimensions {min_dimension}", this);

        manager.AddComponentData<ObstacleBuilder>(entity,
                                      new ObstacleBuilder
                                      {
                                          dimensions = dimensions,

                                          obstaclePrefab = conversionSystem.GetPrimaryEntity(obstaclePrefab),
                                          obstacleRadius = obstacleRadius,
                                          obstacleColor = new float4(obstacleColor.r, obstacleColor.g, obstacleColor.b, obstacleColor.a),
                                          
                                          numberOfRings = numberOfRings,
                                          openingDegrees = openingDegrees
                                      });
    }
}
