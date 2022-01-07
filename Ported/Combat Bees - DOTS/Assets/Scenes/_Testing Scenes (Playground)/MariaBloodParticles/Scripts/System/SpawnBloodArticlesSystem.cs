using Combatbees.Testing.Maria;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using System.Collections;
using UnityEngine;


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
        }
    }
}