using Unity.Entities;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

	public new Camera camera;
	
	public Rect bounds;
	public float moveSpeed = 20;
	public float transitionDuration = 3;

	Vector3 topDownPosition = new Vector3();
	Quaternion topDownRotation = new Quaternion();
	// Vector3 carPosition = new Vector3();
	// Quaternion carRotation = new Quaternion();
	Entity car;

	public static CameraControl instance { get; private set; }

	public enum State {
		TOP_DOWN,
		TO_CAR,
		CAR,
		TO_TOP_DOWN,
	}
	public State state { get; private set; }
	private float time = 0;

	// public void ToCarView(Car car){
	// 	carRef = car;
	// 	state = State.TO_CAR;
	// 	time = 0;
	// 	topDownPosition = transform.position;
	// 	topDownRotation = transform.rotation;
	// 	carPosition = carRef.cameraPos.position;
	// 	carRotation = carRef.cameraPos.rotation;
	// }

	public void ToTopDownView() {
		if (state != State.CAR)
			return;
		state = State.TO_TOP_DOWN;
		time = 0;
		// carRef.Show();
		// carRef = null;
	}

	void Awake() {
		if (instance != null) {
			Destroy (gameObject);
			return;
		}
		instance = this;
		state = State.TOP_DOWN;
	}

	// Use this for initialization
	void Start ()
	{
		camera = GetComponent<Camera>();
		topDownPosition = transform.position;
		topDownRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {

		float dt = Time.unscaledDeltaTime;
		time += dt;

		switch (state) {

		case State.TOP_DOWN:

			Vector2 v = new Vector2 ();

			if (Input.GetKey (KeyCode.LeftArrow)) {
				v.x = -moveSpeed;
			} else if (Input.GetKey (KeyCode.RightArrow)) {
				v.x = moveSpeed;
			} else {
				v.x = 0;
			}

			if (Input.GetKey (KeyCode.DownArrow)) {
				v.y = -moveSpeed;
			} else if (Input.GetKey (KeyCode.UpArrow)) {
				v.y = moveSpeed;
			} else {
				v.y = 0;
			}
			
			// look for car nearest to click; if closest car is close enough, switch to that car's cam
			if (Input.GetMouseButtonDown(0))
			{
				RaycastHit hit;
				Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        
				if (Physics.Raycast(ray, out hit)) {
                
					
					Debug.Log(hit.point);
					
					
				}    
			}
			
			transform.position = new Vector3 (
				Mathf.Clamp (transform.position.x + v.x * dt, bounds.xMin, bounds.xMax),
				transform.position.y,
				Mathf.Clamp (transform.position.z + v.y * dt, bounds.yMin, bounds.yMax)
			);
			topDownPosition = transform.position;
			break;

		// case State.TO_CAR:
		case State.CAR:
		
			// carPosition = car.cameraPos.position;
			// carRotation = car.cameraPos.rotation;
			//
			// if (time >= transitionDuration || state == State.CAR) {
			// 	if (state == State.TO_CAR) {
			// 		state = State.CAR;
			// 		carRef.Hide();
			// 	}
			// 	transform.position = carPosition;
			// 	transform.rotation = carRotation;
			// } else {
			//
			// 	transform.position = Vector3.Lerp(topDownPosition, carPosition, time / transitionDuration);
			// 	transform.rotation = Quaternion.Slerp(topDownRotation, carRotation, time / transitionDuration);
			//
			// }
		
			break;

		// case State.TO_TOP_DOWN:
		//
		// 	transform.position = Vector3.Lerp (carPosition, topDownPosition, time / transitionDuration);
		// 	transform.rotation = Quaternion.Slerp (carRotation, topDownRotation, time / transitionDuration);
		//
		// 	if (time >= transitionDuration || state == State.CAR) {
		// 		state = State.TOP_DOWN;
		// 	} 
		// 	break;
		//
		}

	}

	void OnDestroy() {

		if (instance == this) {
			instance = null;
		}

	}
}

