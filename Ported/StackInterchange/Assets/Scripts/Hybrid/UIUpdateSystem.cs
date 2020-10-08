using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

public class UIUpdateSystem : SystemBase
{
    private UIUpdater TextObject;

    protected override void OnCreate()
    {
        TextObject = GameObject.Find("Panel").GetComponent<UIUpdater>();
    }

    protected override void OnUpdate()
    {
        // Update spawn counter in UI
        int count = 0;
    
        Entities
            .ForEach((Entity entity, ref SpawnCount spawnCount) =>
            {
                count = spawnCount.TotalCount;
            }).Run();
        
        if (count > 0){
            TextObject.UpdateSpawnCount(count);
        }
    }
}
