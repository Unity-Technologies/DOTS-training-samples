using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class that represents a skin vertex.
/// </summary>
public class SkinVertex
{
	#region Fields

	/// <summary>
	/// The skin mesh this vertex is part of.
	/// </summary>
	private SkinMesh _skinMesh;

	/// <summary>
	/// A collection of all the skin triangles this vertex is a part of.
	/// </summary>
	private List<SkinTriangle> _triangles = new List<SkinTriangle>();

	/// <summary>
	/// A collection of all the threads this vertex is a part of.
	/// </summary>
	private List<Thread> _threads = new List<Thread>();

	/// <summary>
	/// The resulting force on this vertex.
	/// </summary>
	private Vector3 _resultingForce = Vector3.zero;

	/// <summary>
	/// The acceleration on this vertex.
	/// </summary>
	private Vector3 _acceleration = Vector3.zero;

	/// <summary>
	/// Indicates whether this object has been disposed already.
	/// </summary>
	private bool _isDisposed;

	#endregion

	#region Properties

	/// <summary>
	/// The index of this vertex.
	/// </summary>
	public int vertexIndex { get; private set; }

	/// <summary>
	/// The current position.
	/// </summary>
	public Vector3 currentPosition { get; set; }

	/// <summary>
	/// The original position.
	/// </summary>
	public Vector3 originalPosition { get; private set; }

	/// <summary>
	/// The uv coordinates.
	/// </summary>
	public Vector2 uvs { get; private set; }

	/// <summary>
	/// Whether this vertex is kinematic (so it won't move during the skin update).
	/// </summary>
	public bool isKinematic { get; set; }

	/// <summary>
	/// Whether this vertex cannot be removed.
	/// </summary>
	public bool cannotBeRemoved { get; set; }

	/// <summary>
	/// A collection of all the skin triangles this vertex is a part of.
	/// </summary>
	public List<SkinTriangle> triangles
	{
		get { return _triangles; }
	}

	/// <summary>
	/// A collection of all the threads this vertex is a part of.
	/// </summary>
	public List<Thread> threads
	{
		get { return _threads; }
	}

	/// <summary>
	/// A collection of all the skin edges this vertex is a part of.
	/// </summary>
	public List<SkinEdge> edges
	{
		get
		{
			return triangles
				.SelectMany(triangle => triangle.edges)
				.Where(edge => edge.vertex0Index == vertexIndex || edge.vertex1Index == vertexIndex)
				.Distinct()
				.ToList();
		}
	}

	/// <summary>
	/// The added forces on this vertex.
	/// </summary>
	public Vector3 addedForces { get; private set; }

	/// <summary>
	/// The resulting force on this vertex.
	/// </summary>
	public Vector3 resultingForce
	{
		get { return _resultingForce; }
	}

	/// <summary>
	/// The normal.
	/// </summary>
	public Vector3 normal
	{
		get
		{
			return triangles.Aggregate(Vector3.zero, (normal, triangle) => normal + triangle.normal, normal => normal / triangles.Count).normalized;
		}
	}

	/// <summary>
	/// The mass.
	/// </summary>
	public float mass
	{
		get
		{
			return triangles
				.Aggregate(0.0f, (mass, triangle) => mass + triangle.cachedArea, mass => mass * _skinMesh.massMultiplier / 3.0f);
		}
	}

	#endregion
	
