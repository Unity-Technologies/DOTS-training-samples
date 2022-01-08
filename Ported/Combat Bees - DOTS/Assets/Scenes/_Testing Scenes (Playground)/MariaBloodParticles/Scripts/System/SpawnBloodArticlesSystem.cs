using Combatbees.Testing.Maria;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using System.Collections;
using UnityEngine;
using Random = Unity.Mathematics.Random;


namespace Combatbees.Testing.Maria
{
    public partial class SpawnBloodArticlesSystem : SystemBase
    {
        // will be moved to field component in future 
        
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingletonBloodParticles>();
        }

        protected override void OnUpdate()
        {
            float3 pos = new float3(0);
            Debug.Log("System has successfully started: Maria");
            if (Input.GetKeyDown(KeyCode.Space)){
                Entities.ForEach((Entity entity, ref Bee bee, ref Translation translation) =>
                {
                    bee.dead = true;
                    pos = translation.Value;
                    Debug.Log("Maria: Space pressed, Bee-entity destroyed");
                    EntityManager.DestroyEntity(entity);           
                }).WithStructuralChanges().Run();

                Entities.ForEach((Entity entity, ref BloodSpawner bloodSpawner, in Translation trans) =>
                {
                    Entity e = EntityManager.Instantiate(bloodSpawner.bloodEntity); 
                    EntityManager.SetComponentData(e, new Translation
                    {
                        Value = trans.Value + new float3(
                            bloodSpawner.random.NextFloat(-3, 3),
                            0f,
                            bloodSpawner.random.NextFloat(-3, 3))       
                    });
                    // EntityManager.SetComponentData(e, new moveData
                    // {
                    //     moveSpeed = chaserSpawn.random.NextFloat(2, 6),
                    //     rotationSpeed = chaserSpawn.random.NextFloat(.3f, .7f)
                    // });
                }).WithStructuralChanges().Run();
                
                
            }
            
            
            
            
        }

        private void letBeeDisappear(){

        }
    }
}