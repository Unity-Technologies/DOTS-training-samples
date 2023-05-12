using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class CameraOrbit : MonoBehaviour {
    public float rotateRate;
    public float maxPanning = 45.0f;
    public float panSpeed = 100.0f;
    public KeyCode keyStopRotate = KeyCode.Space;
    public KeyCode keyStartRotate = KeyCode.Space;

    [Space(10.0f)]
    public int GridSizeFromECSBecauseItWontTalkToMe;
    public Transform DuplicatedTransformBecauseItIsAnnoying;
    public Transform groundPlane;
    public Camera camMain;

    private bool isCamFreeRotating { get { return null != CoroutineFreeRotate; } }
    private Coroutine CoroutineFreeRotate = null;
    private Coroutine CoroutinePanning = null;

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

        CoroutineFreeRotate = StartCoroutine(CameraFreeRotate());
	}

    IEnumerator CameraFreeRotate() {
        while (enabled) {
            transform.Rotate(Vector3.up, rotateRate * Time.deltaTime);
            yield return null;
        }
        CoroutineFreeRotate = null;
    }

    IEnumerator CameraPan() {
        float lastTimePanning = Time.time;
        float direction;
        float currentRotation = 0.0f;

        while (enabled) {
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
                direction = -Time.deltaTime * panSpeed;
                lastTimePanning = Time.time;
			} else if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
                direction = Time.deltaTime * panSpeed;
                lastTimePanning = Time.time;
            } else {
                // Start degrading
                currentRotation *= 0.975f;
                direction = 0.0f;
            }

            // Pan at twice the normal speed of camera rotation on its own
            currentRotation = Mathf.Clamp(currentRotation + direction, -maxPanning, maxPanning);
            transform.Rotate(Vector3.up, currentRotation * Time.deltaTime);

            // If no key press for 2 seconds, stop the panning
            if (Time.time - lastTimePanning > 2.0f)
                break;

            yield return null;
		}

        CoroutinePanning = null;
	}

    private void Update() {
        if (!isCamFreeRotating && Input.GetKeyDown(keyStartRotate)) {
            if (null != CoroutinePanning) {
                StopCoroutine(CoroutinePanning);
                CoroutinePanning = null;
            }
            CoroutineFreeRotate = StartCoroutine(CameraFreeRotate());
        } else if (isCamFreeRotating && Input.GetKeyDown(keyStopRotate)) {
            StopCoroutine(CoroutineFreeRotate);
            CoroutineFreeRotate = null;
        }

        if (null == CoroutinePanning && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))) {
            if (null != CoroutineFreeRotate) {
                StopCoroutine(CoroutineFreeRotate);
                CoroutineFreeRotate = null;
            }
            CoroutinePanning = StartCoroutine(CameraPan());
		}
    }
}