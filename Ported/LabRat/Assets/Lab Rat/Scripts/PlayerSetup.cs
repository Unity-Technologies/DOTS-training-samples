using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ECSExamples {

public class PlayerSetup : MonoBehaviour {
	public GameObject PlayerPrefab;
	public GameObject CPUPrefab;
	public Canvas UICanvas;

	public int NumCPUS = 3;

	public void OnEnable() {
		InitPlayer(PlayerPrefab);
		for (int i = 0; i < NumCPUS; ++i)
			InitPlayer(CPUPrefab);
	}

	void InitPlayer(GameObject prefab) {
		var obj = Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.identity, transform);
		obj.GetComponent<PlayerCursor>().Init(UICanvas);
        var board = FindObjectOfType<Board>();
        var cursorFollow = obj.GetComponent<CursorFollowMouse>();
        if (cursorFollow) {
            cursorFollow.board = board;
        }
	}
}

}
