using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FireSimConfigAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject m_FirePrefab;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        for (int x=0; x<FireSimConfig.xDim; x++)
        {
            for (int y = 0; y < FireSimConfig.yDim; y++)
            {
                int2 coord = new int2(x, y); 
                Entity cellEntity = conversionSystem.GetPrimaryEntity(m_FirePrefab); // instantiation
                dstManager.AddComponentData(cellEntity, 
                    new FireCell
                    {
                        coord=coord
                    });
            }
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(m_FirePrefab);
    }
}
