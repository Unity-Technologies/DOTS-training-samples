using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

    [Range(3f, 10000f)]
    public float volume = 5f;
    private float capacity;
    private Vector3 fullScale;
    private Transform t;

	private void Start()
	{
        Init();
	}
    public void Init(){
        t = transform;
        capacity = volume;
        fullScale = t.localScale;
    }
    public void Subtract(float _amount){
        volume -= _amount;
    }

	public void Update()
	{
        if(volume<=capacity){
            volume += FireSim.INSTANCE.refillRate;
            t.localScale = Vector3.Lerp(Vector3.zero, fullScale, volume / capacity);
        }
	}
}
