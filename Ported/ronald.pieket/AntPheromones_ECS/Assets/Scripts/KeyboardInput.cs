using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KeyboardInput : MonoBehaviour {

  [SerializeField]
  Text text;
  static bool showText = true;

  void Start ()
  {
    text.enabled = showText;
  }
	
  void Update ()
  {
    if (Input.GetKeyDown(KeyCode.H))
    {
      showText = !showText;
      text.enabled = showText;
    }
    if (Input.GetKeyDown(KeyCode.R))
    {
      World.Active.EntityManager.CompleteAllJobs();
      NativeArray<Entity> entities = World.Active.EntityManager.GetAllEntities();
      World.Active.EntityManager.DestroyEntity(entities);
      entities.Dispose();
      FixedTimeStepSystemGroup.UpdateCount = 1;
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    int updateCount = FixedTimeStepSystemGroup.UpdateCount;

    if (Input.GetKeyDown(KeyCode.Alpha1))
    {
      updateCount = 1;
    }
    else if (Input.GetKeyDown(KeyCode.Alpha2))
    {
      updateCount = 2;
    }
    else if (Input.GetKeyDown(KeyCode.Alpha3))
    {
      updateCount = 3;
    }
    else if (Input.GetKeyDown(KeyCode.Alpha4))
    {
      updateCount = 4;
    }
    else if (Input.GetKeyDown(KeyCode.Alpha5))
    {
      updateCount = 5;
    }
    else if (Input.GetKeyDown(KeyCode.Alpha6))
    {
      updateCount = 6;
    }
    else if (Input.GetKeyDown(KeyCode.Alpha7))
    {
      updateCount = 7;
    }
    else if (Input.GetKeyDown(KeyCode.Alpha8))
    {
      updateCount = 8;
    }
    else if (Input.GetKeyDown(KeyCode.Alpha9))
    {
      updateCount = 9;
    }

    FixedTimeStepSystemGroup.UpdateCount = updateCount;
  }
}
