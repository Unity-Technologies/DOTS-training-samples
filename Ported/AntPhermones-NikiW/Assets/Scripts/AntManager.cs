using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public unsafe class AntManager : MonoBehaviour
{
    const int k_InstancesPerBatch = 1023;

    static ProfilerMarker s_RenderSetupMarker = new ProfilerMarker("RenderSetup");
    static ProfilerMarker s_GraphicsDrawMarker = new ProfilerMarker("Graphics.DrawMeshInstanced");
    static ProfilerMarker s_PheromoneFloatToColorCopyMarker = new ProfilerMarker("Pheromone Float > Color32");
    static ProfilerMarker s_TextureApplyMarker = new ProfilerMarker("Texture2D.Apply");
    static ProfilerMarker s_ScheduleSimMarker = new ProfilerMarker("ScheduleSim");
    static ProfilerMarker s_BuildJobsMarker = new ProfilerMarker("BuildJobs");
    static ProfilerMarker s_TickSimulationMarker = new ProfilerMarker("TickSim");
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
    public int obstacleBucketResolution;
    public Vector3 antSize;
    public float antSpeed;
    [Range(0f, 1f)]
    public float antAccel;
    [FormerlySerializedAs("trailAddSpeedWithFood")]
    public float pheromoneAddSpeedWithFood;
    [FormerlySerializedAs("trailAddSpeedWhenSearching")]
    public float pheromoneAddSpeedWhenSearching;
    [FormerlySerializedAs("trailDecay")]
    [Range(0f, 1f)]
    public float pheromoneDecay;
    public float randomSteering;
    public float pheromoneSteerStrengthWithFood;
    public float pheromoneSteerStrengthWhenSearching;
    public float wallSteerStrength;
    public float unknownFoodResourceSteerStrength;
    public float seenTargetSteerStrength;
    public float colonySteerStrength;
    public int rotationResolution = 360;
    public int obstacleRingCount;
    [Range(0f, 1f)]
    public float obstaclesPerRing;
    public float obstacleRadius;
    public bool renderAnts = true;
    public bool renderWalls = true;
    public bool addWallsToTexture;
    public int simulationHz = 60;
    public ushort ticksForAntToDie = 2000;
    public int maxSimulationStepsPerFrame = 20;
    public int manualJobBatcherStep = 10;
    public bool useCompleteInsteadofSchedule = true;
    Vector4[] antColors;
    AntDropPheromonesJob antDropPheromonesJob;
    NativeArray<Matrix4x4> antMatrix;
    NativeArray<Ant> ants;
    AntSetFoodBitsJob antSetFoodBitsJob;
    double antsPerSecond;
    long antsPerSecondCounter;
    double antsPerSecondDt;
    AntUpdateJob antUpdateJob;
    Matrix4x4 colonyMatrix;
    float2 colonyPosition;

    /// <summary>
    ///     0 = Number of times food was picked up.
    ///     1 = Number of times food was dropped off.
    ///     2 = Number of times ants have died.
    /// </summary>
    NativeArray<long> counters;
    NativeBitArray isAntHoldingFood;
    bool mustRebuildMatrices;
    Material myPheromoneMaterial;
    NativeBitArray obstacleCollisionLookup;
    Matrix4x4[][] obstacleMatrices;
    NativeArray<float> pheromonesColony;
    NativeArray<float> pheromonesFood;
    Texture2D pheromoneTexture;
    NativeQueue<int> pickupDropoffRequests;
    Matrix4x4 resourceMatrix;
    float2 resourcePosition;
    MaterialPropertyBlock reusableAntMatProps;
    Matrix4x4[] reusableAntMatrix;
    NativeArray<Matrix4x4> rotationMatrixLookup;
    double simulationElapsedSeconds, renderElapsedSeconds, rerenderElapsedSeconds;
    JobHandle simulationJobHandle;
    double simulationStepsPerRenderFrame;
    float simulationTime;
    WeakenPotencyOfPheromonesJob weakenPotencyOfColonyPheromonesJob;
    WeakenPotencyOfPheromonesJob weakenPotencyOfFoodPheromonesJob;

    void Start()
    {
        GenerateObstacles();

        colonyPosition = new float2(mapSize * .5f);
        colonyMatrix = Matrix4x4.TRS((Vector2)colonyPosition / mapSize, Quaternion.identity, new Vector3(4f, 4f, .1f) / mapSize);
        var resourceAngle = Random.value * 2f * math.PI;
        resourcePosition = colonyPosition + new float2(math.cos(resourceAngle) * mapSize * .475f, math.sin(resourceAngle) * mapSize * .475f);
        resourceMatrix = Matrix4x4.TRS((Vector2)resourcePosition / mapSize, Quaternion.identity, new Vector3(4f, 4f, .1f) / mapSize);

        pheromoneTexture = new Texture2D(mapSize, mapSize);
        pheromoneTexture.wrapMode = TextureWrapMode.Mirror;
        Blit(pheromoneTexture, new Color32());

        counters = new NativeArray<long>(3, Allocator.Persistent);

        pheromonesColony = new NativeArray<float>(mapSize * mapSize, Allocator.Persistent);
        pheromonesFood = new NativeArray<float>(mapSize * mapSize, Allocator.Persistent);
        myPheromoneMaterial = new Material(basePheromoneMaterial);
        myPheromoneMaterial.mainTexture = pheromoneTexture;
        pheromoneRenderer.sharedMaterial = myPheromoneMaterial;
        ants = new NativeArray<Ant>(antCount, Allocator.Persistent);
        isAntHoldingFood = new NativeBitArray(antCount, Allocator.Persistent);
        pickupDropoffRequests = new NativeQueue<int>(Allocator.Persistent);

        antMatrix = new NativeArray<Matrix4x4>(antCount, Allocator.Persistent);
        antColors = new Vector4[antCount];
        reusableAntMatrix = new Matrix4x4[k_InstancesPerBatch];
        reusableAntMatProps = new MaterialPropertyBlock();

        for (var i = 0; i < antCount; i++) ants[i] = new Ant(new float2(Random.Range(-5f, 5f) + mapSize * .5f, Random.Range(-5f, 5f) + mapSize * .5f));

        rotationMatrixLookup = new NativeArray<Matrix4x4>(rotationResolution, Allocator.Persistent);
        for (var i = 0; i < rotationResolution; i++)
        {
            var angle = (float)i / rotationResolution;
            angle *= 360f;
            rotationMatrixLookup[i] = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, angle), antSize);
        }

        simulationTime = Time.time;

        // NW: "Build" the jobs here to avoid expensive copy.
        BuildJobs();
    }

    void Update()
    {
        var ticks = 0;
        for (; ticks < maxSimulationStepsPerFrame && simulationTime < Time.time; ticks++)
        {
            var updateStart = Time.realtimeSinceStartup;

            using (s_ScheduleSimMarker.Auto())
            {
                var dt = 1f / simulationHz;
                simulationTime += dt;
                antsPerSecondCounter += ants.Length;

                using (s_TickSimulationMarker.Auto())
                {
                    TickSimulation();
                }
            }

            // // NW: Trying out different forms and frequencies of job scheduling.
            //
            // if (manualJobBatcherStep > 0 && ticks % manualJobBatcherStep == 0)
            // {
            // 	if (useCompleteInsteadofSchedule)
            // 	{
            // 		using (s_ComputeSimMarker.Auto())
            // 		{
            // 			// When used with a relatively low manualJobBatcherStep, you get a nice balance between scheduling and job utilization.
            // 			// The problem is: Scheduling gets linearly more expensive with the more jobs queued in the handle, so after 10+ you spend more time scheduling.
            // 			simulationJobHandle.Complete();
            // 			simulationJobHandle = default;
            // 		}
            // 	}
            // 	else
            // 	{
            // 		// This is quite nice for shoving queued jobs through, but otherwise
            // 		// doesn't matter. The above handle overhead is far more impactful.
            // 		using (s_ScheduleBatchedJobsMarker.Auto())
            // 		{
            // 			JobHandle.ScheduleBatchedJobs();
            // 		}
            // 	}
            // }

            ConvergeTowards(ref simulationElapsedSeconds, Time.realtimeSinceStartup - updateStart);
        }

        antsPerSecondDt += Time.deltaTime;
        if (antsPerSecondDt >= 1)
        {
            ConvergeTowards(ref antsPerSecond, antsPerSecondCounter);
            antsPerSecondCounter = 0;
            antsPerSecondDt -= 1;
        }

        // using (s_ComputeSimMarker.Auto())
        // {
        // 	simulationJobHandle.Complete();
        // 	simulationJobHandle = default;
        // }

        ConvergeTowards(ref simulationStepsPerRenderFrame, ticks);
    }

    public void LateUpdate()
    {
        // Prepare matrices as this is a render frame:
        using (s_RenderSetupMarker.Auto())
        {
            if (mustRebuildMatrices)
            {
                mustRebuildMatrices = false;
                var rerenderStart = Time.realtimeSinceStartup;

                simulationJobHandle.Complete();

                if (renderAnts)
                {
                    var antMatricesJob = new AntMatricesJob
                    {
                        ants = ants,
                        matrices = antMatrix,

                        rotationResolution = rotationResolution,
                        rotationMatrixLookup = rotationMatrixLookup,
                        oneOverMapSize = 1f / mapSize
                    };
                    antMatricesJob.Schedule(ants.Length, CalculateBatchCount(ants.Length, 64)).Complete();
                }

                // NW: Apply the pheromone float array to the texture:
                using (s_PheromoneFloatToColorCopyMarker.Auto())
                {
                    var colors = pheromoneTexture.GetPixelData<Color32>(0);
                    new PheromoneToColorJob
                    {
                        colors = colors,
                        pheromonesColony = pheromonesColony,
                        pheromonesFood = pheromonesFood,
                        obstacleCollisionLookup = obstacleCollisionLookup,
                        addWallsToTexture = addWallsToTexture
                    }.Run(colors.Length);

                    using (s_TextureApplyMarker.Auto())
                    {
                        pheromoneTexture.Apply(); // NW: ~0.052ms!!
                    }
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
                var start = 0;
                while (start < antMatrix.Length)
                {
                    // NW: Annoying that there is a copy here, but it's a limitation of the Graphics.DrawMeshInstanced API AFAIK.
                    // TODO - Remove copy to managed array once API supports.
                    var end = math.min(antMatrix.Length, start + k_InstancesPerBatch);
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
                for (var i = 0; i < obstacleMatrices.Length; i++)
                    Graphics.DrawMeshInstanced(obstacleMesh, 0, obstacleMaterial, obstacleMatrices[i]);

            Graphics.DrawMesh(colonyMesh, colonyMatrix, colonyMaterial, 0);
            Graphics.DrawMesh(resourceMesh, resourceMatrix, resourceMaterial, 0);

            ConvergeTowards(ref renderElapsedSeconds, Time.realtimeSinceStartup - renderStart);
        }
    }

    void OnDestroy()
    {
        Destroy(pheromoneTexture);
        Destroy(myPheromoneMaterial);

        if (ants.IsCreated) ants.Dispose();
        if (pheromonesColony.IsCreated) pheromonesColony.Dispose();
        if (pheromonesFood.IsCreated) pheromonesFood.Dispose();
        if (rotationMatrixLookup.IsCreated) rotationMatrixLookup.Dispose();
        if (antMatrix.IsCreated) antMatrix.Dispose();
        if (obstacleCollisionLookup.IsCreated) obstacleCollisionLookup.Dispose();
        if (isAntHoldingFood.IsCreated) isAntHoldingFood.Dispose();
        if (pickupDropoffRequests.IsCreated) pickupDropoffRequests.Dispose();
        if (counters.IsCreated) counters.Dispose();
    }

    void GenerateObstacles()
    {
        // TEMP HACK:
        obstacleBucketResolution = mapSize;

        // Generate obstacles in a circle:
        var obstaclePositions = new NativeList<float2>(1024, Allocator.Temp);
        for (var i = 1; i <= obstacleRingCount; i++)
        {
            var ringRadius = i / (obstacleRingCount + 1f) * (mapSize * .5f);
            var circumference = ringRadius * 2f * math.PI;
            var maxCount = Mathf.CeilToInt(circumference / (2f * obstacleRadius) * 2f);
            var offset = Random.Range(0, maxCount);
            var holeCount = Random.Range(1, 3);
            for (var j = 0; j < maxCount; j++)
            {
                var t = (float)j / maxCount;
                if (t * holeCount % 1f < obstaclesPerRing)
                {
                    var angle = (j + offset) / (float)maxCount * (2f * math.PI);
                    var obstaclePosition = new float2(mapSize * .5f + math.cos(angle) * ringRadius, mapSize * .5f + math.sin(angle) * ringRadius);
                    obstaclePositions.Add(obstaclePosition);

                    //Debug.DrawRay(obstacle.position / mapSize,-Vector3.forward * .05f,Color.green,10000f);
                }
            }
        }

        obstacleMatrices = new Matrix4x4[Mathf.CeilToInt((float)obstaclePositions.Length / k_InstancesPerBatch)][];
        for (var i = 0; i < obstacleMatrices.Length; i++)
        {
            obstacleMatrices[i] = new Matrix4x4[math.min(k_InstancesPerBatch, obstaclePositions.Length - i * k_InstancesPerBatch)];
            for (var j = 0; j < obstacleMatrices[i].Length; j++) obstacleMatrices[i][j] = Matrix4x4.TRS((Vector2)obstaclePositions[i * k_InstancesPerBatch + j] / mapSize, Quaternion.identity, new Vector3(obstacleRadius * 2f, obstacleRadius * 2f, 1f) / mapSize);
        }

        // Buckets:
        // NW: Max bucket size seems to be around 4, and the vast majority are empty.
        // Thus, lets just simplify to a collision map.
        obstacleCollisionLookup = new NativeBitArray(obstacleBucketResolution * obstacleBucketResolution, Allocator.Persistent);

        for (var i = 0; i < obstaclePositions.Length; i++)
        {
            var pos = obstaclePositions[i];

            for (var x = Mathf.FloorToInt((pos.x - obstacleRadius) / mapSize * obstacleBucketResolution); x <= Mathf.FloorToInt((pos.x + obstacleRadius) / mapSize * obstacleBucketResolution); x++)
            {
                if (x < 0 || x >= obstacleBucketResolution)
                    continue;

                for (var y = Mathf.FloorToInt((pos.y - obstacleRadius) / mapSize * obstacleBucketResolution); y <= Mathf.FloorToInt((pos.y + obstacleRadius) / mapSize * obstacleBucketResolution); y++)
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
        var log = $"OBSTACLES: {obstaclePositions.Length}\nCOLLISION BUCKETS: [{obstacleCollisionLookup.Length}";
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

    static void Blit(Texture2D texture2D, Color32 color32)
    {
        var color = texture2D.GetPixelData<Color32>(0);

        // NWalker; Find the burst compile single method for this.
        for (var i = 0; i < color.Length; i++)
            color[i] = color32;
        texture2D.Apply();
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

    void TickSimulation()
    {
#if UNITY_EDITOR
        BuildJobs();
#endif
        using (s_BuildJobsMarker.Auto())
        {
            antUpdateJob.perFrameRandomSeed = (uint)(Random.value * uint.MaxValue);
        }

        using (s_ScheduleSimMarker.Auto())
        {
            simulationJobHandle = antUpdateJob.Schedule(ants.Length, CalculateBatchCount(ants.Length, 64), simulationJobHandle);
            JobHandle.ScheduleBatchedJobs();
        }

        simulationJobHandle.Complete();
        simulationJobHandle = default;

        JobHandle weakenPotencyOfColonyPheromonesHandle;
        using (s_ScheduleSimMarker.Auto())
        {
            simulationJobHandle = antSetFoodBitsJob.Schedule();
            weakenPotencyOfColonyPheromonesHandle = weakenPotencyOfColonyPheromonesJob.Schedule();
            JobHandle.ScheduleBatchedJobs();
        }

        weakenPotencyOfFoodPheromonesJob.Run();

        weakenPotencyOfColonyPheromonesHandle.Complete();

        simulationJobHandle.Complete();
        simulationJobHandle = default;

        //simulationJobHandle = antDropPheromonesJob.Schedule(ants.Length, CalculateBatchCount(ants.Length, 64));
        antDropPheromonesJob.Run(ants.Length);

        mustRebuildMatrices = true;
    }

    /// <summary>
    ///     NW: Rebuilding is 0.002ms.
    /// </summary>
    void BuildJobs()
    {
        using (s_BuildJobsMarker.Auto())
        {
            antUpdateJob = new AntUpdateJob
            {
                ants = ants,
                isAntHoldingFood = isAntHoldingFood,
                obstacleCollisionLookup = obstacleCollisionLookup,
                pickupDropoffRequests = pickupDropoffRequests.AsParallelWriter(),
                pheromonesColony = pheromonesColony,
                pheromonesFood = pheromonesFood,
                counters = counters,

                antSpeed = antSpeed,
                pheromoneSteerStrengthWithFood = pheromoneSteerStrengthWithFood,
                pheromoneSteerStrengthWhenSearching = pheromoneSteerStrengthWhenSearching,
                wallSteerStrength = wallSteerStrength,
                obstacleBucketResolution = obstacleBucketResolution,
                randomSteeringStrength = randomSteering,
                mapSize = mapSize,
                unknownFoodResourceSteerStrength = unknownFoodResourceSteerStrength,
                seenTargetSteerStrength = seenTargetSteerStrength,
                colonySteerStrength = colonySteerStrength,
                ticksForAntToDie = ticksForAntToDie,

                colonyPosition = colonyPosition,
                resourcePosition = resourcePosition
            };

            antSetFoodBitsJob = new AntSetFoodBitsJob
            {
                pickupDropoffRequests = pickupDropoffRequests,
                isAntHoldingFood = isAntHoldingFood,
                counters = counters,
            };

            weakenPotencyOfColonyPheromonesJob = new WeakenPotencyOfPheromonesJob
            {
                pheromones = pheromonesColony,
                pheromoneDecay = pheromoneDecay
            };
            weakenPotencyOfFoodPheromonesJob = new WeakenPotencyOfPheromonesJob
            {
                pheromones = pheromonesFood,
                pheromoneDecay = pheromoneDecay
            };

            antDropPheromonesJob = new AntDropPheromonesJob
            {
                ants = ants,
                pheromonesColony = pheromonesColony,
                pheromonesFood = pheromonesFood,
                isAntHoldingFood = isAntHoldingFood,
                mapSize = mapSize,
                pheromoneAddSpeedWithFood = pheromoneAddSpeedWithFood,
                pheromoneAddSpeedWhenSearching = pheromoneAddSpeedWhenSearching,
                simulationDt = 1f / simulationHz,
                ticksForAntToDie = ticksForAntToDie
            };
        }
    }

    static int CalculateBatchCount(int arrayLength, int min)
    {
        return math.max(min, Mathf.CeilToInt((float)arrayLength / JobsUtility.JobWorkerCount));
    }

    static void ConvergeTowards(ref double value, double target)
    {
        value = math.lerp(value, target, .02f);
    }

    public string DumpStatusText()
    {
        if (this && ants.IsCreated)
        {
            var antsPerRenderFrame = ants.Length * simulationStepsPerRenderFrame;
            var antsPerMicros = antsPerSecond / 1000_000.0;
            return $"Fps: {1.0 / Time.unscaledDeltaTime:0.00}, Ants: {ants.Length} ({counters[2]} respawned)\nFood Found: {counters[0]}, Gathered: {counters[1]}\nSim Steps per RFrame: {simulationStepsPerRenderFrame:0.000}  (max: {maxSimulationStepsPerFrame})\nSim CPU: {simulationElapsedSeconds * 1000_000_000:#,000}ns per Sim ({simulationElapsedSeconds * simulationStepsPerRenderFrame * 1000_000_000:#,000}ns per Render)\nRender CPU {renderElapsedSeconds * 1000_000_000:#,000}ns per Render\nRerender CPU {rerenderElapsedSeconds * 1000_000_000:#,000}ns per Render\nAnts per microSecond: {antsPerMicros:#,000.000}\nAnts per render: {antsPerRenderFrame / 1000:0.00}k\nAnts per second: {antsPerSecond / 1000000:#,##0.00}m";
        }

        return null;
    }

    // NWALKER: Investigate https://github.com/stella3d/SharedArray
    [NoAlias]
    [BurstCompile]
    public struct AntMatricesJob : IJobParallelFor
    {
        // NWalker: float4x4?
        [NoAlias]
        [ReadOnly]
        public NativeArray<Matrix4x4> rotationMatrixLookup;

        [NoAlias]
        [ReadOnly]
        public NativeArray<Ant> ants;

        [NoAlias]
        [WriteOnly]
        public NativeArray<Matrix4x4> matrices;

        public float oneOverMapSize;

        public int rotationResolution;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Matrix4x4 GetRotationMatrix(float angle)
        {
            angle /= math.PI * 2f;
            angle -= math.floor(angle);
            angle *= rotationResolution;
            return rotationMatrixLookup[(int)angle % rotationResolution];
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

    [NoAlias]
    [BurstCompile]
    struct AntUpdateJob : IJobParallelFor
    {
        [NoAlias]
        public NativeArray<Ant> ants;

        [NoAlias]
        [ReadOnly]
        public NativeBitArray obstacleCollisionLookup;

        [NoAlias]
        [ReadOnly]
        public NativeArray<float> pheromonesColony;

        [NoAlias]
        [ReadOnly]
        public NativeArray<float> pheromonesFood;

        [NoAlias]
        [ReadOnly]
        public NativeBitArray isAntHoldingFood;

        [NoAlias]
        [WriteOnly]
        public NativeQueue<int>.ParallelWriter pickupDropoffRequests;

        [NoAlias]
        [WriteOnly]
        public NativeArray<long> counters;

        public float antSpeed, pheromoneSteerStrengthWithFood, wallSteerStrength, randomSteeringStrength;
        public int obstacleBucketResolution;
        public float unknownFoodResourceSteerStrength;
        public float seenTargetSteerStrength;
        public float colonySteerStrength;
        public uint perFrameRandomSeed;

        public int mapSize;
        public float2 colonyPosition, resourcePosition;
        public ushort ticksForAntToDie;
        public float pheromoneSteerStrengthWhenSearching;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        float PheromoneSteering(Ant ant, float distance, NativeArray<float> pheromones)
        {
            var output = 0f;
            var quarterPi = math.PI * .25f;
            for (var i = -1; i <= 1; i += 2)
            {
                var angle = ant.facingAngle + i * quarterPi;
                var testX = (int)(ant.position.x + math.cos(angle) * distance);
                var testY = (int)(ant.position.y + math.sin(angle) * distance);
                var isInBounds = CalculateIsInBounds(in testX, in testY, in mapSize, out var index);
                if (math.all(isInBounds)) output += pheromones[index] * i;
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
                if (!math.any(isInBounds) || math.all(isInBounds) && obstacleCollisionLookup.IsSet(obstacleCollisionIndex))
                    output -= i;

                //Debug.DrawLine((Vector2)ant.position / mapSize, new Vector2(testX, testY) / mapSize, Color.red, 0.2f);
            }

            return output;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool DirectPathToTargetIsBlocked(float2 origin, float2 target, out float dist)
        {
            var dx = target.x - origin.x;
            var dy = target.y - origin.y;
            dist = math.sqrt(dx * dx + dy * dy);

            // NW: Test to see if step count can be generalized.
            var stepCount = Mathf.CeilToInt(dist * .5f);
            for (var i = 0; i < stepCount; i++)
            {
                var t = (float)i / stepCount;
                var testX = (int)(origin.x + dx * t);
                var testY = (int)(origin.y + dy * t);
                var isInBounds = CalculateIsInBounds(in testX, in testY, in obstacleBucketResolution, out var collisionIndex);
                if (!math.all(isInBounds) || obstacleCollisionLookup.IsSet(collisionIndex))
                    return true;
            }

            return false;
        }

        public void Execute(int index)
        {
            const float piX2 = math.PI * 2f;
            ref var ant = ref UnsafeUtility.ArrayElementAsRef<Ant>(ants.GetUnsafePtr(), index);

            var isHoldingFood = isAntHoldingFood.IsSet(index);
            var targetPos = math.select(resourcePosition, colonyPosition, isHoldingFood);
            var steerTowardsTargetWeight = math.select(unknownFoodResourceSteerStrength, colonySteerStrength, isHoldingFood);

            // NW: Every time we touch a colony or food, we get EXTRA excited. Linear dropoff per ant = Exponential drop-off. This should allow ants to find their way home.

            if (ant.lifeTicks <= 0)
            {
                // Ant died!
                ant.position = colonyPosition;
                isHoldingFood = false;
                ant.lifeTicks = (ushort)(Squirrel3.NextFloat((uint)index, perFrameRandomSeed, 0.5f, 1.5f) * ticksForAntToDie);

                AtomicIncrementCounter(2, counters);
            }

            ant.lifeTicks--;

            // NW: Add some random rotation to indicate "curiosity"...
            var randRotation = Squirrel3.NextFloat((uint)index, perFrameRandomSeed, -randomSteeringStrength, randomSteeringStrength);
            ant.facingAngle += randRotation;

            // Avoid walls:
            var wallSteering = WallSteering(ant, 1.5f);
            ant.facingAngle += wallSteering * wallSteerStrength;

            // Steer towards target if the ant can "see" it.
            if (DirectPathToTargetIsBlocked(ant.position, targetPos, out var distanceToTarget))
            {
                // Steer out of the way of obstacles and map boundaries if we can't see the target.
                var pheroSteering = PheromoneSteering(ant, 3f, isHoldingFood ? pheromonesColony : pheromonesFood);
                ant.facingAngle += pheroSteering * math.select(pheromoneSteerStrengthWhenSearching, pheromoneSteerStrengthWithFood, isHoldingFood);
            }
            else
            {
                // Steer towards the target.
                steerTowardsTargetWeight = seenTargetSteerStrength;

                // Pick up / Drop off food only when we're within LOS and within distance.
                if (distanceToTarget < 3f)
                {
                    pickupDropoffRequests.Enqueue(index);
                    
                    // The ant either found food or returned home with it, so it gets to live.
                    ant.lifeTicks = (ushort)(Squirrel3.NextFloat((uint)index, perFrameRandomSeed, 0.5f, 1.5f) * ticksForAntToDie);
                }
            }

            // Head towards a target.
            // Much more weight if we've seen it.
            // Much more weight if it's a known location (colony). E.g. Simulating memory.
            var targetAngle = math.atan2(targetPos.y - ant.position.y, targetPos.x - ant.position.x);
            if (targetAngle - ant.facingAngle > math.PI)
            {
                ant.facingAngle -= piX2;
            }
            else if (targetAngle - ant.facingAngle < -math.PI)
            {
                ant.facingAngle += piX2;
            }
            else
            {
                if (math.abs(targetAngle - ant.facingAngle) < math.PI * .5f) ant.facingAngle += (targetAngle - ant.facingAngle) * steerTowardsTargetWeight;
            }

            var velocity = new float2(math.cos(ant.facingAngle), math.sin(ant.facingAngle));
            velocity = math.normalizesafe(velocity) * math.select(antSpeed, antSpeed * 0.6f, isHoldingFood);

            // Move, then undo it later if we moved into invalid.
            var newPos = ant.position + velocity;

            // NW: If we're now colliding with a wall, bounce:
            var isInBounds = CalculateIsInBounds(in newPos, in obstacleBucketResolution, out var collisionIndex);
            if (!math.all(isInBounds))
                ant.facingAngle = math.atan2(math.select(-velocity.y, velocity.y, isInBounds.y), math.select(-velocity.x, velocity.x, isInBounds.x));
            else if (obstacleCollisionLookup.IsSet(collisionIndex))
                ant.facingAngle += math.PI;

            //Debug.DrawLine((Vector2)ant.position / mapSize, (Vector2)(ant.position + velocity) / mapSize, Color.red, 0.2f);
            else
                ant.position = newPos;
        }


    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void AtomicIncrementCounter(int index, NativeArray<long> countersArr)
    {
        ref var count = ref UnsafeUtility.ArrayElementAsRef<long>(countersArr.GetUnsafePtr(), index);
        Interlocked.Increment(ref count);
    }
    
    [BurstCompile]
    [NoAlias]
    public struct PheromoneToColorJob : IJobParallelFor
    {
        [NoAlias]
        public NativeArray<Color32> colors;

        [NoAlias]
        [ReadOnly]
        public NativeArray<float> pheromonesColony;

        [NoAlias]
        [ReadOnly]
        public NativeBitArray obstacleCollisionLookup;

        public bool addWallsToTexture;

        [NoAlias]
        [ReadOnly]
        public NativeArray<float> pheromonesFood;

        public void Execute(int index)
        {
            var colorsPtr = colors.GetUnsafePtr();

            //for (var i = 0; i < colors.Length; i++)
            {
                ref var color = ref UnsafeUtility.ArrayElementAsRef<Color32>(colorsPtr, index);
                color.r = (byte)(pheromonesColony[index] * byte.MaxValue);
                color.b = !addWallsToTexture || !obstacleCollisionLookup.IsSet(index) ? (byte)0 : (byte)150;
                color.g = (byte)(pheromonesFood[index] * byte.MaxValue);
            }
        }
    }

    [BurstCompile]
    [NoAlias]
    public struct WeakenPotencyOfPheromonesJob : IJob
    {
        [NoAlias]
        public NativeArray<float> pheromones;

        public float pheromoneDecay;

        public void Execute()
        {
            var unsafePtr = pheromones.GetUnsafePtr();
            for (var index = 0; index < pheromones.Length; index++)
            {
                ref var pheromone = ref UnsafeUtility.ArrayElementAsRef<float>(unsafePtr, index);
                pheromone = math.clamp(pheromone * pheromoneDecay, 0, 1);
            }
        }
    }

    [BurstCompile]
    [NoAlias]
    public struct AntDropPheromonesJob : IJobParallelFor
    {
        [NoAlias]
        [ReadOnly]
        public NativeArray<Ant> ants;

        [NoAlias]
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<float> pheromonesColony;

        [NoAlias]
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<float> pheromonesFood;

        [NoAlias]
        [ReadOnly]
        public NativeBitArray isAntHoldingFood;

        public float simulationDt, pheromoneAddSpeedWithFood, pheromoneAddSpeedWhenSearching;
        public int mapSize;
        public ushort ticksForAntToDie;

        public void Execute(int index)
        {
            // NW: Ants spread pheromones in tiles around their current pos.
            // Writing to same array locations, so be aware that we're clobbering data here!
            // However, after discussion on slack, it's an acceptable tradeoff.
            var ant = ants[index];
            var position = ant.position;
            var x = Mathf.RoundToInt(position.x);
            var y = Mathf.RoundToInt(position.y);

            var isInBounds = CalculateIsInBounds(in x, in y, in mapSize, out var pheromoneIndex);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (!math.any(isInBounds))
                throw new InvalidOperationException("Attempting to DropPheromones outside the map bounds!");
#endif

            // Not good to have un-inlinable inner loop.
            var isHoldingFood = isAntHoldingFood.IsSet(index);
            var targetArray = isHoldingFood ? pheromonesFood : pheromonesColony;

            ref var color = ref UnsafeUtility.ArrayElementAsRef<float>(targetArray.GetUnsafePtr(), pheromoneIndex);
            var excitement = math.select(pheromoneAddSpeedWhenSearching, pheromoneAddSpeedWithFood, isHoldingFood);

            //excitement *= Mathf.InverseLerp(0, ticksForAntToDie, ant.ticksSinceFoundSomething);
            color += excitement * simulationDt;
        }
    }

    [NoAlias]
    [BurstCompile]
    public struct AntSetFoodBitsJob : IJob
    {
        [NoAlias]
        public NativeQueue<int> pickupDropoffRequests;

        [NoAlias]
        public NativeBitArray isAntHoldingFood;
        
        [NoAlias]
        public NativeArray<long> counters;

        public void Execute()
        {
            while (pickupDropoffRequests.TryDequeue(out var index))
            {
                var isHoldingFood = isAntHoldingFood.IsSet(index);
                isAntHoldingFood.Set(index, !isHoldingFood);
                AtomicIncrementCounter(math.select(0, 1, isHoldingFood), counters);
            }
        }
    }
}
