using Unity.Entities;

/// <summary>
/// Spawner entity
/// </summary>
public struct LbSpawner : IComponentData
{
    
    /// <summary>
    /// Prefab entity to instantiate mice entities
    /// </summary>
    public Entity MousePrefab;

    /// <summary>
    /// How often an mouse instance is made
    /// </summary>
    public float MouseFrequency;
    
    /// <summary>
    /// Prefab entity to instantiate mice entities
    /// </summary>
    public Entity CatPrefab;

    /// <summary>
    /// How often an mouse instance is made
    /// </summary>
    public float CatFrequency;
    
    /// <summary>
    /// The elapsed time after instantiating a Mouse
    /// </summary>
    public float ElapsedTimeForMice;
    
    /// <summary>
    /// The elapsed time after instantiating a Mouse
    /// </summary>
    public float ElapsedTimeForCats;
    

}
