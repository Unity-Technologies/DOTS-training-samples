using System.Xml;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BarMovementSytem : SystemBase
{
    private float tornadoFader;

    protected override void OnCreate()
    {
        
    }

    protected override void OnUpdate()
    {
        throw new System.NotImplementedException();
    }


    void UpdatePoints()
    {
        var settings = this.GetSingleton<TornadoSettings>();
        var tornadoTranslation = EntityManager.GetComponentData<Translation>(GetSingletonEntity<Tornado>());
        
        Random random = new Random(1234);
        
        tornadoFader = math.clamp(tornadoFader + Time.DeltaTime / 10f, 0f, 1f);
        float invDamping = 1f - settings.Damping;

        Entities.ForEach((Node node, ref Translation translation) =>
        {
            if (node.anchor)
            {
                float3 start = translation.Value;
                
                node.oldPosition.y += .01f;
                
                float2 tornadoForce = new float2(tornadoTranslation.Value.x + (math.sin(translation.Value.y / 5f + Time.DeltaTime/4f) * 3f) - translation.Value.x,
                    tornadoTranslation.Value.z - translation.Value.z);

                float tornadoDist = math.length(tornadoForce);

                tornadoForce = math.normalize(tornadoForce);

                if (tornadoDist < settings.TornadoMaxForceDistance)
                {
                    float force = (1f - tornadoDist / settings.TornadoMaxForceDistance);
                    float yFader = math.clamp(1f - translation.Value.y / settings.TornadoHeight, 0f, 1f);
                    force *= tornadoFader * settings.TornadoForce * random.NextFloat(-0.3f, 1.3f);
                    float forceY = settings.TornadoUpForce;
                    node.oldPosition.y -= forceY * force;
                    float forceX = -tornadoForce.y + tornadoForce.x * settings.TornadoInwardForce * yFader;
                    float forceZ = tornadoForce.x + tornadoForce.y * settings.TornadoInwardForce * yFader;
                    node.oldPosition.x -= forceX * force;
                    node.oldPosition.z -= forceZ * force;
                }
                
                translation.Value += (translation.Value - node.oldPosition) * invDamping;
                node.oldPosition = start;

                if (translation.Value.y < 0f)
                {
                    translation.Value.y = 0f;
                    node.oldPosition.y = -translation.Value.y;
                    node.oldPosition.xz += (translation.Value.xz - node.oldPosition.xz) * settings.Friction;
                }

            }
        });


        for (int i = 0; i < bars.Length; i++)
        {
            Bar bar = bars[i];

            Point point1 = bar.point1;
            Point point2 = bar.point2;

            float dx = point2.x - point1.x;
            float dy = point2.y - point1.y;
            float dz = point2.z - point1.z;

            float dist = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
            float extraDist = dist - bar.length;

            float pushX = (dx / dist * extraDist) * .5f;
            float pushY = (dy / dist * extraDist) * .5f;
            float pushZ = (dz / dist * extraDist) * .5f;

            if (point1.anchor == false && point2.anchor == false)
            {
                point1.x += pushX;
                point1.y += pushY;
                point1.z += pushZ;
                point2.x -= pushX;
                point2.y -= pushY;
                point2.z -= pushZ;
            }
            else if (point1.anchor)
            {
                point2.x -= pushX * 2f;
                point2.y -= pushY * 2f;
                point2.z -= pushZ * 2f;
            }
            else if (point2.anchor)
            {
                point1.x += pushX * 2f;
                point1.y += pushY * 2f;
                point1.z += pushZ * 2f;
            }

            if (dx / dist * bar.oldDX + dy / dist * bar.oldDY + dz / dist * bar.oldDZ < .99f)
            {
                // bar has rotated: expensive full-matrix computation
                bar.matrix = Matrix4x4.TRS(
                    new Vector3((point1.x + point2.x) * .5f, (point1.y + point2.y) * .5f, (point1.z + point2.z) * .5f),
                    Quaternion.LookRotation(new Vector3(dx, dy, dz)),
                    new Vector3(bar.thickness, bar.thickness, bar.length));
                bar.oldDX = dx / dist;
                bar.oldDY = dy / dist;
                bar.oldDZ = dz / dist;
            }
            else
            {
                // bar hasn't rotated: only update the position elements
                Matrix4x4 matrix = bar.matrix;
                matrix.m03 = (point1.x + point2.x) * .5f;
                matrix.m13 = (point1.y + point2.y) * .5f;
                matrix.m23 = (point1.z + point2.z) * .5f;
                bar.matrix = matrix;
            }

            if (Mathf.Abs(extraDist) > breakResistance)
            {
                if (point2.neighborCount > 1)
                {
                    point2.neighborCount--;
                    Point newPoint = new Point();
                    newPoint.CopyFrom(point2);
                    newPoint.neighborCount = 1;
                    points[pointCount] = newPoint;
                    bar.point2 = newPoint;
                    pointCount++;
                }
                else if (point1.neighborCount > 1)
                {
                    point1.neighborCount--;
                    Point newPoint = new Point();
                    newPoint.CopyFrom(point1);
                    newPoint.neighborCount = 1;
                    points[pointCount] = newPoint;
                    bar.point1 = newPoint;
                    pointCount++;
                }
            }

            bar.minX = Mathf.Min(point1.x, point2.x);
            bar.maxX = Mathf.Max(point1.x, point2.x);
            bar.minY = Mathf.Min(point1.y, point2.y);
            bar.maxY = Mathf.Max(point1.y, point2.y);
            bar.minZ = Mathf.Min(point1.z, point2.z);
            bar.maxZ = Mathf.Max(point1.z, point2.z);

            matrices[i / instancesPerBatch][i % instancesPerBatch] = bar.matrix;
        }
    }
}