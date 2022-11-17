using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public class AntAuthoring : UnityEngine.MonoBehaviour
{
    
}

class AntBaker : Baker<AntAuthoring>
{
    public override void Bake(AntAuthoring authoring)
    {
        AddComponent<Ant>();
        AddComponent<PostTransformMatrix>();
    }
}