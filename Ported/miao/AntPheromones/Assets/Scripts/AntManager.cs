using System;
using System.Collections;
using System.Collections.Generic;
using AntPheromones_ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Serialization;
using Position = AntPheromones_ECS.Position;
using Random = UnityEngine.Random;
using Unity.Rendering;

public class AntManager : MonoBehaviour {
	public static AntManager Instance { get; private set; }
	
	public Material basePheromoneMaterial;
	public Renderer pheromoneRenderer;
	public Material antMaterial;
	public Material obstacleMaterial;
	public Material resourceMaterial;
	public Material colonyMaterial;
	public Mesh antMesh;
	public Mesh obstacleMesh;
	public Mesh colonyMesh;
	public Mesh resourceMesh;
	public Color searchColor;
	public Color carryColor;
	public int antCount;
//	public int MapWidth = 128;
	public int bucketResolution;
	public Vector3 antSize;
	public float antSpeed;
	[Range(0f,1f)]
	public float antAccel;
	public float trailAddSpeed;
	[Range(0f,1f)]
	public float trailDecay;
	public float randomSteering;
	public float pheromoneSteerStrength;
	public float wallSteerStrength;
	public float goalSteerStrength;
	public float outwardStrength;
	public float inwardStrength;
	public int rotationResolution = 360;
	public int obstacleRingCount;
	[Range(0f,1f)]
	public float obstaclesPerRing;
	public float obstacleRadius;

	Texture2D pheromoneTexture;
	Material myPheromoneMaterial;

//	[FormerlySerializedAs("pheromones")] 
	[FormerlySerializedAs("PheromoneColours")] public Color[] pheromoneColours;
	Ant[] ants;
	Matrix4x4[][] matrices;
	Vector4[][] antColors;
	MaterialPropertyBlock[] matProps;
	Obstacle_Old[] obstacles;
	Matrix4x4[][] obstacleMatrices;
	Obstacle_Old[,][] obstacleBuckets;

	Matrix4x4 resourceMatrix;
	Matrix4x4 colonyMatrix;

	Vector2 resourcePosition;
//	Vector2 colonyPosition;
	float3 colonyPosition;

	const int instancesPerBatch = 1023;

	Matrix4x4[] rotationMatrixLookup;
	private NativeArray<Entity> antEntities;

