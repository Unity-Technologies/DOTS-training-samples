using UnityEngine;

using System;

using Object = UnityEngine.Object;

/// <summary>
/// Class that represents a thread.
/// </summary>
public class Thread
{

	#region Fields

	/// <summary>
	/// Whether to use the rope simulator for thread physics
	/// </summary>
	public static bool ThreadPhysics = true;

	/// <summary>
	/// The stiffness for a thread.
	/// </summary>
	public const float threadStiffness = 0.002f;

	/// <summary>
	/// The skin mesh this thread is part of.
	/// </summary>
	private SkinMesh _skinMesh;

	/// <summary>
	/// The visuals for the thread.
	/// </summary>
	private GameObject _model;

	/// <summary>
	/// Indicates whether this object has been disposed already.
	/// </summary>
	private bool _isDisposed;

	#endregion

	#region Properties

	/// <summary>
	/// The parent transform for the thread models.
	/// </summary>
	public static Transform threadModelsParentTransform { get; set; }

	/// <summary>
	/// The thread model prefab.
	/// </summary>
	public static GameObject threadModelPrefab { get; set; }

	/// <summary>
	/// The index of the first vertex.
	/// </summary>
	public int vertex0Index { get; private set; }

	/// <summary>
	/// The index of the second vertex.
	/// </summary>
	public int vertex1Index { get; private set; }

	/// <summary>
	/// The visuals for the thread.
	/// </summary>
	private GameObject Model
	{
		get { return _model; }
		set
		{
			if (_model != null)
			{
				Object.Destroy(_model);
			}
			_model = value;
		}
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
	/// The cached spring force (this may be wrong if the vertices that make up this thread moved).
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

	#endregion

	#region Constructors

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="skinMesh">The skin mesh this thread is part of.</param>
	/// <param name="vertex0Index">The index of the first vertex.</param>
	/// <param name="vertex1Index">The index of the second vertex.</param>
	/// <param name="visibility">The visibility.</param>
	public Thread(SkinMesh skinMesh, int vertex0Index, int vertex1Index)
	{
		_skinMesh = skinMesh;
		this.vertex0Index = vertex0Index;
		this.vertex1Index = vertex1Index;

		UpdateModel();
	}

	#endregion

	#region Destructor

	/// <summary>
	/// Destructor.
	/// </summary>
	~Thread()
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
			Model = null;
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
	/// Updates the model for this thread.
	/// </summary>
	public void UpdateModel()
	{
		if (Model == null && threadModelPrefab != null)
		{
			Model = Object.Instantiate(threadModelPrefab) as GameObject;
            Debug.Log("Thread model made1");
			if (Model == null) return;

			Model.transform.parent = threadModelsParentTransform;
		}

		if (Model == null) return;
		
		Vector3 sv0Position = _skinMesh.GetSkinVertex(vertex0Index).currentPosition;
		Vector3 sv1Position = _skinMesh.GetSkinVertex(vertex1Index).currentPosition;
		Model.transform.position = (sv0Position + sv1Position) / 2.0f;

		Vector3 localScale = Model.transform.localScale;
		localScale.y = (sv0Position - sv1Position).magnitude / 2.0f;
		Model.transform.localScale = localScale;

		Model.transform.up = Model.transform.position - sv0Position;
	}

	/// <summary>
	/// Recalculates the spring force of this thread and caches it.
	/// </summary>
	/// <returns>The spring force of this thread.</returns>
	public float UpdateCachedSpringForce()
	{
		cachedSpringForce = threadStiffness * _skinMesh.threadStiffnessMultiplier * 100.0f * (currentLength - _skinMesh.threadRestLength);
		
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
	/// Returns a System.String that represents the current System.Object.
	/// </summary>
	/// <returns>A System.String that represents the current System.Object.</returns>
	public override string ToString()
	{
		return string.Format("thread {0} {1}",
				vertex0Index,
				vertex1Index);
	}

	#endregion
}