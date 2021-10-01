using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameObjectRefs : MonoBehaviour 
{
    public float PanSensitivity = 10000;
    public float ZoomSensitivity = 2;
    public float Rotation = 6;

    Vector2 View;
    Vector2 SmoothView;
    float ViewDistance;
    float SmoothDistance;

    void Start () 
    {
        ViewDistance = 200f;
        SmoothDistance = ViewDistance;
    }
	
    void Update () 
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKey(KeyCode.Mouse1)) 
        {
            View.x += Input.GetAxis("Mouse X") * PanSensitivity/Screen.height;
            View.y -= Input.GetAxis("Mouse Y") * PanSensitivity/Screen.height;

            View.y = Mathf.Clamp(View.y,-89f,89f);
        }

        ViewDistance -= Input.GetAxis("Mouse ScrollWheel") * ZoomSensitivity * ViewDistance;
        ViewDistance = Mathf.Clamp(ViewDistance,5f,80f);

        SmoothView = Vector2.Lerp(SmoothView,View,Rotation * Time.deltaTime);
        SmoothDistance = Mathf.Lerp(SmoothDistance,ViewDistance,Rotation * Time.deltaTime);

        transform.rotation = Quaternion.Euler(SmoothView.y,SmoothView.x,0f);
        transform.position = -transform.forward * SmoothDistance;
    }
}