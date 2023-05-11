using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour {
    public float rotateRate;
    public KeyCode keyStopRotate = KeyCode.Space;
    public KeyCode keyStartRotate = KeyCode.Space;
    private bool pauseRotate = false;

    void Update() {
        if (!pauseRotate)
            transform.Rotate(Vector3.up, rotateRate * Time.deltaTime);

        if (pauseRotate && Input.GetKeyDown(keyStartRotate))
            pauseRotate = false;
        else if (!pauseRotate && Input.GetKeyDown(keyStopRotate))
            pauseRotate = true;
    }
}