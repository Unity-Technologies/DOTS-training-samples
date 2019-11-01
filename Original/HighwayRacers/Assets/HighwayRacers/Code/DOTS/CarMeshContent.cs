using UnityEngine;
using System.Collections;

public class CarMeshContent : MonoBehaviour
{
    public static CarMeshContent instance { get; private set; }

    public Mesh carMesh;
    public Material carMaterial;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }

    }
}
