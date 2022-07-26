using Unity.Entities;

class BeeStateDeadAuthoring : UnityEngine.MonoBehaviour
{
}

class BeeStateDeadBaker : Baker<BeeStateDeadAuthoring>
{
    public override void Bake(BeeStateDeadAuthoring authoring)
    {
        AddComponent(new BeeStateDead());
    }
}