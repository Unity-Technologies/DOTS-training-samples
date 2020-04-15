using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class CreateBrigadeSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var prefabs = GetSingleton<GlobalPrefabs>();
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        Entities
            .WithNone<BrigadeLine>()
            .ForEach((Entity e, in BrigadeInitInfo info) =>
            {
                ecb.AddComponent<BrigadeLine>(e);
               // var lineRef = ;
                for (int i = 0; i < info.WorkerCount; i++)
                {
                    var worker = ecb.Instantiate(prefabs.WorkerPrefab);
                    ecb.AddComponent(worker, new Worker());
                //    ecb.AddSharedComponent(worker, new BrigadeLineRef() { BrigadeLineEntity = e });
                }
            }).Run();
        ecb.Playback(EntityManager);
    }
}