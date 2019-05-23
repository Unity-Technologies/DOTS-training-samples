using UnityEngine;
using System.Text;

/// <summary>
/// Static class with math helper functions.
/// </summary>
public static class MathHelper
{
	#region Methods

	/// <summary>
	/// Returns the projected point p' on the line through points a and b.
	/// http://en.wikipedia.org/wiki/Vector_projection
	/// </summary>
	/// <param name="a">The first line point.</param>
	/// <param name="b">The second line point.</param>
	/// <param name="p">The point we want to project on the line through points a and b.</param>
	/// <param name="insideTolerance">The tolerance to determine if the projected point is inside the line segment.</param>
	/// <returns>The projected point p' on the line through points a and b.</returns>
	public static Vector3 GetProjectedPointOnLine(Vector3 a, Vector3 b, Vector3 p,
		float insideTolerance = 0.0f)
	{
		bool insideLineSegment;
		float t;
		return GetProjectedPointOnLine(a, b, p, out insideLineSegment, out t, insideTolerance);
	}

	/// <summary>
	/// Returns the projected point p' on the line through points a and b.
	/// http://en.wikipedia.org/wiki/Vector_projection
	/// </summary>
	/// <param name="a">The first line point.</param>
	/// <param name="b">The second line point.</param>
	/// <param name="p">The point we want to project on the line through points a and b.</param>
	/// <param name="insideLineSegment">Whether the projected point p' lies inside the line segment ab.</param>
	/// <param name="insideTolerance">The tolerance to determine if the projected point is inside the line segment.</param>
	/// <returns>The projected point p' on the line through points a and b.</returns>
	public static Vector3 GetProjectedPointOnLine(Vector3 a, Vector3 b, Vector3 p,
		out bool insideLineSegment, float insideTolerance = 0.0f)
	{
		float t;
		return GetProjectedPointOnLine(a, b, p, out insideLineSegment, out t, insideTolerance);
	}

	/// <summary>
	/// Returns the projected point p' on the line through points a and b.
	/// http://en.wikipedia.org/wiki/Vector_projection
	/// </summary>
	/// <param name="a">The first line point.</param>
	/// <param name="b">The second line point.</param>
	/// <param name="p">The point we want to project on the line through points a and b.</param>
	/// <param name="t">The percentage to multiply the vector from point a to point b with,
	/// to get the projected point p' on the line throught points a and b.</param>
	/// <param name="insideTolerance">The tolerance to determine if the projected point is inside the line segment.</param>
	/// <returns>The projected point p' on the line through points a and b.</returns>
	public static Vector3 GetProjectedPointOnLine(Vector3 a, Vector3 b, Vector3 p,
		out float t, float insideTolerance = 0.0f)
	{
		bool insideLineSegment;
		return GetProjectedPointOnLine(a, b, p, out insideLineSegment, out t, insideTolerance);
	}

	/// <summary>
	/// Returns the projected point p' on the line through points a and b.
	/// http://en.wikipedia.org/wiki/Vector_projection
	/// </summary>
	/// <param name="a">The first line point.</param>
	/// <param name="b">The second line point.</param>
	/// <param name="p">The point we want to project on the line through points a and b.</param>
	/// <param name="insideLineSegment">Whether the projected point p' lies inside the line segment ab.</param>
	/// <param name="t">The percentage to multiply the vector from point a to point b with,
	/// to get the projected point p' on the line throught points a and b.</param>
	/// <param name="insideTolerance">The tolerance to determine if the projected point is inside the line segment.</param>
	/// <returns>The projected point p' on the line through points a and b.</returns>
	public static Vector3 GetProjectedPointOnLine(Vector3 a, Vector3 b, Vector3 p,
		out bool insideLineSegment, out float t, float insideTolerance = 0.0f)
	{
		float abLength = (b - a).magnitude;
		if (Mathf.Approximately(abLength,0f))
		{
			t = 0;
			insideLineSegment = true;
			return a;
		}

		Vector3 abn = (b - a).normalized;
		Vector3 ap = p - a;

		t = Vector3.Dot(ap, abn) / abLength;

		insideLineSegment = (t >= -insideTolerance) && (t <= (1.0f + insideTolerance));

		return (1 - t) * a + t * b;
	}


