using Unity.Entities;

[GenerateAuthoringComponent]
public class CameraRef : IComponentData
{
    public UnityEngine.Camera Camera;
    public float ArmLength;
}
