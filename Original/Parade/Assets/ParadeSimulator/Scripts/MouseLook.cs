using UnityEngine;
using System.Collections;

/// <summary>
/// A really basic Mouse Look script
/// </summary>
public class MouseLook : MonoBehaviour
{

    private Vector2 sensitivity = new Vector2(10.0f, 10.0f);
    private Vector2 minMaxY = new Vector2(-60.0f, 60.0f);

    private float rotationY = 0.0f;

    private bool mouseLookEnabled = true;
    public bool MouseLookEnabled {

        get { return mouseLookEnabled; }

        set {

            mouseLookEnabled = value;

            if (mouseLookEnabled == false)
            {
                Camera.main.transform.localEulerAngles = new Vector3(0.0f,0.0f,0.0f);
            }
            
        }

    }

    void Update()
    {

        if (mouseLookEnabled)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity.x, 0);
            rotationY += Input.GetAxis("Mouse Y") * sensitivity.y;
            rotationY = Mathf.Clamp(rotationY, minMaxY.x, minMaxY.y);
            Camera.main.transform.localEulerAngles = new Vector3(-rotationY, Camera.main.transform.localEulerAngles.y, 0);
        }

    }

}