	void Start()
	{
//		GenerateObstacles();

		this.colonyPosition = new float3(1f, 1f, 0f) * Map.Width * 0.5f;
		this.colonyMatrix = float4x4.TRS(this.colonyPosition / Map.Width, Quaternion.identity,
			new float3(4f, 4f, 0.1f) / Map.Width);

//		this.colonyPosition = 0.5f * Map.Width * Vector2.one;
//		this.colonyMatrix = Matrix4x4.TRS(colonyPosition/MapWidth,Quaternion.identity,new Vector3(4f,4f,.1f)/MapWidth);

		float resourceAngle = Random.value * 2f * Mathf.PI;
		this.resourcePosition = 0.5f * Map.Width * new float2(1f, 1f) +
		                        new float2(Mathf.Cos(resourceAngle) * Map.Width * 0.475f,
			                        Mathf.Sin(resourceAngle) * Map.Width * 0.475f);
		this.resourceMatrix = Matrix4x4.TRS(resourcePosition / Map.Width, Quaternion.identity,
			new float3(4f, 4f, 0.1f) / Map.Width);

		this.pheromoneTexture = new Texture2D(Map.Width, Map.Width) {wrapMode = TextureWrapMode.Mirror};
		this.pheromoneColours = new Color[Map.Width * Map.Width];

		this.pheromoneRenderer.sharedMaterial = new Material(basePheromoneMaterial)
			{mainTexture = this.pheromoneTexture};

		var entityManager = World.Active.EntityManager;
		EntityArchetype antArchetype =
			entityManager.CreateArchetype(
				typeof(Position),
				typeof(FacingAngle),
				typeof(Speed),
				typeof(ColourDisplay),
				typeof(Brightness),
				typeof(ResourceCarrier),
				typeof(RenderMesh),
				typeof(LocalToWorld),
				typeof(Ant));

		this.antEntities = new NativeArray<Entity>(length: 1000, Allocator.Persistent);
		entityManager.CreateEntity(antArchetype, this.antEntities);

		foreach (var entity in this.antEntities)
		{
			entityManager.SetComponentData(
				entity,
				new Position
				{
					Value = new float2(Random.Range(-5f, 5f) + Map.Width * 0.5f,
						Random.Range(-5f, 5f) + Map.Width * .5f)
				});
			entityManager.SetSharedComponentData(entity, new RenderMesh
			{
				mesh = this.antMesh,
				material = this.antMaterial
			});
		}

		Entity pheromoneColourMapEntity = entityManager.CreateEntity(typeof(PheromoneColourMap));
		entityManager.AddBuffer<PheromoneColour>(pheromoneColourMapEntity);
		
//
//		DropPheromoneSystem dropPheromoneSystem = World.Active.GetOrCreateSystem<DropPheromoneSystem>();
//
////		Entity map = entityManager.CreateEntity(typeof(Map));
////		entityManager.AddBuffer<Color>(map);
//		
//		ants = new Ant[antCount];
//		matrices = new Matrix4x4[Mathf.CeilToInt((float)antCount / instancesPerBatch)][];
//		for (int i=0;i<matrices.Length;i++) {
//			if (i<matrices.Length-1) {
//				matrices[i] = new Matrix4x4[instancesPerBatch];
//			} else {
//				matrices[i] = new Matrix4x4[antCount - i * instancesPerBatch];
//			}
//		}
//		matProps = new MaterialPropertyBlock[matrices.Length];
//		antColors = new Vector4[matrices.Length][];
//		for (int i=0;i<matProps.Length;i++) {
//			antColors[i] = new Vector4[matrices[i].Length];
//			matProps[i] = new MaterialPropertyBlock();
//		}
//
//		for (int i = 0; i < antCount; i++) {
//			ants[i] = new Ant(new Vector2(Random.Range(-5f,5f)+Map.Width*.5f,Random.Range(-5f,5f) + Map.Width * .5f));
//		}
//
//		rotationMatrixLookup = new Matrix4x4[rotationResolution];
//		for (int i=0;i<rotationResolution;i++) {
//			float angle = (float)i / rotationResolution;
//			angle *= 360f;
//			rotationMatrixLookup[i] = Matrix4x4.TRS(Vector3.zero,Quaternion.Euler(0f,0f,angle),antSize);
//		}
	}
	
