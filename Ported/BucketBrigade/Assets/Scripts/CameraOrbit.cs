using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class CameraOrbit : MonoBehaviour {
    public float rotateRate;
    public KeyCode keyStopRotate = KeyCode.Space;
    public KeyCode keyStartRotate = KeyCode.Space;
    private bool pauseRotate = false;

    [Space(10.0f)]
    public int GridSizeFromECSBecauseItWontTalkToMe;
    public Transform DuplicatedTransformBecauseItIsAnnoying;
    public Transform groundPlane;
    public Camera camMain;

	// ECS is annoyting.
	private void Start() {
        // Calc center of the simulation, slightly under the spawn point so we don't get z-fighting on the ground plane
        Vector3 offset = new Vector3(GridSizeFromECSBecauseItWontTalkToMe * 1.05f * 0.5f, -0.05f, GridSizeFromECSBecauseItWontTalkToMe * 1.05f * 0.5f);
        transform.position = DuplicatedTransformBecauseItIsAnnoying.position + offset;
        groundPlane.position = transform.position;

        float gridScale = Mathf.Max(100.0f, GridSizeFromECSBecauseItWontTalkToMe);
        groundPlane.localScale = new Vector3(gridScale, 1.0f, gridScale);
        groundPlane.rotation = Quaternion.identity;

        // Offset camera by gridsize approximately
        var updatedPos = camMain.transform.localPosition;
        updatedPos.z = -GridSizeFromECSBecauseItWontTalkToMe;
        camMain.transform.localPosition = updatedPos;
        camMain.transform.LookAt(groundPlane.position + new Vector3(0.0f, -camMain.transform.localPosition.y * 0.5f, 0.0f));
	}

    void Update() {
        if (!pauseRotate)
            transform.Rotate(Vector3.up, rotateRate * Time.deltaTime);

        if (pauseRotate && Input.GetKeyDown(keyStartRotate))
            pauseRotate = false;
        else if (!pauseRotate && Input.GetKeyDown(keyStopRotate))
            pauseRotate = true;
    }
}