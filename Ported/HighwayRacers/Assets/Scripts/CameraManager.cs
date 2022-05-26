using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraManager : MonoBehaviour
{
    private static CameraManager m_Instance;
    public Camera MainCamera;
	public Rect Bounds;

    public float moveSpeed = 20;
    public float transitionDuration = 3;

	Vector3 topDownPosition = new Vector3();
	Quaternion topDownRotation = new Quaternion();
	Vector3 carPosition = new Vector3();
	Quaternion carRotation = new Quaternion();

	public enum State
    {
        TOP_DOWN,
        TO_CAR,
        CAR,
        TO_TOP_DOWN,
    }
    private State m_State { get; set; }
    private float m_Time = 0;

    public static CameraManager Instance {
        get => m_Instance ?? throw new InvalidOperationException();
        private set
        {
			if (m_Instance != null) Destroy(Instance);
            m_Instance = value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
		m_State = State.TOP_DOWN;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
		m_Time += Time.unscaledDeltaTime;

		switch (m_State)
		{
			case State.TOP_DOWN:
                UpdateMainCamera();
				break;

			case State.TO_CAR:
			case State.CAR:
				UpdateCarCamera();
				break;

			case State.TO_TOP_DOWN:
				UpdateMainCameraLerp();
				break;
		}
	}

    private void UpdateMainCamera()
    {
		Vector2 v = new Vector2();

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			v.x = -moveSpeed;
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			v.x = moveSpeed;
		}
		else
		{
			v.x = 0;
		}

		if (Input.GetKey(KeyCode.DownArrow))
		{
			v.y = -moveSpeed;
		}
		else if (Input.GetKey(KeyCode.UpArrow))
		{
			v.y = moveSpeed;
		}
		else
		{
			v.y = 0;
		}

		MainCamera.transform.position = new Vector3(
			Mathf.Clamp(MainCamera.transform.position.x + v.x * Time.unscaledDeltaTime, Bounds.xMin, Bounds.xMax),
			transform.position.y,
			Mathf.Clamp(MainCamera.transform.position.z + v.y * Time.unscaledDeltaTime, Bounds.yMin, Bounds.yMax)
		);
		topDownPosition = MainCamera.transform.position;
	}

	private void UpdateMainCameraLerp()
    {
		MainCamera.transform.position = Vector3.Lerp(carPosition, topDownPosition, m_Time / transitionDuration);
		MainCamera.transform.rotation = Quaternion.Slerp(carRotation, topDownRotation, m_Time / transitionDuration);

		if (m_Time >= transitionDuration || m_State == State.CAR)
		{
			m_State = State.TOP_DOWN;
		}
	}

	private void UpdateCarCamera()
    {
		if (m_Time >= transitionDuration || m_State == State.CAR)
		{
			if (m_State == State.TO_CAR)
			{
				m_State = State.CAR;
			}
			MainCamera.transform.position = carPosition;
			MainCamera.transform.rotation = carRotation;
		}
		else
		{
			MainCamera.transform.position = Vector3.Lerp(topDownPosition, carPosition, m_Time / transitionDuration);
			MainCamera.transform.rotation = Quaternion.Slerp(topDownRotation, carRotation, m_Time / transitionDuration);
		}
	}

	public void ToCarView()
	{
		if (m_State != State.TOP_DOWN) return;
		m_State = State.TO_CAR;
		m_Time = 0;
		topDownPosition = MainCamera.transform.position;
		topDownRotation = MainCamera.transform.rotation;
	}

	public void ToTopDownView()
	{
		if (m_State != State.CAR)
			return;
		m_State = State.TO_TOP_DOWN;
		m_Time = 0;
	}

	public void SetCarCameraTransform(Quaternion rotation, Vector3 position)
    {
		carPosition = position;
		carRotation = rotation;	
    }
}
