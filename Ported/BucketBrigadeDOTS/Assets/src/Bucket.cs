using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : MonoBehaviour {

    public int bucketID;
    public float volume;
    public bool bucketActive = false;
    public bool bucketFull = false;
    private Transform t;
    private Material mat;
    private FireSim fireSim;

    public void Init(int _ID, float _x, float _y){
        t = transform;
        mat = GetComponent<Renderer>().material;
        fireSim = FireSim.INSTANCE;
        volume = 0;
        t.position = new Vector3(_x, t.localScale.y * 0.5f, _y);
        bucketFull = false;
        bucketActive = false;

        UpdateBucket();
    }

    public void UpdateBucket(){
        float _fillFactor = volume / fireSim.bucketCapacity;

        // Colour
        Color _C = Color.Lerp(fireSim.colour_bucket_empty, fireSim.colour_bucket_full, _fillFactor);
        mat.color = _C;

        // scale
        float _SCALE = Mathf.Lerp(fireSim.bucketSize_EMPTY, fireSim.bucketSize_FULL, _fillFactor);
        t.localScale = Vector3.one * _SCALE;
    }
}
