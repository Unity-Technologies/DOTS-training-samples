using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;


partial class PlayerMovement : SystemBase
{

    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;
        Entities
            .WithAll<PlayerComponent>() //ref and in
            .ForEach((Entity entity, ref TransformAspect transform, ref PlayerComponent playerComponent, in Config config) =>
            {
                var pos = transform.Position;
                pos.y = entity.Index;
                var angle = (0.5f + noise.cnoise(pos / 10f)) * 4.0f * math.PI;
                var dir = float3.zero;
                math.sincos(angle, out dir.x, out dir.z);
                transform.Position += dir * dt * 5.0f;
                transform.Rotation = quaternion.RotateY(angle);
            }).ScheduleParallel();
    }
    
    private AABB GetBounds(float yOffset, float3 position){
        AABB bounds = new AABB();
        bounds.Center = position;
        bounds.Extents = new float3(yOffset * 2, yOffset * 2, yOffset * 2);
        return bounds; 
    }

    
    private float3 bouncePosition(float t, PlayerComponent playerComponent, Config config){
        Para para = playerComponent.para; 
        float y = ParabolaCluster.Solve(para.paraA, para.paraB, para.paraC, t);
        //float3 startPos = TerrainArea.instance.LocalPositionFromBox(startBox.col, startBox.row);
        //float3 endPos = TerrainArea.instance.LocalPositionFromBox(endBox.col, endBox.row);
        //float x = math.lerp(startPos.x, endPos.x, t);
        //float z = math.lerp(startPos.z, endPos.z, t);

        return new float3(x, y, z);
    } 

    /*
    private int2 MouseToFloat2(){
        float y = (Game.instance.minTerrainHeight + Game.instance.maxTerrainHeight) / 2;
        float3 mouseWorldPos = new float3(0, y, 0);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float t = (y - ray.origin.y) / ray.direction.y;
        mouseWorldPos.x = ray.origin.x + t * ray.direction.x;
        mouseWorldPos.z = ray.origin.z + t * ray.direction.z;
        float3 mouseLocalPos = mouseWorldPos;
        if (transform.parent != null) {
            mouseLocalPos = transform.parent.InverseTransformPoint(mouseWorldPos);
        }
        int2 mouseBoxPos = TerrainArea.instance.BoxFromLocalPosition(mouseLocalPos);
        return mouseBoxPos; 
    }*/
}
