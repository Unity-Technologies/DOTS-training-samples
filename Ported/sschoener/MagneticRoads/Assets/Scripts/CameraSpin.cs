using UnityEngine;

public class CameraSpin : MonoBehaviour
{
    public Vector3 center;
    public float distance;
    public float tilt;
    public float spinSpeed;

    float m_SpinAngle;

    void Update()
    {
        m_SpinAngle += spinSpeed * Time.deltaTime;
        var t = transform;
        t.rotation = Quaternion.Euler(tilt, m_SpinAngle, 0f);
        t.position = center - t.forward * distance;
    }
}
