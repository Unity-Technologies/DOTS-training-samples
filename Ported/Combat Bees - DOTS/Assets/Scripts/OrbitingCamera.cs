using UnityEngine;

public class OrbitingCamera : MonoBehaviour
{
    public float sensitivity = 3000f;
    public float zoomSensitivity = 1f;
    public float stiffness = 10f;

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

        transform.rotation = Quaternion.Euler(smoothViewAngles.y + 45.9f, smoothViewAngles.x, 0f);
        transform.position = -transform.forward * smoothViewDist;
    }
}