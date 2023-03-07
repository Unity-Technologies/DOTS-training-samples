using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ExecuteCarSpawnAuthoring : MonoBehaviour
{
    class Baker : Baker<ExecuteCarSpawnAuthoring>
    {
        public override void Bake(ExecuteCarSpawnAuthoring authoring)
        {
            AddComponent<ExecuteCarSpawn>();
        }
    }
}

public struct ExecuteCarSpawn : IComponentData
{
}