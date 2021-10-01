using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

public partial class PhysicsMovementSystem : SystemBase
{

    private NativeArray<bool> _voxelOccupancy;

    private static int GetVoxelIndex(in WorldBounds bounds, in Translation trans)
    {
        int xSize = (int) (bounds.AABB.Max.x - bounds.AABB.Min.x);
        int ySize = (int) (bounds.AABB.Max.y - bounds.AABB.Min.y);

        return (int) (trans.Value.x - bounds.AABB.Min.x) * ySize +
                    (int) (trans.Value.z - bounds.AABB.Min.z) * xSize*ySize  +
               (int) (trans.Value.y - bounds.AABB.Min.y);
    } 

    private static int GetTopVoxelIndex(in WorldBounds bounds, in Translation trans)
    {
        int xSize = (int) (bounds.AABB.Max.x - bounds.AABB.Min.x);
        int ySize = (int) (bounds.AABB.Max.y - bounds.AABB.Min.y);

        return (int) (trans.Value.x - bounds.AABB.Min.x) * ySize +
               (int) (trans.Value.z - bounds.AABB.Min.z) * xSize*ySize +
               ySize;
    }

    protected override void OnUpdate()
    {
        var system = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = system.CreateCommandBuffer().AsParallelWriter();        

        var worldBoundsEntity = GetSingletonEntity<WorldBounds>();
        var bounds = GetComponent<WorldBounds>(worldBoundsEntity);

        int totalSize = ((int) (bounds.AABB.Max.x - bounds.AABB.Min.x) +1) * (int) (bounds.AABB.Max.z - bounds.AABB.Min.z) * (int)(bounds.AABB.Max.y - bounds.AABB.Min.y);

        if (_voxelOccupancy.Length == 0)
        {
            _voxelOccupancy= new NativeArray<bool>(totalSize, Allocator.Persistent);
        }

        var voxelOccupancy = _voxelOccupancy;
        for (int i = 0; i < totalSize; ++i)
        {
            voxelOccupancy[i] = false;
        }
        
        float deltaTime = Time.fixedDeltaTime;
        float gravity = 9.8f;
        float floorY = 0.5f;
        float yDelta = deltaTime * gravity;

        //Initialize voxel occupancy for food already on the floor
        Entities
            .WithNativeDisableParallelForRestriction(voxelOccupancy)
            .WithAll<Food,Grounded>()
            .ForEach((Entity entity, ref Translation translation) =>
            {
                voxelOccupancy[GetVoxelIndex(bounds, translation)] = true;
            }).ScheduleParallel();

        //Initialize voxel occupancy for food in the air
        Entities
            .WithAny<Food>()
            .WithNone<Grounded>()
            .ForEach((Entity entity, ref Translation translation) =>
            {
                int voxelIndex = GetVoxelIndex(bounds, translation);
                int topVoxelIndex = GetTopVoxelIndex(bounds, translation);

                //If voxel is already occupied stack up until we find an empty space
                if (voxelOccupancy[voxelIndex] == true)
                {
                    translation.Value.y = math.ceil(translation.Value.y);
                    voxelIndex = GetVoxelIndex(bounds, translation);

                    while (voxelOccupancy[voxelIndex] != false && voxelIndex < topVoxelIndex)
                    {
                        voxelIndex += 1;
                        translation.Value.y += 1.0f;
                    }
                }

                if (voxelIndex < topVoxelIndex)
                    voxelOccupancy[voxelIndex] = true;
            }).Schedule();
        

        //Apply gravity
        Entities
            .WithAny<Food>()
            .WithNone<Grounded>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation) =>
            {
                Translation tmpTranslation = translation;
                tmpTranslation.Value.y = tmpTranslation.Value.y - yDelta - floorY;
                int voxelIndex = GetVoxelIndex(bounds, translation);
                int newVoxelIndex = GetVoxelIndex(bounds, tmpTranslation);
                if (newVoxelIndex < voxelIndex)
                {
                    if (voxelOccupancy[newVoxelIndex] == false)
                    {
                        voxelOccupancy[voxelIndex] = false;
                        translation.Value.y -= yDelta;
                    }
                    else //If there is something below us clamp our position
                        translation.Value.y = math.floor(translation.Value.y) + floorY;
                }
                else
                    translation.Value.y -= yDelta;

                if (translation.Value.y <= floorY)
                {
                    translation.Value.y = floorY;// this assume 0 is at the basis of the cylinder
                    ecb.AddComponent(entityInQueryIndex, entity, new Grounded{});
                }
            }).Schedule();

        system.AddJobHandleForProducer(Dependency);
    }

    protected override void OnDestroy()
    {
        _voxelOccupancy.Dispose();
    }
}