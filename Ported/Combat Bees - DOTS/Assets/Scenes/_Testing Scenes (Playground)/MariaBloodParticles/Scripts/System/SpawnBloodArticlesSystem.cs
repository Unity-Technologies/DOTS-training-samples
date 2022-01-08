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
            Debug.Log("System has successfully started: Maria");
            letBeeDisappearAndInitBloodParticles();
            moveBloodParticles();


        }

        private void letBeeDisappearAndInitBloodParticles(){
            if (Input.GetKeyDown(KeyCode.Space)){
                float3 pos = new float3(0);
                float steps = 0f;
                Entities.ForEach((Entity entity, ref Bee bee, ref Translation translation) =>
                {
                    bee.dead = true;
                    pos = translation.Value;
                    Debug.Log("Maria: Space pressed, Bee-entity destroyed");
                    // EntityManager.DestroyEntity(entity);           
                }).WithStructuralChanges().Run();

                Entities.ForEach((Entity entity, ref BloodSpawner bloodSpawner, in Translation trans) =>
                {
                    // To make the bloodparticles only spawn once 
                    if (!bloodSpawner.dead)
                    {
                        bloodSpawner.dead = true;
                        for (int i = 0; i < bloodSpawner.amountParticles; i++)
                        {
                            Entity e = EntityManager.Instantiate(bloodSpawner.bloodEntity);
                            EntityManager.SetComponentData(e, new Translation
                            {
                                Value = trans.Value + new float3(pos)    
                            });
                            float x = bloodSpawner.random.NextFloat(-3, 3);
                            float y = bloodSpawner.random.NextFloat(0, 3);
                            float z = bloodSpawner.random.NextFloat(-3, 3);
                            
                            EntityManager.SetComponentData(e, new BloodParticle
                            {
                                direction = new float3(x, y, z),
                                destination = trans.Value + new float3(x+pos.x, y + pos.y, z + pos.z),
                                steps = bloodSpawner.steps
                            });
                        }
                    }
                }).WithStructuralChanges().Run();
            }
        }

        private void moveBloodParticles()
        {
            Entities.ForEach((Entity entity, ref BloodParticle bloodparticle, ref Translation translation) =>
            {
                if (0 <= bloodparticle.steps )
                {
                    bloodparticle.steps -= Time.DeltaTime;
                    translation.Value = translation.Value + new float3(bloodparticle.direction * Time.DeltaTime);
                }
                else
                {
                    if(translation.Value.y > 0) translation.Value.y += -10 * Time.DeltaTime;
                }
            }).WithStructuralChanges().Run();
        }
    }
}