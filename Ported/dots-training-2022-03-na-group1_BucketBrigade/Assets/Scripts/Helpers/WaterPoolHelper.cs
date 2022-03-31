using Unity.Entities;

static class WaterPoolHelper
{
    public static void DecreaseVolume(EntityManager manager, Entity waterPool)
    {
        var volume = manager.GetComponentData<Volume>(waterPool);
        volume.Value -= 1f;
        manager.SetComponentData(waterPool, volume);
    }
}