	Matrix4x4 GetRotationMatrix(float angle) {
		angle /= Mathf.PI * 2f;
		angle -= Mathf.Floor(angle);
		angle *= rotationResolution;
		return rotationMatrixLookup[((int)angle)%rotationResolution];
	}

//	int PheromoneIndex(int x, int y) {
//		return x + y * MapWidth;
//	}

//	void DropPheromones(Vector2 position,float strength) {
//		int x = Mathf.FloorToInt(position.x);
//		int y = Mathf.FloorToInt(position.y);
//		if (x < 0 || y < 0 || x >= MapWidth || y >= MapWidth) {
//			return;
//		}
//
//		int index = PheromoneIndex(x,y);
//		pheromoneColours[index].r += (trailAddSpeed*strength*Time.fixedDeltaTime)*(1f-pheromoneColours[index].r);
//		if (pheromoneColours[index].r>1f) {
//			pheromoneColours[index].r = 1f;
//		}
//	}
//
//	float PheromoneSteering(Ant ant,float distance) {
//		float output = 0;
//
//		for (int i=-1;i<=1;i+=2) {
//			float angle = ant.facingAngle + i * Mathf.PI*.25f;
//			float testX = ant.position.x + Mathf.Cos(angle) * distance;
//			float testY = ant.position.y + Mathf.Sin(angle) * distance;
//
//			if (testX <0 || testY<0 || testX>=MapWidth || testY>=MapWidth) {
//
//			} else {
//				int index = PheromoneIndex((int)testX,(int)testY);
//				float value = pheromoneColours[index].r;
//				output += value*i;
//			}
//		}
//		return Mathf.Sign(output);
//	}
//
//	int WallSteering(Ant ant,float distance) {
//		int output = 0;
//
//		for (int i = -1; i <= 1; i+=2) {
//			float angle = ant.facingAngle + i * Mathf.PI*.25f;
//			float testX = ant.position.x + Mathf.Cos(angle) * distance;
//			float testY = ant.position.y + Mathf.Sin(angle) * distance;
//
//			if (testX < 0 || testY < 0 || testX >= MapWidth || testY >= MapWidth) {
//
//			} else {
//				int value = GetObstacleBucket(testX,testY).Length;
//				if (value > 0) {
//					output -= i;
//				}
//			}
//		}
//		return output;
//	}
//
//	bool Linecast(Vector2 point1, Vector2 point2) {
//		float dx = point2.x - point1.x;
//		float dy = point2.y - point1.y;
//		float dist = Mathf.Sqrt(dx * dx + dy * dy);
//
//		int stepCount = Mathf.CeilToInt(dist*.5f);
//		for (int i=0;i<stepCount;i++) {
//			float t = (float)i / stepCount;
//			if (GetObstacleBucket(point1.x+dx*t,point1.y+dy*t).Length>0) {
//				return true;
//			}
//		}
//
//		return false;
//	}
//
//	void GenerateObstacles() {
//		List<Obstacle_Old> output = new List<Obstacle_Old>();
//		for (int i=1;i<=obstacleRingCount;i++) {
//			float ringRadius = (i / (obstacleRingCount+1f)) * (MapWidth * .5f);
//			float circumference = ringRadius * 2f * Mathf.PI;
//			int maxCount = Mathf.CeilToInt(circumference / (2f * obstacleRadius) * 2f);
//			int offset = Random.Range(0,maxCount);
//			int holeCount = Random.Range(1,3);
//			for (int j=0;j<maxCount;j++) {
//				float t = (float)j / maxCount;
//				if ((t * holeCount)%1f < obstaclesPerRing) {
//					float angle = (j + offset) / (float)maxCount * (2f * Mathf.PI);
//					Obstacle_Old obstacle = new Obstacle_Old();
//					obstacle.Position = new Vector2(MapWidth * .5f + Mathf.Cos(angle) * ringRadius,MapWidth * .5f + Mathf.Sin(angle) * ringRadius);
//					obstacle.Radius = obstacleRadius;
//					output.Add(obstacle);
//					//Debug.DrawRay(obstacle.position / mapSize,-Vector3.forward * .05f,Color.green,10000f);
//				}
//			}
//		 }
//
//		obstacleMatrices = new Matrix4x4[Mathf.CeilToInt((float)output.Count / instancesPerBatch)][];
//		for (int i=0;i<obstacleMatrices.Length;i++) {
//			obstacleMatrices[i] = new Matrix4x4[Mathf.Min(instancesPerBatch,output.Count - i * instancesPerBatch)];
//			for (int j=0;j<obstacleMatrices[i].Length;j++) {
//				obstacleMatrices[i][j] = Matrix4x4.TRS(output[i * instancesPerBatch + j].Position / MapWidth,Quaternion.identity,new Vector3(obstacleRadius*2f,obstacleRadius*2f,1f)/MapWidth);
//			}
//		}
//
//		obstacles = output.ToArray();
//
//		List<Obstacle_Old>[,] tempObstacleBuckets = new List<Obstacle_Old>[bucketResolution,bucketResolution];
//
//		for (int x = 0; x < bucketResolution; x++) {
//			for (int y = 0; y < bucketResolution; y++) {
//				tempObstacleBuckets[x,y] = new List<Obstacle_Old>();
//			}
//		}
//
//		for (int i = 0; i < obstacles.Length; i++) {
//			Vector2 pos = obstacles[i].Position;
//			float radius = obstacles[i].Radius;
//			for (int x = Mathf.FloorToInt((pos.x - radius)/MapWidth*bucketResolution); x <= Mathf.FloorToInt((pos.x + radius)/MapWidth*bucketResolution); x++) {
//				if (x < 0 || x >= bucketResolution) {
//					continue;
//				}
//				for (int y = Mathf.FloorToInt((pos.y - radius) / MapWidth * bucketResolution); y <= Mathf.FloorToInt((pos.y + radius) / MapWidth * bucketResolution); y++) {
//					if (y<0 || y>=bucketResolution) {
//						continue;
//					}
//					tempObstacleBuckets[x,y].Add(obstacles[i]);
//				}
//			}
//		}
//
//		obstacleBuckets = new Obstacle_Old[bucketResolution,bucketResolution][];
//		for (int x = 0; x < bucketResolution; x++) {
//			for (int y = 0; y < bucketResolution; y++) {
//				obstacleBuckets[x,y] = tempObstacleBuckets[x,y].ToArray();
//			}
//		}
//	}
//
//	Obstacle_Old[] emptyBucket = new Obstacle_Old[0];
//	private NativeArray<Entity> antEntities;
//
//	Obstacle_Old[] GetObstacleBucket(Vector2 pos) {
//		return GetObstacleBucket(pos.x,pos.y);
//	}
//	Obstacle_Old[] GetObstacleBucket(float posX, float posY) {
//		int x = (int)(posX / MapWidth * bucketResolution);
//		int y = (int)(posY / MapWidth * bucketResolution);
//		if (x<0 || y<0 || x>=bucketResolution || y>=bucketResolution) {
//			return emptyBucket;
//		} else {
//			return obstacleBuckets[x,y];
//		}
//	}
//
//	private void Awake()
//	{
//		Instance = this;
//	}
//
//	void FixedUpdate() {
//		for (int i = 0; i < ants.Length; i++) {
//			Ant ant = ants[i];
//			float targetSpeed = antSpeed;
//
//			ant.facingAngle += Random.Range(-randomSteering,randomSteering);
//
//			float pheroSteering = PheromoneSteering(ant,3f);
//			int wallSteering = WallSteering(ant,1.5f);
//			ant.facingAngle += pheroSteering * pheromoneSteerStrength;
//			ant.facingAngle += wallSteering * wallSteerStrength;
//
//			targetSpeed *= 1f - (Mathf.Abs(pheroSteering) + Mathf.Abs(wallSteering)) / 3f;
//
//			ant.speed += (targetSpeed - ant.speed) * antAccel;
//
//			Vector2 targetPos;
//			int index1 = i / instancesPerBatch;
//			int index2 = i % instancesPerBatch;
//			if (ant.holdingResource == false) {
//				targetPos = resourcePosition;
//
//				antColors[index1][index2] += ((Vector4)searchColor * ant.brightness - antColors[index1][index2])*.05f;
//			} else {
//				targetPos = colonyPosition;
//				antColors[index1][index2] += ((Vector4)carryColor * ant.brightness - antColors[index1][index2]) * .05f;
//			}
//			if (Linecast(ant.position,targetPos)==false) {
//				Color color = Color.green;
//				float targetAngle = Mathf.Atan2(targetPos.y-ant.position.y,targetPos.x-ant.position.x);
//				if (targetAngle - ant.facingAngle > Mathf.PI) {
//					ant.facingAngle += Mathf.PI * 2f;
//					color = Color.red;
//				} else if (targetAngle - ant.facingAngle < -Mathf.PI) {
//					ant.facingAngle -= Mathf.PI * 2f;
//					color = Color.red;
//				} else {
//					if (Mathf.Abs(targetAngle-ant.facingAngle)<Mathf.PI*.5f)
//					ant.facingAngle += (targetAngle-ant.facingAngle)*goalSteerStrength;
//				}
//
//				//Debug.DrawLine(ant.position/mapSize,targetPos/mapSize,color);
//			}
//			if ((ant.position - targetPos).sqrMagnitude < 4f * 4f) {
//				ant.holdingResource = !ant.holdingResource;
//				ant.facingAngle += Mathf.PI;
//			}
//
//			float vx = Mathf.Cos(ant.facingAngle) * ant.speed;
//			float vy = Mathf.Sin(ant.facingAngle) * ant.speed;
//			float ovx = vx;
//			float ovy = vy;
//
//			if (ant.position.x + vx < 0f || ant.position.x + vx > MapWidth) {
//				vx = -vx;
//			} else {
//				ant.position.x += vx;
//			}
//			if (ant.position.y + vy < 0f || ant.position.y + vy > MapWidth) {
//				vy = -vy;
//			} else {
//				ant.position.y += vy;
//			}
//
//			float dx, dy, dist;
//
//			Obstacle_Old[] nearbyObstacles = GetObstacleBucket(ant.position);
//			for (int j=0;j<nearbyObstacles.Length;j++) {
//				Obstacle_Old obstacle = nearbyObstacles[j];
//				dx = ant.position.x - obstacle.Position.x;
//				dy = ant.position.y - obstacle.Position.y;
//				float sqrDist = dx * dx + dy * dy;
//				if (sqrDist<obstacleRadius*obstacleRadius) {
//					dist = Mathf.Sqrt(sqrDist);
//					dx /= dist;
//					dy /= dist;
//					ant.position.x = obstacle.Position.x + dx * obstacleRadius;
//					ant.position.y = obstacle.Position.y + dy * obstacleRadius;
//
//					vx -= dx * (dx * vx + dy * vy) * 1.5f;
//					vy -= dy * (dx * vx + dy * vy) * 1.5f;
//				}
//			}
//
//			float inwardOrOutward = -outwardStrength;
//			float pushRadius = MapWidth * .4f;
//			if (ant.holdingResource) {
//				inwardOrOutward = inwardStrength;
//				pushRadius = MapWidth;
//			}
//			dx = colonyPosition.x - ant.position.x;
//			dy = colonyPosition.y - ant.position.y;
//			dist = Mathf.Sqrt(dx * dx + dy * dy);
//			inwardOrOutward *= 1f-Mathf.Clamp01(dist / pushRadius);
//			vx += dx / dist * inwardOrOutward;
//			vy += dy / dist * inwardOrOutward;
//
//			if (ovx != vx || ovy != vy) {
//				ant.facingAngle = Mathf.Atan2(vy,vx);
//			}
//
//			//if (ant.holdingResource == false) {
//			//float excitement = 1f-Mathf.Clamp01((targetPos - ant.position).magnitude / (mapSize * 1.2f));
//			float excitement = .3f;
//			if (ant.holdingResource) {
//				excitement = 1f;
//			}
//			excitement *= ant.speed / antSpeed;
//			DropPheromones(ant.position,excitement);
//			//}
//
//			Matrix4x4 matrix = GetRotationMatrix(ant.facingAngle);
//			matrix.m03 = ant.position.x / MapWidth;
//			matrix.m13 = ant.position.y / MapWidth;
//			matrices[i / instancesPerBatch][i % instancesPerBatch] = matrix;
//		}
//
//		for (int x = 0; x < MapWidth; x++) {
//			for (int y = 0; y < MapWidth; y++) {
//				int index = PheromoneIndex(x,y);
//				pheromoneColours[index].r *= trailDecay;
//			}
//		}
//
//		pheromoneTexture.SetPixels(pheromoneColours);
//		pheromoneTexture.Apply();
//
//		for (int i=0;i<matProps.Length;i++) {
//			matProps[i].SetVectorArray("_Color",antColors[i]);
//		}
//	}
	private void Update() {

		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			Time.timeScale = 1f;
		} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			Time.timeScale = 2f;
		} else if (Input.GetKeyDown(KeyCode.Alpha3)) {
			Time.timeScale = 3f;
		} else if (Input.GetKeyDown(KeyCode.Alpha4)) {
			Time.timeScale = 4f;
		} else if (Input.GetKeyDown(KeyCode.Alpha5)) {
			Time.timeScale = 5f;
		} else if (Input.GetKeyDown(KeyCode.Alpha6)) {
			Time.timeScale = 6f;
		} else if (Input.GetKeyDown(KeyCode.Alpha7)) {
			Time.timeScale = 7f;
		} else if (Input.GetKeyDown(KeyCode.Alpha8)) {
			Time.timeScale = 8f;
		} else if (Input.GetKeyDown(KeyCode.Alpha9)) {
			Time.timeScale = 9f;
		}

		for (int i = 0; i < matrices.Length; i++) {
			Graphics.DrawMeshInstanced(antMesh,0,antMaterial,matrices[i],matrices[i].Length,matProps[i]);
		}
		for (int i=0;i<obstacleMatrices.Length;i++) {
			Graphics.DrawMeshInstanced(obstacleMesh,0,obstacleMaterial,obstacleMatrices[i]);
		}

		Graphics.DrawMesh(colonyMesh,colonyMatrix,colonyMaterial,0);
		Graphics.DrawMesh(resourceMesh,resourceMatrix,resourceMaterial,0);
	}
}
