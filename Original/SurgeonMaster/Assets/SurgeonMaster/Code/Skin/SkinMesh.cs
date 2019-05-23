using UnityEngine;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

/// <summary>
/// Class that handles the skin mesh.
/// </summary>
public class SkinMesh
{
	#region Types


	/// <summary>
	/// Struct to contain cut line segment data.
	/// </summary>
	public struct CutLineSegment
	{
		public SkinTriangle skinTriangle;
		public Vector3 startPoint;
		public Vector3 endPoint;
	}

	/// <summary>
	/// Struct to represent a skin sub mesh.
	/// </summary>
	private class SkinSubMesh
	{
		#region Fields

		/// <summary>
		/// The list of skin triangles.
		/// </summary>
		private List<SkinTriangle> _triangles = new List<SkinTriangle>();

		/// <summary>
		/// The alpha.
		/// </summary>
		private float _alpha = 1.0f;
		
		/// <summary>
		/// Indicates whether this object has been disposed already.
		/// </summary>
		private bool _isDisposed;

		#endregion

		#region Properties

		/// <summary>
		/// The list of skin triangles.
		/// </summary>
		public List<SkinTriangle> triangles
		{
			get { return _triangles; }
		}

		/// <summary>
		/// The alpha.
		/// </summary>
		public float alpha
		{
			get { return _alpha; }
			set { _alpha = Mathf.Clamp01(value); }
		}

		/// <summary>
		/// Whether this skin sub mesh is currently fading.
		/// </summary>
		public bool fading { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fading">Whether this skin sub mesh is currently fading.</param>
		public SkinSubMesh(bool fading)
		{
			this.fading = fading;
		}

		#endregion

		#region Destructor

		/// <summary>
		/// Destructor.
		/// </summary>
		~SkinSubMesh()
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
				foreach (SkinTriangle skinTriangle in _triangles)
				{
					skinTriangle.Dispose();
				}
				_triangles.Clear();
				_triangles = null;
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
	}

	#endregion

	#region Fields

	/// <summary>
	/// How long it takes for lose skin pieces to fade out.
	/// </summary>
	private const float fadeDuration = 5.0f;

    /// <summary>
    /// The minimum world space distance between two points before a cut is made.
    /// The minimum world space distance between two points before a cut is made.
    /// </summary>
    public const float minimumCutDistance = 0.04f;

	/// <summary>
	/// The bounds of the initial mesh.
	/// </summary>
	private Bounds _initialMeshBounds;

	/// <summary>
	/// The mass multiplier for all vertices.
	/// </summary>
	private float _massMultiplier;

	/// <summary>
	/// A multiplier for the gravity.
	/// </summary>
	private float _gravityMultiplier;

	/// <summary>
	/// A multiplier for the stiffness of skin edges.
	/// </summary>
	private float _skinEdgeStiffnessMultiplier;

	/// <summary>
	/// The length of a thread at rest.
	/// </summary>
	private float _threadRestLength;

	/// <summary>
	/// A multiplier for the stiffness of threads.
	/// </summary>
	private float _threadStiffnessMultiplier;

	/// <summary>
	/// A collection containing all skin vertices.
	/// Can contain null values as to preserve the index references in skin edges and skin triangles.
	/// </summary>
	private List<SkinVertex> _vertices = new List<SkinVertex>();

	/// <summary>
	/// A collection containing all skin submeshes.
	/// </summary>
	private List<SkinSubMesh> _skinSubMeshes = new List<SkinSubMesh>();

	/// <summary>
	/// A dictionary containing all skin edges.
	/// </summary>
	private Dictionary<VertexPair, SkinEdge> _edges = new Dictionary<VertexPair, SkinEdge>();

	/// <summary>
	/// A collection containing all threads.
	/// </summary>
	private List<Thread> _threads = new List<Thread>();

	/// <summary>
	/// The number of vertices in the initial mesh.
	/// </summary>
	private int _numberInitialVertices;

	/// <summary>
	/// Indicates whether this object has been disposed already.
	/// </summary>
	private bool _isDisposed;

	/// <summary>
	/// Whether loose skin pieces have to be faded.
	/// </summary>
	private bool _fadeLooseSkinPieces = true;

	#endregion

	#region Properties

	/// <summary>
	/// The bounds of the initial mesh.
	/// </summary>
	public Bounds initialMeshBounds
	{
		get { return _initialMeshBounds; }
	}

	/// <summary>
	/// The initial length of all original edges.
	/// </summary>
	public float initialEdgeLength { get; private set; }

	/// <summary>
	/// The mass multiplier for all vertices.
	/// </summary>
	public float massMultiplier
	{
		get { return _massMultiplier; }
		set { _massMultiplier = value; }
	}

	/// <summary>
	/// A multiplier for the gravity.
	/// </summary>
	public float gravityMultiplier
	{
		get { return _gravityMultiplier; }
		set { _gravityMultiplier = value; }
	}

	/// <summary>
	/// A multiplier for the stiffness of skin edges.
	/// </summary>
	public float skinEdgeStiffnessMultiplier
	{
		get { return _skinEdgeStiffnessMultiplier; }
		set { _skinEdgeStiffnessMultiplier = value; }
	}

	/// <summary>
	/// The length of a thread at rest.
	/// </summary>
	public float threadRestLength
	{
		get { return _threadRestLength; }
		set { _threadRestLength = value; }
	}

	/// <summary>
	/// A multiplier for the stiffness of threads.
	/// </summary>
	public float threadStiffnessMultiplier
	{
		get { return _threadStiffnessMultiplier; }
		set { _threadStiffnessMultiplier = value; }
	}
	
	/// <summary>
	/// Whether loose skin pieces have to be faded.
	/// </summary>
	public bool fadeLooseSkinPieces
	{
		get { return _fadeLooseSkinPieces; }
		set
		{
			_fadeLooseSkinPieces = value;
			for (int i = 1; i < _skinSubMeshes.Count; ++i)
			{
				_skinSubMeshes[i].fading = _fadeLooseSkinPieces;
			}
		}
	}

	#endregion

	#region Destructor

	/// <summary>
	/// Destructor.
	/// </summary>
	~SkinMesh()
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
			Reset();

			_vertices = null;
			_edges = null;
			_skinSubMeshes = null;
			_threads = null;
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

	#region Initialization

	/// <summary>
	/// Initializes the skin mesh.
	/// </summary>
	/// <param name="position">The position of the skin mesh.</param>
	/// <param name="scale">The scale of the skin mesh.</param>
	/// <param name="meshResolution">The mesh resolution (number of columns) of the skin mesh.</param>
	/// <returns>True if the skin mesh was successfully initialized.</returns>
	public bool Initialize(Vector3 position, Vector3 scale, int meshResolution)
	{
		CalculateInitialMeshBounds(position, scale);

		return Initialize(meshResolution);
	}

	/// <summary>
	/// Initializes the skin mesh.
	/// </summary>
	/// <param name="meshResolution">The mesh resolution (number of columns) of the skin mesh.</param>
	/// <returns>True if the skin mesh was successfully initialized.</returns>
	private bool Initialize(int meshResolution)
	{
		int numberColumns = Mathf.Max(meshResolution, 6);
		float triangleHeight = (_initialMeshBounds.max.x - _initialMeshBounds.min.x) / numberColumns;
		initialEdgeLength = Mathf.Sqrt(triangleHeight * triangleHeight * 4.0f / 3.0f);

		int numberRows = Mathf.FloorToInt(((_initialMeshBounds.max.z - _initialMeshBounds.min.z) / initialEdgeLength));
		int numberVerticesPerRow = numberRows + 1;
		float offset = (numberRows + 0.5f) * initialEdgeLength - (_initialMeshBounds.max.z - _initialMeshBounds.min.z);
		offset /= 2.0f;

        _initialMeshBounds.min -= Vector3.forward * offset;
        _initialMeshBounds.max += Vector3.forward * offset;

		CreateNewSkinSubMesh(false);

		for (int x = 0; x <= numberColumns; ++x)
		{
			for (int z = 0; z <= numberRows; ++z)
			{
				Vector3 position = new Vector3(
					_initialMeshBounds.min.x + x * triangleHeight,
					0.0f,
					(x % 2 == 0) ? (_initialMeshBounds.max.z - z * initialEdgeLength) : (_initialMeshBounds.max.z - (z + 0.5f) * initialEdgeLength));
				
				Vector2 uvs = new Vector2(
					(position.x - _initialMeshBounds.min.x) / _initialMeshBounds.size.x,
					(position.z - _initialMeshBounds.min.z) / _initialMeshBounds.size.z);

				int currentIndex = x * numberVerticesPerRow + z;
				SkinVertex skinVertex = new SkinVertex(this, currentIndex, position, uvs)
					{
						isKinematic = (x == 0 || z == 0 || x == numberColumns || z == numberRows)
					};
				_vertices.Add(skinVertex);

				if (x == 0 || z == 0) continue;

				if (x % 2 == 0)
				{
					CreateNewSkinTriangle(currentIndex, currentIndex - numberVerticesPerRow - 1, currentIndex - 1, 0);
					CreateNewSkinTriangle(currentIndex, currentIndex - numberVerticesPerRow, currentIndex - numberVerticesPerRow - 1, 0);
				}
				else
				{
					CreateNewSkinTriangle(currentIndex - 1, currentIndex - numberVerticesPerRow, currentIndex - numberVerticesPerRow - 1, 0);
					CreateNewSkinTriangle(currentIndex, currentIndex - numberVerticesPerRow, currentIndex - 1, 0);
				}
			}
		}

		foreach (SkinEdge skinEdge in _edges.Values)
		{
			skinEdge.originalEdge = true;
		}

		_numberInitialVertices = _vertices.Count;

		return true;
	}


