using UnityEngine;

public class AdhocResourceManager : MonoBehaviour
{
	public GameObject ResourcePrefab;
	public static AdhocResourceManager instance;

	void Awake()
    {
		if (instance == null)
			instance = this;
		else
			Destroy(this);
    }
}
