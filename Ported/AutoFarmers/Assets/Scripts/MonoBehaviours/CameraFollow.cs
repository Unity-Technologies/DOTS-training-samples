using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Vector2 viewAngles;
	public float viewDist;
	public float mouseSensitivity;
	
    private EntityManager _manager;
    private EntityQuery _query;

    private void Awake()
    {
        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _query = _manager.CreateEntityQuery(ComponentType.ReadOnly<CameraFollow_Tag>());
    }
    
	private void Start () 
	{
		transform.rotation = Quaternion.Euler(viewAngles.y,viewAngles.x,0f);
	}
    
    private void LateUpdate()
    {
        var entities = _query.ToEntityArray(Allocator.TempJob);

        if (entities.Length < 1)
        {
	        entities.Dispose();
	        return;
        }

        Translation translation = _manager.GetComponentData<Translation>(entities[0]);
        UpdateCameraPosition(translation.Value);

        transform.position = translation.Value;
        
        entities.Dispose();
    }
	
	void UpdateCameraPosition(Vector3 pos)
	{
		viewAngles.x += Input.GetAxis("Horizontal") * mouseSensitivity/Screen.height;
		viewAngles.y -= Input.GetAxis("Vertical") * mouseSensitivity/Screen.height;
		viewAngles.y = Mathf.Clamp(viewAngles.y,7f,80f);
		viewAngles.x -= Mathf.Floor(viewAngles.x / 360f) * 360f;
		transform.rotation = Quaternion.Euler(viewAngles.y,viewAngles.x,0f);
		transform.position = pos - transform.forward * viewDist;
	}
}
