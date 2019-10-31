using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// Necessary ?/**/

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ParticleAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    private static uint currentSeed = 777;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Unity.Mathematics.Random random = new Unity.Mathematics.Random(currentSeed);

        currentSeed++;
        
        var data = new ParticleComponent();
        dstManager.AddComponentData(entity, data);
    }
}