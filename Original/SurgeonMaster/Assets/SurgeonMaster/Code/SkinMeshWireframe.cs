using UnityEngine;

public class SkinMeshWireframe : MonoBehaviour {
    private SkinMesh skinMesh;
    private Camera cam;
    public Material wireframeMaterial;

    void Start(){
        skinMesh = FindObjectOfType<Skin>().skinMesh;
        cam = GetComponent<Camera>();
    }

    void OnPostRender(){
        skinMesh.OnPostRender(cam,wireframeMaterial);
    }
}