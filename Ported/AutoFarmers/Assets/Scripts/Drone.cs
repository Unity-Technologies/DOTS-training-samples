using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone {
	public Vector3 position;
	public Matrix4x4 matrix;

	public Vector3 smoothPosition;
	public Plant targetPlant;
	public Plant heldPlant;
	float hoverHeight;
	public int storeX;
	public int storeY;

	const float ySpeed = 2f;
	const float xzSpeed = 6f;

	float searchTimer;

	Matrix4x4 GetMatrix() {
		Vector3 tilt = new Vector3(position.x-smoothPosition.x,2f,position.z-smoothPosition.z);
		return Matrix4x4.TRS(smoothPosition,Quaternion.FromToRotation(Vector3.up,tilt),new Vector3(.35f,.08f,.35f));
	}

	public Drone(Vector3 pos) {
		position = pos;
		smoothPosition = pos;
		matrix = GetMatrix();
		hoverHeight = Random.Range(2f,3f);
		searchTimer = Random.value;
	}

	void GetPlantAI() {
		float targetX = position.x;
		float targetY = hoverHeight;
		float targetZ = position.z;

		int tileX = Mathf.FloorToInt(smoothPosition.x);
		int tileY = Mathf.FloorToInt(smoothPosition.z);
		if (targetPlant==null) {
			if (searchTimer < 0f) {
				int plantTileHash = Pathing.SearchForOne(tileX,tileY,30,Pathing.IsNavigableAll,Farm.IsHarvestableAndUnreserved,Pathing.fullMapZone);
				if (plantTileHash != -1) {
					int x, y;
					Pathing.Unhash(plantTileHash,out x,out y);
					targetPlant = Farm.tilePlants[x,y];
					targetPlant.reserved = true;
				}
				searchTimer = 1f;
			} else {
				searchTimer -= Time.deltaTime;
			}
		} else {
			if (targetPlant.harvested) {
				targetPlant = null;
			} else {
				targetX = targetPlant.x + .5f;
				targetZ = targetPlant.y + .5f;
				float dx = targetX - position.x;
				float dz = targetZ - position.z;
				if (dx * dx + dz * dz < 3f * 3f) {
					targetY = 0f;
				}

				if (tileX == targetPlant.x && tileY == targetPlant.y && position.y < .5f) {
					heldPlant = targetPlant;
					targetPlant.harvested = true;
					targetPlant = null;
					Farm.HarvestPlant(tileX,tileY);
					storeX = -1;
					storeY = -1;
				}
			}
		}

		position.x = Mathf.MoveTowards(position.x,targetX,xzSpeed * Time.deltaTime);
		position.y = Mathf.MoveTowards(position.y,targetY, Time.deltaTime * ySpeed);
		position.z = Mathf.MoveTowards(position.z,targetZ,xzSpeed * Time.deltaTime);

	}
	void SellPlantAI(float plantSmoothing) {
		int tileX = Mathf.FloorToInt(smoothPosition.x);
		int tileY = Mathf.FloorToInt(smoothPosition.z);

		heldPlant.EaseToWorldPosition(smoothPosition.x,smoothPosition.y + .08f,smoothPosition.z,plantSmoothing);

		if (storeX==-1) {
			int storeTileHash = Pathing.SearchForOne(tileX,tileY,30,Pathing.IsNavigableAll,Pathing.IsStore,Pathing.fullMapZone);
			if (storeTileHash!=-1) {
				Pathing.Unhash(storeTileHash,out storeX,out storeY);
			}
		} else {
			if (tileX == storeX && tileY == storeY) {
				Farm.SellPlant(heldPlant,storeX,storeY);
				targetPlant = null;
				heldPlant = null;
			} else {
				position.y = Mathf.MoveTowards(position.y,hoverHeight,ySpeed * Time.deltaTime);
				position.x = Mathf.MoveTowards(position.x,storeX + .5f,xzSpeed * Time.deltaTime);
				position.z = Mathf.MoveTowards(position.z,storeY + .5f,xzSpeed * Time.deltaTime);
			}
		}
	}

	public void Tick(float moveSmooth,float carrySmooth) {
		if (heldPlant == null) {
			GetPlantAI();
		} else {
			SellPlantAI(carrySmooth);
		}

		smoothPosition = Vector3.Lerp(smoothPosition,position,moveSmooth);
		matrix=GetMatrix();
	}
}
