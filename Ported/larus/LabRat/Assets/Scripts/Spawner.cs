using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSExamples {

public class Spawner : MonoBehaviour {
	public float Max = Mathf.Infinity;

	public float Frequency = 0.2f;

	public GameObject Prefab;
    public GameObject AlternatePrefab;

	public Transform targetParent;
	public Board board;

	float Counter = 0f;

	void OnEnable() {
		Counter = Frequency;
	}

    void OnDisable() {
        if (coro != null) {
            StopCoroutine(coro);
            coro = null;
        }

    }

    Coroutine coro;

    void Start() {
        if (AlternatePrefab)
            coro = StartCoroutine(Alternates());
    }

    bool InAlternate = false;

    IEnumerator Alternates() {
        while (enabled) {
            yield return new WaitForSeconds(Random.Range(4.0f, 15f));
            InAlternate = true;
            yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
            Spawn(AlternatePrefab);
            yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
            InAlternate = false;
        }
    }

	void Spawn(GameObject prefab) {
		var parent = targetParent;
		if (!parent)
			parent = transform;

		var obj = Instantiate<GameObject>(prefab, transform.position, Quaternion.identity, parent);
		obj.GetComponent<ISpawnable>().OnSpawned(this);
	}

	int TotalSpawned = 0;

	void Update () {
		if (TotalSpawned >= Max || InAlternate)
			return;

		Counter += Time.deltaTime;
		while (Counter > Frequency) {
			Counter -= Frequency;
			Spawn(Prefab);
			TotalSpawned++;
		}
	}
}

}
