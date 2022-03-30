using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeManager : MonoBehaviour {
	public Mesh beeMesh;
	public Material beeMaterial;
	public Color[] teamColors;
	public float minBeeSize;
	public float maxBeeSize;
	public float speedStretch;
	public float rotationStiffness;
	[Space(10)]
	[Range(0f,1f)]
	public float aggression;
	public float flightJitter;
	public float teamAttraction;
	public float teamRepulsion;
	[Range(0f,1f)]
	public float damping;
	public float chaseForce;
	public float carryForce;
	public float grabDistance;
	public float attackDistance;
	public float attackForce;
	public float hitDistance;
	public float maxSpawnSpeed;
	[Space(10)]
	public int startBeeCount;

	List<Bee> bees;
	List<Bee>[] teamsOfBees;
	List<Bee> pooledBees;

	int activeBatch = 0;
	List<List<Matrix4x4>> beeMatrices;
	List<List<Vector4>> beeColors;

	static BeeManager instance;

	const int beesPerBatch=1023;
	MaterialPropertyBlock matProps;

	public static void SpawnBee(int team) {
		Vector3 pos = Vector3.right * (-Field.size.x * .4f + Field.size.x * .8f * team);
		instance._SpawnBee(pos,team);
	}

	public static void SpawnBee(Vector3 pos,int team) {
		instance._SpawnBee(pos,team);
	}
	void _SpawnBee(Vector3 pos, int team) {
		Bee bee;
		if (pooledBees.Count == 0) {
			bee = new Bee();
		} else {
			bee = pooledBees[pooledBees.Count-1];
			pooledBees.RemoveAt(pooledBees.Count - 1);
		}
		bee.Init(pos,team,Random.Range(minBeeSize,maxBeeSize));
		bee.velocity = Random.insideUnitSphere * maxSpawnSpeed;
		bees.Add(bee);
		teamsOfBees[team].Add(bee);
		if (beeMatrices[activeBatch].Count == beesPerBatch) {
			activeBatch++;
			if (beeMatrices.Count==activeBatch) {
				beeMatrices.Add(new List<Matrix4x4>());
				beeColors.Add(new List<Vector4>());
			}
		}
		beeMatrices[activeBatch].Add(Matrix4x4.identity);
		beeColors[activeBatch].Add(teamColors[team]);
	}
	void DeleteBee(Bee bee) {
		pooledBees.Add(bee);
		bees.Remove(bee);
		teamsOfBees[bee.team].Remove(bee);
		if (beeMatrices[activeBatch].Count == 0 && activeBatch>0) {
			activeBatch--;
		}
		beeMatrices[activeBatch].RemoveAt(beeMatrices[activeBatch].Count - 1);
		beeColors[activeBatch].RemoveAt(beeColors[activeBatch].Count - 1);
	}

	void Awake() {
		instance = this;
	}
	void Start () {
		bees = new List<Bee>(50000);
		teamsOfBees = new List<Bee>[2];
		pooledBees = new List<Bee>(50000);

		beeMatrices = new List<List<Matrix4x4>>();
		beeMatrices.Add(new List<Matrix4x4>());
		beeColors = new List<List<Vector4>>();
		beeColors.Add(new List<Vector4>());

		matProps = new MaterialPropertyBlock();

		for (int i=0;i<2;i++) {
			teamsOfBees[i] = new List<Bee>(25000);
		}
		for (int i=0;i<startBeeCount;i++) {
			int team = i%2;
			SpawnBee(team);
		}

		matProps = new MaterialPropertyBlock();
		matProps.SetVectorArray("_Color",new Vector4[beesPerBatch]);
	}

	void FixedUpdate() {
		float deltaTime = Time.fixedDeltaTime;

		for (int i = 0; i < bees.Count; i++) {
			Bee bee = bees[i];
			bee.isAttacking = false;
			bee.isHoldingResource = false;
			if (bee.dead == false) {
				bee.velocity += Random.insideUnitSphere * (flightJitter * deltaTime);
				bee.velocity *= (1f-damping);

				List<Bee> allies = teamsOfBees[bee.team];
				Bee attractiveFriend = allies[Random.Range(0,allies.Count)];
				Vector3 delta = attractiveFriend.position - bee.position;
				float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
				if (dist > 0f) {
					bee.velocity += delta * (teamAttraction * deltaTime / dist);
				}

				Bee repellentFriend = allies[Random.Range(0,allies.Count)];
				delta = attractiveFriend.position - bee.position;
				dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
				if (dist > 0f) {
					bee.velocity -= delta * (teamRepulsion * deltaTime / dist);
				}

				if (bee.enemyTarget == null && bee.resourceTarget == null) {
					if (Random.value < aggression) {
						List<Bee> enemyTeam = teamsOfBees[1 - bee.team];
						if (enemyTeam.Count > 0) {
							bee.enemyTarget = enemyTeam[Random.Range(0,enemyTeam.Count)];
						}
					} else {
						bee.resourceTarget = ResourceManager.TryGetRandomResource();
					}
				} else if (bee.enemyTarget != null) {
					if (bee.enemyTarget.dead) {
						bee.enemyTarget = null;
					} else {
						delta = bee.enemyTarget.position - bee.position;
						float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
						if (sqrDist > attackDistance * attackDistance) {
							bee.velocity += delta * (chaseForce * deltaTime / Mathf.Sqrt(sqrDist));
						} else {
							bee.isAttacking = true;
							bee.velocity += delta * (attackForce * deltaTime / Mathf.Sqrt(sqrDist));
							if (sqrDist < hitDistance * hitDistance) {
								ParticleManager.SpawnParticle(bee.enemyTarget.position,ParticleType.Blood,bee.velocity * .35f,2f,6);
								bee.enemyTarget.dead = true;
								bee.enemyTarget.velocity *= .5f;
								bee.enemyTarget = null;
							}
						}
					}
				} else if (bee.resourceTarget != null) {
					Resource resource = bee.resourceTarget;
					if (resource.holder == null) {
						if (resource.dead) {
							bee.resourceTarget = null;
						} else if (resource.stacked && ResourceManager.IsTopOfStack(resource) == false) {
							bee.resourceTarget = null;
						} else {
							delta = resource.position - bee.position;
							float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
							if (sqrDist > grabDistance * grabDistance) {
								bee.velocity += delta * (chaseForce * deltaTime / Mathf.Sqrt(sqrDist));
							} else if (resource.stacked) {
								ResourceManager.GrabResource(bee,resource);
							}
						}
					} else if (resource.holder == bee) {
						Vector3 targetPos = new Vector3(-Field.size.x * .45f + Field.size.x * .9f * bee.team,0f,bee.position.z);
						delta = targetPos - bee.position;
						dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
						bee.velocity += (targetPos - bee.position) * (carryForce * deltaTime / dist);
						if (dist < 1f) {
							resource.holder = null;
							bee.resourceTarget = null;
						} else {
							bee.isHoldingResource = true;
						}
					} else if (resource.holder.team != bee.team) {
						bee.enemyTarget = resource.holder;
					} else if (resource.holder.team == bee.team) {
						bee.resourceTarget = null;
					}
				}
			} else {
				if (Random.value<(bee.deathTimer-.5f)*.5f) {
					ParticleManager.SpawnParticle(bee.position,ParticleType.Blood,Vector3.zero);
				}

				bee.velocity.y += Field.gravity * deltaTime;
				bee.deathTimer -= deltaTime / 10f;
				if (bee.deathTimer < 0f) {
					DeleteBee(bee);
				}
			}
			bee.position += deltaTime * bee.velocity;

			
			if (System.Math.Abs(bee.position.x) > Field.size.x * .5f) {
				bee.position.x = (Field.size.x * .5f) * Mathf.Sign(bee.position.x);
				bee.velocity.x *= -.5f;
				bee.velocity.y *= .8f;
				bee.velocity.z *= .8f;
			}
			if (System.Math.Abs(bee.position.z) > Field.size.z * .5f) {
				bee.position.z = (Field.size.z * .5f) * Mathf.Sign(bee.position.z);
				bee.velocity.z *= -.5f;
				bee.velocity.x *= .8f;
				bee.velocity.y *= .8f;
			}
			float resourceModifier = 0f;
			if (bee.isHoldingResource) {
				resourceModifier = ResourceManager.instance.resourceSize;
			}
			if (System.Math.Abs(bee.position.y) > Field.size.y * .5f - resourceModifier) {
				bee.position.y = (Field.size.y * .5f - resourceModifier) * Mathf.Sign(bee.position.y);
				bee.velocity.y *= -.5f;
				bee.velocity.z *= .8f;
				bee.velocity.x *= .8f;
			}

			// only used for smooth rotation:
			Vector3 oldSmoothPos = bee.smoothPosition;
			if (bee.isAttacking == false) {
				bee.smoothPosition = Vector3.Lerp(bee.smoothPosition,bee.position,deltaTime * rotationStiffness);
			} else {
				bee.smoothPosition = bee.position;
			}
			bee.smoothDirection = bee.smoothPosition - oldSmoothPos;
		}
	}
	private void Update() {
		for (int i=0;i<bees.Count;i++) {
			float size = bees[i].size;
			Vector3 scale = new Vector3(size,size,size);
			if (bees[i].dead == false) {
				float stretch = Mathf.Max(1f,bees[i].velocity.magnitude * speedStretch);
				scale.z *= stretch;
				scale.x /= (stretch-1f)/5f+1f;
				scale.y /= (stretch-1f)/5f+1f;
			}
			Quaternion rotation = Quaternion.identity;
			if (bees[i].smoothDirection != Vector3.zero) {
				rotation=Quaternion.LookRotation(bees[i].smoothDirection);
			}
			Color color= teamColors[bees[i].team];
			if (bees[i].dead) {
				color *= .75f;
				scale *= Mathf.Sqrt(bees[i].deathTimer);
			}
			beeMatrices[i/beesPerBatch][i%beesPerBatch] = Matrix4x4.TRS(bees[i].position,rotation,scale);
			beeColors[i/beesPerBatch][i%beesPerBatch] = color;
		}
		for (int i = 0; i <= activeBatch; i++) {
			if (beeMatrices[i].Count > 0) {
				matProps.SetVectorArray("_Color",beeColors[i]);
				Graphics.DrawMeshInstanced(beeMesh,0,beeMaterial,beeMatrices[i],matProps);
			}
		}
	}
}
