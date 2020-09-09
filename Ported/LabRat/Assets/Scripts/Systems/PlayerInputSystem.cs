using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInputSystem : SystemBase
{
    Camera camera;

    //GameObject cube;

    EntityCommandBufferSystem ECBSystem;

    Entity CurrentPlayer;

    EntityArchetype clickEventArchetype;

    const float playAreaHeight = -0.5f;

    protected override void OnCreate()
    {
        base.OnCreate();
        camera = Camera.main;
        //cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        clickEventArchetype = EntityManager.CreateArchetype(typeof(PlaceArrowEvent), typeof(PositionXZ), typeof(Direction));
    }

    protected override void OnUpdate()
    {
        var mouseRay = camera.ScreenPointToRay(Input.mousePosition);
        var clicked = Input.GetMouseButtonDown(0);
        var plane = new Plane(Vector3.up, playAreaHeight);
        if (!plane.Raycast(mouseRay, out var distance))
            return;

        var worldPosition = (float3)mouseRay.GetPoint(distance);
        worldPosition.y += playAreaHeight * 2;
        var worldTile = math.round(worldPosition);

        var cubePosition = worldTile;
        cubePosition.y = 0.6f;
        //cube.transform.position = cubePosition;

        if (!clicked)
            return;

        var tileOffset = worldPosition - worldTile;
        var direction = (byte)tileOffset.x; // TODO: figure out direction from tileoffset

        var ecb = ECBSystem.CreateCommandBuffer();
        var clickEvent = ecb.CreateEntity(clickEventArchetype);
        ecb.SetComponent(clickEvent, new PlaceArrowEvent { Player = CurrentPlayer });
        ecb.SetComponent(clickEvent, new Direction { Value = direction });
        ecb.SetComponent(clickEvent, new PositionXZ { Value = worldTile.xz });

        ECBSystem.AddJobHandleForProducer(Dependency);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        //GameObject.Destroy(cube);
    }
}
