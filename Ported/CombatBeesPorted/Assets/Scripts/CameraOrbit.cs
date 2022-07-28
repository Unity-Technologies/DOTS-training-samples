using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public float sensitivity;
    public float zoomSensitivity;
    public float stiffness;

    Vector2 viewAngles;
    Vector2 smoothViewAngles;
    float viewDist;
    float smoothViewDist;

    void Start()
    {
        viewDist = 70f;
        smoothViewDist = viewDist;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            viewAngles.x += Input.GetAxis("Mouse X") * sensitivity / Screen.height;
            viewAngles.y -= Input.GetAxis("Mouse Y") * sensitivity / Screen.height;

            viewAngles.y = Mathf.Clamp(viewAngles.y, -89f, 89f);
        }

        viewDist -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity * viewDist;
        viewDist = Mathf.Clamp(viewDist, 5f, 80f);

        smoothViewAngles = Vector2.Lerp(smoothViewAngles, viewAngles, stiffness * Time.deltaTime);
        smoothViewDist = Mathf.Lerp(smoothViewDist, viewDist, stiffness * Time.deltaTime);

        transform.rotation = Quaternion.Euler(smoothViewAngles.y, smoothViewAngles.x, 0f);
        transform.position = -transform.forward * smoothViewDist;

        if (Input.GetKeyDown(KeyCode.Backspace)) Time.timeScale = 0.5f;
        if (Input.GetKeyDown(KeyCode.Alpha1)) Time.timeScale = 1f;
        if (Input.GetKeyDown(KeyCode.Alpha2)) Time.timeScale = 2f;
        if (Input.GetKeyDown(KeyCode.Alpha4)) Time.timeScale = 4f;
        if (Input.GetKeyDown(KeyCode.Alpha8)) Time.timeScale = 8f;
    }
}