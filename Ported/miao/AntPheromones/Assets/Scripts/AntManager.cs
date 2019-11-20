using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Unity.Rendering;

namespace AntPheromones_ECS
{
	public class AntManager : MonoBehaviour 
	{
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
		public int AntCount = 1000;
		public int MapWidth = 128;
		public int BucketResolution;
		public Vector3 antSize;
		public float antSpeed;
		
		public float antAccel;
		public float trailAddSpeed;
		
		public float trailDecay;
		public float randomSteering;
		public float pheromoneSteerStrength;
		public float wallSteerStrength;
		public float goalSteerStrength;
		public float outwardStrength;
		public float inwardStrength;
		public int rotationResolution = 360;
		public int ObstacleRingCount = 3;
		public float ObstaclesPerRing = 0.8f;
		public float ObstacleRadius = 2;

		public float2[,][] ObstacleBuckets
		{
			get
			{
				if (!this._obstacleBuckets.IsGenerated)
				{
					GenerateObstacles();
				}
				return this._obstacleBuckets.Values;
			}
		}
		
		public float2[] ObstaclePositions
		{
			get
			{
				if (!this._obstaclePositions.IsGenerated)
				{
					GenerateObstacles();
				}
				return this._obstaclePositions.Values;
			}
		}

		public float2 ColonyPosition
		{
			get
			{
				if (!this._colonyPosition.IsCalculated)
				{
					this._colonyPosition.Value = new float2(1f, 1f) * this.MapWidth * 0.5f;
					this._colonyPosition.IsCalculated = true;
				}
				return this._colonyPosition.Value;
			}
		}

		public float2 ResourcePosition
		{
			get
			{
				if (!this._resourcePosition.IsCalculated)
				{
					float resourceAngle = Random.value * 2f * Mathf.PI;
					this._resourcePosition.Value = 0.5f * this.MapWidth * new float2(1f, 1f) +
					                        new float2(Mathf.Cos(resourceAngle) * this.MapWidth * 0.475f,
						                        Mathf.Sin(resourceAngle) * this.MapWidth * 0.475f);
					
					this._resourcePosition.IsCalculated = true;
				}

				return this._resourcePosition.Value;
			}
		}
		
		private (bool IsCalculated, float2 Value) _colonyPosition;
		private (bool IsCalculated, float2 Value) _resourcePosition;
		private (bool IsGenerated, float2[] Values) _obstaclePositions;
		private (bool IsGenerated, float2[,][] Values) _obstacleBuckets;
		
		Texture2D pheromoneTexture;
		Material myPheromoneMaterial;

		public Color[] pheromoneColours;
		Ant[] ants;
		Matrix4x4[][] matrices;
		Vector4[][] antColors;
		MaterialPropertyBlock[] matProps;
		float4x4[][] _obstacleMatrices;
		

		private Matrix4x4 _resourceMatrix;
		private Matrix4x4 _colonyMatrix;
		private NativeArray<Entity> antEntities;

		const int InstancesPerBatch = 1023;

		Matrix4x4[] rotationMatrixLookup;
		