	/// <summary>
	/// Calculates the bounds for the initial mesh based on the given position and scale.
	/// </summary>
	/// <param name="position">The position of the skin mesh.</param>
	/// <param name="scale">The scale of the skin mesh.</param>
	private void CalculateInitialMeshBounds(Vector3 position, Vector3 scale)
	{
        _initialMeshBounds.SetMinMax(
			new Vector3(
				position.x - (scale.x / 2.0f),
				0,
				position.z - (scale.y / 2.0f)
			),
			new Vector3(
				position.x + (scale.x / 2.0f),
				0,
				position.z + (scale.y / 2.0f)
			));
	}

	#endregion

	#region Methods

	/// <summary>
	/// Returns whether the vertex corresponding with the given index is an original vertex.
	/// </summary>
	/// <param name="vertexIndex">The vertex index.</param>
	/// <returns>Whether the vertex corresponding with the given index is an original vertex.</returns>
	public bool IsOriginalVertex(int vertexIndex)
	{
		return vertexIndex < _numberInitialVertices;
	}

	/// <summary>
	/// Returns the number of skin vertices.
	/// </summary>
	/// <returns>The number of skin vertices.</returns>
	public int GetNumberSkinVertices()
	{
		return _vertices.Count;
	}

	/// <summary>
	/// Returns a read only collection containing all skin vertices.
	/// </summary>
	/// <returns>A read only collection containing all skin vertices.</returns>
	public ReadOnlyCollection<SkinVertex> GetSkinVertices()
	{
		return new ReadOnlyCollection<SkinVertex>(_vertices);
	}

	/// <summary>
	/// Returns the skin vertex corresponding with the given index.
	/// </summary>
	/// <param name="index">The index we want the skin vertex for.</param>
	/// <returns>The skin vertex corresponding with the given index.</returns>
	public SkinVertex GetSkinVertex(int index)
	{
		return _vertices[index];
	}

	/// <summary>
	/// Returns the number of skin edges.
	/// </summary>
	/// <returns>The number of skin edges.</returns>
	public int GetNumberSkinEdges()
	{
		return _edges.Count;
	}

	/// <summary>
	/// Returns a read only collection containing all skin edges.
	/// </summary>
	/// <returns>A read only collection containing all skin edge.</returns>
	public ReadOnlyCollection<SkinEdge> GetSkinEdges()
	{
		return new ReadOnlyCollection<SkinEdge>(_edges.Values.ToList());
	}

	/// <summary>
	/// Returns the skin edge corresponding with the given vertex pair.
	/// </summary>
	/// <param name="vertexPair">The vertex pair we want the skin edge for.</param>
	/// <returns>The skin edge corresponding with the given vertex pair.</returns>
	public SkinEdge GetSkinEdge(VertexPair vertexPair)
	{
		SkinEdge skinEdge;
		return _edges.TryGetValue(vertexPair, out skinEdge) ? skinEdge : null;
	}

	/// <summary>
	/// Returns the number of threads.
	/// </summary>
	/// <returns>The number of threads.</returns>
	public int GetNumberThreads()
	{
		return _threads.Count;
	}

	/// <summary>
	/// Returns a read only collection containing all threads.
	/// </summary>
	/// <returns>A read only collection containing all threads.</returns>
	public ReadOnlyCollection<Thread> GetThreads()
	{
		return new ReadOnlyCollection<Thread>(_threads);
	}

	/// <summary>
	/// Returns all skin triangles of all skin submeshes.
	/// </summary>
	/// <returns>All skin triangles of all skin submeshes.</returns>
	private IEnumerable<SkinTriangle> GetSkinTriangles()
	{
		return _skinSubMeshes.SelectMany(skinSubMesh => skinSubMesh.triangles);
	}

	/// <summary>
	/// Creates a new skin sub mesh.
	/// </summary>
	/// <param name="fadeOut">Whether the skin sub mesh has to fade out.</param>
	/// <returns>The index of the new skin sub mesh.</returns>
	private int CreateNewSkinSubMesh(bool fadeOut)
	{
		_skinSubMeshes.Add(new SkinSubMesh(fadeOut));

		return _skinSubMeshes.Count - 1;
	}

	/// <summary>
	/// Removes the skin sub mesh corresponding with the given index.
	/// </summary>
	/// <param name="skinSubMeshIndex">The index of the skin sub mesh to remove.</param>
	private void RemoveSkinSubMesh(int skinSubMeshIndex)
	{
		if (skinSubMeshIndex < 0 || skinSubMeshIndex >= _skinSubMeshes.Count) return;

		RemoveSkinSubMesh(_skinSubMeshes[skinSubMeshIndex]);
	}

	/// <summary>
	/// Removes the given skin sub mesh.
	/// </summary>
	/// <param name="skinSubMesh">The skin sub mesh to remove.</param>
	private void RemoveSkinSubMesh(SkinSubMesh skinSubMesh)
	{
		while (skinSubMesh.triangles.Count > 0)
		{
			RemoveSkinTriangle(skinSubMesh.triangles[0]);
		}
		skinSubMesh.Dispose();

		_skinSubMeshes.Remove(skinSubMesh);

		RemoveUnusedSkinEdges();
		RemoveThreadsFromUnusedSkinVertices();
	}

	/// <summary>
	/// Creates a new skin triangle and updates references.
	/// </summary>
	/// <param name="vertex0Index">The index of the first vertex.</param>
	/// <param name="vertex1Index">The index of the second vertex.</param>
	/// <param name="vertex2Index">The index of the third vertex.</param>
	/// <param name="skinSubMeshIndex">The index for the skin sub mesh to add the new skin triangle to.</param>
	/// <returns>The new skin triangle; or null.</returns>
	private SkinTriangle CreateNewSkinTriangle(int vertex0Index, int vertex1Index, int vertex2Index, int skinSubMeshIndex)
	{
		if (_skinSubMeshes.Count == 0) return null;

		if (vertex0Index < 0 || vertex0Index >= _vertices.Count) return null;
		if (vertex1Index < 0 || vertex1Index >= _vertices.Count) return null;
		if (vertex2Index < 0 || vertex2Index >= _vertices.Count) return null;

		SkinTriangle skinTriangle = new SkinTriangle(this, vertex0Index, vertex1Index, vertex2Index);

		skinSubMeshIndex = Mathf.Min(Mathf.Max(skinSubMeshIndex, 0), _skinSubMeshes.Count - 1);
		_skinSubMeshes[skinSubMeshIndex].triangles.Add(skinTriangle);

		_vertices[vertex0Index].triangles.Add(skinTriangle);
		_vertices[vertex1Index].triangles.Add(skinTriangle);
		_vertices[vertex2Index].triangles.Add(skinTriangle);

		AddSkinTriangleToSkinEdge(new VertexPair(vertex0Index, vertex1Index), skinTriangle);
		AddSkinTriangleToSkinEdge(new VertexPair(vertex1Index, vertex2Index), skinTriangle);
		AddSkinTriangleToSkinEdge(new VertexPair(vertex2Index, vertex0Index), skinTriangle);

		return skinTriangle;
	}

	/// <summary>
	/// Removes the given skin triangle and returns the index of the skin sub mesh
	/// the given skin triangle was part of (-1 means it wasn't part of any skin sub mesh).
	/// </summary>
	/// <param name="skinTriangle">The skin triangle.</param>
	/// <returns>True the index of the skin sub mesh the given skin triangle
	/// was part of (-1 means it wasn't part of any skin sub mesh).</returns>
	private int RemoveSkinTriangle(SkinTriangle skinTriangle)
	{
		bool removed = false;
		int i = 0;
		while (i < _skinSubMeshes.Count)
		{
			removed = (_skinSubMeshes[i].triangles.Remove(skinTriangle));
			if (removed) break;

			i++;
		}
		if (removed == false) return -1;

		RemoveSkinTriangleFromSkinEdge(new VertexPair(skinTriangle.vertex0Index, skinTriangle.vertex1Index), skinTriangle);
		RemoveSkinTriangleFromSkinEdge(new VertexPair(skinTriangle.vertex1Index, skinTriangle.vertex2Index), skinTriangle);
		RemoveSkinTriangleFromSkinEdge(new VertexPair(skinTriangle.vertex2Index, skinTriangle.vertex0Index), skinTriangle);

		_vertices[skinTriangle.vertex0Index].triangles.Remove(skinTriangle);
		_vertices[skinTriangle.vertex1Index].triangles.Remove(skinTriangle);
		_vertices[skinTriangle.vertex2Index].triangles.Remove(skinTriangle);

		skinTriangle.Dispose();

		return i;
	}
	
