using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockManager : MonoBehaviour {
	public int rockCount;
	public float conveyorMargin;
	public float minRockSize;
	public float maxRockSize;
	public float conveyorSpeed;
	public float gravityStrength;
	public Mesh rockMesh;
	public Material rockMaterial;

	float minConveyorX;
	float maxConveyorX;

	Rock[] allRocks;
	List<Rock> conveyorRocks;

	Matrix4x4[] matrices;

	public static RockManager instance;

	public static Rock NearestConveyorRock(Vector3 pos) {
		Rock output = null;
		float minDist = float.MaxValue;
		for (int i=0;i<instance.conveyorRocks.Count;i++) {
			if (instance.conveyorRocks[i].reserved == false) {
				float sqrDist = (instance.conveyorRocks[i].position - pos).sqrMagnitude;
				if (sqrDist < minDist) {
					minDist = sqrDist;
					output = instance.conveyorRocks[i];
				}
			}
		}
		return output;
	}

	public static void RemoveFromConveyor(Rock rock) {
		instance.conveyorRocks.Remove(rock);
	}

	void Start () {
		instance = this;

		allRocks = new Rock[rockCount];
		conveyorRocks = new List<Rock>(rockCount);
		matrices = new Matrix4x4[rockCount];

		minConveyorX = -conveyorMargin;
		maxConveyorX = ArmManager.armRowWidth+conveyorMargin;
		Vector3 basePos = new Vector3(minConveyorX,0f,1.5f);

		float spacing = (ArmManager.armRowWidth+conveyorMargin*2f) / rockCount;

		for (int i = 0; i < rockCount; i++) {
			Rock rock = new Rock();
			rock.position = basePos + Vector3.right*spacing*i;
			rock.state = Rock.State.Conveyor;
			rock.targetSize = Random.Range(minRockSize,maxRockSize);
			rock.size = 0f;
			matrices[i] = Matrix4x4.TRS(rock.position,Quaternion.identity,Vector3.one*rock.size);
			allRocks[i] = rock;
			conveyorRocks.Add(rock);
		}
	}
	
	void Update () {
		for (int i=0;i<rockCount;i++) {
			Rock rock = allRocks[i];

			// cheesy and fps-dependent
			rock.size += (rock.targetSize - rock.size) * 3f * Time.deltaTime;

			if (rock.state==Rock.State.Conveyor) {
				rock.position.x += conveyorSpeed * Time.deltaTime;
				if (rock.position.x>maxConveyorX) {
					rock.position.x -= ArmManager.armRowWidth+conveyorMargin*2f;
					rock.size = 0f;
				}
			} else if (rock.state==Rock.State.Thrown) {
				rock.position += rock.velocity * Time.deltaTime;
				rock.velocity += Vector3.up * -gravityStrength * Time.deltaTime;

				TinCan nearestCan = TinCanManager.GetNearestCan(rock.position,false);
				if (nearestCan != null) {
					if ((nearestCan.position - rock.position).sqrMagnitude < .5f * .5f) {
						TinCanManager.HitCan(nearestCan,rock.velocity);
						rock.velocity = Random.insideUnitSphere * 3f;
					}
				}

				if (rock.position.y<-5f) {
					rock.state = Rock.State.Conveyor;
					conveyorRocks.Add(rock);
					rock.position = new Vector3(Random.Range(minConveyorX,maxConveyorX),0f,1.5f);
					rock.size = 0f;
				}
			}

			Matrix4x4 matrix = matrices[i];
			matrix.m03 = rock.position.x;
			matrix.m13 = rock.position.y;
			matrix.m23 = rock.position.z;

			matrix.m00 = rock.size;
			matrix.m11 = rock.size;
			matrix.m22 = rock.size;

			matrices[i] = matrix;
		}

		Graphics.DrawMeshInstanced(rockMesh,0,rockMaterial,matrices);
	}
}
