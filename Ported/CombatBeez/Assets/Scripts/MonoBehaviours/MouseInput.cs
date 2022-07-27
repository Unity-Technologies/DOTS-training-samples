using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    public GameObject raycastPrefab;

    public GameObject raycastSphere;
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = this.GetComponent<Camera>();
        raycastSphere = Instantiate(raycastPrefab);
        raycastSphere.SetActive(false);
    }
}
