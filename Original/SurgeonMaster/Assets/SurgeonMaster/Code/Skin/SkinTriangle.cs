using UnityEngine;

using System;
using System.Linq;

/// <summary>
/// Class that represents a skin triangle.
/// </summary>
public class SkinTriangle
{
	#region Fields

	/// <summary>
	/// The skin mesh this triangle is part of.
	/// </summary>
	private SkinMesh _skinMesh;

	/// <summary>
	/// Indicates whether this object has been disposed already.
	/// </summary>
	private bool _isDisposed;

	#endregion

	#region Properties

	/// <summary>
	/// The index of the first vertex.
	/// </summary>
	public int vertex0Index { get; private set; }

	/// <summary>
	/// The index of the second vertex.
	/// </summary>
	public int vertex1Index { get; private set; }

	/// <summary>
	/// The index of the third vertex.
	/// </summary>
	public int vertex2Index { get; private set; }

	/// <summary>
	/// A collection of all the skin edges that make up this triangle.
	/// </summary>
	public SkinEdge[] edges
	{
		get
		{
			SkinEdge[] edges = new SkinEdge[3];
			edges[0] = _skinMesh.GetSkinEdge(new VertexPair(vertex0Index, vertex1Index));
			edges[1] = _skinMesh.GetSkinEdge(new VertexPair(vertex1Index, vertex2Index));
			edges[2] = _skinMesh.GetSkinEdge(new VertexPair(vertex2Index, vertex0Index));
			return edges;
		}
	}

	/// <summary>
	/// The normal.
	/// </summary>
	public Vector3 normal
	{
		get
		{
			Vector3 v0 = _skinMesh.GetSkinVertex(vertex0Index).currentPosition;
			Vector3 v1 = _skinMesh.GetSkinVertex(vertex1Index).currentPosition;
			Vector3 v2 = _skinMesh.GetSkinVertex(vertex2Index).currentPosition;

			return Vector3.Cross(v1 - v0, v2 - v0);
		}
	}

	/// <summary>
	/// The cached area (this may be wrong if the vertices that make up this triangle moved).
	/// Use the Area or UpdateArea() to get the current value and update the cached value.
	/// </summary>
	public float cachedArea { get; private set; }

	/// <summary>
	/// The area.
	/// </summary>
	public float area
	{
		get { return UpdateCachedArea(); }
	}

	#endregion
	
	#region Constructors

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="skinMesh">The skin mesh this triangle is part of.</param>
	/// <param name="vertex0Index">The index of the first vertex.</param>
	/// <param name="vertex1Index">The index of the second vertex.</param>
	/// <param name="vertex2Index">The index of the third vertex.</param>
	public SkinTriangle(SkinMesh skinMesh, int vertex0Index, int vertex1Index, int vertex2Index)
	{
		_skinMesh = skinMesh;
		this.vertex0Index = vertex0Index;
		this.vertex1Index = vertex1Index;
		this.vertex2Index = vertex2Index;
	}

	#endregion

	#region Destructor

	/// <summary>
	/// Destructor.
	/// </summary>
	~SkinTriangle()
	{
		Dispose(false);
	}

	/// <summary>
	/// Disposer.
	/// </summary>
	/// <param name="disposing">Indicates whether to dispose managed resources or not.</param>
	private void Dispose(bool disposing)
	{
		if (_isDisposed == false && disposing)
		{
			//Dispose managed resources.
			_skinMesh = null;
		}
		//Dispose unmanaged resources.
		_isDisposed = true;
	}

	/// <summary>
	/// Disposer.
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	#endregion

	#region Methods

	/// <summary>
	/// Recalculates the area of this skin triangle and caches it.
	/// </summary>
	/// <returns>The area of this skin triangle.</returns>
	public float UpdateCachedArea()
	{
		Vector3 v0 = _skinMesh.GetSkinVertex(vertex0Index).currentPosition;
		Vector3 v1 = _skinMesh.GetSkinVertex(vertex1Index).currentPosition;
		Vector3 v2 = _skinMesh.GetSkinVertex(vertex2Index).currentPosition;

		Vector3 v2P = MathHelper.GetProjectedPointOnLine(v0, v1, v2);

		cachedArea = (v0 - v1).magnitude * (v2 - v2P).magnitude * 0.5f;

		return cachedArea;
	}

	/// <summary>
	/// Shuffles the vertex indices so the given vertex index corresponds with Vertex0Index.
	/// </summary>
	/// <param name="vertexIndex">The vertex index we want up front.</param>
	public void ShuffleVertexIndices(int vertexIndex)
	{
		if (vertex1Index == vertexIndex)
		{
			vertex1Index = vertex2Index;
			vertex2Index = vertex0Index;
			vertex0Index = vertexIndex;
		}
		else if (vertex2Index == vertexIndex)
		{
			vertex2Index = vertex1Index;
			vertex1Index = vertex0Index;
			vertex0Index = vertexIndex;
		}
	}

