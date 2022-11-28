using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSExamples {
[ExecuteInEditMode]
public class BoardCamera : MonoBehaviour {
	public Board board;
	Camera cam;

	void OnEnable() {
		cam = GetComponent<Camera>();
	}

	public float OverheadFactor = 1.5f;

	void Update () {
		if (!cam || !board)
			return;

		cam.orthographic = true;


		var desc = board.boardDesc;

		float maxSize = Mathf.Max(desc.size.x, desc.size.y);
		float maxCellSize = Mathf.Max(desc.cellSize.x, desc.cellSize.y);

		cam.orthographicSize = maxSize * maxCellSize * .65f;
		var posXZ = Vector2.Scale(new Vector2(desc.size.x, desc.size.y) * 0.5f, desc.cellSize);

		transform.position = new Vector3(0, maxSize * maxCellSize * OverheadFactor, 0);
		transform.LookAt(new Vector3(posXZ.x, 0f, posXZ.y));

	}
}
}