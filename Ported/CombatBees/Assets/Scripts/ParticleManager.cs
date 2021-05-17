using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour {
	public Mesh particleMesh;
	public Material particleMaterial;
	public float speedStretch;

	List<BeeParticle> particles;
	Matrix4x4[][] matrices;
	Vector4[][] colors;


	List<BeeParticle> pooledParticles;

	int activeBatch = 0;
	int activeBatchSize=0;

	static ParticleManager instance;

	const int instancesPerBatch = 1023;
	const int maxParticleCount = 10*instancesPerBatch;

	MaterialPropertyBlock matProps;

	public static void SpawnParticle(Vector3 position,ParticleType type,Vector3 velocity,float velocityJitter=6f,int count=1) {
		for (int i = 0; i < count; i++) {
			instance._SpawnParticle(position,type,velocity,velocityJitter);
		}
	}
	void _SpawnParticle(Vector3 position, ParticleType type, Vector3 velocity, float velocityJitter) {
		if (particles.Count==maxParticleCount) {
			return;
		}
		BeeParticle particle;
		if (pooledParticles.Count == 0) {
			particle = new BeeParticle();
		} else {
			particle = pooledParticles[pooledParticles.Count - 1];
			pooledParticles.RemoveAt(pooledParticles.Count - 1);

			particle.stuck = false;
		}
		particle.type = type;
		particle.position = position;
		particle.life = 1f;
		if (type==ParticleType.Blood) {
			particle.velocity = velocity+ Random.insideUnitSphere * velocityJitter;
			particle.lifeDuration = Random.Range(3f,5f);
			particle.size = Vector3.one*Random.Range(.1f,.2f);
			particle.color = Random.ColorHSV(-.05f,.05f,.75f,1f,.3f,.8f);
		} else if (type==ParticleType.SpawnFlash) {
			particle.velocity = Random.insideUnitSphere * 5f;
			particle.lifeDuration = Random.Range(.25f,.5f);
			particle.size = Vector3.one*Random.Range(1f,2f);
			particle.color = Color.white;
		}

		particles.Add(particle);

		if (activeBatchSize == instancesPerBatch) {
			activeBatch++;
			activeBatchSize = 0;
			if (matrices[activeBatch]==null) {
				matrices[activeBatch]=new Matrix4x4[instancesPerBatch];
				colors[activeBatch]=new Vector4[instancesPerBatch];
			}
		}
		activeBatchSize++;
	}

	private void Awake() {
		instance = this;

		particles = new List<BeeParticle>();
		pooledParticles = new List<BeeParticle>();
		matrices = new Matrix4x4[maxParticleCount/instancesPerBatch+1][];
		colors = new Vector4[maxParticleCount/instancesPerBatch+1][];

		matrices[0]=new Matrix4x4[instancesPerBatch];
		colors[0]=new Vector4[instancesPerBatch];
		activeBatch = 0;
		activeBatchSize = 0;

		matProps = new MaterialPropertyBlock();
		matProps.SetVectorArray("_Color",new Vector4[instancesPerBatch]);
	}
	
	void FixedUpdate () {
		float deltaTime = Time.deltaTime;
		for (int i=0;i<particles.Count;i++) {
			BeeParticle particle = particles[i];
			if (!particle.stuck) {
				particle.velocity += Vector3.up * (Field.gravity * deltaTime);
				particle.position += particle.velocity * deltaTime;
				
				if (System.Math.Abs(particle.position.x) > Field.size.x * .5f) {
					particle.position.x = Field.size.x * .5f * Mathf.Sign(particle.position.x);
					float splat = Mathf.Abs(particle.velocity.x*.3f) + 1f;
					particle.size.y *= splat;
					particle.size.z *= splat;
					particle.stuck = true;
				}
				if (System.Math.Abs(particle.position.y) > Field.size.y * .5f) {
					particle.position.y = Field.size.y * .5f * Mathf.Sign(particle.position.y);
					float splat = Mathf.Abs(particle.velocity.y * .3f) + 1f;
					particle.size.z *= splat;
					particle.size.x *= splat;
					particle.stuck = true;
				}
				if (System.Math.Abs(particle.position.z) > Field.size.z * .5f) {
					particle.position.z = Field.size.z * .5f * Mathf.Sign(particle.position.z);
					float splat = Mathf.Abs(particle.velocity.z * .3f) + 1f;
					particle.size.x *= splat;
					particle.size.y *= splat;
					particle.stuck = true;
				}

				if (particle.stuck) {
					particle.cachedMatrix = Matrix4x4.TRS(particle.position,Quaternion.identity,particle.size);
				}
			}

			particle.life -= deltaTime / particle.lifeDuration;
			if (particle.life < 0f) {
				activeBatchSize--;
				if (activeBatchSize==0 && activeBatch>0) {
					activeBatch--;
					activeBatchSize = instancesPerBatch;
				}

				pooledParticles.Add(particle);
				particles.RemoveAt(i);
				i--;
			}
		}
	}

	void Update() {
		for (int j = 0; j <= activeBatch; j++) {
			int batchSize = instancesPerBatch;
			if (j == activeBatch) {
				batchSize = activeBatchSize;
			}
			int batchOffset = j * instancesPerBatch;
			Matrix4x4[] batchMatrices = matrices[j];
			Vector4[] batchColors = colors[j];
			for (int i = 0; i < batchSize; i++) {
				BeeParticle particle = particles[i + batchOffset];

				if (particle.stuck) {
					batchMatrices[i] = particle.cachedMatrix;
				} else {
					Quaternion rotation = Quaternion.identity;
					Vector3 scale = particle.size * particle.life;
					if (particle.type == ParticleType.Blood) {
						rotation = Quaternion.LookRotation(particle.velocity);
						scale.z *= 1f + particle.velocity.magnitude * speedStretch;
					}
					batchMatrices[i] = Matrix4x4.TRS(particle.position,rotation,scale);
				}

				Color color = particle.color;
				color.a = particle.life;
				batchColors[i] = color;
			}
		}

		for (int i = 0; i <= activeBatch; i++) {
			int batchSize = instancesPerBatch;
			if (i==activeBatch) {
				batchSize = activeBatchSize;
			}
			if (batchSize > 0) {
				matProps.SetVectorArray("_Color",colors[i]);
				Graphics.DrawMeshInstanced(particleMesh,0,particleMaterial,matrices[i],batchSize,matProps);
			}
		}
	}
}
