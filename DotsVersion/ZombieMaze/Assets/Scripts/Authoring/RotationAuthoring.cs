using Unity.Entities;

public class RotationAuthoring : UnityEngine.MonoBehaviour
{
   
}
class RotationComponentBaker : Baker<RotationAuthoring>
{
    public override void Bake(RotationAuthoring authoring)
    {
        AddComponent<RotationComponent>();
    }
}
