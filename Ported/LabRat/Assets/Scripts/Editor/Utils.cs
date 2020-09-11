using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

static class Utils
{
    [MenuItem("Tools/MergeMeshHierarchy")]
    static void MergeMeshHierarchySingleMesh()
    {
        MergeMeshHierarchy(true);
    }

    [MenuItem("Tools/MergeMeshHierarchyWithSubmeshes")]
    static void MergeMeshHierarchyWithSubmeshes()
    {
        MergeMeshHierarchy(false);
    }

    static void MergeMeshHierarchy(bool mergeSubMeshes)
    {
        foreach(var gameObject in Selection.GetFiltered<GameObject>(SelectionMode.TopLevel))
        {
            var combineInstances = new List<CombineInstance>();
            foreach (var meshFilter in gameObject.GetComponentsInChildren<MeshFilter>())
            {
                var isRoot = meshFilter.gameObject == gameObject;
                var srcMesh = meshFilter.sharedMesh;
                for (var i = 0; i < srcMesh.subMeshCount; ++i)
                    combineInstances.Add(new CombineInstance { mesh = srcMesh, subMeshIndex = i, transform = isRoot ? Matrix4x4.identity : meshFilter.transform.localToWorldMatrix});
            }
            
            var mesh = new Mesh();
            mesh.name = gameObject.name;
            mesh.CombineMeshes(combineInstances.ToArray(), mergeSubMeshes, true, false);
            mesh.Optimize();

            var path = AssetDatabase.GenerateUniqueAssetPath($"Assets/{gameObject.name}.asset");
            AssetDatabase.CreateAsset(mesh, path);
        }
    }
}
