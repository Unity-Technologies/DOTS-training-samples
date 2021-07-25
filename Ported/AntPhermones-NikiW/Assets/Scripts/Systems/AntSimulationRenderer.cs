using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class AntSimulationRenderer : MonoBehaviour
{
    public static AntSimulationRenderer Instance { get; private set; }
    
    public Material basePheromoneMaterial;
    public Material colonyMaterial; 
    public Mesh colonyMesh;
    public Material resourceMaterial;
    public Mesh resourceMesh;
    public Renderer textureRenderer;
    public Mesh antMesh;
    public Material antMaterialSearching;
    [FormerlySerializedAs("antMaerialHolding")]
    public Material antMaterialHolding;
    public Mesh obstacleMesh;
    public Material obstacleMaterial;

    void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = default;
    }
    
    void OnDrawGizmos()
    {
        if (! Application.isPlaying) return;
    	
        // 	for (int x = 0; x < obstacleBucketResolution; x++)
        // for (int y = 0; y < obstacleBucketResolution; y++)
        // {
        // 	Gizmos.color = new Color(1f, 0f, 0f, 0.27f);
        // 	var isInBounds = CalculateIsInBounds(in x, in y, in obstacleBucketResolution, out var index);
        // 	if (math.all(isInBounds) && obstacleCollisionLookup.IsSet(index))
        // 	{
        // 		Gizmos.DrawSphere(new Vector3(x, y, 0), 1f);
        // 	}
        // }

        foreach (var world in World.All)
        {
            var antSimulationSystem = world.GetExistingSystem<AntSimulationSystem>();
            
            if (antSimulationSystem != null && antSimulationSystem.TryGetSingleton<AntSimulationParams>(out var simParams))
            {
                DrawAnts(antSimulationSystem.antsSearchingQuery, new Color(0.52f, 0f, 0.05f), simParams);
                DrawAnts(antSimulationSystem.antsHoldingFoodQuery, Color.yellow, simParams);
            }
        }
    }

    static void DrawAnts(EntityQuery query, Color color, AntSimulationParams simParams)
    {
        using var antPositions = query.ToComponentDataArray<AntSimulationTransform2D>(Allocator.Temp);
        var oneOverMapSize = 1f/simParams.mapSize;
        var smallGizmo = simParams.antSize.x * 0.2f;
        
        for (int i = 0; i < antPositions.Length; i++)
        {
            var ant = antPositions[i];
            Gizmos.color = color;
            Gizmos.DrawSphere((Vector2)ant.position * oneOverMapSize, smallGizmo);
        }
    }
}
