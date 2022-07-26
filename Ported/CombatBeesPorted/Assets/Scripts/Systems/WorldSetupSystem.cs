using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

[BurstCompile]
partial struct WorldSetupSystem : ISystem
{
    private EntityQuery _baseColorQuery;
    private EntityQuery _translationQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        
        _baseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
        _translationQuery = state.GetEntityQuery(ComponentType.ReadOnly<Translation>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var field = config.PlayVolume * 2;
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var queryColorMask = _baseColorQuery.GetEntityQueryMask();
        var queryTranslationMask = _translationQuery.GetEntityQueryMask();
        
        // spawn blue bees
        var blueBees = CollectionHelper.CreateNativeArray<Entity>(config.StartingBeeCount, Allocator.Temp);
        ecb.Instantiate(config.BeePrefab, blueBees);
        var blueTeamColor = new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)Color.blue };
        Vector3 pos = Vector3.right * -field.x * .4f;
        foreach (var blueBee in blueBees)
        {
            ecb.SetComponentForLinkedEntityGroup(blueBee, queryColorMask, blueTeamColor);
            ecb.SetComponentForLinkedEntityGroup(blueBee, queryTranslationMask, new Translation { Value = pos });
            ecb.SetComponentEnabled<BeeStateAttacking>(blueBee, false);
            ecb.SetComponentEnabled<BeeStateGathering>(blueBee, false);
            ecb.SetComponentEnabled<BeeStateReturning>(blueBee, false);
            ecb.SetComponentEnabled<BeeStateDead>(blueBee, false);
            ecb.AddComponent<BlueTeam>(blueBee);
        }
        
        // spawn yellow bees
        var yellowBees  = CollectionHelper.CreateNativeArray<Entity>(config.StartingBeeCount, Allocator.Temp);
        ecb.Instantiate(config.BeePrefab, yellowBees);
        var yellowTeamColor = new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)Color.yellow };
        pos = Vector3.right * (-field.x * .4f + field.x * .8f);
        foreach (var yellowBee in yellowBees)
        {
            ecb.SetComponentForLinkedEntityGroup(yellowBee, queryColorMask, yellowTeamColor);
            ecb.SetComponentForLinkedEntityGroup(yellowBee, queryTranslationMask, new Translation { Value = pos});
            ecb.SetComponentEnabled<BeeStateAttacking>(yellowBee, false);
            ecb.SetComponentEnabled<BeeStateGathering>(yellowBee, false);
            ecb.SetComponentEnabled<BeeStateReturning>(yellowBee, false);
            ecb.SetComponentEnabled<BeeStateDead>(yellowBee, false);
            ecb.AddComponent<YellowTeam>(yellowBee);
        }

        float resourceSize = 0.75f;
        Vector2Int gridCounts = Vector2Int.RoundToInt(new Vector2(field.x, field.z) / resourceSize);
        Vector2 gridSize = new Vector2(field.x/gridCounts.x,field.z/gridCounts.y);
        Vector2 minGridPos = new Vector2((gridCounts.x-1f)*-.5f*gridSize.x,(gridCounts.y-1f)*-.5f*gridSize.y);
        var resourceColor = new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)new Color(0.1572f, 0.4191f, 0.0739f, 1.0f) };
        var resources = CollectionHelper.CreateNativeArray<Entity>(config.StartingResourceCount, Allocator.Temp);
        ecb.Instantiate(config.ResourcePrefab, resources);
        foreach (var resource in resources)
        {
            ecb.SetComponentForLinkedEntityGroup(resource, queryColorMask, resourceColor);
            Vector3 startPos = new Vector3(minGridPos.x * .25f + UnityEngine.Random.value * field.x * .25f, UnityEngine.Random.value * 10f,
                minGridPos.y + UnityEngine.Random.value * field.z);
            ecb.SetComponentForLinkedEntityGroup(resource, queryTranslationMask, new Translation { Value = startPos});
        }
        
        state.Enabled = false;
    }
}