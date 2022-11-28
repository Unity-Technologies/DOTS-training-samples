using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSExamples {

[RequireComponent(typeof(Renderer))]
[ExecuteInEditMode]
public class InstanceProps : MonoBehaviour {
	public Interval RandomScale = new Interval(0.9f, 1.27f);
	public float myScale = 1f;
	float _lastScale = Mathf.Infinity;
	public enum Mode { Random, Value }
	public Mode mode;

	Renderer _renderer;
	MaterialPropertyBlock block;

	public Color color = Color.white;
	Color _lastColor;
	public bool SetColor;

	public static int _ColorID;
	public static int _UniformScaleID;

	void OnEnable() {
		if (_UniformScaleID == 0) {
			_UniformScaleID = Shader.PropertyToID("_UniformScale");
			_ColorID = Shader.PropertyToID("_Color");
		}

		if (block == null)
			block = new MaterialPropertyBlock();
		_renderer = GetComponent<Renderer>();
		if (mode == Mode.Random)
			myScale = RandomScale.RandomValue();
	}

	void Update () {
		if (!_renderer)
			return;

		bool change = false;

		if (myScale != _lastScale) {
			block.SetFloat(_UniformScaleID, myScale);
			_lastScale = myScale;
			change = true;
		}

		if (SetColor && _lastColor != color) {
			block.SetColor(_ColorID, color);
			_lastColor = color;
			change = true;
		}

		if (change)
			_renderer.SetPropertyBlock(block);
	}
}

}