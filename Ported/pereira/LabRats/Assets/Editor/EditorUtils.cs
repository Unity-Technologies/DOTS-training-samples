using UnityEngine;

public class EditorUtil : MonoBehaviour
{
    public static void DestroyObject(GameObject gameObject)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            GameObject.DestroyImmediate(gameObject);
        else
#endif
            GameObject.Destroy(gameObject);
    }

    public static void DestroyChildren(Transform transform)
    {
        if (transform == null)
            return;

        for (int i = transform.childCount - 1; i >= 0; --i)
            DestroyObject(transform.GetChild(i).gameObject);

        transform.DetachChildren();
    }

    public static Mesh CreatePrimitiveMesh(PrimitiveType primitiveType)
    {
        var primitive = GameObject.CreatePrimitive(primitiveType);
        var mesh = primitive.GetComponent<MeshFilter>().sharedMesh;
        EditorUtil.DestroyObject(primitive);
        return mesh;
    }
}