	/// <summary>
	/// Returns the projected point p' on the triangle through points a, b and c.
	/// http://www.blackpawn.com/texts/pointinpoly/
	/// </summary>
	/// <param name="a">The first triangle point.</param>
	/// <param name="b">The second triangle point.</param>
	/// <param name="c">The third triangle point.</param>
	/// <param name="p">The point we want to project on the triangle through points a, b and c.</param>
	/// <param name="insideTolerance">The tolerance to determine if the projected point is inside the triangle.</param>
	/// <returns>The projected point p' on the triangle through points a, b and c.</returns>
	public static Vector3 GetProjectedPointOnTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p,
		float insideTolerance = 0.0f)
	{
		bool insideTriangle;
		float u, v;
		return GetProjectedPointOnTriangle(a, b, c, p, out insideTriangle, out u, out v, insideTolerance);
	}

	/// <summary>
	/// Returns the projected point p' on the triangle through points a, b and c.
	/// http://www.blackpawn.com/texts/pointinpoly/
	/// </summary>
	/// <param name="a">The first triangle point.</param>
	/// <param name="b">The second triangle point.</param>
	/// <param name="c">The third triangle point.</param>
	/// <param name="p">The point we want to project on the triangle through points a, b and c.</param>
	/// <param name="insideTriangle">Whether the projected point p' lies inside the triangle abc.</param>
	/// <param name="insideTolerance">The tolerance to determine if the projected point is inside the triangle.</param>
	/// <returns>The projected point p' on the triangle through points a, b and c.</returns>
	public static Vector3 GetProjectedPointOnTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p,
		out bool insideTriangle, float insideTolerance = 0.0f)
	{
		float u, v;
		return GetProjectedPointOnTriangle(a, b, c, p, out insideTriangle, out u, out v, insideTolerance);
	}

	/// <summary>
	/// Returns the projected point p' on the triangle through points a, b and c.
	/// http://www.blackpawn.com/texts/pointinpoly/
	/// </summary>
	/// <param name="a">The first triangle point.</param>
	/// <param name="b">The second triangle point.</param>
	/// <param name="c">The third triangle point.</param>
	/// <param name="p">The point we want to project on the triangle through points a, b and c.</param>
	/// <param name="u">The barycentric coordinate in the direction of the vector ac.</param>
	/// <param name="v">The barycentric coordinate in the direction of the vector ab.</param>
	/// <param name="insideTolerance">The tolerance to determine if the projected point is inside the triangle.</param>
	/// <returns>The projected point p' on the triangle through points a, b and c.</returns>
	public static Vector3 GetProjectedPointOnTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p,
		out float u, out float v, float insideTolerance = 0.0f)
	{
		bool insideTriangle;
		return GetProjectedPointOnTriangle(a, b, c, p, out insideTriangle, out u, out v, insideTolerance);
	}

	/// <summary>
	/// Returns the projected point p' on the triangle through points a, b and c.
	/// http://www.blackpawn.com/texts/pointinpoly/
	/// </summary>
	/// <param name="a">The first triangle point.</param>
	/// <param name="b">The second triangle point.</param>
	/// <param name="c">The third triangle point.</param>
	/// <param name="p">The point we want to project on the triangle through points a, b and c.</param>
	/// <param name="insideTriangle">Whether the projected point p' lies inside the triangle abc.</param>
	/// <param name="u">The barycentric coordinate in the direction of the vector ac.</param>
	/// <param name="v">The barycentric coordinate in the direction of the vector ab.</param>
	/// <param name="insideTolerance">The tolerance to determine if the projected point is inside the triangle.</param>
	/// <returns>The projected point p' on the triangle through points a, b and c.</returns>
	public static Vector3 GetProjectedPointOnTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p,
		out bool insideTriangle, out float u, out float v, float insideTolerance = 0.0f)
	{
		Vector3 v0 = c - a;
		Vector3 v1 = b - a;
		Vector3 v2 = a - p;

		float dot00 = Vector3.Dot(v0, v0);
		float dot01 = Vector3.Dot(v0, v1);
		float dot02 = Vector3.Dot(v0, v2);
		float dot11 = Vector3.Dot(v1, v1);
		float dot12 = Vector3.Dot(v1, v2);
		float det = Mathf.Abs(dot00 * dot11 - dot01 * dot01);
		float invDet = (1.0f) / det;
		u = (dot01 * dot12 - dot11 * dot02) * invDet;
		v = (dot01 * dot02 - dot00 * dot12) * invDet;

		insideTriangle = (u >= -insideTolerance) && (v >= -insideTolerance) && (u + v < (1.0f + insideTolerance));

		return a + u * v0 + v * v1;
	}

	/// <summary>
	/// Returns the closest point p' on the triangle through points a, b and c.
	/// http://www.blackpawn.com/texts/pointinpoly/
	/// http://www.geometrictools.com/LibMathematics/Distance/Distance.html
	/// </summary>
	/// <param name="a">The first triangle point.</param>
	/// <param name="b">The second triangle point.</param>
	/// <param name="c">The third triangle point.</param>
	/// <param name="p">The point we want the closest point for on the triangle through points a, b and c.</param>
	/// <returns>The closest point p' on the triangle through points a, b and c.</returns>
	public static Vector3 GetClosestPointInsideTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p)
	{
		float u, v;
		return GetClosestPointInsideTriangle(a, b, c, p, out u, out v);
	}

	/// <summary>
	/// Returns the closest point p' on the triangle through points a, b and c.
	/// http://www.blackpawn.com/texts/pointinpoly/
	/// http://www.geometrictools.com/LibMathematics/Distance/Distance.html
	/// </summary>
	/// <param name="a">The first triangle point.</param>
	/// <param name="b">The second triangle point.</param>
	/// <param name="c">The third triangle point.</param>
	/// <param name="p">The point we want the closest point for on the triangle through points a, b and c.</param>
	/// <param name="u">The barycentric coordinate in the direction of the vector ac.</param>
	/// <param name="v">The barycentric coordinate in the direction of the vector ab.</param>
	/// <returns>The closest point p' on the triangle through points a, b and c.</returns>
	public static Vector3 GetClosestPointInsideTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p, out float u, out float v)
	{
		Vector3 v0 = c - a;
		Vector3 v1 = b - a;
		Vector3 v2 = a - p;

		float dot00 = Vector3.Dot(v0, v0);
		float dot01 = Vector3.Dot(v0, v1);
		float dot02 = Vector3.Dot(v0, v2);
		float dot11 = Vector3.Dot(v1, v1);
		float dot12 = Vector3.Dot(v1, v2);
		float det = Mathf.Abs(dot00 * dot11 - dot01 * dot01);
		u = dot01 * dot12 - dot11 * dot02;
		v = dot01 * dot02 - dot00 * dot12;

		if (u + v <= det)
		{
			if (u < 0.0f)
			{
				if (v < 0.0f)
				{
					if (dot02 < 0.0f)
					{
						v = 0.0f;
						if (-dot02 >= dot00)
						{
							u = 1.0f;
						}
						else
						{
							u = -dot02 / dot00;
						}
					}
					else
					{
						u = 0.0f;
						if (dot12 >= 0.0f)
						{
							v = 0.0f;
						}
						else if (-dot12 >= dot11)
						{
							v = 1.0f;
						}
						else
						{
							v = -dot12 / dot11;
						}
					}
				}
				else
				{
					u = 0.0f;
					if (dot12 >= 0.0f)
					{
						v = 0.0f;
					}
					else if (-dot12 >= dot11)
					{
						v = 1.0f;
					}
					else
					{
						v = -dot12 / dot11;
					}
				}
			}
			else if (v < 0.0f)
			{
				v = 0.0f;
				if (dot02 >= 0.0f)
				{
					u = 0.0f;
				}
				else if (-dot02 >= dot00)
				{
					u = 1.0f;
				}
				else
				{
					u = -dot02 / dot00;
				}
			}
			else
			{
				float invDet = (1.0f) / det;
				u *= invDet;
				v *= invDet;
			}
		}
		else
		{
			float tmp0, tmp1, numer, denom;

			if (u < 0.0f)
			{
				tmp0 = dot01 + dot02;
				tmp1 = dot11 + dot12;
				if (tmp1 > tmp0)
				{
					numer = tmp1 - tmp0;
					denom = dot00 - 2.0f * dot01 + dot11;
					if (numer >= denom)
					{
						u = 1.0f;
						v = 0.0f;
					}
					else
					{
						u = numer / denom;
						v = 1.0f - u;
					}
				}
				else
				{
					u = 0.0f;
					if (tmp1 <= 0.0f)
					{
						v = 1.0f;
					}
					else if (dot12 >= 0.0f)
					{
						v = 0.0f;
					}
					else
					{
						v = -dot12 / dot11;
					}
				}
			}
			else if (v < 0.0f)
			{
				tmp0 = dot01 + dot12;
				tmp1 = dot00 + dot02;
				if (tmp1 > tmp0)
				{
					numer = tmp1 - tmp0;
					denom = dot00 - 2.0f * dot01 + dot11;
					if (numer >= denom)
					{
						v = 1.0f;
						u = 0.0f;
					}
					else
					{
						v = numer / denom;
						u = 1.0f - v;
					}
				}
				else
				{
					v = 0.0f;
					if (tmp1 <= 0.0f)
					{
						u = 1.0f;
					}
					else if (dot02 >= 0.0f)
					{
						u = 0.0f;
					}
					else
					{
						u = -dot02 / dot00;
					}
				}
			}
			else
			{
				numer = dot11 + dot12 - dot01 - dot02;
				if (numer <= 0.0f)
				{
					u = 0.0f;
					v = 1.0f;
				}
				else
				{
					denom = dot00 - 2.0f * dot01 + dot11;
					if (numer >= denom)
					{
						u = 1.0f;
						v = 0.0f;
					}
					else
					{
						u = numer / denom;
						v = 1.0f - u;
					}
				}
			}
		}

		return a + u * v0 + v * v1;
	}

	/// <summary>
	/// Returns the intersection point p' between the triangle defined by points a, b and c and the line defined by the points p and q.
	/// http://en.wikipedia.org/wiki/Line-plane_intersection
	/// </summary>
	/// <param name="a">The first triangle point.</param>
	/// <param name="b">The second triangle point.</param>
	/// <param name="c">The third triangle point.</param>
	/// <param name="p">The first line point.</param>
	/// <param name="q">The second line point.</param>
	/// <param name="insideTolerance">The tolerance to determine if the intersection point is inside the triangle and inside the line segment.</param>
	/// <returns>The intersection point p' between the triangle defined by points a, b and c and the line defined by the points p and q.</returns>
	public static Vector3 GetTriangleLineIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 p, Vector3 q,
		float insideTolerance = 0.0f)
	{
		float u, v, t;
		bool insideTriangle, insideLineSegment;
		return GetTriangleLineIntersection(a, b, c, p, q, out insideTriangle, out insideLineSegment, out u, out v, out t, insideTolerance);
	}

	/// <summary>
	/// Returns the intersection point p' between the triangle defined by points a, b and c and the line defined by the points p and q.
	/// http://en.wikipedia.org/wiki/Line-plane_intersection
	/// </summary>
	/// <param name="a">The first triangle point.</param>
	/// <param name="b">The second triangle point.</param>
	/// <param name="c">The third triangle point.</param>
	/// <param name="p">The first line point.</param>
	/// <param name="q">The second line point.</param>
	/// <param name="insideTriangle">Whether the intersection point p' lies inside the triangle abc.</param>
	/// <param name="insideLineSegment">Whether the intersection point p' lies inside the line segment ab.</param>
	/// <param name="insideTolerance">The tolerance to determine if the intersection point is inside the triangle and inside the line segment.</param>
	/// <returns>The intersection point p' between the triangle defined by points a, b and c and the line defined by the points p and q.</returns>
	public static Vector3 GetTriangleLineIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 p, Vector3 q,
		out bool insideTriangle, out bool insideLineSegment, float insideTolerance = 0.0f)
	{
		float u, v, t;
		return GetTriangleLineIntersection(a, b, c, p, q, out insideTriangle, out insideLineSegment, out u, out v, out t, insideTolerance);
	}

	/// <summary>
	/// Returns the intersection point p' between the triangle defined by points a, b and c and the line defined by the points p and q.
	/// http://en.wikipedia.org/wiki/Line-plane_intersection
	/// </summary>
	/// <param name="a">The first triangle point.</param>
	/// <param name="b">The second triangle point.</param>
	/// <param name="c">The third triangle point.</param>
	/// <param name="p">The first line point.</param>
	/// <param name="q">The second line point.</param>
	/// <param name="u">The coordinate of the intersection point in the direction of the vector ac.</param>
	/// <param name="v">The coordinate of the intersection point in the direction of the vector ab.</param>
	/// <param name="t">The coordinate of the intersection point in the direction of the vector pq.</param>
	/// <param name="insideTolerance">The tolerance to determine if the intersection point is inside the triangle and inside the line segment.</param>
	/// <returns>The intersection point p' between the triangle defined by points a, b and c and the line defined by the points p and q.</returns>
	public static Vector3 GetTriangleLineIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 p, Vector3 q,
		out float u, out float v, out float t, float insideTolerance = 0.0f)
	{
		bool insideTriangle, insideLineSegment;
		return GetTriangleLineIntersection(a, b, c, p, q, out insideTriangle, out insideLineSegment, out u, out v, out t, insideTolerance);
	}

	/// <summary>
	/// Returns the intersection point p' between the triangle defined by points a, b and c and the line defined by the points p and q.
	/// http://en.wikipedia.org/wiki/Line-plane_intersection
	/// </summary>
	/// <param name="a">The first triangle point.</param>
	/// <param name="b">The second triangle point.</param>
	/// <param name="c">The third triangle point.</param>
	/// <param name="p">The first line point.</param>
	/// <param name="q">The second line point.</param>
	/// <param name="insideTriangle">Whether the intersection point p' lies inside the triangle abc.</param>
	/// <param name="insideLineSegment">Whether the intersection point p' lies inside the line segment ab.</param>
	/// <param name="u">The coordinate of the intersection point in the direction of the vector ac.</param>
	/// <param name="v">The coordinate of the intersection point in the direction of the vector ab.</param>
	/// <param name="t">The coordinate of the intersection point in the direction of the vector pq.</param>
	/// <param name="insideTolerance">The tolerance to determine if the intersection point is inside the triangle and inside the line segment.</param>
	/// <returns>The intersection point p' between the triangle defined by points a, b and c and the line defined by the points p and q.</returns>
    public static Vector3 GetTriangleLineIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 p, Vector3 q,
		out bool insideTriangle, out bool insideLineSegment, out float u, out float v, out float t, float insideTolerance = 0.0f)
	{
		float[,] matrix = new float[3, 4];
		matrix[0, 0] = c.x - a.x;
		matrix[1, 0] = c.y - a.y;
		matrix[2, 0] = c.z - a.z;

		matrix[0, 1] = b.x - a.x;
		matrix[1, 1] = b.y - a.y;
		matrix[2, 1] = b.z - a.z;

		matrix[0, 2] = p.x - q.x;
		matrix[1, 2] = p.y - q.y;
		matrix[2, 2] = p.z - q.z;

		matrix[0, 3] = p.x - a.x;
		matrix[1, 3] = p.y - a.y;
		matrix[2, 3] = p.z - a.z;

		matrix = ConvertToReducedRowEchelonForm(matrix);

		if (Mathf.Approximately(matrix[0, 0],0f) ||
			Mathf.Approximately(matrix[1, 1],0f) ||
			Mathf.Approximately(matrix[2, 2],0f) ||
			!Mathf.Approximately(matrix[1, 2],0f) ||
			!Mathf.Approximately(matrix[0, 2],0f) )
		{
			u = v = t = float.PositiveInfinity;
			insideLineSegment = insideTriangle = false;
			return new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
		}

		u = matrix[0, 3] / matrix[0, 0];
		v = matrix[1, 3] / matrix[1, 1];
		t = matrix[2, 3] / matrix[2, 2];

		insideLineSegment = (t >= -insideTolerance) && (t <= (1.0f + insideTolerance));
		insideTriangle = (u >= -insideTolerance) && (v >= -insideTolerance) && (u + v < (1.0f + insideTolerance));

		return (1 - t) * p + t * q;
	}

	/// <summary>
	/// Prints the given matrix to the debug log.
	/// </summary>
	/// <param name="matrix">The matrix to log.</param>
	private static void PrintMatrix(float[,] matrix)
	{
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < matrix.GetLength(0); ++i)
		{
			for (int j = 0; j < matrix.GetLength(1); ++j)
			{
				sb.Append(matrix[i, j] + " ");
			}
			sb.AppendLine();
		}

		Debug.Log(sb.ToString());
	}

	/// <summary>
	/// Convert the given matrix to its reduced row echelon form.
	/// </summary>
	/// <param name="matrix">The matrix to convert.</param>
	/// <returns>The reduced row echolon form of the given matrix.</returns>
	public static float[,] ConvertToReducedRowEchelonForm(float[,] matrix)
	{
		int lead = 0;
		int rowCount = matrix.GetLength(0);
		int columnCount = matrix.GetLength(1);
		
		for (int r = 0; r < rowCount; ++r)
		{
			if (columnCount <= lead) break;
			
			int i = r;
			while (Mathf.Abs(matrix[i, lead]) < 0.001f)
			{
				i++;
				if (i != rowCount) continue;
				
				i = r;
				lead++;
				if (columnCount != lead) continue;
				
				lead--;
				break;
			}
			
			for (int j = 0; j < columnCount; ++j)
			{
				float temp = matrix[r, j];
				matrix[r, j] = matrix[i, j];
				matrix[i, j] = temp;
			}

			float div = matrix[r, lead];
			for (int j = 0; j < columnCount; ++j)
			{
				matrix[r, j] /= div;
			}

			for (int j = 0; j < rowCount; ++j)
			{
				if (j == r) continue;
				
				float sub = matrix[j, lead];
				for (int k = 0; k < columnCount; ++k)
				{
					matrix[j, k] -= (sub * matrix[r, k]);
				}
			}
			lead++;
		}
		return matrix;
	}

	/// <summary>
	/// Returns whether the triangles abc and pqr intersect.
	/// </summary>
	/// <param name="a">The first point of the first triangle.</param>
	/// <param name="b">The second point of the first triangle.</param>
	/// <param name="c">The third point of the first triangle.</param>
	/// <param name="p">The first point of the second triangle.</param>
	/// <param name="q">The second point of the second triangle.</param>
	/// <param name="r">The third point of the second triangle.</param>
	/// <param name="l1">The first point of the intersection line.</param>
	/// <param name="l2">The second point of the intersection line.</param>
	/// <param name="insideTolerance">The tolerance to determine if the intersection point is inside the triangle and inside the line segment.</param>
	/// <returns>whether the triangles abc and pqr intersect.</returns>
	public static bool GetTriangleTriangleIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 p, Vector3 q, Vector3 r,
		out Vector3 l1, out Vector3 l2, float insideTolerance = 0.0f)
	{
		int numberIntersectionPointsFound = 0;
		Vector3[] intersectionPoints = new Vector3[2];

		GetIntersectionPoint(a, b, c, p, q, ref numberIntersectionPointsFound, ref intersectionPoints, insideTolerance);
		GetIntersectionPoint(a, b, c, q, r, ref numberIntersectionPointsFound, ref intersectionPoints, insideTolerance);
		GetIntersectionPoint(a, b, c, r, p, ref numberIntersectionPointsFound, ref intersectionPoints, insideTolerance);
		GetIntersectionPoint(p, q, r, a, b, ref numberIntersectionPointsFound, ref intersectionPoints, insideTolerance);
		GetIntersectionPoint(p, q, r, b, c, ref numberIntersectionPointsFound, ref intersectionPoints, insideTolerance);
		GetIntersectionPoint(p, q, r, c, a, ref numberIntersectionPointsFound, ref intersectionPoints, insideTolerance);

		l1 = intersectionPoints[0];
		l2 = intersectionPoints[1];

		return numberIntersectionPointsFound == 2;
	}

	/// <summary>
	/// Checks if the intersection point p' between the triangle defined by points a, b and c and the line defined by the points p and q
	/// lies inside the triangle abc and inside line segment pq. If so, adds it to the array that contains the intersection points that have already been found.
	/// </summary>
	/// <param name="a">The first triangle point.</param>
	/// <param name="b">The second triangle point.</param>
	/// <param name="c">The third triangle point.</param>
	/// <param name="p">The first line point.</param>
	/// <param name="q">The second line point.</param>
	/// <param name="numberIntersectionPointsFound">The number of intersection points that have already been found.</param>
	/// <param name="intersectionPoints">The array containing the intersection points that have already been found.</param>
	/// <param name="insideTolerance">The tolerance to determine if the intersection point is inside the triangle and inside the line segment.</param>
	private static void GetIntersectionPoint(Vector3 a, Vector3 b, Vector3 c, Vector3 p, Vector3 q,
		ref int numberIntersectionPointsFound, ref Vector3[] intersectionPoints, float insideTolerance = 0.0f)
	{
		if (numberIntersectionPointsFound >= 2) return;
		
		bool insideLineSegment;
		bool insideTriangle;
		Vector3 intersectionPoint = GetTriangleLineIntersection(a, b, c, p, q, out insideTriangle, out insideLineSegment, insideTolerance);
		if (insideLineSegment == false || insideTriangle == false) return;

		if (numberIntersectionPointsFound == 1 && Mathf.Approximately((intersectionPoints[0] - intersectionPoint).magnitude, 0f)) return;

		intersectionPoints[numberIntersectionPointsFound] = intersectionPoint;
		numberIntersectionPointsFound++;
	}




	#endregion
}