using Unity.Entities;


class WaterAuth : UnityEngine.MonoBehaviour
{
    public float MaxCapacity;
    public float CurrCapacity;
    class WaterBaker : Baker<WaterAuth>
    {
        public override void Bake(WaterAuth authoring)
        {
            AddComponent(new Water
            {
                MaxCapacity = authoring.MaxCapacity,
                CurrCapacity = authoring.CurrCapacity
            });
        }
    }
   
}


public struct Water : IComponentData
{
    public float MaxCapacity;
    public float CurrCapacity;
}