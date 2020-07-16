using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSearch : MonoBehaviour
{
    public static int[] GetNeighbors(int index, int width, int height)
    {
        int[] neighBors = new int[4];
        neighBors[0] = index > width-1 ? index - height : -1; //Top
        neighBors[1] = index % width-1 != 0 ? index + 1 : -1; //Right
        neighBors[2] = (width * height) - index > width ? index + height : -1; //Bottom
        neighBors[3] = index % width != 0 ? index - 1 : -1; //Left

        
        return neighBors;
    }
}