		void Start()
		{
			EntityManager entityManager = World.Active.EntityManager;
//			Entity mapEntity = entityManager.CreateEntity(typeof(Map));
//			EntityQuery mapSingletonGroup = entityManager.CreateEntityQuery(typeof(Entity));
//			mapSingletonGroup.SetSingleton<Map>(mapEntity);

			this._colonyMatrix = float4x4.TRS(
				new float3(this._colonyPosition.Value / this.MapWidth, 0), 
				Quaternion.identity, 
				new float3(4f, 4f, 0.1f) / this.MapWidth);

			this._resourceMatrix = float4x4.TRS(
				new float3(this._resourcePosition.Value / this.MapWidth, 0f),
				Quaternion.identity,
				new float3(4f, 4f, 0.1f) / this.MapWidth);

			this.pheromoneTexture = new Texture2D(MapComponent.Width, MapComponent.Width) {wrapMode = TextureWrapMode.Mirror};
			this.pheromoneColours = new Color[MapComponent.Width * MapComponent.Width];

			this.pheromoneRenderer.sharedMaterial = new Material(basePheromoneMaterial)
				{mainTexture = this.pheromoneTexture};

			
			EntityArchetype antArchetype =
				entityManager.CreateArchetype(
					typeof(PositionComponent),
					typeof(FacingAngleComponent),
					typeof(SpeedComponent),
					typeof(ColourDisplay),
					typeof(Brightness),
					typeof(ResourceCarrier),
					typeof(RenderMesh),
					typeof(LocalToWorld),
					typeof(Ant));

			this.antEntities = new NativeArray<Entity>(length: this.AntCount, Allocator.Persistent);
			entityManager.CreateEntity(antArchetype, this.antEntities);

			foreach (var entity in this.antEntities)
			{
				entityManager.SetComponentData(
					entity,
					new PositionComponent
					{
						Value = new float2(Random.Range(-5f, 5f) + this.MapWidth * 0.5f,
							Random.Range(-5f, 5f) + this.MapWidth * .5f)
					});
				entityManager.SetSharedComponentData(entity, new RenderMesh
				{
					mesh = this.antMesh,
					material = this.antMaterial
				});
			}

			Entity pheromoneColourMapEntity = entityManager.CreateEntity(typeof(PheromoneColourRValue));
			entityManager.AddBuffer<PheromoneColourRValue>(pheromoneColourMapEntity);
			
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
		private void GenerateObstacles()
		{
			this._obstaclePositions.Values = CalculateObstaclePositions().ToArray();
			this._obstaclePositions.IsGenerated = true;
			
			this._obstacleMatrices = CalculateObstacleMatrices();
			
			List<float2>[,] temporaryObstacleBuckets = GenerateObstacleBuckets();
			
			this._obstacleBuckets.Values = new float2[this.BucketResolution, this.BucketResolution][];

			for (int x = 0; x < this.BucketResolution; x++)
			{
				for (int y = 0; y < this.BucketResolution; y++)
				{
					this._obstacleBuckets.Values[x, y] = temporaryObstacleBuckets[x, y].ToArray();
				}
			}

			this._obstacleBuckets.IsGenerated = true;
		}

		private List<float2>[,] GenerateObstacleBuckets()
		{
			List<float2>[,] buckets = new List<float2>[this.BucketResolution,this.BucketResolution];

			for (int x = 0; x < this.BucketResolution; x++)
			{
				for (int y = 0; y < this.BucketResolution; y++)
				{
					buckets[x, y] = new List<float2>();
				}
			}

			for (int i = 0; i < this._obstaclePositions.Values.Length; i++)
			{
				float2 position = this._obstaclePositions.Values[i];
				
				for (int x = Mathf.FloorToInt((position.x - this.ObstacleRadius) / this.MapWidth * this.BucketResolution);
					x <= Mathf.FloorToInt((position.x + this.ObstacleRadius) / this.MapWidth * this.BucketResolution);
					x++)
				{
					if (x < 0 || x >= BucketResolution)
					{
						continue;
					}

					for (int y = Mathf.FloorToInt((position.y - this.ObstacleRadius) / this.MapWidth * this.BucketResolution);
						y <= Mathf.FloorToInt((position.y + this.ObstacleRadius) / this.MapWidth * this.BucketResolution);
						y++)
					{
						if (y < 0 || y >= this.BucketResolution)
						{
							continue;
						}

						buckets[x, y].Add(this._obstaclePositions.Values[i]);
					}
				}
			}

			return buckets;
		}

		private List<float2> CalculateObstaclePositions()
		{
			var positions = new List<float2>();
			
            for (int i = 1; i <= this.ObstacleRingCount; i++)
            {
            	float ringRadius = i / (this.ObstacleRingCount + 1f) * (this.MapWidth * .5f);
            	float circumference = ringRadius * 2f * Mathf.PI;
            	int maxCount = Mathf.CeilToInt(circumference / (2f * this.ObstacleRadius) * 2f);
            	int offset = Random.Range(0, maxCount);
            	int holeCount = Random.Range(1, 3);
            	
            	for (int j = 0; j < maxCount; j++)
            	{
            		float t = (float) j / maxCount;

            		if (!(t * holeCount % 1f < this.ObstaclesPerRing))
            		{
            			continue;
            		}
            		
            		float angle = (j + offset) / (float) maxCount * (2f * Mathf.PI);
            		float2 obstacle = 
            			new float2(this.MapWidth * 0.5f + math.cos(angle) * ringRadius,
            			this.MapWidth * 0.5f + math.sin(angle) * ringRadius); 

                    positions.Add(obstacle);
            	}
            }

            return positions;
		}

		private float4x4[][] CalculateObstacleMatrices()
		{
			float4x4[][] matrices = new float4x4[Mathf.CeilToInt((float)this._obstaclePositions.Values.Length / InstancesPerBatch)][];

			for (int i = 0; i < this.matrices.Length; i++)
			{
				matrices[i] = new float4x4[Mathf.Min(InstancesPerBatch, this._obstaclePositions.Values.Length - i * InstancesPerBatch)];
				
				for (int j = 0; j < matrices[i].Length; j++)
				{
					matrices[i][j] =
						float4x4.TRS(new float3(this._obstaclePositions.Values[i * InstancesPerBatch + j] / this.MapWidth, 0),
							Quaternion.identity, new float3(this.ObstacleRadius * 2f, this.ObstacleRadius * 2f, 1f) / MapWidth);
				}
			}

			return matrices;
		}
		
		float2[] GetObstacleBucket(float2 position) {
			int y = (int)(position.y / this.MapWidth * this.BucketResolution);
			int x = (int)(position.x / this.MapWidth * this.BucketResolution);

			if (x < 0 || y < 0 || x >= this.BucketResolution || y >= this.BucketResolution)
			{
				return null;
			}
			else
			{
				return this._obstacleBuckets.Values[x, y];
			}
		}
	
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
			for (int i=0;i<_obstacleMatrices.Length;i++) {
				Graphics.DrawMeshInstanced(obstacleMesh,0,obstacleMaterial,_obstacleMatrices[i]);
			}

			Graphics.DrawMesh(colonyMesh,_colonyMatrix,colonyMaterial,0);
			Graphics.DrawMesh(resourceMesh,_resourceMatrix,resourceMaterial,0);
		}
	}
}