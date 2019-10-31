using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TornadoSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{	
	public float Height = 50.0f;
	public float Force = 5.0f;
	public float MaxForceDistance = 10f;
	public float UpForce = 2f;
	public float InwardForce = 5f;
	public float Damping = 0.99f;

	public GameObject Particle;
	public int NumberOfParticle = 4000;
	public float ParticleSpinRate = 37.0f;
	public float ParticleUpSpeed = 12.0f;
	public float BreakResistance = 5.0f;	
	
	public float Friction = 0.1f;
	public float Gravity = 5.0f;

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
		tornado.maxForceDist = MaxForceDistance;
		tornado.upForce = UpForce;
		tornado.friction = Friction;
		tornado.gravity = Gravity;
		tornado.inwardForce = InwardForce;
		tornado.damping = Damping;

		dstManager.AddComponentData<TornadoSpawner>(entity, tornado);
	}
}
