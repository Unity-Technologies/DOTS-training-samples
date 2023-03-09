using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct MouseSpawnSystem : ISystem
{
    private Config config;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        config = SystemAPI.GetSingleton<Config>();

        MouseInput mouseInput = GameObject.Find("Main Camera")?.GetComponent<MouseInput>();

        if (mouseInput == null)
            return;

        RaycastHit hitPoint;
        Ray screenToWorldRay = mouseInput.cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(screenToWorldRay, out hitPoint))
        {
            if (!mouseInput.raycastSphere.activeSelf)
                mouseInput.raycastSphere.SetActive(true);

            mouseInput.raycastSphere.transform.position = hitPoint.point;
        }
        else
        {
            if (mouseInput.raycastSphere.activeSelf)
                mouseInput.raycastSphere.SetActive(false);
        }

        if (mouseInput.raycastSphere.activeSelf && Input.GetMouseButtonDown(0))
        {
            Entity foodRes = state.EntityManager.Instantiate(config.FoodResourcePrefab);

            Vector3 startPos = mouseInput.raycastSphere.transform.position;
            startPos.y += 5f;

            state.EntityManager.SetComponentData(foodRes, new Translation { Value = startPos });
            state.EntityManager.SetComponentData(foodRes, new FoodResource { State = FoodState.FALLING });
        }
    }
}
