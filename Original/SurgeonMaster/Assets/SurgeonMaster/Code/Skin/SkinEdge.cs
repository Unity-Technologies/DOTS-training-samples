using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class that represents a skin edge.
/// </summary>
public class SkinEdge
{
	#region Fields

	/// <summary>
	/// The skin mesh this edge is part of.
	/// </summary>
	private SkinMesh _skinMesh;

	/// <summary>
	/// A collection of all the skin triangles this edge is a part of.
	/// </summary>
	private List<SkinTriangle> _triangles = new List<SkinTriangle>();

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
	/// Whether this is part of an original edge.
	/// </summary>
	public bool originalEdge { get; set; }

	/// <summary>
	/// The stiffness.
	/// </summary>
	public float stiffness
	{
		get
		{
			return triangles.Aggregate(0.0f, (stiffness, triangle) => stiffness + triangle.cachedArea);
		}
	}

	/// <summary>
	/// A collection of all the skin triangles this edge is a part of.
	/// </summary>
	public List<SkinTriangle> triangles
	{
		get { return _triangles; }
	}

	/// <summary>
	/// The current length.
	/// </summary>
	public float currentLength
	{
		get
		{
			return (_skinMesh.GetSkinVertex(vertex0Index).currentPosition - _skinMesh.GetSkinVertex(vertex1Index).currentPosition).magnitude;
		}
	}

	/// <summary>
	/// The initial length.
	/// </summary>
	public float initialLength
	{
		get
		{
			return (_skinMesh.GetSkinVertex(vertex0Index).originalPosition - _skinMesh.GetSkinVertex(vertex1Index).originalPosition).magnitude;
		}
	}

	/// <summary>
	/// The cached spring force (this may be wrong if the vertices that make up this edge moved).
	/// Use the SpringForce or UpdateCachedSpringForce() to get the current value and update the cached value.
	/// </summary>
	public float cachedSpringForce { get; private set; }

	/// <summary>
	/// The spring force.
	/// </summary>
	public float springForce
	{
		get { return UpdateCachedSpringForce(); }
	}

	/// <summary>
	/// Whether this skin edge has to be split by the splitting algorithm.
	/// </summary>
	public bool hasToBeSplit { get; set; }

	/// <summary>
	/// Whether this skin edge is part of a cut.
	/// </summary>
	public bool isCutEdge
	{
		get { return triangles.Count == 1; }
	}

	#endregion

	#region Constructors

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="skinMesh">The skin mesh this edge is part of.</param>
	/// <param name="vertex0Index">The index of the first vertex.</param>
	/// <param name="vertex1Index">The index of the second vertex.</param>
	public SkinEdge(SkinMesh skinMesh, int vertex0Index, int vertex1Index)
	{
		_skinMesh = skinMesh;
		this.vertex0Index = vertex0Index;
		this.vertex1Index = vertex1Index;
	}

	#endregion

	#region Destructor

	/// <summary>
	/// Destructor.
	/// </summary>
	~SkinEdge()
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
			_triangles.Clear();
			_triangles = null;

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
	/// Recalculates the spring force of this skin edge and caches it.
	/// </summary>
	/// <returns>The spring force of this skin edge.</returns>
	public float UpdateCachedSpringForce()
	{
		cachedSpringForce = stiffness * _skinMesh.skinEdgeStiffnessMultiplier * 100.0f * (currentLength - initialLength);
		
		return cachedSpringForce;
	}

	/// <summary>
	/// Shuffles the vertex indices so the given vertex index corresponds with Vertex0Index.
	/// </summary>
	/// <param name="vertexIndex">The vertex index we want up front.</param>
	public void ShuffleVertexIndices(int vertexIndex)
	{
		if (vertex1Index != vertexIndex) return;
		
		vertex1Index = vertex0Index;
		vertex0Index = vertexIndex;
	}

	/// <summary>
	/// Returns the skin triangle that the given skin triangle shares this skin edge with.
	/// </summary>
	/// <param name="skinTriangle">The skin triangle for which we want the adjacent skin triangle on this skin edge.</param>
	/// <returns>The skin triangle that the given skin triangle shares this skin edge with.</returns>
	public SkinTriangle GetAdjacentSkinTriangle(SkinTriangle skinTriangle)
	{
		if (triangles.Count <= 1) return null;

		return triangles[0] == skinTriangle ? triangles[1] : triangles[0];
	}

	/// <summary>
	/// Returns a System.String that represents the current System.Object.
	/// </summary>
	/// <returns>A System.String that represents the current System.Object.</returns>
	public override string ToString()
	{
		return string.Format("edge {0} {1} - {2}" + Environment.NewLine + "{3}",
				vertex0Index,
				vertex1Index,
			originalEdge ? "original edge" : "new edge",
			triangles.Aggregate("triangles :",
				(s, triangle) =>
				s + " " + (triangle != null ? triangle.vertex0Index + " " + triangle.vertex1Index + " " + triangle.vertex2Index : "null") + ",",
				s => s.Remove(s.Length - 1)));
	}

	#endregion
}