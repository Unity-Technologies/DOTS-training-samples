using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TornadoSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{	
	public float Height = 10.0f;
	public float Force = 5.0f;
	public GameObject Particle;
	public int NumberOfParticle = 100;
	public float ParticleSpinRate = 1.0f;
	public float ParticleUpSpeed = 2.0f;
	public float BreakResistance = 5.0f;

	public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
	{
		referencedPrefabs.Add(Particle);
	}

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		TornadoSpawner tornado = new TornadoSpawner();
		tornado.particle = conversionSystem.GetPrimaryEntity(Particle);
		tornado.height = Height;
		tornado.force = Force;
		tornado.numOfParticles = NumberOfParticle;
		tornado.particleSpinRate = ParticleSpinRate;
		tornado.particleUpSpeed = ParticleUpSpeed;
		tornado.breakResist = BreakResistance;

		dstManager.AddComponentData<TornadoSpawner>(entity, tornado);
	}
}
