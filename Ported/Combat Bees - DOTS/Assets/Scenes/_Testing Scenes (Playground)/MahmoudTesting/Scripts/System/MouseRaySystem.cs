using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class MouseRaySystem : SystemBase
{
    private Camera camera;
    private Material markerMaterial;
    private Transform marker;
	bool isMouseTouchingField;
	static Vector3 worldMousePosition;
	Vector3 Field= new Vector3(40f,1f,40f);
	
    protected override void OnCreate()
    {
	    // This prevents OnUpdate() from running if the singleton is not present in the scene
	    RequireSingletonForUpdate<SingeltonSpawner>();
	    
	    // The code below is affecting other scenes (a sphere is instantiated and visible)
	    
	    // marker = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
		// marker.gameObject.name = "Mouse Raycast Marker";
		// marker.GetComponent<Renderer>().sharedMaterial = markerMaterial;
		
		// Maybe we should instantiate the sphere as an entity in the "MouseRayInitSystem" and then do a query here
		// to be able to modify its Transform?
    }

    protected override void OnUpdate()
    {
	    camera=this.GetSingleton<GameObjectRef>().Camera;
	    markerMaterial = this.GetSingleton<GameObjectRef>().MarkerMaterial;
        Ray mouseRay = camera.ScreenPointToRay(Input.mousePosition);

		isMouseTouchingField = false;
		
		for (int i=0;i<3;i++) {
			for (int j=-1;j<=1;j+=2) {
				
				Vector3 wallCenter = new Vector3();
				wallCenter[i] = Field[i] * .5f*j;
				Plane plane = new Plane(-wallCenter,wallCenter);
				float hitDistance;
				if (Vector3.Dot(plane.normal,mouseRay.direction) < 0f) {
					if (plane.Raycast(mouseRay,out hitDistance)) {
						Vector3 hitPoint = mouseRay.GetPoint(hitDistance);
						bool insideField = true;
						for (int k = 0; k < 3; k++) {
							if (Mathf.Abs(hitPoint[k]) > Field[k] * .5f+.01f) {
								insideField = false;
								break;
							}
						}
						if (insideField) {
							isMouseTouchingField = true;
							worldMousePosition = hitPoint;
							break;
						}
					}
				}
			}
			if (isMouseTouchingField) {
				break;
			}
		}
		if (isMouseTouchingField) {
			marker.position = worldMousePosition;
			if (marker.gameObject.activeSelf == false) {
				marker.gameObject.SetActive(true);
			}
		} else {
			if (marker.gameObject.activeSelf) {
				marker.gameObject.SetActive(false);
			}
		}
		var ecb = new EntityCommandBuffer(Allocator.Temp);
		Entities
			.ForEach((Entity entity, in ResourceComponent resourceComponent) =>
			{
				
				// ecb.DestroyEntity(entity);
				if (Input.GetKey(KeyCode.Mouse0))
				{
					var instance = ecb.Instantiate(resourceComponent.resourcePrefab);
					if(isMouseTouchingField)
					{ 
						var translation = new Translation {Value = worldMousePosition};
						ecb.SetComponent(instance, translation);
					}
					
				}
		
			
			}).WithoutBurst().Run();
		ecb.Playback(EntityManager);
		ecb.Dispose();
    }
  
    
}

