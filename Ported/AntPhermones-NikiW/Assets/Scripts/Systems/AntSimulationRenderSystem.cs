using System;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using Object = UnityEngine.Object;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public unsafe class AntSimulationRenderSystem : SystemBase
{
    public const int k_InstancesPerBatch = 1023;

    static ProfilerMarker s_RenderSetupMarker = new ProfilerMarker("RenderSetup");
    static ProfilerMarker s_GraphicsDrawMarker = new ProfilerMarker("Graphics.DrawMeshInstanced");
    static ProfilerMarker s_PheromoneFloatToColorCopyMarker = new ProfilerMarker("Pheromone Float > Color32");
    static ProfilerMarker s_TextureApplyMarker = new ProfilerMarker("Texture2D.Apply");
    Vector4[] antColors;
    double antsPerSecond;
    long antsPerSecondCounter;
    double antsPerSecondDt;
    Matrix4x4 colonyMatrix;
    AntSimulationSystem m_AntSimulationSystem;
    ObstacleManagementSystem m_ObstacleManagementSystem;
    public Material obstacleMaterial;
    Material pheromoneMaterialInstance;
    Texture2D pheromoneTextureInstance;
    AverageState renderElapsedSeconds, rerenderElapsedSeconds;
    Matrix4x4 resourceMatrix;
    Matrix4x4[] reusableMatrices;
    NativeArray<Matrix4x4> rotationMatrixLookup;
    JobHandle simulationJobHandle;
    double simulationStepsPerRenderFrame;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_ObstacleManagementSystem = World.GetOrCreateSystem<ObstacleManagementSystem>();
        m_AntSimulationSystem = World.GetOrCreateSystem<AntSimulationSystem>();

        RequireSingletonForUpdate<AntSimulationParams>();
        RequireSingletonForUpdate<AntSimulationRuntimeData>();

        reusableMatrices = new Matrix4x4[k_InstancesPerBatch];
        new MaterialPropertyBlock();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Object.Destroy(pheromoneTextureInstance);
        Object.Destroy(pheromoneMaterialInstance);
    }

    protected override void OnUpdate()
    {
        var simParams = GetSingleton<AntSimulationParams>();
        var simRuntimeData = GetSingleton<AntSimulationRuntimeData>();
        var data = AntSimulationRenderer.Instance;
        
        if (pheromoneTextureInstance == null)
        {
            var colonyDiameter = simParams.colonyRadius*2;
            colonyMatrix = Matrix4x4.TRS((Vector2)simRuntimeData.colonyPos / simParams.mapSize, Quaternion.identity, new Vector3(colonyDiameter, colonyDiameter, .1f) / simParams.mapSize);
            resourceMatrix = Matrix4x4.TRS((Vector2)simRuntimeData.foodPosition / simParams.mapSize, Quaternion.identity, new Vector3(colonyDiameter, colonyDiameter, .1f) / simParams.mapSize);

            pheromoneTextureInstance = new Texture2D(simParams.mapSize, simParams.mapSize);
            pheromoneTextureInstance.wrapMode = TextureWrapMode.Mirror;
            Blit(pheromoneTextureInstance, new Color32());

            pheromoneMaterialInstance = new Material(data.basePheromoneMaterial);
            pheromoneMaterialInstance.mainTexture = pheromoneTextureInstance;
            data.textureRenderer.sharedMaterial = pheromoneMaterialInstance;
 
            rotationMatrixLookup = new NativeArray<Matrix4x4>(simParams.antRotationResolution, Allocator.Persistent);
            for (var i = 0; i < simParams.antRotationResolution; i++)
            {
                var angle = (float)i / simParams.antRotationResolution;
                angle *= 360f;
                rotationMatrixLookup[i] = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, angle), simParams.antSize);
            }
        }

        // Prepare matrices as this is a render frame:
        using (s_RenderSetupMarker.Auto())
        {
            if (pheromoneTextureInstance) // NWALKER: Find a way to determine if the entities have changed.
            {
                var rerenderStart = UnityEngine.Time.realtimeSinceStartupAsDouble;
     
                // NW: Apply the pheromone float array to the texture:
                using (s_PheromoneFloatToColorCopyMarker.Auto())
                {
                    var colors = pheromoneTextureInstance.GetPixelData<Color32>(0);
                    
                    new PheromoneToColorJob
                    {
                        colors = colors,
                        pheromonesColony = m_AntSimulationSystem.pheromonesColony,
                        pheromonesFood = m_AntSimulationSystem.pheromonesFood,
                        obstacleCollisionLookup = m_ObstacleManagementSystem.obstacleCollisionLookup,
                        addWallsToTexture = simParams.addWallsToTexture
                    }.Schedule(colors.Length, 4).Complete();
                

                    using (s_TextureApplyMarker.Auto())
                    {
                        pheromoneTextureInstance.Apply(); // NW: ~0.052ms!!
                    }
                }

                rerenderElapsedSeconds.AddSample(UnityEngine.Time.realtimeSinceStartupAsDouble - rerenderStart);
            }
        }

        using (s_GraphicsDrawMarker.Auto())
        {
            var renderStart = UnityEngine.Time.realtimeSinceStartupAsDouble;
            
            if (simParams.renderAnts)
            {
                RenderForAntsInQuery(simParams, data, m_AntSimulationSystem.antsHoldingFoodQuery, data.antMaerialHolding);
                RenderForAntsInQuery(simParams, data, m_AntSimulationSystem.antsSearchingQuery, data.antMaterialSearching);
            }

            if (simParams.renderObstacles && !m_ObstacleManagementSystem.obstaclesQuery.IsEmpty)
            {
                var obstacleTransforms = m_ObstacleManagementSystem.obstaclesQuery.ToComponentDataArray<AntSimulationTransform2D>(Allocator.TempJob);
                var obstacleMatrices = new NativeArray<Matrix4x4>(obstacleTransforms.Length, Allocator.TempJob);
                new ObstacleMatricesJob
                {
                    obstacleTransform2Ds = obstacleTransforms,
                    matrices = obstacleMatrices,
                    oneOverMapSize = 1f/simParams.mapSize,
                    obstacleRadius = simParams.obstacleRadius / simParams.mapSize,
                    
                }.Schedule(obstacleTransforms.Length, 4).Complete();
                RenderAllNativeArrayMatrices(obstacleMatrices, data.obstacleMesh, data.obstacleMaterial);
                obstacleMatrices.Dispose();
            }

            if (simParams.renderTargets)
            {
                Graphics.DrawMesh(data.colonyMesh, colonyMatrix, data.colonyMaterial, 0);
                Graphics.DrawMesh(data.resourceMesh, resourceMatrix, data.resourceMaterial, 0);
            }

            renderElapsedSeconds.AddSample(UnityEngine.Time.realtimeSinceStartupAsDouble - renderStart);
        }
    }

    void RenderForAntsInQuery(AntSimulationParams simParams, AntSimulationRenderer data, EntityQuery antQuery, Material antMaterial)
    {
        if (antQuery.IsEmpty) return;
        
        // NWALKER: hybrid render may be faster here!
        var antTransform2Ds = antQuery.ToComponentDataArray<AntSimulationTransform2D>(Allocator.TempJob);
        var antMatrices = new NativeArray<Matrix4x4>(antTransform2Ds.Length, Allocator.TempJob);
        var antMatricesJob = new AntMatricesJob
        {
            antTransform2Ds = antTransform2Ds,
            matrices = antMatrices,
            rotationMatrixLookup = rotationMatrixLookup,

            rotationResolution = simParams.antRotationResolution,
            oneOverMapSize = 1f / simParams.mapSize
        };
        antMatricesJob.Schedule(antTransform2Ds.Length, 4).Complete();
        antTransform2Ds.Dispose();

        RenderAllNativeArrayMatrices(antMatrices, data.antMesh, antMaterial);

        antMatrices.Dispose();
    }

    void RenderAllNativeArrayMatrices(NativeArray<Matrix4x4> matrices, Mesh mesh, Material material)
    {
        var start = 0;
        while (start < matrices.Length)
        {
            // NW: Annoying that there is a copy here, but it's a limitation of the Graphics.DrawMeshInstanced API AFAIK.
            // TODO - Remove copy to managed array once API supports.
            var end = math.min(matrices.Length, start + k_InstancesPerBatch);
            var length = end - start;
            NativeArray<Matrix4x4>.Copy(matrices, start, reusableMatrices, 0, length);
            Graphics.DrawMeshInstanced(mesh, 0, material, reusableMatrices, length);
            start = end;
        }
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
    public string DumpStatusText()
    {
        if (m_AntSimulationSystem.antsQuery == default) return "Waiting for AntSimulationSystem...";
        
        var antsCount = m_AntSimulationSystem.antsQuery.CalculateEntityCount();
        var antsPerRenderFrame = antsCount * simulationStepsPerRenderFrame;
        var antsPerMicros = antsPerSecond / 1000_000.0;

        return $"Fps: {1.0 / UnityEngine.Time.unscaledDeltaTime:0.00}, Sim Steps per RFrame: {simulationStepsPerRenderFrame:0.000}\nRender CPU: {renderElapsedSeconds.Average * 1000_000_000:#,000}ns per Render\nRerender CPU {rerenderElapsedSeconds.Average * 1000_000_000:#,000}ns per Render";
    }

    // NWALKER: Investigate https://github.com/stella3d/SharedArray
    [NoAlias]
    [BurstCompile]
    public struct AntMatricesJob : IJobParallelFor
    {
        [NoAlias]
        [ReadOnly]
        public NativeArray<AntSimulationTransform2D> antTransform2Ds;
        [NoAlias]
        [ReadOnly]
        public NativeArray<Matrix4x4> rotationMatrixLookup;

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
            var ant = antTransform2Ds[index];
            var matrix = GetRotationMatrix(ant.facingAngle);
            matrix.m03 = ant.position.x * oneOverMapSize;
            matrix.m13 = ant.position.y * oneOverMapSize;
            matrices[index] = matrix;
        }
    }
    
    [NoAlias]
    [BurstCompile]
    public struct ObstacleMatricesJob : IJobParallelFor
    {
        [NoAlias]
        [ReadOnly]
        public NativeArray<AntSimulationTransform2D> obstacleTransform2Ds;
        [NoAlias]
        [WriteOnly]
        public NativeArray<Matrix4x4> matrices;

        public float oneOverMapSize;
        public float obstacleRadius;

        public void Execute(int index)
        {
            var trans = obstacleTransform2Ds[index];
            matrices[index] = Matrix4x4.TRS(new Vector3(trans.position.x  * oneOverMapSize, trans.position.y  * oneOverMapSize), Quaternion.identity, new float3(obstacleRadius));
        }
    }

    [BurstCompile]
    [NoAlias]
    public struct PheromoneToColorJob : IJobParallelFor
    {
        // NW: Because the allocator is None, the job safety system will throw.
        [NoAlias, NativeDisableContainerSafetyRestriction]
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
}
