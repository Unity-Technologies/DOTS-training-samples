using UnityEngine;

using System.Collections.Generic;
using System.Linq;

using System.Collections.ObjectModel;

/// <summary>
/// Script that represents the skin. It handles the vertex and triangle arrays etc.
/// </summary>
public class Skin : MonoBehaviour
{
#region Editor Fields

    [Header("Peformance")]
    /// <summary>
    /// The mesh resolution (number of columns) of the original skin mesh.
    /// </summary>
    public int meshResolution = 12;

    [Header("Behavior ")]

    /// <summary>
    /// The skin material (when lighting is enabled).
    /// </summary>
    public Material skinMaterial;

    /// <summary>
    /// The mass multiplier for all vertices.
    /// </summary>
    public float massMultiplier = 2.0f;

    /// <summary>
    /// A multiplier for the gravity.
    /// </summary>
    public float gravityMultiplier = 0.05f;

    /// <summary>
    /// A multiplier for the stiffness of skin edges.
    /// </summary>
    public float skinEdgeStiffnessMultiplier = 0.5f;

    /// <summary>
    /// The length of a thread at rest.
    /// </summary>
    public float threadRestLength = 0.03f;

    /// <summary>
    /// A multiplier for the stiffness of threads.
    /// </summary>
    public float threadStiffnessMultiplier = 4.0f;

    /// <summary>
    /// The damping factor.
    /// </summary>
    public float dampingFactor = 0.9f;

    /// <summary>
    /// A multiplier for the resulting forces on skin vertices.
    /// </summary>
    public float forcesMultiplier = 2.0f;

    /// <summary>
    /// The parent transform for the thread models.
    /// </summary>
    public Transform threadModelsParentTransform;

    /// <summary>
    /// The thread model prefab.
    /// </summary>
    public GameObject threadModelPrefab;

    /// <summary>
    /// The distance that is used by the tools to decide if a new vertice should be made.
    /// </summary>
    public float snapDistance = 0.02f;

#endregion

#region Fields

    /// <summary>
    /// The mesh filter component.
    /// </summary>
    private MeshFilter _meshFilter;

    /// <summary>
    /// The mesh renderer component.
    /// </summary>
    private MeshRenderer _meshRenderer;

    private int currentMeshResolution = 12;

#endregion

#region Properties

    /// <summary>
    /// The skin mesh.
    /// </summary>
    public SkinMesh skinMesh { get; private set; }

#endregion

#region Initialization

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    public void Awake()
    {
        //Don't remove the below lines or strange behaviour will occur. The code assumes the skin always has these transform values.
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        _meshFilter = GetComponent<MeshFilter>();
        if (_meshFilter == null)
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer == null)
        {
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        if (skinMaterial == null)
        {
            Debug.LogWarning("No skin material set!");
        }

        if (threadModelsParentTransform == null)
        {
            Debug.LogWarning("No thread models parent transform set!");
        }
        else
        {
            Thread.threadModelsParentTransform = threadModelsParentTransform;
        }

        if (threadModelPrefab == null)
        {
            Debug.LogWarning("No thread model prefab set!");
        }
        else
        {
            Thread.threadModelPrefab = threadModelPrefab;
        }

        _meshRenderer.material = skinMaterial;
        skinMesh = new SkinMesh();
        InitializeSkinMesh();
    }

    /// <summary>
    /// Initializes the skin mesh.
    /// </summary>
    private void InitializeSkinMesh()
    {
        currentMeshResolution = meshResolution;
        float ratio = (float)Screen.width / (float)Screen.height;
        skinMesh.Initialize(Vector3.zero, new Vector3(ratio,1f,1f), meshResolution);
    }

    /// <summary>
    /// Resets the skin.
    /// </summary>
    public void Reset()
    {
        skinMesh.Reset();
        InitializeSkinMesh();
    }

#endregion


#region Methods

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        skinMesh.massMultiplier = massMultiplier;
        skinMesh.gravityMultiplier = gravityMultiplier;
        skinMesh.skinEdgeStiffnessMultiplier = skinEdgeStiffnessMultiplier;
        skinMesh.threadRestLength = threadRestLength;
        skinMesh.threadStiffnessMultiplier = threadStiffnessMultiplier;

        //meshResolution = Mathf.Min(Mathf.Max(5,meshResolution),14);
        if(meshResolution != currentMeshResolution){
            Reset();
        }

    }


    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    public void FixedUpdate()
    {
        skinMesh.Update(Time.fixedDeltaTime, dampingFactor, forcesMultiplier);
        UpdateMesh();
    }

    /// <summary>
    /// Updates the mesh in the mesh filter component with the latest vertices, uvs and triangles arrays.
    /// </summary>
    private void UpdateMesh()
    {
        Vector3[] vertices = _meshFilter.mesh.vertices;
        Vector2[] uvs = _meshFilter.mesh.uv;
        Vector3[] normals = _meshFilter.mesh.vertices;
        int[][] triangles;
        float[] alphas;

        skinMesh.GetMeshArrays(ref vertices, ref uvs, ref normals, out triangles, out alphas);

        int subMeshCount = triangles.Length;

        _meshFilter.mesh.subMeshCount = subMeshCount;
        _meshFilter.mesh.vertices = vertices;
        _meshFilter.mesh.uv = uvs;
        _meshFilter.mesh.normals = normals;

        for (int i = 0; i < subMeshCount; ++i)
        {
            _meshFilter.mesh.SetTriangles(triangles[i], i);
        }

        List<Material> materials = _meshRenderer.materials.ToList();

        while (materials.Count > subMeshCount)
        {
            Material lastMaterial = materials.Last();
            materials.Remove(lastMaterial);
            Destroy(lastMaterial);
        }
        while (materials.Count < subMeshCount)
        {
            materials.Add(new Material(skinMaterial));
        }
        for (int i = 0; i < materials.Count; ++i)
        {
            Color color = materials[i].color;
            color.a = alphas[i];
            materials[i].color = color;
        }

        _meshRenderer.materials = materials.ToArray();
    }

#endregion
}