	/// <summary>
	/// Adds the given skin triangle to the list of triangles of the skin edge corresponding with the given vertex pair.
	/// </summary>
	/// <param name="vertexPair">The vertex pair.</param>
	/// <param name="skinTriangle">The skin triangle.</param>
	private void AddSkinTriangleToSkinEdge(VertexPair vertexPair, SkinTriangle skinTriangle)
	{
		SkinEdge skinEdge;
		if (_edges.TryGetValue(vertexPair, out skinEdge) == false)
		{
			skinEdge = new SkinEdge(this, vertexPair.vertex0Index, vertexPair.vertex1Index);
			_edges.Add(vertexPair, skinEdge);
		}
		skinEdge.triangles.Add(skinTriangle);
		if (Application.isEditor && skinEdge.triangles.Count > 2)
		{
			Debug.LogError("Skin edge with more than 2 edges!" + Environment.NewLine + skinEdge);
		}
	}

	/// <summary>
	/// Removes the given skin triangle from the list of triangles of the skin edge corresponding with the given vertex pair.
	/// </summary>
	/// <param name="vertexPair">The vertex pair.</param>
	/// <param name="skinTriangle">The skin triangle.</param>
	private void RemoveSkinTriangleFromSkinEdge(VertexPair vertexPair, SkinTriangle skinTriangle)
	{
		SkinEdge skinEdge;
		if (_edges.TryGetValue(vertexPair, out skinEdge) == false) return;

		skinEdge.triangles.Remove(skinTriangle);
	}

	/// <summary>
	/// Removes unused skin edges (skin edges that have zero skin triangles).
	/// </summary>
	private void RemoveUnusedSkinEdges()
	{
		ReadOnlyCollection<SkinEdge> skinEdges = GetSkinEdges();
		foreach (SkinEdge skinEdge in skinEdges
			.Where(skinEdge => skinEdge.triangles.Count == 0))
		{
			_edges.Remove(new VertexPair(skinEdge.vertex0Index, skinEdge.vertex1Index));
			skinEdge.Dispose();
		}
	}

	/// <summary>
	/// Adds a thread between the vertices corresponding with the given indices.
	/// </summary>
	/// <param name="vertex0Index">The index of the first vertex.</param>
	/// <param name="vertex1Index">The index of the second vertex.</param>
	/// <param name="visibility">The thread visibility.</param>
	/// <returns>The new thread.</returns>
	public Thread AddThread(int vertex0Index, int vertex1Index)
	{
		if (vertex0Index == vertex1Index) return null;
		if (_vertices[vertex0Index].triangles.Count == 0 || _vertices[vertex1Index].triangles.Count == 0) return null;

		Thread thread = new Thread(this, vertex0Index, vertex1Index);
		_threads.Add(thread);

		_vertices[vertex0Index].threads.Add(thread);
		_vertices[vertex1Index].threads.Add(thread);

		return thread;
	}
    public Material lineMaterial;
	/// <summary>
	/// Removes the given thread.
	/// </summary>
	/// <param name="thread">The thread to remove.</param>
	private void RemoveThread(Thread thread)
	{
		if (_threads.Remove(thread) == false) return;

		_vertices[thread.vertex0Index].threads.Remove(thread);
		_vertices[thread.vertex1Index].threads.Remove(thread);

		thread.Dispose();
	}

	/// <summary>
	/// Removes the threads from unused skin vertices (skin vertices that have zero skin triangles).
	/// </summary>
	private void RemoveThreadsFromSkinVertex(SkinVertex skinVertex)
	{
		List<Thread> threads = new List<Thread>(skinVertex.threads);
		foreach (Thread thread in threads)
		{
			RemoveThread(thread);
		}
	}

	/// <summary>
	/// Removes the threads from unused skin vertices (skin vertices that have zero skin triangles).
	/// </summary>
	private void RemoveThreadsFromUnusedSkinVertices()
	{
		List<Thread> threads = new List<Thread>(_threads);
		foreach (Thread thread in threads
			.Where(thread => _vertices[thread.vertex0Index].triangles.Count == 0 || _vertices[thread.vertex1Index].triangles.Count == 0))
		{
			RemoveThread(thread);
		}
	}

	/// <summary>
	/// Returns the closest skin triangle to a given point p.
	/// </summary>
	/// <param name="p">The point we want the closest skin triangle for.</param>
	/// <returns>The closest skin triangle to a given point p.</returns>
	public SkinTriangle GetClosestSkinTriangleToPoint(Vector3 p)
	{
		Vector3 closestPoint;
		float distance, u, v;
		return GetClosestSkinTriangleToPoint(p, out closestPoint, out distance, out u, out v);
	}

	/// <summary>
	/// Returns the closest skin triangle to a given point p.
	/// </summary>
	/// <param name="p">The point we want the closest skin triangle for.</param>
	/// <param name="closestPoint">The closest point on the closest skin triangle.</param>
	/// <param name="distance">The distance to the closest point on the closest skin triangle.</param>
	/// <param name="u">The barycentric coordinate of the closest point (in the direction of v2 - v0 of the closest skin triangle).</param>
	/// <param name="v">The barycentric coordinate of the closest point (in the direction of v1 - v0 of the closest skin triangle).</param>
	/// <returns>The closest skin triangle to a given point p.</returns>
	public SkinTriangle GetClosestSkinTriangleToPoint(Vector3 p, out Vector3 closestPoint, out float distance, out float u, out float v)
	{
		closestPoint = Vector3.zero;
		distance = u = v = 0.0f;

		SkinTriangle closestSkinTriangle = null;
		float minSqrDistance = float.PositiveInfinity;

		foreach (SkinTriangle skinTriangle in GetSkinTriangles())
		{
			float currentU, currentV;
			Vector3 currentClosestPoint = MathHelper.GetClosestPointInsideTriangle(
				_vertices[skinTriangle.vertex0Index].currentPosition,
				_vertices[skinTriangle.vertex1Index].currentPosition,
				_vertices[skinTriangle.vertex2Index].currentPosition,
				p, out currentU, out currentV);

			float sqrDistance = (p - currentClosestPoint).sqrMagnitude;
			if (closestSkinTriangle == null || sqrDistance < minSqrDistance)
			{
				closestSkinTriangle = skinTriangle;
				minSqrDistance = sqrDistance;
				closestPoint = currentClosestPoint;
				u = currentU;
				v = currentV;
			}
		}

		if (float.IsPositiveInfinity(minSqrDistance) == false)
		{
			distance = Mathf.Sqrt(minSqrDistance);
		}

		return closestSkinTriangle;
	}

    /// <summary>
    /// Returns the closest skin triangle to a given point p.
    /// </summary>
    /// <param name="a">Start point of line segment</param>
    /// <param name="b">End point of line segment</param>
    /// <param name="closestPoint">The closest point on the closest skin triangle.</param>
    /// <param name="distance">The distance to the closest point on the closest skin triangle.</param>
    /// <param name="u">The barycentric coordinate of the closest point (in the direction of v2 - v0 of the closest skin triangle).</param>
    /// <param name="v">The barycentric coordinate of the closest point (in the direction of v1 - v0 of the closest skin triangle).</param>
    /// <returns>Returns the SkinTriangle that intersects with the linesegment pq and is closest to p.</returns>
    public SkinTriangle GetClosestSkinTriangleToLineSegment(Vector3 p, Vector3 q, out Vector3 closestPoint, out float distance, out float u, out float v)
    {
        closestPoint = Vector3.zero;
        distance = u = v = 0.0f;

        SkinTriangle closestSkinTriangle = null;
        float closestSquareDistance = float.MaxValue;

        foreach (SkinTriangle skinTriangle in GetSkinTriangles())
        {
            bool insideTriangle, insideLineSegment;
            float _u, _v, t;

            Vector3 intersection = MathHelper.GetTriangleLineIntersection(
                    _vertices[skinTriangle.vertex0Index].currentPosition,
                    _vertices[skinTriangle.vertex1Index].currentPosition,
                    _vertices[skinTriangle.vertex2Index].currentPosition,
                    p,  q,
                    out insideTriangle, out insideLineSegment, out _u, out _v, out t);

            if (insideTriangle && insideLineSegment)
            {
                float sqrDistance = (p - intersection).sqrMagnitude;
                if(sqrDistance < closestSquareDistance){
					closestPoint = intersection;
                    closestSkinTriangle = skinTriangle;
                    closestSquareDistance = sqrDistance;
                    u = _u;
                    v = _v;
                }
            }
        }

        if(closestSkinTriangle != null){
            distance = Mathf.Sqrt(closestSquareDistance);
        }

        return closestSkinTriangle;
    }

