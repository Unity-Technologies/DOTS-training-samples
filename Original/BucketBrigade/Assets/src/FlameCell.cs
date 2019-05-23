using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameCell : MonoBehaviour {

    public float temperature = 0f;
    public bool onFire = false;
    public bool neighbourOnFire = false;
    [Range(0.001f, 10f)]
    public float flickerRate = 0.1f;
    [Range(0f, 1f)]
    public float flickerRange = 0.1f;
    private float flameHeight;
    public int index;
    private Transform t;
    private Material mat;
    private FireSim fireSim;

    public void Init(Transform _parent,int _index, int _rowIndex, int _columnIndex, float _size, float _maxHeight, Color _col){
        t = transform;
        t.parent = _parent;
        index = _index;
        flameHeight = _maxHeight;
        mat = GetComponent<Renderer>().material;
        fireSim = FireSim.INSTANCE;

        // scaling
        t.localScale = new Vector3(_size, _maxHeight, _size);

        // position
        t.position = new Vector3(_size * _rowIndex, -(_maxHeight * 0.5f) + Random.Range(0.01f, 0.02f), _columnIndex * _size);
        mat.color = _col;
    }
    public void Extinguish(float _coolingStrength){
            
        temperature -= _coolingStrength;
        if (onFire)
        {
            if (temperature < FireSim.INSTANCE.flashpoint)
            {
                onFire = false;
                FireSim.INSTANCE.cellsOnFire--;
                Vector3 _POS = t.localPosition;
                _POS.y = -(flameHeight * 0.5f) + Random.Range(0.01f, 0.02f);
                t.localPosition = _POS;
                SetColour(FireSim.INSTANCE.colour_fireCell_neutral);
            }
        }
    }
    public void SetColour(Color _c){
        mat.color = _c;
    }

    // Ignite flame cell on user click
	public void OnMouseDown()
	{
        Scorch();
	}
    public void Scorch(){
        temperature = Random.Range(fireSim.flashpoint, 1f);
        IgnitionTest();
    }
    public void IgnitionTest(){
        if(temperature > fireSim.flashpoint){
            if (!onFire)
            {
                FireSim.INSTANCE.cellsOnFire++;
                onFire = true;
            }   
        }

    }

	void Update()
	{
        if(onFire){
            Vector3 _POS = t.localPosition;
            _POS.y = (-flameHeight*0.5f + (temperature * flameHeight)) - flickerRange;
            //_POS.y += (flickerRange*0.5f) + Mathf.PerlinNoise((Time.time -index)* flickerRate - temperature,index) * flickerRange;
            _POS.y += (flickerRange * 0.5f) + Mathf.PerlinNoise((Time.time - index) * flickerRate - temperature, temperature) * flickerRange;
            t.localPosition = _POS;
            SetColour(Color.Lerp(fireSim.colour_fireCell_cool, fireSim.colour_fireCell_hot, temperature));
        }
	}
}
