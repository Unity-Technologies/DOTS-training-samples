using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private GameObject mousePointer;
    [SerializeField] private GameObject resourceSpawner;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        mousePointer.SetActive(false);
    }

    private void Update()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        var isHit = Physics.Raycast(ray, out var hitInfo);
        if (isHit)
        {
            mousePointer.transform.position = hitInfo.point;
            
            if(Input.GetMouseButtonDown(0))
            {
                GenerateResourceAt(hitInfo.point);
            }
        }
        mousePointer.SetActive(isHit);
    }

    private void GenerateResourceAt(Vector3 hitInfoPoint)
    {
        Instantiate(resourceSpawner, hitInfoPoint, Quaternion.identity);
    }
}
