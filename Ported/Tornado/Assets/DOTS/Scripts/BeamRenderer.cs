using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamRenderer : MonoBehaviour
{
    static Matrix4x4[][] matrices;
    static MaterialPropertyBlock[] matProps;

    public Mesh beamMesh;
    public Material beamMaterial;

    public static void Setup(int nbBeam, int nbBatch, int instancesPerBatch)
    {
        if (matrices != null)
            return;

        matrices = new Matrix4x4[nbBatch][];
        for (int i = 0; i < matrices.Length; i++)
            matrices[i] = new Matrix4x4[instancesPerBatch];

        matProps = new MaterialPropertyBlock[nbBeam];
        var colors = new Vector4[instancesPerBatch];
        for (int i = 0; i < nbBeam; i++)
        {
            colors[i % instancesPerBatch] = new Vector4(0.4f, 0.4f, 0.4f, 1f);
            if ((i + 1) % instancesPerBatch == 0 || i == nbBeam - 1)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetVectorArray("_Color", colors);
                matProps[i / instancesPerBatch] = block;
            }
        }
    }

    void Start()
    {
        Debug.Log("Mesh Renderer start");
    }

    // Update is called once per frame
    void Update()
    {

        // Debug.Log("Mesh Renderer Update!");

        if (matrices == null)
            return;

        for (int i = 0; i < matrices.Length; i++)
            Graphics.DrawMeshInstanced(beamMesh, 0, beamMaterial, matrices[i], matrices[i].Length, matProps[i]);
    }
}
