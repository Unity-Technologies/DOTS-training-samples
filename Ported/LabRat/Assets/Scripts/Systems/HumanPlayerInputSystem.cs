using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
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

        var mouseWorldPosition = mouseRay.GetPoint(distance);
        var worldPosition = ((float3)mouseWorldPosition).xz;
        var worldTile = math.round(worldPosition);

        var cubePosition = worldTile;
        cubePosition.y = 0.6f;
        //cube.transform.position = cubePosition;

        var tileOffset = worldPosition - worldTile;
        var inputDirection = Direction.Attributes.None;
        if (math.abs(tileOffset.x) > (math.abs(tileOffset.y)))
        {
            inputDirection = tileOffset.x > 0 ? Direction.Attributes.Right : Direction.Attributes.Left;
        }
        else
        {
            inputDirection = tileOffset.y > 0 ? Direction.Attributes.Up : Direction.Attributes.Down;
        }

        Entities
            .WithAll<HumanPlayerTag>()
            .WithAll<Child>()
            .ForEach((ref PositionXZ position, ref Direction direction) =>
            {
                position.Value = worldTile;
                direction.Value = inputDirection;
            }).ScheduleParallel();

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
            ecb.SetComponent(entityInQueryIndex, clickEvent, new Direction { Value = inputDirection });
            ecb.SetComponent(entityInQueryIndex, clickEvent, new PositionXZ { Value = worldTile });
        }).ScheduleParallel();

        ECBSystem.AddJobHandleForProducer(Dependency);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        //GameObject.Destroy(cube);
    }
}
