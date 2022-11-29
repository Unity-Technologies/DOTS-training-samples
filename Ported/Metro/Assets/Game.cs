using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Game : MonoBehaviour
{
    private Entity person;
    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;

    void Awake()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
