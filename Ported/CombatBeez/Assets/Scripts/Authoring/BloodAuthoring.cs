using Unity.Entities;

public class BloodAuthoring : UnityEngine.MonoBehaviour
{
}

public class BloodAuthoringBaker : Baker<BloodAuthoring>
{
    public override void Bake(BloodAuthoring authoring)
    {
        AddComponent<Blood>();
    }
}