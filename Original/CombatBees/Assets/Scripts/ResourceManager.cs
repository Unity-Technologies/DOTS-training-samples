using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {
	public Mesh resourceMesh;
	public Material resourceMaterial;
	public float resourceSize;
	public float snapStiffness;
	public float carryStiffness;
	public float spawnRate=.1f;
	public int beesPerResource;
	[Space(10)]
	public int startResourceCount;

	List<Resource> resources;
	List<Matrix4x4> matrices;
	Vector2Int gridCounts;
	Vector2 gridSize;
	Vector2 minGridPos;

	int[,] stackHeights;

	float spawnTimer = 0f;

	public static ResourceManager instance;

	//randomly chooses one resource 
	public static Resource TryGetRandomResource() {
		//check if there is any resource left
		if (instance.resources.Count==0) {
			return null;
		} else {
			
			Resource resource = instance.resources[Random.Range(0,instance.resources.Count)];
			int stackHeight = instance.stackHeights[resource.gridX,resource.gridY];
			//check if anyone is holding the resource and if it is the toppest resource or not (resources can be stacked on top of each other) 
			if (resource.holder == null || resource.stackIndex==stackHeight-1) {
				return resource;
			} else {
				return null;
			}
		}
	}
	//check if the resource is on the top or not 
	public static bool IsTopOfStack(Resource resource) {
		int stackHeight = instance.stackHeights[resource.gridX,resource.gridY];
		return resource.stackIndex == stackHeight - 1;
	}

	// again why multiply by 0.5 ? 
	Vector3 GetStackPos(int x, int y, int height) {
		return new Vector3(minGridPos.x+x*gridSize.x,-Field.size.y*.5f+(height+.5f)*resourceSize,minGridPos.y+y*gridSize.y);
	}

	Vector3 NearestSnappedPos(Vector3 pos) {
		int x, y;
		GetGridIndex(pos,out x,out y);
		return new Vector3(minGridPos.x + x * gridSize.x,pos.y,minGridPos.y + y * gridSize.y);
	}
	//converts the vector3 positions of resources to a custom grid system ? how does this affects in the scene  (only affects the initial spawn of the resources) 
	void GetGridIndex(Vector3 pos,out int gridX,out int gridY) {
		gridX=Mathf.FloorToInt((pos.x - minGridPos.x + gridSize.x * .5f) / gridSize.x);
		gridY=Mathf.FloorToInt((pos.z - minGridPos.y + gridSize.y * .5f) / gridSize.y);

		gridX = Mathf.Clamp(gridX,0,gridCounts.x - 1);
		gridY = Mathf.Clamp(gridY,0,gridCounts.y - 1);
	}

	// here the initial resources in the scene are spawned (the ones in the middle at the start and it only called once in the start()) 
	void SpawnResource() {
		Vector3 pos = new Vector3(minGridPos.x * .25f + Random.value * Field.size.x * .25f,Random.value * 10f,minGridPos.y + Random.value * Field.size.z);
		SpawnResource(pos);
	}
	// actual spawnresource function which is used when we spawn them by clicking 
	void SpawnResource(Vector3 pos) {
		Resource resource = new Resource(pos);

		resources.Add(resource);
		matrices.Add(Matrix4x4.identity);
	}
	void DeleteResource(Resource resource) {
		resource.dead = true;
		resources.Remove(resource);
		matrices.RemoveAt(matrices.Count - 1);
	}

	public static void GrabResource(Bee bee, Resource resource) {
		resource.holder = bee;
		resource.stacked = false;
		instance.stackHeights[resource.gridX,resource.gridY]--;
	}

	void Awake() {
		instance = this;
	}
	void Start () {
		resources = new List<Resource>();
		matrices = new List<Matrix4x4>();

		gridCounts = Vector2Int.RoundToInt(new Vector2(Field.size.x,Field.size.z) / resourceSize);
		gridSize = new Vector2(Field.size.x/gridCounts.x,Field.size.z/gridCounts.y);
		minGridPos = new Vector2((gridCounts.x-1f)*-.5f*gridSize.x,(gridCounts.y-1f)*-.5f*gridSize.y);
		stackHeights = new int[gridCounts.x,gridCounts.y];

		for (int i=0;i<startResourceCount;i++) {
			SpawnResource();
		}
	}

	void Update() {
		//spawns resources by clicking 
		if (resources.Count < 1000 && MouseRaycaster.isMouseTouchingField) {
			if (Input.GetKey(KeyCode.Mouse0)) {
				spawnTimer += Time.deltaTime;
				while (spawnTimer > 1f/spawnRate) {
					spawnTimer -= 1f/spawnRate;
					SpawnResource(MouseRaycaster.worldMousePosition);
				}
			}
		}

		for (int i=0;i<resources.Count;i++) {
			//why new variable? why not using resource[i] for more performance ? 
			Resource resource = resources[i];
			
			//if some alive bee is holding the resource
			if (resource.holder != null) {
				if (resource.holder.dead) {
					resource.holder = null;
				}
				else
				{
					Vector3 targetPos = resource.holder.position -
					                    Vector3.up * (resourceSize + resource.holder.size) * .5f;
					//updating the resource position based on the bee holding it 
					resource.position = Vector3.Lerp(resource.position, targetPos, carryStiffness * Time.deltaTime);
					resource.velocity = resource.holder.velocity;
				}
				//goes through the else if below for the resources that are falling (ones that bees drop in their base)? 
			} else if (resource.stacked == false) {
				resource.position = Vector3.Lerp(resource.position,NearestSnappedPos(resource.position),snapStiffness * Time.deltaTime);
				resource.velocity.y += Field.gravity * Time.deltaTime;
				resource.position += resource.velocity * Time.deltaTime;
				GetGridIndex(resource.position,out resource.gridX,out resource.gridY);
				float floorY = GetStackPos(resource.gridX,resource.gridY,stackHeights[resource.gridX,resource.gridY]).y;
				for (int j = 0; j < 3; j++) {
					if (System.Math.Abs(resource.position[j]) > Field.size[j] * .5f) {
						resource.position[j] = Field.size[j] * .5f * Mathf.Sign(resource.position[j]);
						resource.velocity[j] *= -.5f;
						resource.velocity[(j + 1) % 3] *= .8f;
						resource.velocity[(j + 2) % 3] *= .8f;
					}
				}
				if (resource.position.y < floorY) {
					resource.position.y = floorY;
					//if the resource reaches the ground we find the relative team and spawn new bees and flash particle 
					if (Mathf.Abs(resource.position.x) > Field.size.x * .4f) {
						int team = 0;
						if (resource.position.x > 0f) {
							team = 1;
						}
						for (int j = 0; j < beesPerResource; j++) {
							BeeManager.SpawnBee(resource.position,team);
						}
						ParticleManager.SpawnParticle(resource.position,ParticleType.SpawnFlash,Vector3.zero,6f,5);
						DeleteResource(resource);
					}//goes through this else, if the holder dies and the falling resource doesnt reach the base of either of bee teams  
					else {
						resource.stacked = true;
						resource.stackIndex = stackHeights[resource.gridX,resource.gridY];
						if ((resource.stackIndex + 1) * resourceSize < Field.size.y) {
							stackHeights[resource.gridX,resource.gridY]++;
						} else {
							DeleteResource(resource);
						}
						
					}
				}
			}
		}

		Vector3 scale = new Vector3(resourceSize,resourceSize * .5f,resourceSize);
		for (int i=0;i<resources.Count;i++) {
			matrices[i] = Matrix4x4.TRS(resources[i].position,Quaternion.identity,scale);
		}
		Graphics.DrawMeshInstanced(resourceMesh,0,resourceMaterial,matrices);
	}

	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(Vector3.zero,Field.size);
	}
}
