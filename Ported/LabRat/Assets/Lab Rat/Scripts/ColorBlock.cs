using UnityEngine;

public class ColorBlock : MonoBehaviour {
	static MaterialPropertyBlock block;
	public Color color = Color.white;
	Renderer myRenderer;

	int _ColorID;

	void OnEnable() {
		block = new MaterialPropertyBlock();
		_ColorID = Shader.PropertyToID("_Color");
		myRenderer = GetComponent<Renderer>();
	}
	
	void Update () {
		block.SetColor(_ColorID, color);
		myRenderer.SetPropertyBlock(block);
	}
}
