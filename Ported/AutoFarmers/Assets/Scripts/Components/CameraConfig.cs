using Unity.Entities;
using Unity.Mathematics;

public struct CameraConfig : IComponentData
{
	public float2 ViewAngles;
	public float ViewDist;
	public float MouseSensitivity;
}