	/// <summary>
	/// Returns the skin vertices whose projections on the XZ plane are within the given distance
	/// to the given coordinates in the XZ plane and whose height is higher than the given y coordinate.
	/// Also returns the distance from the projected skin vertices to the given coordinates.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	/// <param name="maxDistance">The max distance.</param>
	/// <returns>The skin vertices whose projections on the XZ plane are within the given distance
	/// to the given coordinates in the XZ plane and whose height is higher than the given y coordinate.
	/// Also returns the distance from the projected skin vertices to the given coordinates.</returns>
	public List<KeyValuePair<SkinVertex, float>> GetSkinVerticesWithinProjectionDistance(float x, float y, float z, float maxDistance)
	{
		float sqrMaxDistance = maxDistance * maxDistance;

		List<KeyValuePair<SkinVertex, float>> result = new List<KeyValuePair<SkinVertex, float>>();

		foreach (SkinVertex skinVertex in GetSkinVertices()
			.Where(skinVertex => skinVertex.isKinematic == false &&
				skinVertex.currentPosition.y > y - 0.02f &&
				skinVertex.triangles.Count > 0))
		{
			float sqrDistance = new Vector2(skinVertex.currentPosition.x - x, skinVertex.currentPosition.z - z).sqrMagnitude;
			if (sqrDistance > sqrMaxDistance) continue;

			result.Add(new KeyValuePair<SkinVertex, float>(skinVertex, Mathf.Sqrt(sqrDistance)));
		}

		return result;
	}

	/// <summary>
	/// Returns the closest skin vertex to the given point.
	/// First we find the closest skin triangle (T) and the closest point (p') on that triangle to the given point p.
	/// Then we check if the closest skin vertex of T is within the given snap distance to P' and if so, return it.
	/// If not, we check if it's possible to create a new skin vertex on one of the edges of T that is within the given snap distance to P' and if so, return it.
	/// If that is also not possible, then we create a new skin vertex for p'.
	/// </summary>
	/// <param name="p">The point we want the closest skin vertex for.</param>
	/// <param name="snapDistance">The snap distance.</param>
	/// <returns>The closest skin vertex to the line segment pq if pq intersects with the skin mesh.</returns>
	public SkinVertex GetClosestSkinVertexToPoint(Vector3 p, float snapDistance)
	{
		Vector3 closestPoint;
		float distance, u, v;
		SkinTriangle closestSkinTriangle = GetClosestSkinTriangleToPoint(p, out closestPoint, out distance, out u, out v);
		if (closestSkinTriangle == null) return null;

		SkinVertex closestSkinVertex = GetSkinVertexOnClosestEdge(closestSkinTriangle, closestPoint, snapDistance);
		if (closestSkinVertex != null) return closestSkinVertex;

		return CreateSkinVertexInTriangle(closestSkinTriangle, u, v);
	}

    /// <summary>
    /// Returns the closest skin vertex to the given point.
    /// First we find the closest skin triangle (T) and the closest point (p') on that triangle to the given point p.
    /// Then we check if the closest skin vertex of T is within the given snap distance to P' and if so, return it.
    /// If not, we check if it's possible to create a new skin vertex on one of the edges of T that is within the given snap distance to P' and if so, return it.
    /// If that is also not possible, then we create a new skin vertex for p'.
    /// </summary>
    /// <param name="p">The point we want the closest skin vertex for.</param>
    /// <param name="snapDistance">The snap distance.</param>
    /// <returns>The closest skin vertex to the given point.</returns>
    public SkinVertex GetClosestSkinVertexToLineSgement(Vector3 p, Vector3 q, float snapDistance)
    {
        Vector3 closestPoint;
        float distance, u, v;
        SkinTriangle closestSkinTriangle = GetClosestSkinTriangleToLineSegment(p, q, out closestPoint, out distance, out u, out v);
        if (closestSkinTriangle == null) return null;

        SkinVertex closestSkinVertex = GetSkinVertexOnClosestEdge(closestSkinTriangle, closestPoint, snapDistance);
        if (closestSkinVertex != null) return closestSkinVertex;

        return CreateSkinVertexInTriangle(closestSkinTriangle, u, v);
    }

	/// <summary>
	/// Returns a skin vertex on the given skin triangle's closest skin edge to the given point.
	/// If one of the skin vertices on the closest edge is within the snap distance of given point
	/// projected on the closest skin edge, it will be returned. Otherwise a new skin vertex will
	/// be created in the projected point.
	/// Returns null if no skin edge is within the given snap distance from the given point.
	/// </summary>
	/// <param name="skinTriangle">The skin triangle.</param>
	/// <param name="p">The point.</param>
	/// <param name="snapDistance">The snap distance.</param>
	/// <returns>A skin vertex on the given skin triangle's closest skin edge to the given point.
	/// Returns null if no skin edge is within the given snap distance from the given point.</returns>
	private SkinVertex GetSkinVertexOnClosestEdge(SkinTriangle skinTriangle, Vector3 p, float snapDistance)
	{
		float sqrSnapDistance = snapDistance * snapDistance;

		SkinVertex v0 = _vertices[skinTriangle.vertex0Index];
		SkinVertex v1 = _vertices[skinTriangle.vertex1Index];
		SkinVertex v2 = _vertices[skinTriangle.vertex2Index];
		float t0, t1, t2;
		Vector3 p0 = MathHelper.GetProjectedPointOnLine(v1.currentPosition, v2.currentPosition, p, out t0);
		Vector3 p1 = MathHelper.GetProjectedPointOnLine(v2.currentPosition, v0.currentPosition, p, out t1);
		Vector3 p2 = MathHelper.GetProjectedPointOnLine(v0.currentPosition, v1.currentPosition, p, out t2);
		float sqrDistanceToP0 = (p - p0).sqrMagnitude;
		float sqrDistanceToP1 = (p - p1).sqrMagnitude;
		float sqrDistanceToP2 = (p - p2).sqrMagnitude;

		float minSqrDistance = Mathf.Min(sqrDistanceToP0, sqrDistanceToP1, sqrDistanceToP2);
		if (minSqrDistance <= sqrSnapDistance)
		{
			if (Mathf.Approximately((minSqrDistance - sqrDistanceToP0), 0))
			{
				float sqrDistanceP0V1 = (p0 - v1.currentPosition).sqrMagnitude;
				float sqrDistanceP0V2 = (p0 - v2.currentPosition).sqrMagnitude;
				if (sqrDistanceP0V1 <= sqrSnapDistance && sqrDistanceP0V1 < sqrDistanceP0V2) return v1;
				if (sqrDistanceP0V2 <= sqrSnapDistance && sqrDistanceP0V2 < sqrDistanceP0V1) return v2;
				
				return CreateSkinVertexOnEdge(v1, v2, t0);
			}
			if (Mathf.Approximately((minSqrDistance - sqrDistanceToP1), 0f))
			{
				float sqrDistanceP1V0 = (p1 - v0.currentPosition).sqrMagnitude;
				float sqrDistanceP1V2 = (p1 - v2.currentPosition).sqrMagnitude;
				if (sqrDistanceP1V0 <= sqrSnapDistance && sqrDistanceP1V0 < sqrDistanceP1V2) return v0;
				if (sqrDistanceP1V2 <= sqrSnapDistance && sqrDistanceP1V2 < sqrDistanceP1V0) return v2;

				return CreateSkinVertexOnEdge(v2, v0, t1);
			}
			if (Mathf.Approximately((minSqrDistance - sqrDistanceToP2), 0f))
			{
				float sqrDistanceP2V0 = (p2 - v0.currentPosition).sqrMagnitude;
				float sqrDistanceP2V1 = (p2 - v1.currentPosition).sqrMagnitude;
				if (sqrDistanceP2V0 <= sqrSnapDistance && sqrDistanceP2V0 < sqrDistanceP2V1) return v0;
				if (sqrDistanceP2V1 <= sqrSnapDistance && sqrDistanceP2V1 < sqrDistanceP2V0) return v1;

				return CreateSkinVertexOnEdge(v0, v1, t2);
			}
		}

		return null;
	}

	/// <summary>
	/// Creates a new skin vertex on the skin edge between the given skin vertices.
	/// </summary>
	/// <param name="v0">The first skin vertex.</param>
	/// <param name="v1">The second skin vertex.</param>
	/// <param name="t">The interpolation factor between the given vertices to the new skin vertex.</param>
	/// <returns>A new skin vertex on the skin edge between the given skin vertices.</returns>
	private SkinVertex CreateSkinVertexOnEdge(SkinVertex v0, SkinVertex v1, float t)
	{
		VertexPair vertexPair = new VertexPair(v0.vertexIndex, v1.vertexIndex);
		if (_edges.ContainsKey(vertexPair) == false) return null;

		bool originalEdge = _edges[vertexPair].originalEdge;
		List<SkinTriangle> skinTrianglesToReplace = new List<SkinTriangle>(_edges[vertexPair].triangles);

		int newSkinVertexIndex = _vertices.Count;
		SkinVertex skinVertex = new SkinVertex(
			this,
			newSkinVertexIndex,
			(1 - t) * v0.currentPosition + t * v1.currentPosition,
			(1 - t) * v0.originalPosition + t * v1.originalPosition,
			(1 - t) * v0.uvs + t * v1.uvs);
		_vertices.Add(skinVertex);
		
		foreach (SkinTriangle skinTriangleToReplace in skinTrianglesToReplace)
		{
			skinTriangleToReplace.ShuffleVertexIndices(vertexPair);

			int skinSubMeshIndex = RemoveSkinTriangle(skinTriangleToReplace);
			CreateNewSkinTriangle(newSkinVertexIndex, skinTriangleToReplace.vertex1Index, skinTriangleToReplace.vertex2Index, skinSubMeshIndex);
			CreateNewSkinTriangle(newSkinVertexIndex, skinTriangleToReplace.vertex2Index, skinTriangleToReplace.vertex0Index, skinSubMeshIndex);
		}
		RemoveUnusedSkinEdges();

		SkinEdge newSkinEdge;
		if (_edges.TryGetValue(new VertexPair(v0.vertexIndex, newSkinVertexIndex), out newSkinEdge))
		{
			newSkinEdge.originalEdge = originalEdge;
		}
		if (_edges.TryGetValue(new VertexPair(v1.vertexIndex, newSkinVertexIndex), out newSkinEdge))
		{
			newSkinEdge.originalEdge = originalEdge;
		}

		return skinVertex;
	}

