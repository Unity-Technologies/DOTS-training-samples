using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HumanPlayerInputSystem : SystemBase
{
    Camera camera;

    //GameObject cube;

    EntityCommandBufferSystem ECBSystem;

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

        var tileOffset = worldPosition - worldTile;
        var direction = (byte)tileOffset.x; // TODO: figure out direction from tileoffset

        if (!clicked)
            return;
        
        var ecb = ECBSystem.CreateCommandBuffer().AsParallelWriter();

        var eventArchetype = clickEventArchetype;

        Entities
            .WithAll<HumanPlayerTag>()
            .ForEach((int entityInQueryIndex, in Entity humanPlayerEntity, in Player player) =>
        {
            var clickEvent = ecb.CreateEntity(entityInQueryIndex, eventArchetype);
            ecb.SetComponent(entityInQueryIndex, clickEvent, new PlaceArrowEvent { Player = humanPlayerEntity });
            ecb.SetComponent(entityInQueryIndex, clickEvent, new Direction { Value = direction });
            ecb.SetComponent(entityInQueryIndex, clickEvent, new PositionXZ { Value = worldTile.xz });
        }).ScheduleParallel();

        ECBSystem.AddJobHandleForProducer(Dependency);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        //GameObject.Destroy(cube);
    }
}
