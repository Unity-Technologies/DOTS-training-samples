using UnityEngine;

/// <summary>
/// Class that represents a pair of vertices.
/// </summary>
public struct VertexPair
{
	#region Fields

	/// <summary>
	/// The index of the first vertex.
	/// </summary>
	public readonly int vertex0Index;

	/// <summary>
	/// The index of the second vertex.
	/// </summary>
	public readonly int vertex1Index;

	#endregion

	#region Constructors

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="vertex0Index">The index of the first vertex.</param>
	/// <param name="vertex1Index">The index of the second vertex.</param>
	public VertexPair(int vertex0Index, int vertex1Index)
	{
		this.vertex0Index = vertex0Index;
		this.vertex1Index = vertex1Index;
	}

	#endregion

	#region Methods

	/// <summary>
	/// Equality operator.
	/// </summary>
	/// <param name="vertexPair0">The first vertex pair.</param>
	/// <param name="vertexPair1">The second vertex pair.</param>
	/// <returns>True if both vertex pairs are equal.</returns>
	public static bool operator ==(VertexPair vertexPair0, VertexPair vertexPair1)
	{
		return
			((vertexPair0.vertex0Index == vertexPair1.vertex0Index) && (vertexPair0.vertex1Index == vertexPair1.vertex1Index)) ||
			((vertexPair0.vertex0Index == vertexPair1.vertex1Index) && (vertexPair0.vertex1Index == vertexPair1.vertex0Index));
	}

	/// <summary>
	/// Inequality operator.
	/// </summary>
	/// <param name="vertexPair0">The first vertex pair.</param>
	/// <param name="vertexPair1">The second vertex pair.</param>
	/// <returns>True if both vertex pairs are not equal.</returns>
	public static bool operator !=(VertexPair vertexPair0, VertexPair vertexPair1)
	{
		return !(vertexPair0 == vertexPair1);
	}

	/// <summary>
	/// Returns whether the given vertex pair is considered equal to this object.
	/// </summary>
	/// <param name="other">The other vertex pair.</param>
	/// <returns>Whether the given vertex pair is considered equal to this object.</returns>
	public bool Equals(VertexPair other)
	{
		return
			((vertex0Index == other.vertex0Index) && (vertex1Index == other.vertex1Index)) ||
			((vertex0Index == other.vertex1Index) && (vertex1Index == other.vertex0Index));
	}

	/// <summary>
	/// Returns whether the given object is considered equal to this object.
	/// </summary>
	/// <param name="obj">The other object.</param>
	/// <returns>Whether the given object is considered equal to this object.</returns>
	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (obj.GetType() != GetType()) return false;

		return Equals((VertexPair)obj);
	}

	/// <summary>
	/// Returns a hash code for this object.
	/// </summary>
	/// <returns>A hash code for this object.</returns>
	public override int GetHashCode()
	{
		unchecked
		{
			int min = Mathf.Min(vertex0Index, vertex1Index);
			int max = Mathf.Max(vertex0Index, vertex1Index);
			return (min * 397) ^ max;
		}
	}

	/// <summary>
	/// Returns a System.String that represents the current System.Object.
	/// </summary>
	/// <returns>A System.String that represents the current System.Object.</returns>
	public override string ToString()
	{
		return string.Format("verter pair {0} {1}",
				vertex0Index,
				vertex1Index);
	}

	#endregion
}