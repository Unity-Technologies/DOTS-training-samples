using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class HumanPlayerInputSystem : SystemBase
{
    //GameObject cube;

    EntityCommandBufferSystem ECBSystem;

    EntityArchetype clickEventArchetype;

    const float playAreaHeight = -0.5f;

    protected override void OnCreate()
    {
        base.OnCreate();
        //cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        clickEventArchetype = EntityManager.CreateArchetype(typeof(PlaceArrowEvent), typeof(PositionXZ), typeof(Direction));
    }

    protected override void OnUpdate()
    {
        var camera = Camera.main;
        if (camera == null)
            return;

        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        var clicked = Input.GetMouseButtonDown(0);
        var plane = new Plane(Vector3.up, playAreaHeight);
        if (!plane.Raycast(mouseRay, out var distance))
            return;

        var mouseWorldPosition = (float3)mouseRay.GetPoint(distance);
        var worldPosition = mouseWorldPosition.xz;
        var worldTile = math.round(worldPosition);

        var cubePosition = worldTile;
        cubePosition.y = 0.6f;
        //cube.transform.position = cubePosition;

        var tileOffset = worldPosition - worldTile;
        var inputDirection = Direction.Attributes.None;
        var directionBitShift = 0;
        if (math.abs(tileOffset.x) > (math.abs(tileOffset.y)))
        {
            directionBitShift = tileOffset.x > 0 ? 2 : 0; // Should be direct enum values, but we want to get angle for arrow preview as well
        }
        else
        {
            directionBitShift = tileOffset.y > 0 ? 1 : 3;
        }
        inputDirection = (Direction.Attributes)(1 << directionBitShift); // God help us if bit value meanings change
        var directionQuaternion = quaternion.Euler(0, directionBitShift * math.PI / 2 + math.PI, 0);

        Entities
            .WithAll<HumanPlayerTag>()
            .WithAll<Child>() // Only Arrow Preview has a Child
            .ForEach((ref Translation position, ref Rotation rotation) =>
            {
                position.Value = new float3(worldTile.x, 0, worldTile.y);
                rotation.Value = directionQuaternion;
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
