using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Init : IComponentData
{
	public int antCount;
	public Entity antPrefab;
	public Entity obstaclePrefab;
	public Entity goalPrefab;
	public Entity homePrefab;
	public Entity boardPrefab;
}
