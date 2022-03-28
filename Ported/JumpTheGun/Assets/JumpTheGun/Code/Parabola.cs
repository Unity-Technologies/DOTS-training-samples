using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpTheGun {

	public class Parabola {

		/// <summary>
		/// Creates a parabola (in the form y = a*t*t + b*t + c) that matches the following conditions:
		/// - Contains the points (0, startY) and (1, endY)
		/// - Reaches the given height.
		/// </summary>
		public static void Create(float startY, float height, float endY, out float a, out float b, out float c) {
			
			c = startY;

			float k = Mathf.Sqrt(Mathf.Abs(startY - height)) / (Mathf.Sqrt(Mathf.Abs(startY - height)) + Mathf.Sqrt(Mathf.Abs(endY - height)));
			a = (height - startY - k * (endY - startY)) / (k * k - k);

			b = endY - startY - a;
		}

		/// <summary>
		/// Solves a parabola (in the form y = a*t*t + b*t + c) for y.
		/// </summary>
		public static float Solve(float a, float b, float c, float t){
			return a * t * t + b * t + c;
		}

	}

}