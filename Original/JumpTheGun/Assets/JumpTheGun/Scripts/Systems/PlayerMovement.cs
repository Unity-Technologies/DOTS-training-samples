using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;


partial class PlayerMovement : SystemBase
{
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;
        UnityEngine.Camera camera = CameraSingleton.Instance.GetComponent<UnityEngine.Camera>(); 
        UnityEngine.Ray ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
        float3 rayOrigin = ray.origin;
        float3 rayDirection = ray.direction; 

        //var system = World.GetExistingSystem<ComponentSystemBase>();
        Config config = World.GetExistingSystem<ComponentSystemBase>().GetSingleton<Config>();

        Entities
            .WithAll<PlayerComponent>() //ref and in
            .ForEach((ref TransformAspect transform, ref PlayerComponent playerComponent) =>
            {
                if (config.isPaused)
				    return;
                Boxes startBox = GetComponent<Boxes>(playerComponent.startBox); 
                Boxes endBox = GetComponent<Boxes>(playerComponent.endBox); 
                var mouseBoxPos = MouseToFloat2(config, rayOrigin, rayDirection, transform);
                int2 movePos = GetMovePos(mouseBoxPos, config, transform, endBox);
                 
                bool occupied = false; 
                foreach (var tank in SystemAPI.Query<Entity>().WithAll<Tank>()) { 
                    var tankComponent =  GetComponent<Tank>(tank);
                    if (tankComponent.column == movePos.x && tankComponent.row == movePos.y)
                        occupied = true; 
                }
                if (occupied) 
                    movePos = new int2(endBox.column, endBox.row);
                playerComponent.time += dt;
                
                if (playerComponent.isBouncing){
                    if (playerComponent.time >= playerComponent.duration) {
                        transform.Position = TerrainAreaClusters.LocalPositionFromBox(endBox.column, endBox.row, config, endBox.top + playerComponent.yOffset);
                        playerComponent.isBouncing = false;
                        playerComponent.time = 0;
                    } else {
                        transform.Position = bouncePosition(playerComponent.time / playerComponent.duration, playerComponent, config, startBox, endBox);
                    }

                } else {
                    startBox = endBox;
                    if (movePos.x == startBox.column && movePos.y == startBox.row) // look for box to bounce to
                        endBox = startBox;  // don't go to new box
                    else {
                        //endBox = TerrainAreaClusters.GetBox(movePos.x, movePos.y, config);
                        foreach (var box in SystemAPI.Query<Entity>().WithAll<Boxes>()) { 
                            Boxes newStartBox = GetComponent<Boxes>(box);
                            if (newStartBox.row == movePos.x && newStartBox.column == movePos.y){
                                endBox = newStartBox;
                                break; 
                            }
                        }
                        //if (endBox == null) // failsafe
                            //endBox = startBox;
                    }

                    Boxes boxRef1 = new Boxes();
                    Boxes boxRef2 = new Boxes(); 
                    foreach (var box in SystemAPI.Query<Entity>().WithAll<Boxes>()) { 
                            Boxes newStartBox = GetComponent<Boxes>(box);
                            if (newStartBox.row == startBox.column && newStartBox.column == endBox.row){
                                boxRef1 = newStartBox;
                            }else if (newStartBox.row == endBox.column && newStartBox.column == startBox.row){
                                boxRef2 = newStartBox;
                            }
                        }
                    Bounce(endBox, playerComponent, endBox, startBox, boxRef1, boxRef2);
                }
                
            }).ScheduleParallel();
    }
    
    private static AABB GetBounds(float yOffset, float3 position){
        AABB bounds = new AABB();
        bounds.Center = position;
        bounds.Extents = new float3(yOffset * 2, yOffset * 2, yOffset * 2);
        return bounds; 
    }

    public float3 Spawn(int col, int row, PlayerComponent playerComponent, Config config, Boxes startBox){
        Boxes newStartBox; 
        Entity newStartBoxEntity = new Entity(); 
        foreach (var box in SystemAPI.Query<Entity>().WithAll<Boxes>()) { 
            newStartBox = GetComponent<Boxes>(box);
            if (newStartBox.row == row && newStartBox.column == col){
                newStartBoxEntity = box;
                break; 
            }
        }
        playerComponent.startBox = newStartBoxEntity;
		playerComponent.endBox = playerComponent.startBox;

        float top = startBox.top; 
        return TerrainAreaClusters.LocalPositionFromBox(col, row, config, top + playerComponent.yOffset);
    }

    private static float3 bouncePosition(float t, PlayerComponent playerComponent, Config config, Boxes startBox, Boxes endBox){
        Para para = playerComponent.para; 
        float y = ParabolaCluster.Solve(para.paraA, para.paraB, para.paraC, t);
        float3 startPos = TerrainAreaClusters.LocalPositionFromBox(startBox.column, startBox.row, config);
        float3 endPos = TerrainAreaClusters.LocalPositionFromBox(endBox.column, endBox.row, config);
        float x = math.lerp(startPos.x, endPos.x, t);
        float z = math.lerp(startPos.z, endPos.z, t);
        return new float3(x, y, z);
    } 

    private static int2 GetMovePos(int2 mouseBoxPos, Config config, TransformAspect transform, Boxes endBox){
        int2 movePos = mouseBoxPos;
        int2 currentPos = TerrainAreaClusters.BoxFromLocalPosition(transform.Position, config);
        if (math.abs(mouseBoxPos.x - currentPos.x) > 1 || math.abs(mouseBoxPos.y - currentPos.y) > 1) {
            // mouse position is too far away.  Find closest position
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
        /*if (transform.parent != null) {
            mouseLocalPos = transform.parent.InverseTransformPoint(mouseWorldPos);
        }*/
        int2 mouseBoxPos = TerrainAreaClusters.BoxFromLocalPosition(mouseLocalPos, config);
        return mouseBoxPos; 
    }

    
    public static void Bounce(Boxes boxTo, PlayerComponent playerComponent, Boxes endBox, Boxes startBox, Boxes box1, Boxes box2){
        if (playerComponent.isBouncing)
            return;

        // set start and end positions
        endBox = boxTo;

        // solving parabola path
        float startY = startBox.top + playerComponent.yOffset;
        float endY = endBox.top + playerComponent.yOffset;
        float height = math.max(startY, endY);
        // make height max of adjacent boxes when moving diagonally
        if (startBox.column != endBox.column && startBox.row != endBox.row) {
            height = math.max(math.max(height, box1.top), box2.top);
        }
        height += playerComponent.bounceHeight;

        var para = playerComponent.para;
        ParabolaCluster.Create(startY, height, endY, out para.paraA, out para.paraB, out para.paraC);

        playerComponent.isBouncing = true;
        playerComponent.time = 0;

        // duration affected by distance to end box
        float2 startPos = new float2(startBox.column, startBox.row);
        float2 endPos = new float2(endBox.column, endBox.row);
        float dist = math.distance(startPos, endPos);
        playerComponent.duration = math.max(1, dist) * playerComponent.bounceDuration;

    }
}
