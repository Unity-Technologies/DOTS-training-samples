using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Farmer {
	public Vector2 position;
	public Vector2 smoothPosition;
	public Intention intention;
	public Path path;
	public Matrix4x4 matrix;
	
	float walkSpeed = 4f;
	Rock targetRock;
	RectInt tillingZone;
	bool foundTillingZone;
	Plant heldPlant;
	bool attackingARock;
	bool hasBoughtSeeds;

	int tileX {
		get {
			return Mathf.FloorToInt(position.x);
		}
	}
	int tileY {
		get {
			return Mathf.FloorToInt(position.y);
		}
	}

	public Farmer (Vector2 pos) {
		position = pos;
		smoothPosition = pos;
		intention = Intention.None;
		path = new Path();
		matrix = Matrix4x4.Translate(new Vector3(smoothPosition.x,.5f,smoothPosition.y)) * Matrix4x4.Scale(Vector3.one * .5f);
	}

	public Vector3 GetSmoothWorldPos() {
		return new Vector3(smoothPosition.x,0f,smoothPosition.y);
	}

	public bool IsTillableInZone(int x, int y) {
		if (Farm.groundStates[x,y]==GroundState.Default) {
			if (x>=tillingZone.xMin && x<=tillingZone.xMax) {
				if (y>=tillingZone.yMin && y<=tillingZone.yMax) {
					return true;
				}
			}
		}
		return false;
	}

	public void PickNewIntention() {
		path.Clear();

		
		int rand = Random.Range(0,4);
		if (rand == 0) {
			intention = Intention.SmashRocks;
		} else if (rand == 1) {
			intention = Intention.TillGround;
			foundTillingZone = false;
		} else if (rand == 2) {
			intention = Intention.PlantSeeds;
		} else if (rand==3) {
			intention = Intention.SellPlants;
		}
	}

	void SmashRocksAI() {
		if (targetRock == null) {
			targetRock = Pathing.FindNearbyRock(tileX,tileY,20,path);
			if (targetRock == null) {
				intention = Intention.None;
			}
		} else {
			if (path.xPositions.Count == 1) {
				attackingARock = true;

				targetRock.TakeDamage(1);
			}
			if (targetRock.health <= 0) {
				targetRock = null;
				intention = Intention.None;
			}
		}
	}

	void TillGroundAI() {
		if (foundTillingZone == false) {
			int width = Random.Range(1,8);
			int height = Random.Range(1,8);
			int minX = tileX + Random.Range(-10,10 - width);
			int minY = tileY + Random.Range(-10,10 - height);
			if (minX < 0) minX = 0;
			if (minY < 0) minY = 0;
			if (minX + width >= Farm.instance.mapSize.x) minX = Farm.instance.mapSize.x - 1 - width;
			if (minY + height >= Farm.instance.mapSize.y) minY = Farm.instance.mapSize.y - 1 - height;

			bool blocked = false;
			for (int x=minX;x<=minX+width;x++) {
				for (int y=minY;y<=minY+height;y++) {
					GroundState groundState = Farm.groundStates[x,y];
					if (groundState!=GroundState.Default && groundState!=GroundState.Tilled) {
						blocked = true;
						break;
					}
					if (Farm.tileRocks[x,y]!=null || Farm.storeTiles[x,y]) {
						blocked = true;
						break;
					}
				}
				if (blocked) {
					break;
				}
			}
			if (blocked==false) {
				tillingZone = new RectInt(minX,minY,width,height);
				foundTillingZone = true;
			} else {
				if (Random.value < .2f) {
					intention = Intention.None;
				}
			}
		} else {
			Debug.DrawLine(new Vector3(tillingZone.min.x,.1f,tillingZone.min.y),new Vector3(tillingZone.max.x + 1f,.1f,tillingZone.min.y),Color.green);
			Debug.DrawLine(new Vector3(tillingZone.max.x+1f,.1f,tillingZone.min.y),new Vector3(tillingZone.max.x + 1f,.1f,tillingZone.max.y+1f),Color.green);
			Debug.DrawLine(new Vector3(tillingZone.max.x+1f,.1f,tillingZone.max.y+1f),new Vector3(tillingZone.min.x,.1f,tillingZone.max.y+1f),Color.green);
			Debug.DrawLine(new Vector3(tillingZone.min.x,.1f,tillingZone.max.y+1f),new Vector3(tillingZone.min.x,.1f,tillingZone.min.y),Color.green);
			if (IsTillableInZone(tileX,tileY)) {
				path.Clear();
				Farm.TillGround(tileX,tileY);
			} else {
				if (path.count == 0) {
					int tileHash = Pathing.SearchForOne(tileX,tileY,25,Pathing.IsNavigableDefault,Pathing.IsTillable,tillingZone);
					if (tileHash != -1) {
						int tileX, tileY;
						Pathing.Unhash(tileHash,out tileX,out tileY);
						Pathing.AssignLatestPath(path,tileX,tileY);
					} else {
						intention = Intention.None;
					}
				}
			}
		}
	}

	void PlantSeedsAI() {
		if (hasBoughtSeeds == false) {
			if (Farm.storeTiles[tileX,tileY]) {
				hasBoughtSeeds = true;
			} else if (path.count==0) {
				Pathing.WalkTo(tileX,tileY,40,Pathing.IsStore,path);
				if (path.count==0) {
					intention = Intention.None;
				}
			}
		} else if (Pathing.IsReadyForPlant(tileX,tileY)) {
			path.Clear();
			int seed = Mathf.FloorToInt(Mathf.PerlinNoise(tileX / 10f,tileY / 10f) * 10)+Farm.seedOffset;
			Farm.SpawnPlant(tileX,tileY,seed);
		} else {
			if (path.count==0) {
				if (Random.value < .1f) {
					intention = Intention.None;
				} else {
					int tileHash = Pathing.SearchForOne(tileX,tileY,25,Pathing.IsNavigableDefault,Pathing.IsReadyForPlant,Pathing.fullMapZone);
					if (tileHash != -1) {
						int tileX, tileY;
						Pathing.Unhash(tileHash,out tileX,out tileY);
						Pathing.AssignLatestPath(path,tileX,tileY);
					} else {
						intention = Intention.None;
					}
				}
			}
		}
	}

	void SellPlantsAI(float moveSmooth=0f) {
		if (heldPlant==null) {
			if (Farm.IsHarvestable(tileX,tileY)) {
				heldPlant = Farm.tilePlants[tileX,tileY];
				Farm.HarvestPlant(tileX,tileY);
				path.Clear();
			} else if (path.count == 0) {
				Pathing.WalkTo(tileX,tileY,25,Farm.IsHarvestableAndUnreserved,path);
				if (path.count==0) {
					intention = Intention.None;
				} else {
					Farm.tilePlants[path.xPositions[0],path.yPositions[0]].reserved = true;
				}
			}
			
		} else {
			heldPlant.EaseToWorldPosition(smoothPosition.x,1f,smoothPosition.y,moveSmooth);
			if (Farm.storeTiles[tileX,tileY]) {
				Farm.SellPlant(heldPlant,tileX,tileY);
				heldPlant = null;
				path.Clear();
				
			} else if (path.count==0) {
				Pathing.WalkTo(tileX,tileY,40,Pathing.IsStore,path);
			}
		}
	}

	void FollowPath() {
		if (path.count > 0) {
			for (int i = 0; i < path.xPositions.Count - 1; i++) {
				Debug.DrawLine(new Vector3(path.xPositions[i] + .5f,.5f,path.yPositions[i] + .5f),new Vector3(path.xPositions[i + 1] + .5f,.5f,path.yPositions[i + 1] + .5f),Color.red);
			}

			int nextTileX = path.xPositions[path.xPositions.Count - 1];
			int nextTileY = path.yPositions[path.yPositions.Count - 1];
			if (tileX == nextTileX && tileY == nextTileY) {
				path.xPositions.RemoveAt(path.xPositions.Count - 1);
				path.yPositions.RemoveAt(path.yPositions.Count - 1);
			} else {
				if (Farm.IsBlocked(nextTileX,nextTileY) == false) {
					float offset = .5f;
					if (Farm.groundStates[nextTileX,nextTileY]==GroundState.Plant) {
						offset = .01f;
					}
					Vector2 targetPos = new Vector2(nextTileX + offset,nextTileY + offset);
					position = Vector2.MoveTowards(position,targetPos,walkSpeed * Time.deltaTime);
				}
			}
		}
	}

	public void Tick(float moveSmooth) {
		attackingARock = false;
		if (intention == Intention.None) {
			PickNewIntention();
		} else if (intention == Intention.SmashRocks) {
			SmashRocksAI();
		} else if (intention == Intention.TillGround) {
			TillGroundAI();
		} else if (intention == Intention.PlantSeeds) {
			PlantSeedsAI();
		} else if (intention == Intention.SellPlants) {
			SellPlantsAI(moveSmooth);
		}

		FollowPath();

		smoothPosition = Vector3.Lerp(smoothPosition,position,moveSmooth);

		float xOffset = 0f;
		float zOffset = 0f;
		if (attackingARock) {
			float rand = Random.value*.5f;
			xOffset = (path.xPositions[0]+.5f - smoothPosition.x) * rand;
			zOffset = (path.yPositions[0]+.5f - smoothPosition.y) * rand;
		}

		// update our x and z position in our matrix:
		matrix.m03 = smoothPosition.x+xOffset;
		matrix.m23 = smoothPosition.y+zOffset;
	}
}
