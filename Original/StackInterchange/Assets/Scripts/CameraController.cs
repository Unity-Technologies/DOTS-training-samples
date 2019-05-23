using System;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera[] Cameras;
    private Car _carWithCamera;
    private int _currentCameraIndex;
    public bool IsOnCarCam { get; private set; }
    public float CameraTranslateAnimationTime = 0.5f;
    public static CameraController Instance { get; private set; }

    #region Unity Callbacks
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    #region Public API
    public void SwitchCameraIfNeeded(Car carToBeDespawned)
    {
        if (IsOnCarCam && carToBeDespawned == _carWithCamera)
        {
            Cameras[_currentCameraIndex].transform.SetParent(Cameras[0].transform.parent);
            SwitchCamera();
        }
    }

    public void SwitchCamera()
    {
        Cameras[_currentCameraIndex].gameObject.SetActive(false);
        _currentCameraIndex++;
        _currentCameraIndex %= Cameras.Length;
        Cameras[_currentCameraIndex].gameObject.SetActive(true);

        IsOnCarCam = _currentCameraIndex == Cameras.Length - 1;
        if (IsOnCarCam)
        {
            _carWithCamera = GameManager.Instance.ActiveSpawnManager.GetRandomActiveCar();
            Cameras[_currentCameraIndex].transform.SetParent(_carWithCamera.transform);
            Cameras[_currentCameraIndex].transform.localPosition = new Vector3(0, 1, -1);
            Cameras[_currentCameraIndex].transform.localRotation = Quaternion.identity;
        }
    }

    private void GoToDefaultCamera()
    {
        Cameras[_currentCameraIndex].gameObject.SetActive(false);
        _currentCameraIndex = 0;
        Cameras[_currentCameraIndex].gameObject.SetActive(true);
        IsOnCarCam = false;
    }

    public IEnumerator MoveCamera(Vector3 newPosition)
    {
        GoToDefaultCamera();
        float elapsedTime = 0;
        while (elapsedTime < CameraTranslateAnimationTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, newPosition, elapsedTime / CameraTranslateAnimationTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    #endregion
}
