using Unity.Entities;

public class SiloAuthoring : UnityEngine.MonoBehaviour
{
}


public class SiloBaker : Baker<SiloAuthoring>
{
    public override void Bake(SiloAuthoring authoring)
    {
        AddComponent(new Silo
        {
        });
    }
}