using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace AutoFarmers
{

	public class CamFollow : MonoBehaviour
	{

		public Vector2 viewAngles;
		public float viewDist;
		public float mouseSensitivity;
		EntityQuery query;
		Entity farmerEntity;
		void Start()
		{
			query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Farmer>(), ComponentType.ReadOnly<Translation>());
			transform.rotation = Quaternion.Euler(viewAngles.y, viewAngles.x, 0f);
		}

		void LateUpdate()
		{
			if(farmerEntity == Entity.Null)
            {
				using var farmers = query.ToEntityArray(Unity.Collections.Allocator.Temp);
                if (farmers.Length > 0)
                {
					farmerEntity = farmers[0];
                }
                else
                {
					return;
                }
            }

			Vector3 pos = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(farmerEntity).Value;
			viewAngles.x += Input.GetAxis("Mouse X") * mouseSensitivity / Screen.height;
			viewAngles.y -= Input.GetAxis("Mouse Y") * mouseSensitivity / Screen.height;
			viewAngles.y = Mathf.Clamp(viewAngles.y, 7f, 80f);
			viewAngles.x -= Mathf.Floor(viewAngles.x / 360f) * 360f;
			transform.rotation = Quaternion.Euler(viewAngles.y, viewAngles.x, 0f);
			transform.position = pos - transform.forward * viewDist;
		}
	}

}

