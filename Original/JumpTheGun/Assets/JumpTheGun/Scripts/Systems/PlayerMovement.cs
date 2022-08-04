using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics; 
 

[BurstCompile]
partial struct PlayerComponentJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public float DeltaTime;
    public Config config;
    [ReadOnly] public float3 rayOrigin, rayDirection;

    [ReadOnly] public NativeArray<Entity> boxes;
    [ReadOnly] public ComponentDataFromEntity<Boxes> boxesFromEntity; 

    void Execute([ChunkIndexInQuery] int chunkIndex, ref PlayerComponent playerComponent, ref TransformAspect transform)
    {  
        if (config.isPaused)
            return;

        Boxes startBox = boxesFromEntity[playerComponent.startBox];
        Boxes endBox = boxesFromEntity[playerComponent.endBox];

        var mouseBoxPos = MouseToFloat2(config, rayOrigin, rayDirection, transform);
        int2 movePos = GetMovePos(mouseBoxPos, config, transform, endBox);

        bool occupied = false; 

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
                float y = para.paraA * t * t + para.paraB * t + para.paraC;
                float3 startPos = TerrainAreaClusters.LocalPositionFromBox(startBox.column, startBox.row, config);
                float3 endPos = TerrainAreaClusters.LocalPositionFromBox(endBox.column, endBox.row, config);
                float x = math.lerp(startPos.x, endPos.x, t);
                float z = math.lerp(startPos.z, endPos.z, t);
                transform.Position = new float3(x, y, z);
            }

        } else {
            startBox = endBox;
            if (movePos.x == startBox.column && movePos.y == startBox.row) 
                endBox = startBox;  
            else {
                foreach (var box in boxes) { 
                    if (boxesFromEntity[box].row == movePos.y && boxesFromEntity[box].column == movePos.x){
                        endBox = boxesFromEntity[box];
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
            float startY = startBox.top + playerComponent.yOffset;
            float endY = endBox.top + playerComponent.yOffset;
            float height = math.max(startY, endY);
            if (startBox.column != endBox.column && startBox.row != endBox.row) {
                height = math.max(math.max(height, boxRef1.top), boxRef2.top);
            }
            height += playerComponent.bounceHeight;

            playerComponent.para.paraC = startY;
            float k = math.sqrt(math.abs(startY - height)) / (math.sqrt(math.abs(startY - height)) + math.sqrt(math.abs(endY - height)));
            playerComponent.para.paraA = (height - startY - k * (endY - startY)) / (k * k - k);
            playerComponent.para.paraB = endY - startY - playerComponent.para.paraA;

            playerComponent.isBouncing = true;
            playerComponent.time = 0;
            float2 startPos = new float2(startBox.column, startBox.row);
            float2 endPos = new float2(endBox.column, endBox.row);
            float dist = math.distance(startPos, endPos);
            playerComponent.duration = math.max(1, dist) * playerComponent.bounceDuration;
        } 
        
        foreach (Entity box in boxes){
            Boxes newBox = boxesFromEntity[box];
            if (newBox.row == startBox.row && newBox.column == startBox.column ){
                playerComponent.startBox = box;
            } else if (newBox.row == endBox.row && newBox.column == endBox.column ){
                playerComponent.endBox = box;
            }
        }

        boxes.Dispose();


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
    ComponentDataFromEntity<Boxes> boxesFromEntity;

    public void OnCreate(ref SystemState state) 
    {
        state.RequireForUpdate<Config>();
        var queryBuilderBoxes = new EntityQueryDescBuilder(Allocator.Temp);
        queryBuilderBoxes.AddAll(ComponentType.ReadWrite<Boxes>());
        queryBuilderBoxes.FinalizeQuery(); 

        boxQuery = state.GetEntityQuery(queryBuilderBoxes);
        boxesFromEntity = state.GetComponentDataFromEntity<Boxes>(true);

        queryBuilderBoxes.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
        
        //var cameraSingleton = CameraSingleton.Instance; 
        //var ray = cameraSingleton.ScreenPointToRay(UnityEngine.Input.mousePosition);

        //float3 rayOrigin  = ray.origin; 
        //float3 rayDirection = ray.direction;

        NativeArray<Entity> boxes = boxQuery.ToEntityArray(allocator);
        //NativeArray<Boxes> boxesComponents = boxQuery.ToComponentDataArray<Boxes>(allocator);

        var config = SystemAPI.GetSingleton<Config>();
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        boxesFromEntity.Update(ref state);     

        var PlayerJob = new PlayerComponentJob
        {
            boxes = boxes,
            boxesFromEntity = boxesFromEntity,
            ECB = ecb.AsParallelWriter(),
            DeltaTime = state.Time.DeltaTime, // Time cannot be directly accessed from a job, so DeltaTime has to be passed in as a parameter.
            rayOrigin = config.rayOrigin,
            rayDirection = config.rayDirection,   
            config = config,
        };

        //PlayerJob.Schedule();
        PlayerJob.Run(); 

        //state.Dependency = PlayerJob.Schedule(state.Dependency);
        boxes.Dispose();
        
    }
}
