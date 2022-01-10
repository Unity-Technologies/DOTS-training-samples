using Unity.Entities;
using UnityEngine;

public class CameraAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	public Vector2 viewAngles;
	public float viewDist;
	public float mouseSensitivity;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
		Camera.main.transform.rotation = Quaternion.Euler(viewAngles.y, viewAngles.x, 0f);

        dstManager.AddComponentData(entity, new CameraConfig
        {
            ViewAngles = viewAngles,
            ViewDist = viewDist,
            MouseSensitivity = mouseSensitivity
        });
    }
}