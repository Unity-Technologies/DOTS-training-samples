using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;

public class KeyboardCommands : MonoBehaviour {
	public Text text;
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.H)) {
			text.enabled = !text.enabled;
		}
		if (Input.GetKeyDown(KeyCode.R)) {
            //Delete all entities.... somehow
            World.Active.EntityManager.CompleteAllJobs();
            NativeArray<Entity> entities = World.Active.EntityManager.GetAllEntities();
            World.Active.EntityManager.DestroyEntity(entities);
            entities.Dispose();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
