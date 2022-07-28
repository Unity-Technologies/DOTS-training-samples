using Unity.Burst;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
partial struct PlayerComponentJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    public float DeltaTime;
    public Config config;
    public float3 rayOrigin;
    public float3 rayDirection;
    public EntityManager entityManager;
    
    public Unity.Collections.NativeArray<Entity> tanks; 
    public Unity.Collections.NativeArray<Entity> boxes; 

    [Unity.Collections.ReadOnly] public ComponentDataFromEntity<Tank> tankFromEntity;
    [Unity.Collections.ReadOnly] public ComponentDataFromEntity<Boxes> boxesFromEntity;

    void Execute([ChunkIndexInQuery] int chunkIndex, ref PlayerComponent playerComponent, ref TransformAspect transform)
    { 
        
        if (config.isPaused)
            return;

        Boxes startBox = boxesFromEntity[playerComponent.startBox];
        Boxes endBox = boxesFromEntity[playerComponent.endBox];

        var mouseBoxPos = MouseToFloat2(config, rayOrigin, rayDirection, transform);
        int2 movePos = GetMovePos(mouseBoxPos, config, transform, endBox);
            
        bool occupied = false; 

        foreach (var tank in tanks) { 
            var tankComponent =  tankFromEntity[tank]; 
            if (tankComponent.column == movePos.x && tankComponent.row == movePos.y)
                occupied = true; 
        }
        if (occupied) 
            movePos = new int2(endBox.column, endBox.row);

        playerComponent.time += DeltaTime;
        
        if (playerComponent.isBouncing){
            if (playerComponent.time >= playerComponent.duration) {
                transform.Position = TerrainAreaClusters.LocalPositionFromBox(endBox.column, endBox.row, config, endBox.top + playerComponent.yOffset);
                playerComponent.isBouncing = false;
                playerComponent.time = 0;
            } else {
                var t = playerComponent.time / playerComponent.duration;
                Para para = playerComponent.para; 
                float y = ParabolaCluster.Solve(para.paraA, para.paraB, para.paraC, t);
                float3 startPos = TerrainAreaClusters.LocalPositionFromBox(startBox.column, startBox.row, config);
                float3 endPos = TerrainAreaClusters.LocalPositionFromBox(endBox.column, endBox.row, config);
                float x = math.lerp(startPos.x, endPos.x, t);
                float z = math.lerp(startPos.z, endPos.z, t);
                transform.Position = new float3(x, y, z);
            }

        } else {
            startBox = endBox;
            if (movePos.x == startBox.column && movePos.y == startBox.row) // look for box to bounce to
                endBox = startBox;  // don't go to new box
            else {
                foreach (var box in boxes) { 
                    Boxes newStartBox = boxesFromEntity[box];
                    if (newStartBox.row == movePos.x && newStartBox.column == movePos.y){
                        endBox = newStartBox;
                        break; 
                    }
                }
            }


            // Get references for later
            Boxes boxRef1 = new Boxes();
            Boxes boxRef2 = new Boxes(); 
            foreach (var box in boxes) { 
                Boxes newStartBox = boxesFromEntity[box];
                if (newStartBox.row == startBox.column && newStartBox.column == endBox.row){
                    boxRef1 = newStartBox;
                }else if (newStartBox.row == endBox.column && newStartBox.column == startBox.row){
                    boxRef2 = newStartBox;
                }
            }

            // Bounce
            if (playerComponent.isBouncing)
                return;

            float startY = startBox.top + playerComponent.yOffset;
            float endY = endBox.top + playerComponent.yOffset;
            float height = math.max(startY, endY);
            if (startBox.column != endBox.column && startBox.row != endBox.row) {
                height = math.max(math.max(height, boxRef1.top), boxRef2.top);
            }
            height += playerComponent.bounceHeight;

            var para = playerComponent.para;
            ParabolaCluster.Create(startY, height, endY, out para.paraA, out para.paraB, out para.paraC);

            playerComponent.isBouncing = true;
            playerComponent.time = 0;
            float2 startPos = new float2(startBox.column, startBox.row);
            float2 endPos = new float2(endBox.column, endBox.row);
            float dist = math.distance(startPos, endPos);
            playerComponent.duration = math.max(1, dist) * playerComponent.bounceDuration;

        } 
    }
    
    private static AABB GetBounds(float yOffset, float3 position){
        AABB bounds = new AABB();
        bounds.Center = position;
        bounds.Extents = new float3(yOffset * 2, yOffset * 2, yOffset * 2);
        return bounds; 
    }

    private static int2 GetMovePos(int2 mouseBoxPos, Config config, TransformAspect transform, Boxes endBox){
        int2 movePos = mouseBoxPos;
        int2 currentPos = TerrainAreaClusters.BoxFromLocalPosition(transform.Position, config);
        if (math.abs(mouseBoxPos.x - currentPos.x) > 1 || math.abs(mouseBoxPos.y - currentPos.y) > 1) {
            movePos = currentPos;
            if (mouseBoxPos.x != currentPos.x) {
                movePos.x += mouseBoxPos.x > currentPos.x ? 1 : -1;
            }
            if (mouseBoxPos.y != currentPos.y) {
                movePos.y += mouseBoxPos.y > currentPos.y ? 1 : -1;
            }
        }
        return movePos;
    }

    private static int2 MouseToFloat2(Config config, float3 rayOrigin, float3 rayDirection, TransformAspect transform){
        float y = (config.minTerrainHeight + config.maxTerrainHeight) / 2;
        float3 mouseWorldPos = new float3(0, y, 0);

        float t = (y - rayOrigin.y) / rayDirection.y;
        mouseWorldPos.x = rayOrigin.x + t * rayDirection.x;
        mouseWorldPos.z = rayOrigin.z + t * rayDirection.z;
        float3 mouseLocalPos = mouseWorldPos;
        int2 mouseBoxPos = TerrainAreaClusters.BoxFromLocalPosition(mouseLocalPos, config);
        return mouseBoxPos; 
    }
}

[BurstCompile]
partial struct PlayerMovement : ISystem
{
    private EntityQuery boxQuery;
    private EntityQuery tankQuery;

    ComponentDataFromEntity<Tank> tankFromEntity;
    ComponentDataFromEntity<Boxes> boxesFromEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        tankFromEntity = state.GetComponentDataFromEntity<Tank>(true);
        boxesFromEntity = state.GetComponentDataFromEntity<Boxes>(true);
        boxQuery = state.GetEntityQuery(typeof(Boxes));
        tankQuery = state.GetEntityQuery(typeof(Tank));
        
        //state.RequireForUpdate<EndSimulationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var boxEntities = boxQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
        var tankEntities = tankQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
        var camera = CameraSingleton.Instance.GetComponent<UnityEngine.Camera>();
        var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        World world = World.DefaultGameObjectInjectionWorld;
        EntityManager entityManager = world.EntityManager;
        tankFromEntity.Update(ref state);
        boxesFromEntity.Update(ref state);

        var PlayerJob = new PlayerComponentJob
        {
            tanks = tankEntities,
            boxes = boxEntities,
            tankFromEntity = tankFromEntity, 
            boxesFromEntity = boxesFromEntity,
            ECB = ecb.AsParallelWriter(),
            DeltaTime = state.Time.DeltaTime, // Time cannot be directly accessed from a job, so DeltaTime has to be passed in as a parameter.
            rayOrigin = ray.origin,
            rayDirection = ray.direction, 
            config = config,
            entityManager = entityManager, 
        };

        //PlayerJob.Schedule();

        PlayerJob.Run(); 
    }
}
