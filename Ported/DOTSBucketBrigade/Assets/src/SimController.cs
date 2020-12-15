using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimController : MonoBehaviour
{
    [Header("BOARD")]
    public int rows = 50;
    public int columns = 50;

    [Header("COLORS")]
    public Color color_ground;
    public Color color_fire_low;
    public Color color_fire_high;
    public Color color_bucket_empty;
    public Color color_bucket_full;
    public Color color_role_scooper;
    public Color color_role_thrower;
    public Color color_role_passer_full;
    public Color color_role_passer_empty;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
