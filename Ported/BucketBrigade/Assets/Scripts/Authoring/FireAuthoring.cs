using Unity.Entities;

class FireAuthoring : UnityEngine.MonoBehaviour
{
    public int Index;
}

class FireBaker : Baker<FireAuthoring>
{
    public override void Bake(FireAuthoring authoring)
    {
        AddComponent(new Fire
        {
            Index = authoring.Index
        });
    }
}