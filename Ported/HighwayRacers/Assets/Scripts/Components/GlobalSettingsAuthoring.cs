using Unity.Entities;
using UnityEngine;

public class GlobalSettingsAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private int amount;
    [SerializeField] public int NumLanes = 4;
    [SerializeField] public float LengthLanes = 100.0f;
    [SerializeField] public float MinVelocity = 1.0f;
    [SerializeField] public float MaxVelocity = 2.0f;
    [SerializeField] public float MinOvertakeVelocity = 1.0f;
    [SerializeField] public float MaxOvertakeVelocity = 2.0f;
 
    public class Baker : Baker<GlobalSettingsAuthoring>
    {
        public override void Bake(GlobalSettingsAuthoring authoring)
        {
            AddComponent(new GlobalSettings
            {
                carPrefab = GetEntity(authoring.carPrefab),
                amount = authoring.amount,
                MaxVelocity = authoring.MaxVelocity,
                MinVelocity = authoring.MinVelocity,
                MinOvertakeVelocity = authoring.MinOvertakeVelocity,
                MaxOvertakeVelocity = authoring.MaxOvertakeVelocity,
                LengthLanes = authoring.LengthLanes,
                NumLanes = authoring.NumLanes
            });
        }
    }
}
