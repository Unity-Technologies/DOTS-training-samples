using Cinemachine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraControls : MonoBehaviour
{
    public float zoomSpeed = 7.5f;
    public float camDollySpeed = 50.0f;
    
    private CinemachineVirtualCamera m_VCam;
    
    void Start()
    {
        m_VCam = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        var framing = m_VCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (Input.GetKey(KeyCode.UpArrow))
            framing.m_CameraDistance -= Time.deltaTime * zoomSpeed;
        if (Input.GetKey(KeyCode.DownArrow))
            framing.m_CameraDistance += Time.deltaTime * zoomSpeed;
        
        // todo: ideally this would be controlled via CinemachinePOV but would need to figure out how to send
        // camera plane to ActiveCam component
        if (Input.GetKey(KeyCode.LeftArrow))
            m_VCam.transform.Rotate(Vector3.up, Time.deltaTime * -camDollySpeed, Space.World);
        if (Input.GetKey(KeyCode.RightArrow))
            m_VCam.transform.Rotate(Vector3.up, Time.deltaTime * camDollySpeed, Space.World);
        
        framing.m_CameraDistance = Mathf.Clamp(framing.m_CameraDistance, 5.0f, 165.0f);
    }
}
