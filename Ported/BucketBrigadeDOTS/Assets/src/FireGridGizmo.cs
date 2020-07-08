using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FireGridGizmo : MonoBehaviour {

    FireSim fireSim;
    float sim_WIDTH, sim_DEPTH;
	void Start () {
        fireSim = FindObjectOfType<FireSim>();
	}

	private void OnDrawGizmos()
	{
        sim_WIDTH = fireSim.rows * fireSim.cellSize;
        sim_DEPTH = fireSim.columns * fireSim.cellSize;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(sim_WIDTH * 0.5f, 0.25f, sim_DEPTH * 0.5f), new Vector3(sim_WIDTH, 0.5f, sim_DEPTH));
	}
}
