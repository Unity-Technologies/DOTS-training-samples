using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee {
	public Vector3 position;
	public Vector3 velocity;
	public Vector3 smoothPosition;
	public Vector3 smoothDirection;
	public int team;
	public float size;
	public Bee enemyTarget;
	public Resource resourceTarget;

	public bool dead = false;
	public float deathTimer = 1f;
	public bool isAttacking;
	public bool isHoldingResource;
	public int index;

	public void Init(Vector3 myPosition,int myTeam,float mySize) {
		position = myPosition;
		velocity = Vector3.zero;
		smoothPosition = position+Vector3.right*.01f;
		smoothDirection = Vector3.zero;
		velocity = Vector3.zero;
		team = myTeam;
		size = mySize;

		dead = false;
		deathTimer = 1f;
		isAttacking = false;
		isHoldingResource = false;
		index = -1;

		enemyTarget = null;
		resourceTarget = null;
	}
}
