using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinCanManager : MonoBehaviour {
	public Mesh canMesh;
	public Material canMaterial;
	public Vector3 canSize;
	public float gravityStrength;

	List<TinCan> sittingCans;
	List<TinCan> fallingCans;

	Matrix4x4[] matrices;

	const int canCount = 100;

	static TinCanManager instance;

	public static TinCan GetNearestCan(Vector3 pos, bool skipReservedCans = true, float xRange = float.MaxValue) {
		float minDist = float.MaxValue;
		TinCan output = null;
		for (int i=0;i<instance.sittingCans.Count;i++) {
			float xDelta = Mathf.Abs(pos.x - instance.sittingCans[i].position.x);
			if (xDelta < xRange) {
				if (instance.sittingCans[i].reserved == false || skipReservedCans == false) {
					Vector3 delta = instance.sittingCans[i].position - pos;
					float sqrDist = delta.sqrMagnitude;
					if (sqrDist < minDist) {
						minDist = sqrDist;
						output = instance.sittingCans[i];
					}
				}
			}
		}
		return output;
	}

	public static void HitCan(TinCan can, Vector3 velocity) {
		instance.sittingCans.Remove(can);
		instance.fallingCans.Add(can);
		can.velocity = velocity;
		can.angularVelocity = Random.onUnitSphere * velocity.magnitude * 40f;
	}

	void ResetCan(TinCan can) {
		can.position = new Vector3(Random.Range(0f,ArmManager.armRowWidth+10f),Random.Range(3f,8f),15f);
		can.rotation = Quaternion.identity;
		can.scale = 0f;
		matrices[can.index] = Matrix4x4.TRS(can.position,can.rotation,canSize);
	}

	public static void UnreserveCanAfterDelay(TinCan can, float delay) {
		instance.StartCoroutine(instance.UnreserveCoroutine(can,delay));
	}

	IEnumerator UnreserveCoroutine(TinCan can, float delay) {
		yield return new WaitForSeconds(delay);
		can.reserved = false;
	}

	void Start () {
		instance = this;

		sittingCans = new List<TinCan>(canCount);
		fallingCans = new List<TinCan>(canCount);
		matrices = new Matrix4x4[canCount];

		for (int i=0;i<canCount;i++) {
			TinCan can = new TinCan();
			can.index = i;
			ResetCan(can);
			sittingCans.Add(can);
		}
	}
	
	void Update () {
		for (int i=0;i<fallingCans.Count;i++) {
			TinCan can = fallingCans[i];
			can.position += can.velocity * Time.deltaTime;
			can.velocity += new Vector3(0f,-gravityStrength*Time.deltaTime,0f);
			can.rotation = Quaternion.AngleAxis(can.angularVelocity.magnitude * Time.deltaTime,can.angularVelocity) * can.rotation;

			matrices[can.index] = Matrix4x4.TRS(can.position,can.rotation,canSize);

			if (can.position.y<-5f) {
				fallingCans.RemoveAt(i);
				i--;
				sittingCans.Add(can);
				ResetCan(can);
			}
		}
		for (int i = 0; i < sittingCans.Count; i++) {
			TinCan can = sittingCans[i];

			// cheesy and fps-dependent
			can.scale += (1f - can.scale) * 3f * Time.deltaTime;

			can.velocity = -Vector3.right * 3f;
			can.position += can.velocity * Time.deltaTime;
			if (can.position.x < 0f && can.reserved == false) {
				can.position.x = ArmManager.armRowWidth+10f;
				can.scale = 0f;
			}
			matrices[can.index].m03 = can.position.x;
			matrices[can.index].m13 = can.position.y;
			matrices[can.index].m23 = can.position.z;
			matrices[can.index].m00 = canSize.x * can.scale;
			matrices[can.index].m11 = canSize.y * can.scale;
			matrices[can.index].m22 = canSize.z * can.scale;
		}

		Graphics.DrawMeshInstanced(canMesh,0,canMaterial,matrices);
	}
}
