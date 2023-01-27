using Unity.Entities;
public partial struct DebugSystem : ISystem
{
    public float elapsedTime;
    public int farmers;
    public int rocks;
    public int plants;
    public void OnCreate(ref SystemState state)
    {
        elapsedTime = (float)SystemAPI.Time.ElapsedTime;
        PullData(state);
    }

    public void OnDestroy(ref SystemState state)
    {
     }

    public void PullData(SystemState state)
    {
        farmers = 0;
        foreach (var farmer in SystemAPI.Query<FarmerAspect>())
            farmers++;
        rocks = 0;
        foreach (var rock in SystemAPI.Query<RockAspect>())
            rocks++;
        plants = 0;
        foreach (var plant in SystemAPI.Query<PlantAspect>())
            plants++;

        UnityEngine.Debug.Log("Elapsed Time:" + (int)SystemAPI.Time.ElapsedTime + " Farmers " + farmers + " | Tree Count " + rocks + " | Plants " + plants);
    }
    public void OnUpdate(ref SystemState state)
    {
        elapsedTime += SystemAPI.Time.DeltaTime;
        if (elapsedTime > 15)
        {
            elapsedTime -= 15;
            PullData(state);            
        }

    }
}
