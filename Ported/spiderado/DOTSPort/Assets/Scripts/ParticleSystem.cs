using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ParticleSystem : ComponentSystem
{
    public static float tornadoX;
    public static float tornadoZ;

	public static float TornadoSway(float y, float time) {
        return Mathf.Sin(y / 5f + time / 4f) * 3f;
	}

    protected override void OnUpdate() {
		tornadoX = Mathf.Cos(Time.time/6f) * 30f;
		tornadoZ = Mathf.Sin(Time.time/6f * 1.618f) * 30f;
        float curTime = Time.time;

        Entities.ForEach((Entity e, ParticleSharedData tornado, ref Point point, ref PartData partData) => {
			float3 tornadoPos = new float3(
                tornadoX + TornadoSway(point.pos.y, curTime),
                point.pos.y, 
                tornadoZ);
			float3 delta = (tornadoPos - point.pos);
			float dist = math.length(delta);
			delta /= dist;
			float inForce = dist - Mathf.Clamp01(tornadoPos.y / 50f)*30f*partData.radiusMult+2f;
			point.pos += new float3(-delta.z*tornado.spinRate+delta.x*inForce,tornado.upwardSpeed,delta.x*tornado.spinRate+delta.z*inForce)*Time.deltaTime;
			if (point.pos.y>50f) {
				point.pos = new Vector3(point.pos.x,0f,point.pos.z);
			}

            Matrix4x4 matrix = partData.matrix;
            matrix.m03 = point.pos.x;
            matrix.m13 = point.pos.y;
            matrix.m23 = point.pos.z;
            partData.matrix = matrix;
            Graphics.DrawMesh(tornado.particleMesh, partData.matrix, tornado.particleMaterial, 1);
        });
        
    }
}