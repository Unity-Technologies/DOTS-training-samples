using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var player = GetSingletonEntity<Player>();
        var translation = GetComponent<Translation>(player);

        if (Input.GetKeyDown(KeyCode.UpArrow)) translation.Value.z += 1;
        if (Input.GetKeyDown(KeyCode.DownArrow)) translation.Value.z -= 1;
        if (Input.GetKeyDown(KeyCode.RightArrow)) translation.Value.x += 1;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) translation.Value.x -= 1;

        SetComponent(player, translation);

        var offset = GetComponent<Player>(player).CameraOffset;

        // Note the use of the explicit "this" in the following line, the GetSingleton that takes a managed
        // component is an extension method with the same name as a member function.
        var camera = this.GetSingleton<GameObjectRefs>().Camera;

        camera.transform.position = translation.Value + offset;
        camera.transform.LookAt(translation.Value);
    }
}