using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DATA;
using COLOUR;
public class UI_Core : MonoBehaviour
{
    public Material GL_MAT;
    public List<Color> palette;
    PRESENTATION PREZ;

    private void Awake()
    {
        PRESENTATION.INIT(palette.ToArray(), GL_MAT);
        PREZ = new PRESENTATION(new SCREEN_1_WELCOME(), new SCREEN_2_DRAWING(), new SCREEN_3_GRIDS(), new SCREEN_4_GRAPHS(), new SCREEN_5_MORE_FUN(), new SCREEN_EXAMPLE1(), new SCREEN_EXAMPLE2());
    }
    private void OnPostRender()
    {
        CheckKeys();
        PREZ.Update();
    }
    private void Update()
    {
        CheckKeys();
        Anim.time += Time.deltaTime;
    }
    private void CheckKeys()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PREZ.GotoPreviousScreen();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            PREZ.GotoNextScreen();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PREZ.GotoScreen(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PREZ.GotoScreen(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PREZ.GotoScreen(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PREZ.GotoScreen(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PREZ.GotoScreen(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            PREZ.GotoScreen(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            PREZ.GotoScreen(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            PREZ.GotoScreen(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            PREZ.GotoScreen(8);
        }
    }
}
