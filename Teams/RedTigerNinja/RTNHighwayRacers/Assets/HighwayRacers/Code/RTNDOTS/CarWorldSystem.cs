using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Scenes;

public class CarWorldSystem : MonoBehaviour
{
    private World SavedWorld = null;
    private Unity.Entities.EntityManagerDiffer SavedDiffState;
    private bool HasSavedDiff = false;
    public LiveLinkScene OtherScene = null;

    [ContextMenu("Test Live Link")]
    public void DoTestLiveLink()
    {
        OtherScene = new LiveLinkScene(World.Active, LiveLinkMode.LiveConvertGameView);
        OtherScene.RequestCleanConversion();
    }


    [ContextMenu("Do Diff Test")]
    public void DoDiffTest()
    {
        if (!HasSavedDiff)
        {
            HasSavedDiff = true;
            SavedDiffState = new EntityManagerDiffer(World.Active, Unity.Collections.Allocator.Persistent);
            SavedDiffState.GetChanges(EntityManagerDifferOptions.Default, Unity.Collections.Allocator.Persistent).Dispose();
            return;
        }
        else
        {
            var diff = SavedDiffState.GetChanges(EntityManagerDifferOptions.Default, Unity.Collections.Allocator.Persistent);
            Debug.Log("AnyChanges=" + diff.AnyChanges + " Fwd=" + diff.ForwardChangeSet + " Back=" + diff.HasReverseChangeSet);
            Debug.Log("All=" + diff.ToString());
            var fchng = diff.ForwardChangeSet;
            foreach (var ec in fchng.EntityPatches)
            {
                Debug.Log(ec.ToString());
            }
            diff.Dispose();
        }
    }



    [ContextMenu("Do World Test!")]
    public void DoSaveLoadTest()
    {
        var world = World.Active;
        Debug.Log("SN=" + world.SequenceNumber + " Version=" + world.Version + " EV=" + world.EntityManager.Version + " GEV=" + world.EntityManager.GlobalSystemVersion);
        var em = world.EntityManager;

        if (this.SavedWorld == null)
        {
            this.SavedWorld = new World("SavedWorld");
            this.SavedWorld.EntityManager.CopyAndReplaceEntitiesFrom(em);
        }
        else
        {
            em.CompleteAllJobs();
            em.CopyAndReplaceEntitiesFrom(this.SavedWorld.EntityManager);
            this.SavedWorld = null;
        }

        //var myWorld = new World("OtherWorld");
        //myWorld.EntityManager.CopyAndReplaceEntitiesFrom(em);


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            this.DoSaveLoadTest();
        }
    }
}
