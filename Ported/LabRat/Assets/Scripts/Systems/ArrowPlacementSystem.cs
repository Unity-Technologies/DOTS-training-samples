using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PlayerInputSystem))]
public class ArrowPlacementSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<CellData>();
        RequireSingletonForUpdate<BoardSize>();
        RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<ArrowPlacementRequest>(), ComponentType.ReadOnly<CellComponentLink>()));
    }

    protected override void OnUpdate()
    {
        var boardSize = GetSingleton<BoardSize>();

        var cellDataEntity = GetSingletonEntity<CellData>();
        var cellData = EntityManager.GetComponentObject<CellData>(cellDataEntity);

        EntityCommandBufferSystem sys = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer ecb = sys.CreateCommandBuffer();

        var cellDirections = cellData.directions;

        Entities.ForEach((in Entity entity, in ArrowPlacementRequest arrowRequest, in CellComponentLink cellLink) => {
            var arrayPos = arrowRequest.position.y * boardSize.Value.x + arrowRequest.position.x;

            if (arrowRequest.remove)
            {
                cellDirections[arrayPos] = 0;
                ecb.SetComponent(cellLink.arrow, new Color { Value = float4.zero });
                ecb.SetComponent(cellLink.arrowOutline, new Color { Value = float4.zero });
            }
            else
            {
                cellDirections[arrayPos] = (byte)(1 << (int)arrowRequest.direction);
                ecb.SetComponent(cellLink.arrow, new Color { Value = new float4(1, 1, 1, 1) });
                ecb.SetComponent(cellLink.arrowOutline, new Color { Value = new float4(0, 0, 0, 1) });

                quaternion rot;

                switch (arrowRequest.direction)
                {
                    case DirectionEnum.North:
                        rot = quaternion.EulerXYZ(math.PI / 2f, 0, 0);
                        break;
                    case DirectionEnum.South:
                        rot = quaternion.EulerXYZ(math.PI / 2f, math.PI, 0);
                        break;
                    case DirectionEnum.East:
                        rot = quaternion.EulerXYZ(math.PI / 2f, math.PI / 2f, 0);
                        break;
                    case DirectionEnum.West:
                        rot = quaternion.EulerXYZ(math.PI / 2f, -math.PI / 2f, 0);
                        break;
                    default:
                        rot = quaternion.EulerXYZ(math.PI / 2f, 0, 0);
                        break;
                }

                ecb.SetComponent(cellLink.arrow, new Rotation { Value = rot });
            }

            ecb.RemoveComponent<ArrowPlacementRequest>(entity);
        }).Schedule();
    }
}