	/// <summary>
	/// Creates a new skin vertex on the given skin triangle with the given barycentric coordinates.
	/// </summary>
	/// <param name="skinTriangle">The skin triangle.</param>
	/// <param name="u">The first barycentric coordinate of the new skin vertex in the given triangle.</param>
	/// <param name="v">The second barycentric coordinate of the new skin vertex in the given triangle.</param>
	/// <returns>A new skin vertex on the given skin triangle with the given barycentric coordinates.</returns>
	private SkinVertex CreateSkinVertexInTriangle(SkinTriangle skinTriangle, float u, float v)
	{
		SkinVertex v0 = _vertices[skinTriangle.vertex0Index];
		SkinVertex v1 = _vertices[skinTriangle.vertex1Index];
		SkinVertex v2 = _vertices[skinTriangle.vertex2Index];
		
		int newSkinVertexIndex = _vertices.Count;
		SkinVertex skinVertex = new SkinVertex(
			this,
			newSkinVertexIndex,
			(1 - u - v) * v0.currentPosition + u * v2.currentPosition + v * v1.currentPosition,
			(1 - u - v) * v0.originalPosition + u * v2.originalPosition + v * v1.originalPosition,
			(1 - u - v) * v0.uvs + u * v2.uvs + v * v1.uvs);
		_vertices.Add(skinVertex);

		int skinSubMeshIndex = RemoveSkinTriangle(skinTriangle);
		CreateNewSkinTriangle(newSkinVertexIndex, skinTriangle.vertex0Index, skinTriangle.vertex1Index, skinSubMeshIndex);
		CreateNewSkinTriangle(newSkinVertexIndex, skinTriangle.vertex1Index, skinTriangle.vertex2Index, skinSubMeshIndex);
		CreateNewSkinTriangle(newSkinVertexIndex, skinTriangle.vertex2Index, skinTriangle.vertex0Index, skinSubMeshIndex);
		RemoveUnusedSkinEdges();

		return skinVertex;
	}

	/// <summary>
	/// Cut the mesh in a straight line between the given skin vertices.
	/// </summary>
	/// <param name="cutStart">The skin vertex where to start the cut.</param>
	/// <param name="cutEnd">The skin vertex where to end the cut.</param>
	/// <param name="snapDistance">The snap distance.</param>
	/// <param name="cutDepth">How deep to cut.</param>
	public void Cut(SkinVertex cutStart, SkinVertex cutEnd, float snapDistance, float cutDepth = 0.1f)
	{
		if (cutStart == null || cutEnd == null) return;

		List<Thread> threadsToRemove;
		List<CutLineSegment> cutLineSegments = GetCutLineSegments(cutStart, cutEnd, cutDepth, out threadsToRemove).ToList();
		//CreateDebugCutLineSegments(cutLineSegments);

		foreach (Thread threadToRemove in threadsToRemove)
		{
			RemoveThread(threadToRemove);
		}

		CreateSkinVerticesForCutLineSegments(cutLineSegments, snapDistance);
		
		List<VertexPair> skinEdgesToSplit = GetSkinEdgesToSplit(cutLineSegments, snapDistance).ToList();
		skinEdgesToSplit.ForEach(vertexPair => GetSkinEdge(vertexPair).hasToBeSplit = true);

		//CreateDebugCutSkinEdges(skinEdgesToSplit);

		Split(skinEdgesToSplit, false);
	}

	/// <summary>
	/// Creates debug primitives for the cut line segments.
	/// </summary>
	/// <param name="cutLineSegments">The cut line segments (cubes).</param>
	private static void CreateDebugCutLineSegments(IEnumerable<CutLineSegment> cutLineSegments)
	{
		foreach (CutLineSegment cutLineSegment in cutLineSegments)
		{
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.localScale = new Vector3(0.005f, (cutLineSegment.startPoint - cutLineSegment.endPoint).magnitude, 0.005f);
			cube.transform.position = (cutLineSegment.startPoint + cutLineSegment.endPoint) / 2.0f;
			cube.transform.up = cutLineSegment.startPoint - cutLineSegment.endPoint;
			cube.name = "line segment (" + cutLineSegment.skinTriangle.vertex0Index + " " +
				cutLineSegment.skinTriangle.vertex1Index + " " + cutLineSegment.skinTriangle.vertex2Index + ")";
		}
	}

	/// <summary>
	/// Creates debug primitives for the skin edges that are selected to be split.
	/// </summary>
	/// <param name="skinEdgesToSplit">The selected skin edges to split (cylinders).</param>
	private void CreateDebugCutSkinEdges(IEnumerable<VertexPair> skinEdgesToSplit)
	{
		foreach (VertexPair skinEdgeToSplit in skinEdgesToSplit)
		{
			SkinVertex sv0 = GetSkinVertex(skinEdgeToSplit.vertex0Index);
			SkinVertex sv1 = GetSkinVertex(skinEdgeToSplit.vertex1Index);

			GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			cylinder.transform.localScale = new Vector3(0.005f, (sv0.currentPosition - sv1.currentPosition).magnitude / 2.0f, 0.005f);
			cylinder.transform.position = (sv0.currentPosition + sv1.currentPosition) / 2.0f;
			cylinder.transform.up = sv0.currentPosition - sv1.currentPosition;
			cylinder.name = " vertex pair (" + skinEdgeToSplit.vertex0Index + " " + skinEdgeToSplit.vertex1Index + ")";
		}
	}

	/// <summary>
	/// Returns a list of line segments for a cut between the two given skin vertices with the given cut depth.
	/// </summary>
	/// <param name="cutStart">The skin vertex where to start the cut.</param>
	/// <param name="cutEnd">The skin vertex where to end the cut.</param>
	/// <param name="cutDepth">How deep to cut.</param>
	/// <param name="threadsToRemove">The threads that get cut and have to be removed.</param>
	/// <returns>A list of line segments for a cut between the two given skin vertices with the given cut depth.</returns>
	private IEnumerable<CutLineSegment> GetCutLineSegments(SkinVertex cutStart, SkinVertex cutEnd, float cutDepth, out List<Thread> threadsToRemove)
	{
		Vector3 cutPlaneA = Camera.main.transform.position;
		Vector3 cutPlaneB = cutStart.currentPosition + (cutStart.currentPosition - cutPlaneA).normalized * cutDepth;
		Vector3 cutPlaneC = cutEnd.currentPosition + (cutEnd.currentPosition - cutPlaneA).normalized * cutDepth;

		List<CutLineSegment> cutLineSegments = new List<CutLineSegment>();
		foreach (SkinTriangle skinTriangle in GetSkinTriangles())
		{
			Vector3 v0 = GetSkinVertex(skinTriangle.vertex0Index).currentPosition;
			Vector3 v1 = GetSkinVertex(skinTriangle.vertex1Index).currentPosition;
			Vector3 v2 = GetSkinVertex(skinTriangle.vertex2Index).currentPosition;

			Vector3 l1, l2;
			if (MathHelper.GetTriangleTriangleIntersection(cutPlaneA, cutPlaneB, cutPlaneC, v0, v1, v2, out l1, out l2, 0.001f) == false) continue;

			if (Mathf.Approximately((l1 - l2).magnitude , 0f)) continue;

			cutLineSegments.Add(new CutLineSegment
				{
					skinTriangle = skinTriangle,
					startPoint = l1,
					endPoint = l2
				});
		}

		threadsToRemove = new List<Thread>();
		foreach (Thread thread in GetThreads())
		{
			Vector3 v0 = GetSkinVertex(thread.vertex0Index).currentPosition;
			Vector3 v1 = GetSkinVertex(thread.vertex1Index).currentPosition;

			bool insideTriangle;
			bool insideLineSegment;
			MathHelper.GetTriangleLineIntersection(cutPlaneA, cutPlaneB, cutPlaneC, v0, v1, out insideTriangle, out insideLineSegment, 0.001f);

			if (insideTriangle && insideLineSegment)
			{
				threadsToRemove.Add(thread);
			}
		}

		return cutLineSegments;
	}

	/// <summary>
	/// Creates the necessary skin vertices for the given cut line segments.
	/// </summary>
	/// <param name="cutLineSegments">The cut line segments.</param>
	/// <param name="snapDistance">The snap distance.</param>
	private void CreateSkinVerticesForCutLineSegments(IEnumerable<CutLineSegment> cutLineSegments, float snapDistance)
	{
		foreach (CutLineSegment cutLineSegment in cutLineSegments)
		{
			GetClosestSkinVertexToPoint(cutLineSegment.startPoint, snapDistance);
			GetClosestSkinVertexToPoint(cutLineSegment.endPoint, snapDistance);
		}
	}

