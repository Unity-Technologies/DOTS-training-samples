using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path {
	public List<int> xPositions;
	public List<int> yPositions;

	public int count {
		get {
			return xPositions.Count;
		}
	}

	public Path() {
		xPositions = new List<int>(100);
		yPositions = new List<int>(100);
	}

	public void Clear() {
		xPositions.Clear();
		yPositions.Clear();
	}
}
