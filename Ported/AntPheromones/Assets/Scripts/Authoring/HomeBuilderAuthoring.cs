using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HomeBuilderAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [SerializeField] private GameObject homePrefab = null;
    [Range(0.01F, 10.0F)] [SerializeField] private float homeRadius = 0.5F;
    [SerializeField] private Color homeColor = new Color(1, 0.5F, 0, 1);
    
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(homePrefab);
    }

    public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
    {
        manager.AddComponentData(entity,
            new HomeBuilder
            {
                homePrefab = conversionSystem.GetPrimaryEntity(homePrefab),
                homeRadius = homeRadius,
                homeColor = new float4(homeColor.r, homeColor.g, homeColor.b, homeColor.a),
            });
    }
}
