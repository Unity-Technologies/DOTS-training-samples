using Unity.Entities;


class WaterAuth : UnityEngine.MonoBehaviour
{
    public float baseHealth;
    class WaterAuthBaker : Baker<WaterAuth>
    {
        public override void Bake(WaterAuth authoring)
        {
            AddComponent<Water>();
        }
    }
   
}


public struct Water : IComponentData
{
    
}