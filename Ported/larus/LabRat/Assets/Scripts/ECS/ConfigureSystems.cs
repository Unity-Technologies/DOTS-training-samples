using Unity.Entities;
using Unity.NetCode;
using Unity.Scenes;

[UpdateInGroup(typeof(ClientInitializationSystemGroup))]
public class ConfigureClientSystems : ComponentSystem
{
    public static Hash128 ClientBuildSettingsGUID => new Hash128("03da2c4d38c284fab8e28fc0f5d24529");

    protected override void OnCreate()
    {
        World.GetOrCreateSystem<SceneSystem>().BuildSettingsGUID = ClientBuildSettingsGUID;
    }

    protected override void OnUpdate()
    {
    }
}

[UpdateInGroup(typeof(ServerInitializationSystemGroup))]
public class ConfigureServerSystems : ComponentSystem
{
    public static Hash128 ServerBuildSettingsGUID => new Hash128("38064942d5d9b47b1a810a59bbedf636");

    protected override void OnCreate()
    {
        World.GetOrCreateSystem<SceneSystem>().BuildSettingsGUID = ServerBuildSettingsGUID;
    }

    protected override void OnUpdate()
    {
    }
}