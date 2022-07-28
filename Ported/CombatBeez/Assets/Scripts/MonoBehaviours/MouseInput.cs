using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseInput : MonoBehaviour
{
    public GameObject raycastPrefab;

    public GameObject raycastSphere;
    public Camera cam;

    public float sensitivity;
    public float zoomSensitivity;
    public float stiffness;

    private Vector2 viewAngles;
    private Vector2 smoothViewAngles;
    private float viewDist;
    private float smoothViewDist;

    // Start is called before the first frame update
    void Start()
    {
        cam = this.GetComponent<Camera>();
        raycastSphere = Instantiate(raycastPrefab);
        raycastSphere.SetActive(false);

        viewDist = 150f;
        smoothViewDist = viewDist;
    }

    private void Update()
    {
        MouseOrbit();
    }

    private void MouseOrbit()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            viewAngles.x += Input.GetAxis("Mouse X") * sensitivity / Screen.height;
            viewAngles.y -= Input.GetAxis("Mouse Y") * sensitivity / Screen.height;

            viewAngles.y = Mathf.Clamp(viewAngles.y, -89f, 89f);
        }

        viewDist -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity * viewDist;
        viewDist = Mathf.Clamp(viewDist, 5f, 150f);

        smoothViewAngles = Vector2.Lerp(smoothViewAngles, viewAngles, stiffness * Time.deltaTime);
        smoothViewDist = Mathf.Lerp(smoothViewDist, viewDist, stiffness * Time.deltaTime);

        transform.rotation = Quaternion.Euler(smoothViewAngles.y, smoothViewAngles.x, 0f);
        transform.position = -transform.forward * smoothViewDist;
    }
}