	/// <summary>
	/// Returns the skin edges to split for the given cut line segments.
	/// </summary>
	/// <param name="cutLineSegments">The cut line segments.</param>
	/// <param name="snapDistance">The snap distance.</param>
	/// <returns>The skin edges corresponding with all the given cut line segments.</returns>
	private IEnumerable<VertexPair> GetSkinEdgesToSplit(IEnumerable<CutLineSegment> cutLineSegments, float snapDistance)
	{
		List<VertexPair> skinEdgesToSplit = new List<VertexPair>();

		foreach (CutLineSegment cutLineSegment in cutLineSegments)
		{
			SkinVertex sv0 = GetClosestSkinVertexToPoint(cutLineSegment.startPoint, snapDistance);
			SkinVertex sv1 = GetClosestSkinVertexToPoint(cutLineSegment.endPoint, snapDistance);
			if (sv0 == sv1) continue;

			VertexPair vertexPair = new VertexPair(sv0.vertexIndex, sv1.vertexIndex);
			SkinEdge skinEdge = GetSkinEdge(vertexPair);
			if (skinEdge != null)
			{
				skinEdgesToSplit.Add(vertexPair);
				continue;
			}

			SkinEdge intersectionSkinEdge = sv0.triangles.SelectMany(triangle => triangle.edges)
				.Intersect(sv1.triangles.SelectMany(triangle => triangle.edges))
				.FirstOrDefault();
			if (intersectionSkinEdge == null)
			{
				if (Application.isEditor)
				{
					Debug.LogWarning("Couldn't create a skin edge between vertices " + sv0.vertexIndex + " and " + sv1.vertexIndex + ".");
				}
				continue;
			}

			VertexPair vertexPair00 = new VertexPair(sv0.vertexIndex, intersectionSkinEdge.vertex0Index);
			SkinEdge skinEdge00 = GetSkinEdge(vertexPair00);
			VertexPair vertexPair10 = new VertexPair(sv1.vertexIndex, intersectionSkinEdge.vertex0Index);
			SkinEdge skinEdge10 = GetSkinEdge(vertexPair10);
			VertexPair vertexPair01 = new VertexPair(sv0.vertexIndex, intersectionSkinEdge.vertex1Index);
			SkinEdge skinEdge01 = GetSkinEdge(vertexPair01);
			VertexPair vertexPair11 = new VertexPair(sv1.vertexIndex, intersectionSkinEdge.vertex1Index);
			SkinEdge skinEdge11 = GetSkinEdge(vertexPair11);

			if (skinEdge00.currentLength + skinEdge10.currentLength < skinEdge01.currentLength + skinEdge11.currentLength)
			{
				skinEdgesToSplit.Add(vertexPair00);
				skinEdgesToSplit.Add(vertexPair10);
			}
			else
			{
				skinEdgesToSplit.Add(vertexPair01);
				skinEdgesToSplit.Add(vertexPair11);
			}
		}

		return skinEdgesToSplit.Distinct();
	}

	/// <summary>
	/// Splits all the edges that are currently marked as to be split.
	/// </summary>
	/// <param name="splitSmallAttachments">Whether to split off small attachments after splitting the given skin edges.</param>
	public void Split(bool splitSmallAttachments)
	{
		Split(_edges.Where(edge => edge.Value.hasToBeSplit).Select(kvp => kvp.Key), splitSmallAttachments);
	}

