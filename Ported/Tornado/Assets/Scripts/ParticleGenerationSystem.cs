using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ParticleGenerationSystem : ComponentSystem
{
    private EntityQuery settingsQuery;
    private int numParticles = 1000;
	
    public struct State : IComponentData
    {
    }

    protected override void OnCreate()
    {
        base.OnCreate();
		
        settingsQuery = GetEntityQuery( new EntityQueryDesc {All=new ComponentType[]{typeof(ParticleSettings)}, 
            None=new ComponentType[]{typeof(State)}});
		
        RequireForUpdate(settingsQuery);
    }
    
    protected override void OnUpdate()
    {
        var settingsEntity = settingsQuery.GetSingletonEntity();
        var particleSettings = EntityManager.GetComponentData<ParticleSettings>(settingsEntity);
        var rand = new Unity.Mathematics.Random((uint)DateTime.UtcNow.Ticks);
        
        for (int i = 0; i < numParticles; i++) 
        {
            float3 pos = new float3(UnityEngine.Random.Range(-50f,50f),
                UnityEngine.Random.Range(0f,50f),UnityEngine.Random.Range(-50f,50f));

            var instance = PostUpdateCommands.Instantiate(particleSettings.prefab);
            
            PostUpdateCommands.SetComponent(instance, new Translation {Value = pos});
            PostUpdateCommands.AddComponent(instance, new Particle {RandomOffset = rand.NextFloat(0f, 1f)});
        }
        
        PostUpdateCommands.AddComponent<State>(settingsEntity);
    }
}
