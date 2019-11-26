using UnityEngine;

static class Intersections
{
	// each side (top/bottom) has its own "occupied" flag
	// (a car on the underside doesn't block a car on the top)
	public static OccupiedSides[] Occupied;
}