	/// <summary>
	/// Shuffles the vertex indices so the two vertex indices contained in the given vertex pair correspond with Vertex0Index and Vertex1Index.
	/// </summary>
	/// <param name="vertexPair">The vertex pair whose vertex indices we want up front.</param>
	public void ShuffleVertexIndices(VertexPair vertexPair)
	{
		if (new VertexPair(vertex1Index, vertex2Index) == vertexPair)
		{
			int temp = vertex1Index;
			vertex1Index = vertex2Index;
			vertex2Index = vertex0Index;
			vertex0Index = temp;
		}
		else if (new VertexPair(vertex0Index, vertex2Index) == vertexPair)
		{
			int temp = vertex2Index;
			vertex2Index = vertex1Index;
			vertex1Index = vertex0Index;
			vertex0Index = temp;
		}
	}

	/// <summary>
	/// Equality operator.
	/// </summary>
	/// <param name="skinTriangle0">The first skin triangle.</param>
	/// <param name="skinTriangle1">The second skin triangle.</param>
	/// <returns>True if both skin triangles are equal.</returns>
	public static bool operator ==(SkinTriangle skinTriangle0, SkinTriangle skinTriangle1)
	{
		if (ReferenceEquals(skinTriangle0, skinTriangle1))
		{
			return true;
		}

		//Cast to object to avoid infinite loop.
		if (((object)skinTriangle0 == null) || ((object)skinTriangle1 == null))
		{
			return false;
		}

		return
			((skinTriangle0.vertex0Index == skinTriangle1.vertex0Index) && (skinTriangle0.vertex1Index == skinTriangle1.vertex1Index) && (skinTriangle0.vertex2Index == skinTriangle1.vertex2Index)) ||
			((skinTriangle0.vertex0Index == skinTriangle1.vertex1Index) && (skinTriangle0.vertex1Index == skinTriangle1.vertex2Index) && (skinTriangle0.vertex2Index == skinTriangle1.vertex0Index)) ||
			((skinTriangle0.vertex0Index == skinTriangle1.vertex2Index) && (skinTriangle0.vertex1Index == skinTriangle1.vertex0Index) && (skinTriangle0.vertex2Index == skinTriangle1.vertex1Index));
	}

	/// <summary>
	/// Inequality operator.
	/// </summary>
	/// <param name="skinTriangle0">The first skin triangle.</param>
	/// <param name="skinTriangle1">The second skin triangle.</param>
	/// <returns>True if both skin triangles are not equal.</returns>
	public static bool operator !=(SkinTriangle skinTriangle0, SkinTriangle skinTriangle1)
	{
		return !(skinTriangle0 == skinTriangle1);
	}

	/// <summary>
	/// Returns whether the given skin triangle is considered equal to this object.
	/// </summary>
	/// <param name="other">The other skin triangle.</param>
	/// <returns>Whether the given skin triangle is considered equal to this object.</returns>
	public bool Equals(SkinTriangle other)
	{
		if ((object)other == null)
		{
			return false;
		}

		return
			((vertex0Index == other.vertex0Index) && (vertex1Index == other.vertex1Index) && (vertex2Index == other.vertex2Index)) ||
			((vertex0Index == other.vertex1Index) && (vertex1Index == other.vertex2Index) && (vertex2Index == other.vertex0Index)) ||
			((vertex0Index == other.vertex2Index) && (vertex1Index == other.vertex0Index) && (vertex2Index == other.vertex1Index));
	}

	/// <summary>
	/// Returns whether the given object is considered equal to this object.
	/// </summary>
	/// <param name="obj">The other object.</param>
	/// <returns>Whether the given object is considered equal to this object.</returns>
	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;

		return Equals((SkinTriangle)obj);
	}

	/// <summary>
	/// Returns a hash code for this object.
	/// </summary>
	/// <returns>A hash code for this object.</returns>
	public override int GetHashCode()
	{
		unchecked
		{
			int min = Mathf.Min(vertex0Index, vertex1Index, vertex2Index);
			int max = Mathf.Max(vertex0Index, vertex1Index, vertex2Index);
			int median = vertex0Index + vertex1Index + vertex2Index - min - max;
				
			int hashCode = min;
			hashCode = (hashCode * 397) ^ median;
			hashCode = (hashCode * 397) ^ max;
			return hashCode;
		}
	}

	/// <summary>
	/// Returns a System.String that represents the current System.Object.
	/// </summary>
	/// <returns>A System.String that represents the current System.Object.</returns>
	public override string ToString()
	{
		return string.Format("triangle {0} {1} {2}" + Environment.NewLine + "{3}",
				vertex0Index,
				vertex1Index,
				vertex2Index,
			edges.Aggregate("edges :",
			(s, edge) => s + " " + (edge != null ? edge.vertex0Index + " " + edge.vertex1Index : "null") + ",",
				s => s.Remove(s.Length - 1)));
	}

	#endregion
}