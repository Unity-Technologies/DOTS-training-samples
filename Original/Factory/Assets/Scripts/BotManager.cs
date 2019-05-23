using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour {
	public int maxBotCount = 2000;
	[Space(10)]
	public Mesh botMesh;
	public Material botMaterial;
	public Color emptyBotColor;
	public Color fullBotColor;
	public float botMoveSpeed;

	FactoryManager factory;
	List<FactoryBot> bots;

	List<Matrix4x4[]> botMatrices;
	List<Vector4[]> botColors;
	List<MaterialPropertyBlock> botMatProperties;

	Camera mainCam;


	int instanceCount = 1023;

	public void TrySpawnBot() {
		Vector2Int tile = new Vector2Int(Random.Range(1,factory.mapWidth - 1),Random.Range(1,factory.mapHeight - 1));
		TrySpawnBot(tile);
	}
	public void TrySpawnBot(Vector2Int tile) {
		if (bots.Count < maxBotCount) {
			if (factory.map.IsWall(tile) == false) {
				bots.Add(new FactoryBot(tile));
			}
		}
	}

	Vector3 GetBotPos(Vector2 pos) {
		Vector3 botPos = new Vector3(pos.x,0f,pos.y);
		return factory.factoryMarker.TransformPoint(botPos);
	}

	void SortBots() {
		int i, j;

		for (i=1;i<bots.Count;i++) {
			for (j=i-1;j>=0;j--) {
				if (bots[j].position.x-bots[j].radius>bots[j+1].position.x-bots[j+1].radius) {
					FactoryBot temp = bots[j];
					bots[j] = bots[j + 1];
					bots[j + 1] = temp;
				} else {
					break;
				}
			}
		}
	}

	public void Init () {
		int i,j;

		mainCam = Camera.main;
		botMatrices = new List<Matrix4x4[]>();
		botColors = new List<Vector4[]>();
		botMatProperties = new List<MaterialPropertyBlock>();
		factory = GetComponent<FactoryManager>();
		bots = new List<FactoryBot>();
		
		for (i=0;i<maxBotCount;i+=instanceCount) {
			botMatrices.Add(new Matrix4x4[instanceCount]);
			botColors.Add(new Vector4[instanceCount]);
			botMatProperties.Add(new MaterialPropertyBlock());
			for (j = i; j < Mathf.Min(instanceCount,bots.Count - i); j++) {
				botMatrices[i / instanceCount][j] = Matrix4x4.identity;
				botColors[i / instanceCount][j] = emptyBotColor;
			}
		}
	}
	
	void Update () {
		int i,j;

		factory.map.occupantTicker++;

		for (i = 0; i < bots.Count; i++) {
			FactoryBot bot = bots[i];
			Vector2Int tile = new Vector2Int(Mathf.FloorToInt(bot.position.x),Mathf.FloorToInt(bot.position.y));
			Vector2 uv = bot.position - new Vector2(tile.x,tile.y);
			uv.x = 3f * uv.x * uv.x - 2f * uv.x * uv.x * uv.x;
			uv.y = 3f * uv.y * uv.y - 2f * uv.y * uv.y * uv.y;

			Vector2Int hitTile = new Vector2Int(Mathf.FloorToInt(bot.position.x+.5f),Mathf.FloorToInt(bot.position.y+.5f));
			if (factory.map.occupantTickers[hitTile.x,hitTile.y]!=factory.map.occupantTicker) {
				factory.map.occupantTickers[hitTile.x,hitTile.y] = factory.map.occupantTicker;
				factory.map.occupants[hitTile.x,hitTile.y] = 1;
			} else {
				factory.map.occupants[hitTile.x,hitTile.y]+=1;
			}

			if (bot.targetCrafter == null) {
				bot.targetCrafter = factory.GetRequestingCrafter();
				bot.targetCrafter.workerCount++;
			} else if (bot.targetCrafter.destroyed) {
				bot.targetCrafter.workerCount--;
				bot.targetCrafter = null;
			} else {
				if (bot.movingToResource==false && bot.holdingResource==false) {
					bot.movingToResource = true;
					bot.navigator = factory.resourceNavigator;
				} else {
					if (bot.holdingResource==false) {
						if (factory.map.tiles[hitTile.x,hitTile.y].isResourceSpawner) {
							bot.holdingResource = true;
						}
					} else {
						bot.navigator = bot.targetCrafter.navigator;
						if (hitTile==bot.targetCrafter.position) {
							bot.targetCrafter.inventory++;
							bot.holdingResource = false;
							bot.movingToResource = false;
							bot.targetCrafter.workerCount--;
							bot.targetCrafter = null;
						}
					}
				}
			}

			if (bot.navigator != null) {
				if (factory.map.IsInsideMap(tile) && factory.map.IsInsideMap(new Vector2Int(tile.x + 1,tile.y + 1))) {
					Vector2 a = bot.navigator.flowField[tile.x,tile.y];
					Vector2 b = bot.navigator.flowField[tile.x + 1,tile.y];

					if (a == Vector2.zero) {
						a = b;
					}
					if (b == Vector2.zero) {
						b = a;
					}

					Vector2 c = bot.navigator.flowField[tile.x,tile.y + 1];
					Vector2 d = bot.navigator.flowField[tile.x + 1,tile.y + 1];
					if (c == Vector2.zero) {
						c = d;
					}
					if (d == Vector2.zero) {
						d = c;
					}

					Vector2 flow = Vector2.Lerp(Vector2.Lerp(a,b,uv.x),
												Vector2.Lerp(c,d,uv.x),
												uv.y);
					Vector2 moveVector = flow * botMoveSpeed * Time.deltaTime;

					moveVector += Random.insideUnitCircle * .002f;

					Vector2Int newTile = new Vector2Int(Mathf.FloorToInt(bot.position.x + moveVector.x + .5f),Mathf.FloorToInt(bot.position.y + .5f));
					if (factory.map.IsWall(newTile)) {
						moveVector.x = 0f;
					}

					newTile = new Vector2Int(Mathf.FloorToInt(bot.position.x + .5f),Mathf.FloorToInt(bot.position.y + moveVector.y + .5f));
					if (factory.map.IsWall(newTile)) {
						moveVector.y = 0f;
					}
					bot.position += moveVector * (1f - Mathf.Clamp01(bot.hitCount / 10f));
				}
			}
			bot.hitCount = 0;
			bots[i] = bot;
		}


		SortBots();

		for (i = 0; i < bots.Count; i++) {
			FactoryBot bot1 = bots[i];
			for (j = i + 1; j < bots.Count; j++) {
				FactoryBot bot2 = bots[j];
				if (bot2.position.x-bot2.radius > bot1.position.x + bot1.radius) {
					break;
				} else {
					Vector2 delta = bot2.position - bot1.position;
					float dist = delta.magnitude;
					if (dist<bot1.radius+bot2.radius) {
						bot1.hitCount++;
						bot2.hitCount++;
						Vector2 moveVector = delta.normalized * (dist - (bot1.radius + bot2.radius)) * .4f;

						Vector2 moveVector1 = moveVector;
						Vector2 moveVector2 = -moveVector;

						Vector2Int newTile = new Vector2Int(Mathf.FloorToInt(bot1.position.x + moveVector1.x + .5f),Mathf.FloorToInt(bot1.position.y + .5f));
						if (factory.map.IsInsideMap(newTile)==false) {
							moveVector1.x = 0f;
						} else if (factory.map.IsWall(newTile)) {
							moveVector1.x = 0f;
						}

						newTile = new Vector2Int(Mathf.FloorToInt(bot1.position.x + .5f),Mathf.FloorToInt(bot1.position.y + moveVector1.y + .5f));
						if (factory.map.IsInsideMap(newTile) == false) {
							moveVector1.y = 0f;
						} else if (factory.map.IsWall(newTile)) {
							moveVector1.y = 0f;
						}
						

						newTile = new Vector2Int(Mathf.FloorToInt(bot2.position.x + moveVector2.x + .5f),Mathf.FloorToInt(bot2.position.y + .5f));
						if (factory.map.IsInsideMap(newTile) == false) {
							moveVector2.x = 0f;
						} else if (factory.map.IsWall(newTile)) {
							moveVector2.x = 0f;
						}

						newTile = new Vector2Int(Mathf.FloorToInt(bot2.position.x + .5f),Mathf.FloorToInt(bot2.position.y + moveVector2.y + .5f));
						if (factory.map.IsInsideMap(newTile) == false) {
							moveVector2.y = 0f;
						} else if (factory.map.IsWall(newTile)) {
							moveVector2.y = 0f;
						}

						bot1.position += moveVector1;
						bots[i] = bot1;
						bot2.position += moveVector2;
						bots[j] = bot2;
					}
				}
			}
		}

		for (i=0;i<bots.Count;i++) {
			FactoryBot bot = bots[i];
			botMatrices[i/instanceCount][i-(i/instanceCount)*instanceCount] = Matrix4x4.TRS(GetBotPos(bot.position),Quaternion.Euler(90f,0f,0f),Vector3.one*.2f);
			if (bot.holdingResource==false) {
				botColors[i / instanceCount][i - (i / instanceCount) * instanceCount] = emptyBotColor;
			} else {
				botColors[i / instanceCount][i - (i / instanceCount) * instanceCount] = fullBotColor;
			}
		}

		for (i = 0; i < botMatrices.Count; i++) {
			if (bots.Count >= i * instanceCount) {
				botMatProperties[i].SetVectorArray("_Color",botColors[i]);
				Graphics.DrawMeshInstanced(botMesh,0,botMaterial,botMatrices[i],Mathf.Min(instanceCount,bots.Count - i * instanceCount),botMatProperties[i]);
			}
		}
	}
}
