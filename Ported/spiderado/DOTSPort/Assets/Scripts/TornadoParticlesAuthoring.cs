using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[DisallowMultipleComponent]
[RequiresEntityConversion]
public class TornadoParticlesAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [Range(0, 10000)] public int quantity;
    public Mesh particleMesh;
	public Material particleMaterial;
	public float spinRate;
	public float upwardSpeed;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        MaterialPropertyBlock matProps = new MaterialPropertyBlock();
        // Used for instance rendering, not yet implemented
        //matProps.SetVectorArray("_Color",colors);
        ParticleSharedData sharedData = new ParticleSharedData {
            particleMesh = particleMesh,
            particleMaterial = particleMaterial,
            spinRate = spinRate,
            upwardSpeed = upwardSpeed,
            matProps = matProps
        };

        for (int i = 0; i < quantity; i++) {
            Entity particleEntity = conversionSystem.DstEntityManager.CreateEntity();
			float3 pos = new float3(UnityEngine.Random.Range(-50f,50f), UnityEngine.Random.Range(0f,50f),UnityEngine.Random.Range(-50f,50f));
            Point pointData = new Point { pos=pos };
            PartData partData = new PartData {
                radiusMult = UnityEngine.Random.value,
                color =Color.white * UnityEngine.Random.Range(0.3f, 0.7f),
                matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one * UnityEngine.Random.Range(0.2f, 0.7f))
            };
            conversionSystem.DstEntityManager.AddComponentData(particleEntity, pointData);
            conversionSystem.DstEntityManager.AddComponentData(particleEntity, partData);
            conversionSystem.DstEntityManager.AddSharedComponentData(particleEntity, sharedData);
        }
    }
} 