	#region Constructors

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="skinMesh">The skin mesh this vertex is part of.</param>
	/// <param name="vertexIndex">The index of this vertex.</param>
	/// <param name="position">The position (both the current and original).</param>
	/// <param name="uvs">The uv coordinates.</param>
	public SkinVertex(SkinMesh skinMesh, int vertexIndex, Vector3 position, Vector2 uvs)
	{
		_skinMesh = skinMesh;
		this.vertexIndex = vertexIndex;
		currentPosition = position;
		originalPosition = position;
		this.uvs = uvs;
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="skinMesh">The skin mesh this vertex is part of.</param>
	/// <param name="vertexIndex">The index of this vertex.</param>
	/// <param name="originalPosition">The current position.</param>
	/// <param name="currentPosition">The original position.</param>
	/// <param name="uvs">The uv coordinates.</param>
	public SkinVertex(SkinMesh skinMesh, int vertexIndex, Vector3 currentPosition, Vector3 originalPosition, Vector2 uvs)
	{
		_skinMesh = skinMesh;
		this.vertexIndex = vertexIndex;
		this.currentPosition = currentPosition;
		this.originalPosition = originalPosition;
		this.uvs = uvs;
	}

	#endregion

	#region Destructor

	/// <summary>
	/// Destructor.
	/// </summary>
	~SkinVertex()
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

			_threads.Clear();
			_threads = null;

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
	/// Adds the given force to this skin vertex.
	/// </summary>
	/// <param name="force">The force to add.</param>
	public void AddForce(Vector3 force)
	{
		addedForces += force;
		_resultingForce += force;
	}

	/// <summary>
	/// Calculates the resulting force on this skin vertex based on
	/// the gravity and the spring forces of all edges starting in this skin vertex.
	/// This value will be cached and used in the associated 'ApplyAcceleration()' method.
	/// </summary>
	public void CalculateResultingForce()
	{
		_resultingForce = Mathf.Atan(currentPosition.y) * Physics.gravity * 0.05f * _skinMesh.gravityMultiplier + addedForces;

		Vector3 skinEdgesResultingForce = Vector3.zero;
		foreach (SkinEdge skinEdge in edges)
		{
			skinEdge.ShuffleVertexIndices(vertexIndex);
			SkinVertex otherSkinVertex = _skinMesh.GetSkinVertex(skinEdge.vertex1Index);
			skinEdgesResultingForce += skinEdge.cachedSpringForce * (otherSkinVertex.currentPosition - currentPosition).normalized;
		}

		if (threads.Count == 0)
		{
			_resultingForce += skinEdgesResultingForce;
		}
		else
		{
			Vector3 threadsResultingForce = Vector3.zero;
			foreach (Thread thread in threads)
			{
				thread.ShuffleVertexIndices(vertexIndex);
				SkinVertex otherSkinVertex = _skinMesh.GetSkinVertex(thread.vertex1Index);
				threadsResultingForce += thread.cachedSpringForce * (otherSkinVertex.currentPosition - currentPosition).normalized;
			}

			_resultingForce += skinEdgesResultingForce + threadsResultingForce;
		}
	}

	/// <summary>
	/// Resets the resulting force on this skin vertex.
	/// </summary>
	public void ResetResultingForce()
	{
		addedForces = Vector3.zero;
		_resultingForce = Vector3.zero;
	}

	/// <summary>
	/// Applies the cached acceleration that was calculated in the associated 'CalculateResultingForce()' method.
	/// Only call this function when the resulting forces have been calculated for all skin vertices,
	/// otherwise some vertices will have an erroneous acceleration as some of their neighbouring vertices
	/// moved before the correct acceleration could be calculated.
	/// </summary>
	/// <param name="time">The time passed since the last time the acceleration was applied.</param>
	/// <param name="dampingFactor">The damping factor.</param>
	/// <param name="forcesMultiplier">A multiplier for the resulting forces on skin vertices.</param>
	/// <param name="maximumDisplacement">The maximum displacement.</param>
	public void ApplyAcceleration(float time, float dampingFactor, float forcesMultiplier, float maximumDisplacement)
	{
		if (isKinematic) return;

		float mass = this.mass;
		if (Mathf.Approximately(mass, 0f)) return;

		_acceleration = forcesMultiplier * (_resultingForce / mass) + (1.0f - Mathf.Clamp01(dampingFactor)) * _acceleration;

		Vector3 velocity = _acceleration * time;
		Vector3 displacement = velocity * time;
		if (displacement.sqrMagnitude >= maximumDisplacement * maximumDisplacement)
		{
			displacement = displacement.normalized * maximumDisplacement;
		}

		currentPosition += displacement;
		addedForces = Vector3.zero;
	}

	/// <summary>
	/// Returns all the skin edges this vertex is a part of, sorted clockwise, starting from the given angle.
	/// Every returned edge has it's vertices shuffles in a way that this skin vertex corresponds with the first vertex index of the edge.
	/// </summary>
	/// <param name="startAngle">The start angle (with 0.0f being Vector.right)</param>
	/// <returns>All the skin edges this vertex is a part of, sorted clockwise, starting from the given angle.</returns>
	public List<SkinEdge> GetEdgesSorted(float startAngle = 0.0f)
	{
		startAngle = startAngle - Mathf.FloorToInt(startAngle / 360.0f) * 360.0f;
		triangles.ForEach(skinTriangle => skinTriangle.ShuffleVertexIndices(vertexIndex));

		List<SkinEdge> edges = this.edges;
		List<KeyValuePair<float, SkinEdge>> angles = edges.ConvertAll(skinEdge =>
			{
				skinEdge.ShuffleVertexIndices(vertexIndex);
				Vector3 v = _skinMesh.GetSkinVertex(skinEdge.vertex1Index).originalPosition -
					_skinMesh.GetSkinVertex(skinEdge.vertex0Index).originalPosition;
				float angle = Vector3.Angle(Vector3.right, v);
				if (v.z > 0) angle = 360.0f - angle;
				angle -= startAngle;
				if (angle < 0.0f) angle += 360.0f;

				return new KeyValuePair<float, SkinEdge>(angle, skinEdge);
			});

		angles.Sort((kvp0, kvp1) =>
			{
				if (Mathf.Approximately(kvp0.Key,kvp1.Key))
				{
					if (kvp0.Value == kvp1.Value) return 0;

					return kvp0.Value.isCutEdge && kvp1.Value.isCutEdge
						? (triangles.Any(
							skinTriangle =>
							skinTriangle.vertex2Index == kvp0.Value.vertex1Index)
								? -1
								: 1)
						: (triangles.Any(
							skinTriangle =>
							skinTriangle.vertex1Index == kvp0.Value.vertex1Index && skinTriangle.vertex2Index == kvp1.Value.vertex1Index)
								? -1
								: 1);
				}

				if (kvp0.Key < kvp1.Key) return -1;
				if (kvp0.Key > kvp1.Key) return 1;

				return 0;
			});

		return angles.Select(kvp => kvp.Value).ToList();
	}

	/// <summary>
	/// Returns a System.String that represents the current System.Object.
	/// </summary>
	/// <returns>A System.String that represents the current System.Object.</returns>
	public override string ToString()
	{
		return string.Format("vertex {0} (position ({1}, {2}, {3}), uvs ({4}, {5}))",
				vertexIndex,
			currentPosition.x.ToString("F2"),
			currentPosition.y.ToString("F2"),
			currentPosition.z.ToString("F2"),
			uvs.x.ToString("F2"),
			uvs.y.ToString("F2"));
	}

	#endregion
}