using Unity.Entities;
using UnityEngine;

public class CarSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private int amount;
    [SerializeField] public int NumLanes = 4;
    [SerializeField] public float LengthLanes = 100.0f;
    [SerializeField] public float MinVelocity = 1.0f;
    [SerializeField] public float MaxVelocity = 2.0f;
 
    public class Baker : Baker<CarSpawnerAuthoring>
    {
        public override void Bake(CarSpawnerAuthoring authoring)
        {
            AddComponent(new CarSpawner
            {
                carPrefab = GetEntity(authoring.carPrefab),
                amount = authoring.amount,
                MaxVelocity = authoring.MaxVelocity,
                MinVelocity = authoring.MinVelocity,
                LengthLanes = authoring.LengthLanes,
                NumLanes = authoring.NumLanes
            });
        }
    }
}