	/// <summary>
	/// Splits the given skin edges.
	/// </summary>
	/// <param name="skinEdgesToSplit">The skin edges to split.</param>
	/// <param name="splitSmallAttachments">Whether to split off small attachments after splitting the given skin edges.</param>
	private void Split(IEnumerable<VertexPair> skinEdgesToSplit, bool splitSmallAttachments)
	{
		try
		{
			List<SkinVertex> verticesToSplit = new List<SkinVertex>();

			foreach (VertexPair skinEdgeToSplit in skinEdgesToSplit)
			{
				verticesToSplit.Add(GetSkinVertex(skinEdgeToSplit.vertex0Index));
				verticesToSplit.Add(GetSkinVertex(skinEdgeToSplit.vertex1Index));
			}

			verticesToSplit = verticesToSplit.Distinct().ToList();

			foreach (SkinVertex skinVertexToSplit in verticesToSplit)
			{
				List<SkinEdge> edges = skinVertexToSplit.edges;

				int numberEdgesToBeSplit = edges.Count(skinEdge => skinEdge.hasToBeSplit);
				int numberCutEdges = edges.Count(skinEdge => skinEdge.isCutEdge);
				if (numberEdgesToBeSplit == 2 && numberCutEdges == 0)
				{
					SplitSkinVertexStartCut(skinVertexToSplit);
				}
				else if (numberEdgesToBeSplit == 1 && numberCutEdges == 2)
				{
					SplitSkinVertexExtendCut(skinVertexToSplit);
				}
				else if (numberEdgesToBeSplit == 0 && numberCutEdges == 4)
				{
					SplitSkinVertexConnectCuts(skinVertexToSplit);
				}
			}

			if (splitSmallAttachments)
			{
				SplitOneVertexAttachments();
				RemoveDisconnectedSkinTriangles();
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Error during splitting!" + Environment.NewLine + e);
		}
	}

	/// <summary>
	/// Split method for a skin vertex that has two edges that are marked as to be split.
	/// </summary>
	/// <param name="skinVertex">The skin vertex to split.</param>
	private void SplitSkinVertexStartCut(SkinVertex skinVertex)
	{
		RemoveThreadsFromSkinVertex(skinVertex);

		List<SkinTriangle> triangles = skinVertex.triangles;
		List<SkinEdge> sortedEdges = skinVertex.GetEdgesSorted();

		int indexFirstEdgeThatHasToBeSplit = sortedEdges.FindIndex(skinEdge => skinEdge.hasToBeSplit);
		int indexSecondEdgeThatHasToBeSplit = sortedEdges.FindIndex(indexFirstEdgeThatHasToBeSplit + 1, skinEdge => skinEdge.hasToBeSplit);

		SkinVertex newSkinVertex = DuplicateVertex(skinVertex);
		for (int i = indexFirstEdgeThatHasToBeSplit; i < indexSecondEdgeThatHasToBeSplit; ++i)
		{
			SkinTriangle skinTriangleToReplace = triangles.Find(skinTriangle => skinTriangle.vertex1Index == sortedEdges[i].vertex1Index);
			if (skinTriangleToReplace == null) continue;

			int skinSubMeshIndex = RemoveSkinTriangle(skinTriangleToReplace);
			CreateNewSkinTriangle(newSkinVertex.vertexIndex, skinTriangleToReplace.vertex1Index, skinTriangleToReplace.vertex2Index, skinSubMeshIndex);
		}

		sortedEdges.ForEach(skinEdge => skinEdge.hasToBeSplit = false);
		foreach (SkinEdge newSkinEdge in sortedEdges
			.Where(skinEdge => skinEdge.originalEdge)
			.Select(skinEdge => GetSkinEdge(new VertexPair(newSkinVertex.vertexIndex, skinEdge.vertex1Index)))
			.Where(newSkinEdge => newSkinEdge != null))
		{
			newSkinEdge.originalEdge = true;
		}

		RemoveUnusedSkinEdges();
	}

	/// <summary>
	/// Split method for a skin vertex that has one edge that is marked as to be split
	/// and two edges that are part of an existing cut.
	/// </summary>
	/// <param name="skinVertex">The skin vertex to split.</param>
	private void SplitSkinVertexExtendCut(SkinVertex skinVertex)
	{
		RemoveThreadsFromSkinVertex(skinVertex);

		List<SkinTriangle> triangles = skinVertex.triangles;
		List<SkinEdge> sortedEdges = skinVertex.GetEdgesSorted();

		int indexEdgeThatHasToBeSplit = sortedEdges.FindIndex(skinEdge => skinEdge.hasToBeSplit);
		int indexCutLeftEdge = sortedEdges.FindIndex(skinEdge => skinEdge.isCutEdge &&
			triangles.Any(skinTriangle => skinTriangle.vertex2Index == skinEdge.vertex1Index));
		int indexCutRightEdge = (indexCutLeftEdge + 1) % sortedEdges.Count;
		
		SkinVertex newSkinVertex = DuplicateVertex(skinVertex);
		int startIndex = indexEdgeThatHasToBeSplit < indexCutLeftEdge ? indexEdgeThatHasToBeSplit : indexCutLeftEdge;
		int endIndex = indexEdgeThatHasToBeSplit < indexCutLeftEdge ? indexCutRightEdge : indexEdgeThatHasToBeSplit;
		for (int i = startIndex; i < endIndex; ++i)
		{
			SkinTriangle skinTriangleToReplace = triangles.Find(skinTriangle => skinTriangle.vertex1Index == sortedEdges[i].vertex1Index);
			if (skinTriangleToReplace == null) continue;

			int skinSubMeshIndex = RemoveSkinTriangle(skinTriangleToReplace);
			CreateNewSkinTriangle(newSkinVertex.vertexIndex, skinTriangleToReplace.vertex1Index, skinTriangleToReplace.vertex2Index, skinSubMeshIndex);
		}

		sortedEdges.ForEach(skinEdge => skinEdge.hasToBeSplit = false);
		foreach (SkinEdge newSkinEdge in sortedEdges
			.Where(skinEdge => skinEdge.originalEdge)
			.Select(skinEdge => GetSkinEdge(new VertexPair(newSkinVertex.vertexIndex, skinEdge.vertex1Index)))
			.Where(newSkinEdge => newSkinEdge != null))
		{
			newSkinEdge.originalEdge = true;
		}

		RemoveUnusedSkinEdges();
	}

	/// <summary>
	/// Split method for a skin vertex that has four edges that are part of existing cuts.
	/// </summary>
	/// <param name="skinVertex">The skin vertex to split.</param>
	private void SplitSkinVertexConnectCuts(SkinVertex skinVertex)
	{
		RemoveThreadsFromSkinVertex(skinVertex);

		List<SkinTriangle> triangles = skinVertex.triangles;
		List<SkinEdge> sortedEdges = skinVertex.GetEdgesSorted();

		int indexFirstCutRightEdge = sortedEdges.FindIndex(skinEdge => skinEdge.isCutEdge &&
			triangles.Any(skinTriangle => skinTriangle.vertex2Index == skinEdge.vertex1Index)) + 1;
		int indexSecondCutLeftEdge = sortedEdges.FindIndex(indexFirstCutRightEdge + 1, skinEdge => skinEdge.isCutEdge &&
			triangles.Any(skinTriangle => skinTriangle.vertex2Index == skinEdge.vertex1Index));

		SkinVertex newSkinVertex = DuplicateVertex(skinVertex);
		for (int i = indexFirstCutRightEdge; i < indexSecondCutLeftEdge; ++i)
		{
			SkinTriangle skinTriangleToReplace = triangles.Find(skinTriangle => skinTriangle.vertex1Index == sortedEdges[i].vertex1Index);
			if (skinTriangleToReplace == null) continue;

			int skinSubMeshIndex = RemoveSkinTriangle(skinTriangleToReplace);
			CreateNewSkinTriangle(newSkinVertex.vertexIndex, skinTriangleToReplace.vertex1Index, skinTriangleToReplace.vertex2Index, skinSubMeshIndex);
		}

		foreach (SkinEdge newSkinEdge in sortedEdges
			.Where(skinEdge => skinEdge.originalEdge)
			.Select(skinEdge => GetSkinEdge(new VertexPair(newSkinVertex.vertexIndex, skinEdge.vertex1Index)))
			.Where(newSkinEdge => newSkinEdge != null))
		{
			newSkinEdge.originalEdge = true;
		}

		RemoveUnusedSkinEdges();
	}

	/// <summary>
	/// Split method for a skin vertex that has more than four edges that are part of existing cuts.
	/// This will split the vertex in as many vertices as there are triangles that use this vertex.
	/// </summary>
	/// <param name="skinVertex">The skin vertex to split.</param>
	private void SplitSkinVertexCompletely(SkinVertex skinVertex)
	{
		RemoveThreadsFromSkinVertex(skinVertex);

		List<SkinTriangle> triangles = skinVertex.triangles;
		triangles.ForEach(skinTriangle => skinTriangle.ShuffleVertexIndices(skinVertex.vertexIndex));
		skinVertex.edges.ForEach(skinEdge => skinEdge.hasToBeSplit = false);

		while (triangles.Count > 1)
		{
			int vertex1Index = triangles[0].vertex1Index;
			int vertex2Index = triangles[0].vertex2Index;

			int skinSubMeshIndex = RemoveSkinTriangle(triangles[0]);

			SkinVertex newSkinVertex = DuplicateVertex(skinVertex);
			CreateNewSkinTriangle(newSkinVertex.vertexIndex, vertex1Index, vertex2Index, skinSubMeshIndex);
		}

		RemoveUnusedSkinEdges();
	}

	/// <summary>
	/// Looks for any pieces of skin that are only attached to each other by one vertex and splits them.
	/// </summary>
	public void SplitOneVertexAttachments()
	{
		List<SkinVertex> verticesCopy = new List<SkinVertex>(_vertices);

		foreach (SkinVertex skinVertex in verticesCopy)
		{
			List<SkinEdge> edges = skinVertex.edges;

			int numberCutEdges = edges.Count(skinEdge => skinEdge.isCutEdge);
			if (numberCutEdges == 4)
			{
				SplitSkinVertexConnectCuts(skinVertex);
			}
			//else if (numberCutEdges > 4)
			//{
			//	SplitSkinVertexCompletely(skinVertex);
			//}
		}
	}

	/// <summary>
	/// Removes all disconnected skin triangles and splits off all pieces of skin that are only connected through one short skin edge.
	/// </summary>
	public void RemoveDisconnectedSkinTriangles()
	{
		List<SkinEdge> skinEdgesToSplit = new List<SkinEdge>();
		List<SkinTriangle> disconnectedSkinTriangles = GetDisconnectedSkinTriangles(skinEdgesToSplit);
		if (disconnectedSkinTriangles.Count == 0) return;

		int newSkinSubMeshIndex = CreateNewSkinSubMesh(fadeLooseSkinPieces);
		if (Application.isEditor)
		{
			//Debug.Log("Created new skin sub mesh with " + disconnectedSkinTriangles.Count + " skin triangles. (" + _skinSubMeshes.Count + " skin sub meshes total).");
		}

		foreach (SkinTriangle disconnectedSkinTriangle in disconnectedSkinTriangles)
		{
			int vertex0Index = disconnectedSkinTriangle.vertex0Index;
			int vertex1Index = disconnectedSkinTriangle.vertex1Index;
			int vertex2Index = disconnectedSkinTriangle.vertex2Index;
			RemoveSkinTriangle(disconnectedSkinTriangle);
			CreateNewSkinTriangle(vertex0Index, vertex1Index, vertex2Index, newSkinSubMeshIndex);
		}

		RemoveUnusedSkinEdges();

		skinEdgesToSplit.ForEach(skinEdgeToSplit => skinEdgeToSplit.hasToBeSplit = true);
		Split(false);
	}

	/// <summary>
	/// Returns the disconnected skin triangles.
	/// </summary>
	/// <param name="skinEdgesToSplit">The skin edges that have to be split.</param>
	/// <returns>The disconnected skin triangles.</returns>
	private List<SkinTriangle> GetDisconnectedSkinTriangles(ICollection<SkinEdge> skinEdgesToSplit)
	{
		if(_skinSubMeshes.Count == 0) return new List<SkinTriangle>();

		const float threshold = minimumCutDistance * 2.0f;

		List<SkinTriangle> reachableSkinTriangles = new List<SkinTriangle>();

		SkinSubMesh mainSubMesh = _skinSubMeshes.First();
		if (mainSubMesh.triangles.Count == 0) return reachableSkinTriangles;

		SkinTriangle startTriangle = mainSubMesh.triangles.First();
		AddNeighbouringSkinTriangles(startTriangle, threshold, reachableSkinTriangles, skinEdgesToSplit);

		return mainSubMesh.triangles.Except(reachableSkinTriangles).ToList();
	}

	/// <summary>
	/// Adds the neighbouring skin triangles of the given skin triangle to the collection of reachable skin triangles.
	/// If some neighbouring skin triangles are connected through a short skin edge, the neighbouring skin triangle is
	/// not included.
	/// </summary>
	/// <param name="skinTriangle">The skin triangle to process.</param>
	/// <param name="threshold">A threshold for how long a skin edge has to be before the adjoining skin is split off.</param>
	/// <param name="reachableSkinTriangles">A collection with all the currently reachable skin triangles.</param>
	/// <param name="skinEdgesToSplit">The skin edges that have to be split.</param>
	private void AddNeighbouringSkinTriangles(SkinTriangle skinTriangle, float threshold,
		ICollection<SkinTriangle> reachableSkinTriangles, ICollection<SkinEdge> skinEdgesToSplit)
	{
		if (skinTriangle == null || reachableSkinTriangles.Contains(skinTriangle)) return;

		reachableSkinTriangles.Add(skinTriangle);

		foreach (SkinEdge skinEdge in skinTriangle.edges.Where(skinEdge => !skinEdge.isCutEdge))
		{
			if (skinEdge.initialLength <= threshold)
			{
				SkinVertex sv0 = GetSkinVertex(skinEdge.vertex0Index);
				SkinVertex sv1 = GetSkinVertex(skinEdge.vertex1Index);
				int numberCutEdges0 = sv0.edges.Count(edge => edge.isCutEdge);
				int numberCutEdges1 = sv1.edges.Count(edge => edge.isCutEdge);
				if (numberCutEdges0 == 2 && numberCutEdges1 == 2)
				{
					skinEdgesToSplit.Add(skinEdge);
					continue;
				}
			}

			SkinTriangle neighbour = skinEdge.GetAdjacentSkinTriangle(skinTriangle);

			AddNeighbouringSkinTriangles(neighbour, threshold, reachableSkinTriangles, skinEdgesToSplit);
		}
	}

	/// <summary>
	/// Duplicates the given skin vertex.
	/// </summary>
	/// <param name="skinVertex">The skin vertex to duplicate.</param>
	/// <returns>The duplicated skin vertex.</returns>
	private SkinVertex DuplicateVertex(SkinVertex skinVertex)
	{
		SkinVertex newSkinVertex = new SkinVertex(this, _vertices.Count, skinVertex.currentPosition,
			skinVertex.originalPosition, skinVertex.uvs);
		_vertices.Add(newSkinVertex);

		return newSkinVertex;
	}

	/// <summary>
	/// Checks if there are skin edges whose spring force is too big, effectively breaking the skin edge.
	/// </summary>
	/// <param name="springForceBreakThreshold">The threshold for the spring force of a skin edge before it breaks.</param>
	private void CheckForBreakingSkinEdges(float springForceBreakThreshold)
	{
		List<SkinEdge> skinEdgesToSplit = new List<SkinEdge>();

		foreach (SkinEdge skinEdge in GetSkinEdges()
			.Where(skinEdge => skinEdge.cachedSpringForce >= springForceBreakThreshold))
		{
			SkinVertex sv0 = GetSkinVertex(skinEdge.vertex0Index);
			SkinVertex sv1 = GetSkinVertex(skinEdge.vertex1Index);
			SkinVertex newSkinVertex = CreateSkinVertexOnEdge(sv0, sv1, 0.5f);
			if (newSkinVertex == null) continue;

			foreach (SkinEdge newSkinEdge in newSkinVertex.edges)
			{
				newSkinEdge.ShuffleVertexIndices(newSkinVertex.vertexIndex);
				if (newSkinEdge.vertex1Index == sv0.vertexIndex || newSkinEdge.vertex1Index == sv1.vertexIndex) continue;

				skinEdgesToSplit.Add(newSkinEdge);
			}
		}

		skinEdgesToSplit.ForEach(skinEdgeToSplit => skinEdgeToSplit.hasToBeSplit = true);
		Split(false);
		skinEdgesToSplit.ForEach(skinEdgeToSplit => skinEdgeToSplit.hasToBeSplit = false);
	}

	/// <summary>
	/// Checks if there are threads whose spring force is too big, effectively breaking the thread.
	/// </summary>
	/// <param name="springForceBreakThreshold">The threshold for the spring force of a thread before it breaks.</param>
	private void CheckForBreakingThreads(float springForceBreakThreshold)
	{
		List<Thread> threadsToSplit = new List<Thread>();

		foreach (Thread thread in GetThreads()
			.Where(thread => thread.cachedSpringForce >= springForceBreakThreshold))
		{
			RemoveThread(thread);
		}

		threadsToSplit.ForEach(RemoveThread);
	}

	/// <summary>
	/// Resets the skin mesh.
	/// </summary>
	public void Reset()
	{
		foreach (SkinVertex skinVertex in _vertices.Where(vertex => vertex != null))
		{
			skinVertex.Dispose();
		}
		_vertices.Clear();

		foreach (SkinEdge skinEdge in _edges.Values)
		{
			skinEdge.Dispose();
		}
		_edges.Clear();

		foreach (SkinSubMesh skinSubMesh in _skinSubMeshes)
		{
			skinSubMesh.Dispose();
		}
		_skinSubMeshes.Clear();

		foreach (Thread thread in _threads)
		{
			thread.Dispose();
		}
		_threads.Clear();

		_numberInitialVertices = 0;
		_initialMeshBounds = new Bounds();
	}

	/// <summary>
	/// Returns a vertex, uv and triangle array that represent the current skin mesh.
	/// </summary>
	/// <param name="vertices">The vertex array.</param>
	/// <param name="uvs">The uv array.</param>
	/// <param name="normals">The normal array.</param>
	/// <param name="triangles">The triangle array per sub mesh.</param>
	/// <param name="alphas">The alphas per sub mesh.</param>
	public void GetMeshArrays(ref Vector3[] vertices, ref Vector2[] uvs, ref Vector3[] normals, out int[][] triangles, out float[] alphas)
	{
		if (vertices == null || vertices.Length < _vertices.Count)
		{
			vertices = new Vector3[_vertices.Count];
		}
		if (uvs == null || uvs.Length < _vertices.Count)
		{
			uvs = new Vector2[_vertices.Count];
		}
		if (normals == null || normals.Length < _vertices.Count)
		{
			normals = new Vector3[_vertices.Count];
		}
		for (int i = 0; i < _vertices.Count; ++i)
		{
			if (_vertices[i] == null) continue;

			vertices[i] = _vertices[i].currentPosition;
			uvs[i] = _vertices[i].uvs;
			normals[i] = _vertices[i].normal;
		}

		triangles = new int[_skinSubMeshes.Count][];
		alphas = new float[_skinSubMeshes.Count];
		for (int i = 0; i < _skinSubMeshes.Count; ++i)
		{
			SkinSubMesh skinSubMesh = _skinSubMeshes[i];
			triangles[i] = new int[skinSubMesh.triangles.Count * 3];
			for (int j = 0; j < skinSubMesh.triangles.Count; ++j)
			{
				triangles[i][3 * j + 0] = skinSubMesh.triangles[j].vertex0Index;
				triangles[i][3 * j + 1] = skinSubMesh.triangles[j].vertex1Index;
				triangles[i][3 * j + 2] = skinSubMesh.triangles[j].vertex2Index;
			}
			alphas[i] = skinSubMesh.alpha;
		}
	}

	/// <summary>
	/// Updates the skin mesh.
	/// </summary>
	/// <param name="time">The time since the last skin mesh update.</param>
	/// <param name="dampingFactor">The damping factor.</param>
	/// <param name="forcesMultiplier">A multiplier for the resulting forces on skin vertices.</param>
	/// <param name="breakWhenSpringForcesTooBig">Does the skin have to break when spring forces in skin edges get too big.</param>
	/// <param name="springForceBreakThreshold">The threshold for the spring force of a skin edge before it breaks.</param>
	public void Update(float time, float dampingFactor, float forcesMultiplier)
	{
		//Move every skin vertex according to its resulting force.
		foreach (SkinVertex skinVertex in GetSkinVertices())
		{
			float maximumDisplacement = _initialMeshBounds.size.z / 10.0f;
			skinVertex.ApplyAcceleration(time, dampingFactor, forcesMultiplier, maximumDisplacement);
		}

		//Recalculate the area of every skin triangle.
		foreach (SkinTriangle skinTriangle in GetSkinTriangles())
		{
			skinTriangle.UpdateCachedArea();
		}

		//Recalculate the spring force of every skin edge.
		foreach (SkinEdge skinEdge in GetSkinEdges())
		{
			skinEdge.UpdateCachedSpringForce();
		}

		//Recalculate the spring force of every thread.
		foreach (Thread thread in GetThreads())
		{
			thread.UpdateModel();
			thread.UpdateCachedSpringForce();
		}

		//Recalculates the resulting force of every skin vertex.
		foreach (SkinVertex skinVertex in GetSkinVertices())
		{
			skinVertex.CalculateResultingForce();
		}

		//Adjust the alpha of every skin sub mesh.
		foreach (SkinSubMesh skinSubMesh in _skinSubMeshes.Where(skinSubMesh => skinSubMesh.fading))
		{
			skinSubMesh.alpha = Mathf.Max(skinSubMesh.alpha - Time.deltaTime / fadeDuration, 0.0f);
		}

		//Remove the empty skin sub meshes and the ones that are fully fade out.
		List<SkinSubMesh> emptySkinSubMeshes = _skinSubMeshes.Where(skinSubMesh => skinSubMesh.triangles.Count == 0 || Mathf.Approximately(skinSubMesh.alpha, 0f)).ToList();
		foreach (SkinSubMesh emptySkinSubMesh in emptySkinSubMeshes)
		{
			RemoveSkinSubMesh(emptySkinSubMesh);
		}
	}

	/// <summary>
	/// Returns a System.String that represents the current System.Object.
	/// </summary>
	/// <returns>A System.String that represents the current System.Object.</returns>
	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("VERTICES");
		foreach (SkinVertex skinVertex in _vertices)
		{
			sb.AppendLine(skinVertex.ToString());
		}

		sb.AppendLine("TRIANGLES");
		foreach (SkinTriangle skinTriangle in GetSkinTriangles())
		{
			sb.AppendLine(skinTriangle.ToString());
		}

		sb.AppendLine("EDGES");
		foreach (SkinEdge skinEdge in _edges.Values)
		{
			sb.AppendLine(skinEdge.ToString());
		}

		return sb.ToString();
	}

    public void OnPostRender(Camera cam, Material mat) {
        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        foreach(VertexPair vp in _edges.Keys){
            GL.Vertex(cam.WorldToViewportPoint(_vertices[vp.vertex0Index].currentPosition+Vector3.up*0.1f));
            GL.Vertex(cam.WorldToViewportPoint(_vertices[vp.vertex1Index].currentPosition+Vector3.up*0.1f));
        }
        GL.End();
        GL.PopMatrix();
    }

	#endregion
}