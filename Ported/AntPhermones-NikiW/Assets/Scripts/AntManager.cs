using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public unsafe class AntManager : MonoBehaviour
{
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
	public int mapSize = 128;
	[FormerlySerializedAs("bucketResolution")]
	public int obstacleBucketResolution;
	public Vector3 antSize;
	public float antSpeed;
	[Range(0f, 1f)]
	public float antAccel;
	public float trailAddSpeed;
	[Range(0f, 1f)]
	public float trailDecay;
	public float randomSteering;
	public float pheromoneSteerStrength;
	public float wallSteerStrength;
	public float goalSteerStrength;
	public float outwardStrength;
	public float inwardStrength;
	public int rotationResolution = 360;
	public int obstacleRingCount;
	[Range(0f, 1f)]
	public float obstaclesPerRing;
	public float obstacleRadius;

	public bool renderAnts = true;
	public bool renderWalls = true;
	public bool addWallsToTexture = false;

	Texture2D pheromoneTexture;
	Material myPheromoneMaterial;
	NativeArray<float> pheromones;
	NativeArray<Ant> ants;
	NativeArray<Matrix4x4> antMatrix;
	Matrix4x4[] reusableAntMatrix;
	Vector4[] antColors;
	MaterialPropertyBlock reusableAntMatProps;
	Matrix4x4[][] obstacleMatrices;
	NativeBitArray obstacleCollisionLookup;

	Matrix4x4 resourceMatrix;
	Matrix4x4 colonyMatrix;

	float2 resourcePosition;
	float2 colonyPosition;

	const int instancesPerBatch = 1023;

	NativeArray<Matrix4x4> rotationMatrixLookup;
	
	void GenerateObstacles()
	{
		// TEMP HACK:
		obstacleBucketResolution = mapSize;
		
		// Generate obstacles in a circle:
		var obstaclePositions = new NativeList<float2>(1024, Allocator.Temp);
		for (var i = 1; i <= obstacleRingCount; i++)
		{
			var ringRadius = (i / (obstacleRingCount + 1f)) * (mapSize * .5f);
			var circumference = ringRadius * 2f * math.PI;
			var maxCount = Mathf.CeilToInt(circumference / (2f * obstacleRadius) * 2f);
			var offset = Random.Range(0, maxCount);
			var holeCount = Random.Range(1, 3);
			for (var j = 0; j < maxCount; j++)
			{
				var t = (float)j / maxCount;
				if ((t * holeCount) % 1f < obstaclesPerRing)
				{
					var angle = (j + offset) / (float)maxCount * (2f * math.PI);
					var obstaclePosition = new float2(mapSize * .5f + math.cos(angle) * ringRadius, mapSize * .5f + math.sin(angle) * ringRadius);
					obstaclePositions.Add(obstaclePosition);

					//Debug.DrawRay(obstacle.position / mapSize,-Vector3.forward * .05f,Color.green,10000f);
				}
			}
		}

		obstacleMatrices = new Matrix4x4[Mathf.CeilToInt((float)obstaclePositions.Length / instancesPerBatch)][];
		for (var i = 0; i < obstacleMatrices.Length; i++)
		{
			obstacleMatrices[i] = new Matrix4x4[math.min(instancesPerBatch, obstaclePositions.Length - i * instancesPerBatch)];
			for (var j = 0; j < obstacleMatrices[i].Length; j++)
			{
				obstacleMatrices[i][j] = Matrix4x4.TRS((Vector2)obstaclePositions[i * instancesPerBatch + j] / mapSize, Quaternion.identity, new Vector3(obstacleRadius * 2f, obstacleRadius * 2f, 1f) / mapSize);
			}
		}

		// Buckets:
		// NW: Max bucket size seems to be around 4, and the vast majority are empty.
		// Thus, lets just simplify to a collision map.
		obstacleCollisionLookup = new NativeBitArray(obstacleBucketResolution * obstacleBucketResolution, Allocator.Persistent, NativeArrayOptions.ClearMemory);

		for (int i = 0; i < obstaclePositions.Length; i++)
		{
			float2 pos = obstaclePositions[i];

			for (int x = Mathf.FloorToInt((pos.x - obstacleRadius) / mapSize * obstacleBucketResolution); x <= Mathf.FloorToInt((pos.x + obstacleRadius) / mapSize * obstacleBucketResolution); x++)
			{
				if (x < 0 || x >= obstacleBucketResolution)
					continue;

				for (int y = Mathf.FloorToInt((pos.y - obstacleRadius) / mapSize * obstacleBucketResolution); y <= Mathf.FloorToInt((pos.y + obstacleRadius) / mapSize * obstacleBucketResolution); y++)
				{
					if (y < 0 || y >= obstacleBucketResolution)
						continue;

					var isInBounds = CalculateIsInBounds(in x, in y, in obstacleBucketResolution, out var obstacleBucketIndex);
					if (math.all(isInBounds))
						obstacleCollisionLookup.Set(obstacleBucketIndex, true);
				}
			}
		}


		// Assert collision map:
		string log = $"OBSTACLES: {obstaclePositions.Length}\nCOLLISION BUCKETS: [{obstacleCollisionLookup.Length}";
		{
			for (var x = 0; x < obstacleBucketResolution; x++)
			{
				log += $"\n{x:0000}|";
				for (var y = 0; y < obstacleBucketResolution; y++)
				{
					var isInBounds = CalculateIsInBounds(x, y, obstacleBucketResolution, out var obstacleIndex);
					if (math.all(isInBounds))
						log += obstacleCollisionLookup.IsSet(obstacleIndex) ? "/" : ".";
					else throw new InvalidOperationException();
				}
			}
		}
		Debug.Log(log);
		
		obstaclePositions.Dispose();
	}
	void Start()
	{
		GenerateObstacles();

		colonyPosition = new float2(mapSize * .5f);
		colonyMatrix = Matrix4x4.TRS((Vector2)colonyPosition / mapSize, Quaternion.identity, new Vector3(4f, 4f, .1f) / mapSize);
		var resourceAngle = Random.value * 2f * math.PI;
		resourcePosition = (colonyPosition + new float2(math.cos(resourceAngle) * mapSize * .475f, math.sin(resourceAngle) * mapSize * .475f));
		resourceMatrix = Matrix4x4.TRS((Vector2)resourcePosition / mapSize, Quaternion.identity, new Vector3(4f, 4f, .1f) / mapSize);

		pheromoneTexture = new Texture2D(mapSize, mapSize);
		pheromoneTexture.wrapMode = TextureWrapMode.Mirror;
		Blit(pheromoneTexture, new Color32());
		
		pheromones = new NativeArray<float>(mapSize * mapSize, Allocator.Persistent);
		myPheromoneMaterial = new Material(basePheromoneMaterial);
		myPheromoneMaterial.mainTexture = pheromoneTexture;
		pheromoneRenderer.sharedMaterial = myPheromoneMaterial;
		ants = new NativeArray<Ant>(antCount, Allocator.Persistent);

		antMatrix = new NativeArray<Matrix4x4>(antCount, Allocator.Persistent);
		var numBatches = Mathf.CeilToInt((float)antCount / instancesPerBatch);
		antColors = new Vector4[antCount];
		reusableAntMatrix = new Matrix4x4[instancesPerBatch];
		reusableAntMatProps = new MaterialPropertyBlock();

		for (var i = 0; i < antCount; i++)
		{
			ants[i] = new Ant(new float2(Random.Range(-5f, 5f) + mapSize * .5f, Random.Range(-5f, 5f) + mapSize * .5f));
		}

		rotationMatrixLookup = new NativeArray<Matrix4x4>(rotationResolution, Allocator.Persistent);
		for (var i = 0; i < rotationResolution; i++)
		{
			var angle = (float)i / rotationResolution;
			angle *= 360f;
			rotationMatrixLookup[i] = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, angle), antSize);
		}

		simulationTime = Time.time;
		
		// NW: "Build" the job here to avoid expensive copy.
		m_AntUpdateJob = new AntUpdateJob
		{
			ants = ants,
			obstacleCollisionLookup = obstacleCollisionLookup,

			pheromones = pheromones,
		
			antAccel = antAccel,
			antSpeed = antSpeed,
			pheromoneSteerStrength = pheromoneSteerStrength,
			wallSteerStrength = wallSteerStrength,
			obstacleBucketResolution = obstacleBucketResolution,
			randomSteering = randomSteering,
			mapSize = mapSize,
			goalSteerStrength = goalSteerStrength,
			outwardStrength = outwardStrength,
			inwardStrength = inwardStrength,
			
			perFrameRandomSeed = (uint)(Random.value * uint.MaxValue),

			colonyPosition = colonyPosition,
			resourcePosition = resourcePosition,
		};
		m_AntDropPheromonesJob = new AntDropPheromonesJob
		{
			ants = ants,
			pheromones = pheromones,

			antSpeed = antSpeed,
			mapSize = mapSize,
			trailAddSpeed = trailAddSpeed,
			trailDecay = trailDecay,
			simulationDt = 1f / simulationHz
		};
	}

	void Blit(Texture2D texture2D, Color32 color32)
	{
		var color = pheromoneTexture.GetPixelData<Color32>(0);
		for (var i = 0; i < color.Length; i++) 
			color[i] = color32;
		pheromoneTexture.Apply();
	}

	void OnDestroy()
	{
		Destroy(pheromoneTexture);
		Destroy(myPheromoneMaterial);

		if (ants.IsCreated) ants.Dispose();
		if (pheromones.IsCreated) pheromones.Dispose();
		if (rotationMatrixLookup.IsCreated) rotationMatrixLookup.Dispose();
		if (antMatrix.IsCreated) antMatrix.Dispose();
		if (obstacleCollisionLookup.IsCreated) obstacleCollisionLookup.Dispose();
	}

	// void OnDrawGizmos()
	// {
	// 	if (! Application.isPlaying) return;
	// 	
	// 		for (int x = 0; x < obstacleBucketResolution; x++)
	// 	for (int y = 0; y < obstacleBucketResolution; y++)
	// 	{
	// 		Gizmos.color = new Color(1f, 0f, 0f, 0.27f);
	// 		var isInBounds = CalculateIsInBounds(in x, in y, in obstacleBucketResolution, out var index);
	// 		if (math.all(isInBounds) && obstacleCollisionLookup.IsSet(index))
	// 		{
	// 			Gizmos.DrawSphere(new Vector3(x, y, 0), 1f);
	// 		}
	// 	}
	//
	// 	for (int i = 0; i < ants.Length; i++)
	// 	{
	// 		var ant = ants[i];
	// 		Gizmos.color = ant.holdingResource ? Color.black : Color.magenta;
	// 		Gizmos.DrawSphere((Vector2)ant.position, .8f);
	// 	}
	// }

	// NWALKER: Trying out in keyword to improve perf.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool2 CalculateIsInBounds(in int x, in int y, in int size, out int index)
	{
		index = x + y * size;
		return new bool2(x >= 0 && x < size, y >= 0 && y < size);
	}	
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool2 CalculateIsInBounds(in float2 pos, in int size, out int index)
	{
		var x = Mathf.RoundToInt(pos.x);
		var y = Mathf.RoundToInt(pos.y);
		index = x + y * size;
		return new bool2(x >= 0 && x < size, y >= 0 && y < size);
	}

	// NWALKER: Investigate https://github.com/stella3d/SharedArray
	[BurstCompile]
	public struct AntMatricesJob : IJobParallelFor
	{
		// NWalker: float4x4?
		[NoAlias, ReadOnly]
		public NativeArray<Matrix4x4> rotationMatrixLookup;

		[NoAlias, ReadOnly]
		public NativeArray<Ant> ants;
		
		[NoAlias, WriteOnly]
		public NativeArray<Matrix4x4> matrices;

		public float oneOverMapSize;

		public int rotationResolution;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		Matrix4x4 GetRotationMatrix(float angle)
		{
			angle /= math.PI * 2f;
			angle -= math.floor(angle);
			angle *= rotationResolution;
			return rotationMatrixLookup[((int)angle) % rotationResolution];
		}

		public void Execute(int index)
		{
			var ant = ants[index];
			var matrix = GetRotationMatrix(ant.facingAngle);
			matrix.m03 = ant.position.x * oneOverMapSize;
			matrix.m13 = ant.position.y * oneOverMapSize;
			matrices[index] = matrix;
		}
	}

	[BurstCompile]
	struct AntUpdateJob : IJobParallelFor
	{
		[NoAlias]
		public NativeArray<Ant> ants;

		[NoAlias, ReadOnly]
		public NativeBitArray obstacleCollisionLookup;

		[NoAlias, ReadOnly]
		public NativeArray<float> pheromones;

		public float antSpeed, pheromoneSteerStrength, wallSteerStrength, antAccel, randomSteering;
		public int obstacleBucketResolution;
		public float goalSteerStrength;
		public float outwardStrength, inwardStrength;
		public uint perFrameRandomSeed;

		public int mapSize;
		public float2 colonyPosition, resourcePosition;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		float PheromoneSteering(Ant ant, float distance)
		{
			var output = 0f;
			var quarterPi = (math.PI * .25f);
			for (var i = -1; i <= 1; i += 2)
			{
				var angle = (ant.facingAngle + i * quarterPi);
				int testX = (int)(ant.position.x + math.cos(angle) * distance);
				int testY = (int)(ant.position.y + math.sin(angle) * distance);
				var isInBounds = CalculateIsInBounds(in testX, in testY, in mapSize, out var index);
				if (math.all(isInBounds))
					output += pheromones[index] * i;
			}

			return math.sign(output);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		int WallSteering(Ant ant, float distance)
		{
			var output = 0;
			for (var i = -1; i <= 1; i += 2)
			{
				var angle = ant.facingAngle + i * math.PI * .25f;
				var testX = (int)(ant.position.x + math.cos(angle) * distance);
				var testY = (int)(ant.position.y + math.sin(angle) * distance);

				var isInBounds = CalculateIsInBounds(in testX, in testY, in obstacleBucketResolution, out var obstacleCollisionIndex);
				if (! math.any(isInBounds) || math.all(isInBounds) && obstacleCollisionLookup.IsSet(obstacleCollisionIndex))
				{
					output -= i;
				}
			}

			return output;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool Linecast(float2 point1, float2 point2)
		{
			var dx = point2.x - point1.x;
			var dy = point2.y - point1.y;
			var dist = math.sqrt(dx * dx + dy * dy);

			// NW: Test to see if step count can be generalized.
			var stepCount = Mathf.CeilToInt(dist * .5f);
			for (var i = 0; i < stepCount; i++)
			{
				var t = (float)i / stepCount;
				var testX = (int) (point1.x + dx * t);
				var testY = (int) (point1.y + dy * t);
				var isInBounds = CalculateIsInBounds(in testX, in testY, in obstacleBucketResolution, out var collisionIndex);
				if (! math.all(isInBounds) || obstacleCollisionLookup.IsSet(collisionIndex))
					return true;
			}

			return false;
		}

		public void Execute(int index)
		{
			var antsUnsafePtr = ants.GetUnsafePtr();
			const float piX2 = math.PI * 2f;
			
			ref var ant = ref UnsafeUtility.ArrayElementAsRef<Ant>(antsUnsafePtr, index);
			float targetSpeed = antSpeed;

			// NW: Random "enough" for our case.
			var randRotation = Squirrel3.NextFloat((uint)index, perFrameRandomSeed, -randomSteering, randomSteering);
			ant.facingAngle += randRotation;

			var pheroSteering = PheromoneSteering(ant, (3f));
			var wallSteering = WallSteering(ant, 1.5f);
			ant.facingAngle += pheroSteering * pheromoneSteerStrength;
			ant.facingAngle += wallSteering * wallSteerStrength;

			targetSpeed *= 1f - (math.abs(pheroSteering) + math.abs(wallSteering)) / 3f;
			
			//ant.speed += ((targetSpeed - ant.speed) * antAccel);
			ant.speed = targetSpeed;
			
			var targetPos = math.@select(resourcePosition, colonyPosition, ant.holdingResource);
			if (! Linecast(ant.position, targetPos))
			{
				//var color = Color.green;
				float targetAngle = math.atan2(targetPos.y - ant.position.y, targetPos.x - ant.position.x);

				if (targetAngle - ant.facingAngle > math.PI)
				{
					ant.facingAngle += (piX2);
					//color = Color.red;
				}
				else if (targetAngle - ant.facingAngle < -math.PI)
				{
					ant.facingAngle -= (piX2);
					//color = Color.red;
				}
				else
				{
					if (math.abs(targetAngle - ant.facingAngle) < math.PI * .5f)
						ant.facingAngle += ((targetAngle - ant.facingAngle) * goalSteerStrength);
				}

				//Debug.DrawLine((Vector2)ant.position/mapSize,(Vector2)targetPos/mapSize,color);
			}

			if (math.distancesq(ant.position, targetPos) < 3f * 3f)
			{
				ant.holdingResource = !ant.holdingResource;
				ant.facingAngle += (math.PI);
			}

			float vx = math.cos(ant.facingAngle) * ant.speed;
			float vy = math.sin(ant.facingAngle) * ant.speed;
			var ovx = vx;
			var ovy = vy;

			var originalPosition = ant.position;
			ant.position.x += vx;
			ant.position.y += vy;
			
			// NW: If we're now colliding with a wall, bounce:
			var isInBounds = CalculateIsInBounds(in ant.position, in obstacleBucketResolution, out var collisionIndex);
			
			if (! math.all(isInBounds))
			{
				ant.position -= (ant.position - originalPosition) * 2f;
				if(! isInBounds.x) vx = -vx;
				if(! isInBounds.y) vy = -vy;
			}
			else if (obstacleCollisionLookup.IsSet(collisionIndex))
			{
				ant.position -= (ant.position - originalPosition) * 2f;
				vx = -vx;
				vy = -vy;
			}
			
			float dx, dy, dist;
			// var nearbyObstacles = GetObstacleBucket(ant.position);
			// for (var j = 0; j < nearbyObstacles.Length; j++)
			// {
			// 	var obstacle = nearbyObstacles[j];
			// 	dx = ant.position.x - obstacle.position.x;
			// 	dy = ant.position.y - obstacle.position.y;
			// 	float sqrDist = dx * dx + dy * dy;
			// 	if (sqrDist < obstacleRadiusSqr)
			// 	{
			// 		dist = math.sqrt(sqrDist);
			// 		dx /= dist;
			// 		dy /= dist;
			// 		ant.position.x = (obstacle.position.x + dx * obstacleRadius);
			// 		ant.position.y = (obstacle.position.y + dy * obstacleRadius);
			//
			// 		vx -= dx * (dx * vx + dy * vy) * 1.5f;
			// 		vy -= dy * (dx * vx + dy * vy) * 1.5f;
			// 	}
			// }
			
			float inwardOrOutward = -outwardStrength;
			float pushRadius = mapSize * .4f;
			if (ant.holdingResource)
			{
				inwardOrOutward = inwardStrength;
				pushRadius = mapSize;
			}
			
			dx = colonyPosition.x - ant.position.x;
			dy = colonyPosition.y - ant.position.y;
			dist = math.sqrt(dx * dx + dy * dy);
			inwardOrOutward *= 1f - Mathf.Clamp01(dist / pushRadius);
			vx += dx / dist * inwardOrOutward;
			vy += dy / dist * inwardOrOutward;
			
			if (math.abs(ovx - vx) > float.Epsilon || math.abs(ovy - vy) > float.Epsilon)
			{
				ant.facingAngle = math.atan2(vy, vx);
			}
		}
	}

	JobHandle simulationJobHandle;
	
	void TickSimulation()
	{
		simulationJobHandle = m_AntUpdateJob.Schedule(ants.Length, 16, simulationJobHandle);
		//m_AntUpdateJob.Run(ants.Length);

		simulationJobHandle = m_AntDropPheromonesJob.Schedule(simulationJobHandle);
		//simulationJobHandle.Complete();
		//simulationJobHandle = default;
		//m_AntsDropPheromonesJob.Run();
		
		mustRerender = true;
	}

	private void Update()
	{
		int ticks = 0;
		for (; ticks < maxSimulationStepsPerFrame && simulationTime < Time.time; ticks++)
		{
			float updateStart = Time.realtimeSinceStartup;

			using (s_ScheduleSimMarker.Auto())
			{
				var dt = 1f / simulationHz;
				simulationTime += dt;
				m_AntsPerSecondCounter += ants.Length;
				TickSimulation();
			}

			using (s_ComputeSimMarker.Auto())
			{
				if (simulationStepsPerRenderFrame % manualJobBatcherStep == 0)
				{
					simulationJobHandle.Complete();
					simulationJobHandle = default;
				}
				/*if (i % manualJobBatcherStep == 0)
				{
					JobHandle.ScheduleBatchedJobs();
				}*/
			}
			
			ConvergeTowards(ref simulationElapsedSeconds, Time.realtimeSinceStartup - updateStart);
		}
		
		m_AntsPerSecondDt += Time.deltaTime;
		if (m_AntsPerSecondDt >= 1)
		{
			ConvergeTowards(ref m_AntsPerSecond, m_AntsPerSecondCounter);
			m_AntsPerSecondCounter = 0;
			m_AntsPerSecondDt -= 1;
		}

		ConvergeTowards(ref simulationStepsPerRenderFrame, ticks);
	}

	static void ConvergeTowards(ref double value, double target)
	{
		value = math.lerp(value, target, .02f);
	}

	static ProfilerMarker s_RenderSetupMarker = new ProfilerMarker("RenderSetup");
	static ProfilerMarker s_GraphicsDrawMarker = new ProfilerMarker("Graphics.DrawMeshInstanced");
	static ProfilerMarker s_PheromoneFloatToColorCopyMarker = new ProfilerMarker("Pheromone Float > Color32");
	static ProfilerMarker s_ScheduleSimMarker = new ProfilerMarker("ScheduleSim");
	static ProfilerMarker s_ComputeSimMarker = new ProfilerMarker("ComputeSim");
	
	bool mustRerender;
	[FormerlySerializedAs("simulationRate")]
	public int simulationHz = 60;
	public int maxSimulationStepsPerFrame = 20;
	float simulationTime;
	AntUpdateJob m_AntUpdateJob;
	AntDropPheromonesJob m_AntDropPheromonesJob;
	public int manualJobBatcherStep = 10;
	double simulationStepsPerRenderFrame;
	double simulationElapsedSeconds, renderElapsedSeconds, rerenderElapsedSeconds;
	double m_AntsPerSecond;
	long m_AntsPerSecondCounter;
	double m_AntsPerSecondDt;

	public void LateUpdate()
	{
		
		
		// Prepare matrices as this is a render frame:
		using (s_RenderSetupMarker.Auto())
		{
			if (mustRerender)
			{
				mustRerender = false;
				float rerenderStart = Time.realtimeSinceStartup;

				simulationJobHandle.Complete();

				if (renderAnts)
				{
					var antMatricesJob = new AntMatricesJob
					{
						ants = ants,
						matrices = antMatrix,

						rotationResolution = rotationResolution,
						rotationMatrixLookup = rotationMatrixLookup,
						oneOverMapSize = 1f / mapSize,
					};
					antMatricesJob.Schedule(ants.Length, 4).Complete();
				}
				
				// NW: Apply the pheromone float array to the texture:
				using (s_PheromoneFloatToColorCopyMarker.Auto())
				{
					var colors = pheromoneTexture.GetPixelData<Color32>(0);
					var colorsPtr = colors.GetUnsafePtr();
					for (var i = 0; i < colors.Length; i++)
					{
						ref var color = ref UnsafeUtility.ArrayElementAsRef<Color32>(colorsPtr, i);
						color.r = (byte)(pheromones[i] * byte.MaxValue);
						color.b = ! addWallsToTexture || ! obstacleCollisionLookup.IsSet(i) ? (byte)0 : (byte)150;
						color.g = 0;
					}
					
					pheromoneTexture.Apply();
				}

				ConvergeTowards(ref rerenderElapsedSeconds, Time.realtimeSinceStartup - rerenderStart);
			}
		}

		
		using (s_GraphicsDrawMarker.Auto())
		{
			var renderStart = Time.realtimeSinceStartup;
			
			// NW: Create batches when we actually render them:
			if (renderAnts)
			{
				int start = 0;
				while (start < antMatrix.Length)
				{
					// NW: Annoying that there is a copy here, but it's a limitation of the Graphics.DrawMeshInstanced API AFAIK.
					// TODO - Remove copy to managed array once API supports.
					var end = math.min(antMatrix.Length, start + instancesPerBatch);
					var length = end - start;
					NativeArray<Matrix4x4>.Copy(antMatrix, start, reusableAntMatrix, 0, length);

					// var index1 = index / instancesPerBatch;
					// var index2 = index % instancesPerBatch;
					// if (ant.holdingResource == false)
					// {
					// 	targetPos = resourcePosition;
					//
					// 	// NWALKER - SPLIT presentation from simulation.
					// 	//antColors[index1][index2] += ((Vector4)searchColor * ant.brightness - antColors[index1][index2]) * .05f;
					// }
					// else
					// {
					// 	targetPos = colonyPosition;
					//
					// 	//antColors[index1][index2] += ((Vector4)carryColor * ant.brightness - antColors[index1][index2]) * .05f;
					// }
					//
					// reusableAntMatProps.SetVectorArray("_Color", );

					Graphics.DrawMeshInstanced(antMesh, 0, antMaterial, reusableAntMatrix, length, reusableAntMatProps);
					start = end;
				}
			}

			if (renderWalls)
			{
				for (var i = 0; i < obstacleMatrices.Length; i++)
				{
					Graphics.DrawMeshInstanced(obstacleMesh, 0, obstacleMaterial, obstacleMatrices[i]);
				}
			}

			Graphics.DrawMesh(colonyMesh, colonyMatrix, colonyMaterial, 0);
			Graphics.DrawMesh(resourceMesh, resourceMatrix, resourceMaterial, 0);
			
			ConvergeTowards(ref renderElapsedSeconds, Time.realtimeSinceStartup - renderStart);
		}
	}


	[BurstCompile]
	public struct AntDropPheromonesJob : IJob
	{
		[NoAlias, ReadOnly]
		public NativeArray<Ant> ants;

		[NoAlias]
		public NativeArray<float> pheromones;

		public float simulationDt, antSpeed, trailAddSpeed, trailDecay;
		public int mapSize;
		
		public void Execute()
		{
			var pheromonesPtr = pheromones.GetUnsafePtr();
			
			// NW: Ants spread pheromones in tiles around their current pos.
			// Writing to same array locations, so simpler to just do this in 1 flat update.
			// Forced single-threaded though!
			for (var i = 0; i < ants.Length; i++)
			{
				var position = ants[i].position;
				var x = Mathf.RoundToInt(position.x);
				var y = Mathf.RoundToInt(position.y);

				var isInBounds = CalculateIsInBounds(in x, in y, in mapSize, out var pheromoneIndex);
				if (!math.any(isInBounds))
					throw new InvalidOperationException("Attempting to DropPheromones outside the map bounds!");
				
				ref var color = ref UnsafeUtility.ArrayElementAsRef<float>(pheromonesPtr, pheromoneIndex);
				var excitement = math.@select(0.3f, 1f, ants[i].holdingResource) * ants[i].speed / antSpeed;
				color += (trailAddSpeed * excitement * simulationDt) * (1f - color);
				color = Mathf.Clamp01(color);
			}

			// NWALKER - See if this is faster if we make it its own job.
			// Decay:
			for (var i = 0; i < pheromones.Length; i++)
			{
				ref var pheromone = ref UnsafeUtility.ArrayElementAsRef<float>(pheromonesPtr, i);
				pheromone *= trailDecay;
			}
		}
	}

	public string DumpStatusText()
	{
		if (this && ants.IsCreated)
		{
			var antsPerRenderFrame = ants.Length * simulationStepsPerRenderFrame;
			var antsPerMicros = m_AntsPerSecond / 1000_000.0;
			return $"Fps: {(1.0 / Time.unscaledDeltaTime):0.00}\nAnts: {ants.Length}\nAvg Simulation Steps per Render Frame: {simulationStepsPerRenderFrame:0.000}  (max: {maxSimulationStepsPerFrame})\nSim CPU: {(simulationElapsedSeconds * 1000_000_000):#,000}ns per Sim ({(simulationElapsedSeconds * simulationStepsPerRenderFrame * 1000_000_000):#,000}ns per Render)\nRender CPU {(renderElapsedSeconds * 1000_000_000):#,000}ns per Render\nRerender CPU {(rerenderElapsedSeconds * 1000_000_000):#,000}ns per Render\nAnts per microSecond: {antsPerMicros:#,000.000}\nAnts per render: {(antsPerRenderFrame / 1000):0.00}k\nAnts per second: {m_AntsPerSecond / 1000000:#,##0.00}m";
		}
		return null;
	}
}
