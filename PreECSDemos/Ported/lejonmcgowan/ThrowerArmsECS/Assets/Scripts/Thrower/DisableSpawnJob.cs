using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public struct DisableSpawnJob: IJob
{
    public void Execute()
    {
        //World.DefaultGameObjectInjectionWorld.GetExistingSystem<ProjectileSpawnerSystem>().Enabled = false;
    }
}
