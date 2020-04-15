using UnityEngine;
using System.Collections;
 
public class InputManager : MonoBehaviour
{
    Ray RayOrigin;
    RaycastHit HitInfo;
 
    // Work in progress, not done!

    
    // Update is called once per frame
    void Update ()
    {
        if(Input.GetKey(KeyCode.E))
        {
            RayOrigin = Camera.main.ViewportPointToRay(new Vector3(0,0,0));
            
            var result = Physics.Raycast(RayOrigin,out HitInfo,1000f);
            if (result)
            {
                Debug.DrawRay(RayOrigin.direction,HitInfo.point,Color.yellow);
            }
        }
    }
}