using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;

public partial class CameraMovementSystem : SystemBase
{
    private float HeightOffset;
    private float ySpeed = 5;
    private Vector3 camVel = new Vector3();
    
    protected override void OnUpdate()
    {
        var camera = this.GetSingleton<GameObjectRefs>().Camera;
        Vector3 pos = camera.transform.localPosition;
        // controlling height offset
        if (Input.GetKey(KeyCode.DownArrow)) {
            HeightOffset += ySpeed * Time.DeltaTime;
        } else if (Input.GetKey(KeyCode.UpArrow)) {
            HeightOffset -= ySpeed * Time.DeltaTime;
        }
        HeightOffset = Mathf.Max(HeightOffset, Constants.FOLLOW_HEIGHT_OFFSET_MIN);
        
        var terrainData = this.GetSingleton<TerrainData>();
        float height = terrainData.MaxTerrainHeight + HeightOffset;
        // Query player
        var player = this.GetSingletonEntity<PlayerTag>();
        var translation = GetComponent<Translation>(player);

        float3 target = translation.Value;
        target.y = height;

        target = ApplyCameraRotationOffset(target, terrainData.MaxTerrainHeight);
        pos = Vector3.SmoothDamp(camera.transform.localPosition, target, ref camVel, Constants.SMOOTH_DAMP_DURATION, float.MaxValue, Time.DeltaTime);
        camera.transform.localPosition = pos;
    }
    
    private Vector3 ApplyCameraRotationOffset(Vector3 pos, float maxTerrainHeight)
    {
        var camera = this.GetSingleton<GameObjectRefs>().Camera;
        float zOff = (pos.y - maxTerrainHeight) / Mathf.Tan(camera.transform.rotation.eulerAngles.x * Mathf.Rad2Deg);
        pos.z += zOff;
        return pos;
    }
}