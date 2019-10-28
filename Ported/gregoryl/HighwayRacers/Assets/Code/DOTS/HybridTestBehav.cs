using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class HybridTestBehav : MonoBehaviour
{
    private EntityQuery QueryToDraw;
    public Entity DebugThis;
    public Object SelectionObj;
    public System.Type SelectionType;
    [Multiline]
    public string SelectionTypeName;

    // Start is called before the first frame update
    void Start()
    {
        UnityEditor.Selection.selectionChanged += (() => this.UpdateEntityInfo());
    }

    [ContextMenu("Debug Current Item")]
    public void CheckCurrentSelection()
    {
        var ob = UnityEditor.Selection.activeObject;
        var esp = (ob as Unity.Entities.Editor.EntitySelectionProxy);
        if (esp)
        {
            var ent = esp.Entity;
            if (ent != this.DebugThis)
            {
                this.DebugThis = esp.Entity;
                this.UpdateEntityInfo();
            }
        }
    }

    public void UpdateEntityInfo()
    {
        this.SelectionTypeName = this.DebugThis.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        this.CheckCurrentSelection();

        Debug.DrawLine(Vector3.zero, Vector3.one);

        var world = Unity.Entities.World.Active;
        if (this.QueryToDraw == null)
        {
            this.QueryToDraw = world.EntityManager.CreateEntityQuery(typeof(Unity.Transforms.LocalToWorld));
        }
        var eids = this.QueryToDraw.ToEntityArray(Unity.Collections.Allocator.TempJob);
        this.DebugThis = eids[0];
        eids.Dispose();

        //Unity.Properties.PropertyContainer
        //var sel = (new Unity.Entities.Editor.EntitySelectionProxy());
        //sel.Entity = this.DebugThis;
        //Unity.Entities.Editor.EntitySelectionProxy

        //UnityEditor.Selection.activeObject = this.DebugThis;

        if (false)
        {
            var data = this.QueryToDraw.ToComponentDataArray<Unity.Transforms.LocalToWorld>(Unity.Collections.Allocator.TempJob);
            Debug.Log("Data.Length=" + data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                var ob = data[i];
                Debug.DrawLine(ob.Position, ob.Position + (ob.Forward * 10.0f), Color.green);
            }
            Debug.DrawLine(Vector3.zero, Vector3.one, Color.red);
            data.Dispose();
        }

    }
}
