using Unity.Entities;
using Unity.Mathematics;

public struct Bar : IComponentData
{
    public int p1, p2;
    public float length;
    public float thickness;
    
    public void AssignPoints(int a, int b, float l) {
        p1 = a;
        p2 = b;
        length = l;

        thickness = 0.25f;//Random.Range(.25f,.35f);

//        Vector3 pos = new Vector3(point1.x + point2.x,point1.y + point2.y,point1.z + point2.z) * .5f;
//        Quaternion rot = Quaternion.LookRotation(delta);
//        Vector3 scale = new Vector3(thickness,thickness,length);
//        matrix = Matrix4x4.TRS(pos,rot,scale);
//        
//        float upDot = Mathf.Acos(Mathf.Abs(Vector3.Dot(Vector3.up,delta.normalized)))/Mathf.PI;
//        color = Color.white * upDot*Random.Range(.7f,1f);
    }

}

public struct BarEntry : IBufferElementData
{
    public Bar Value;
}
