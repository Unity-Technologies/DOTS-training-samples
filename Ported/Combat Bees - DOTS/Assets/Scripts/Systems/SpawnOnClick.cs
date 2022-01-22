using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class SpawnOnClick : SystemBase
{
	private Camera camera;
	private Material markerMaterial;
	private Transform marker;
	bool isMouseTouchingField;
	static Vector3 worldMousePosition;
	private bool markerCreated = false;
	private float spawnTimer=0;
	private float spawnRate = 10; // TODO: Extract to authoring component to be modifiable from the Editor
	
	protected override void OnCreate()
	{
		RequireSingletonForUpdate<SingletonMainScene>();
	}

	protected override void OnUpdate() // TODO: Extract some parts into methods for better readability and re-usability
	{
		Vector3 field = new Vector3();
		Vector3 fieldCenter = new Vector3();
		
		Entities.ForEach((in Container container) =>
		{
			field = container.Dimensions;
			
			// TODO: Use "fieldCenter" to account for cases when the container is not at the world origin (0,0,0)
			fieldCenter = container.Center;
		}).Run();
		
		if (!markerCreated)
		{
			// TODO: Instantiate marker as entity instead of a game object
			
			marker = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
			marker.gameObject.name = "Mouse Raycast Marker";
			marker.GetComponent<Renderer>().sharedMaterial = markerMaterial;
			markerCreated = true;
		}
		
		camera = Camera.main;
		Ray mouseRay = camera.ScreenPointToRay(Input.mousePosition);
		float deltaTime = World.Time.DeltaTime;
		isMouseTouchingField = false;

		for (int i = 0; i < 3; i++)
		{
			for (int j = -1; j <= 1; j += 2)
			{
				Vector3 wallCenter = new Vector3();
				wallCenter[i] = field[i] * .5f * j;
				Plane plane = new Plane(-wallCenter, wallCenter);
				float hitDistance;
				if (Vector3.Dot(plane.normal, mouseRay.direction) < 0f)
				{
					if (plane.Raycast(mouseRay, out hitDistance))
					{
						Vector3 hitPoint = mouseRay.GetPoint(hitDistance);
						bool insideField = true;
						for (int k = 0; k < 3; k++)
						{
							if (Mathf.Abs(hitPoint[k]) > field[k] * .5f + .01f)
							{
								insideField = false;
								break;
							}
						}

						if (insideField)
						{
							isMouseTouchingField = true;
							worldMousePosition = hitPoint;
							break;
						}
					}
				}
			}

			if (isMouseTouchingField)
			{
				break;
			}
		}

		if (isMouseTouchingField)
		{
			marker.position = worldMousePosition;
			if (marker.gameObject.activeSelf == false)
			{
				marker.gameObject.SetActive(true);
			}
		}
		else
		{
			if (marker.gameObject.activeSelf)
			{
				marker.gameObject.SetActive(false);
			}
		}

		var ecb = new EntityCommandBuffer(Allocator.Temp);
		
		Entities
			.ForEach((in SpawnedPrefab spawnedPrefab) =>
			{
				
				if (isMouseTouchingField)
				{
					
					if (Input.GetKey(KeyCode.Mouse0)) // TODO: Some clicks are not being detected
					{
						spawnTimer += deltaTime;
						while (spawnTimer > 1f/spawnRate) {
						    spawnTimer -= 1f/spawnRate;
						    var instance = ecb.Instantiate(spawnedPrefab.Value);
						    var translation = new Translation {Value = worldMousePosition};
						    ecb.SetComponent(instance, translation);
						}
						
					}
				}

			}).WithoutBurst().Run();
		ecb.Playback(EntityManager);
		ecb.Dispose();
	}
}
