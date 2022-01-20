using Unity.Entities;

[GenerateAuthoringComponent]
public struct SingletonMainScene : IComponentData
{
    // Singleton that should be required by systems in the main scene
}
