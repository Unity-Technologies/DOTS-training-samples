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

        if (TextObject.QuadButtonPressed){
            float3 tmpTransl = (0);
            int tmpIdx = TextObject.ActiveQuad;

            Entities.ForEach((Entity entity, in Translation trans, in QuadNum quad) =>
            {
                if (quad.QuadID == tmpIdx){
                    tmpTransl.x = trans.Value.x;
                    tmpTransl.y = trans.Value.y;
                    tmpTransl.z = trans.Value.z;
                }
            }).Run();

            TextObject.QuadX = tmpTransl.x;
            TextObject.QuadY = tmpTransl.y;
            TextObject.QuadZ = tmpTransl.z;
            TextObject.ChangeQuad();
        }
    }
}
