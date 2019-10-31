using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Scenes;

public class CarWorldItem : MonoBehaviour
{
    public struct ItemData : Unity.Entities.IComponentData
    {
        public int InstanceData;
        public int PublicData;
    }

    public List<Entity> Ids = new List<Entity>();
    public LiveLinkScene OtherScene = null;
    public World CopyWorld = null;

    [ContextMenu("Do Scene Change")]
    public void UpdateRandomData()
    {
        var em = World.Active.EntityManager;
        foreach (var ent in this.Ids)
        {
            var cur = em.GetComponentData<ItemData>(ent);
            cur.PublicData = Random.Range(201, 300);
            em.SetComponentData(ent, cur);
        }

        this.DebugWorlds();

        this.OtherScene.RequestCleanConversion();
        this.DebugWorlds();
    }

    private string DebugWorld(World w)
    {
        var q = w.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<ItemData>());
        var data = q.ToComponentDataArray<ItemData>(Unity.Collections.Allocator.TempJob);
        var ans = "[";
        foreach (var i in data)
        {
            ans += "{" + i.InstanceData + ":" + i.PublicData + "},";
        }
        ans += "]";
        data.Dispose();
        q.Dispose();
        return ans;
    }

    [ContextMenu("Debug Stuff")]
    public void DebugWorlds()
    {
        var active = DebugWorld(this.CopyWorld);
        Debug.Log("Active=" + active);
        var otherWorld = this.OtherScene.ConvertedWorld;
        Debug.Assert(otherWorld != World.Active);
        var other = DebugWorld(otherWorld);
        Debug.Log("OtherW=" + other);
    }

    // Start is called before the first frame update
    void Start()
    {
        var em = World.Active.EntityManager;
        for (var i=0; i<3; i++)
        {
            var ent = em.CreateEntity();
            em.AddComponentData(ent, new ItemData() { InstanceData = i, PublicData = Random.Range(100, 200) });
            this.Ids.Add(ent);
        }

        CopyWorld = new World("CopyWorld");
        OtherScene = new LiveLinkScene(CopyWorld, LiveLinkMode.LiveConvertGameView);

        this.DebugWorlds();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
