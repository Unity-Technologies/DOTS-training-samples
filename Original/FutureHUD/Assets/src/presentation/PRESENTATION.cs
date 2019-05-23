using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using COLOUR;

public class PRESENTATION
{
    public const float SCREEN_TITLE_X = 0.01f;
    public const float SCREEN_TITLE_Y = 0.98f;
    public const float DEFAULT_TXT_CELL_HEIGHT = 0.0025f;
    public static Material mat;
    public static float MX, MY, MX_EASED, MY_EASED = 0f;
    public float M_EASE = 20f;

    List<SCREEN> screens;
    int currentScreen_index = 0;
    int totalScreens = 0;
    SCREEN currentScreen, previousScreen;
    float screenTimer;
    public PRESENTATION(params SCREEN[] _screens)
    {
        screens = new List<SCREEN>();
        for (int i = 0; i < _screens.Length; i++)
        {
            AddScreen(_screens[i]);
        }

        GotoScreen(0);
    }
    public static void INIT(Color[] _palette, Material _mat)
    {
        // init material
        mat = _mat;

        GL_FONT_3x5.Init();
        GL_MATRIX_ANIMS.Init();
        COL.INIT_PALETTES(_palette);
    }
    public void AddScreen(SCREEN _screen)
    {
        screens.Add(_screen);
        totalScreens = screens.Count;
    }
    public void GotoScreen(int _index)
    {
        if (_index < totalScreens)
        {
            currentScreen_index = _index;
            currentScreen = screens[currentScreen_index];
            screenTimer = currentScreen.duration;
        }
    }
    public void GotoNextScreen()
    {
        GotoScreen((currentScreen_index + 1) % totalScreens);
    }
    public void GotoPreviousScreen()
    {
        int _prev = (currentScreen_index - 1) % totalScreens;
        if (_prev < 0)
        {
            _prev = totalScreens - 1;
        }
        GotoScreen(_prev);
    }
    public void Update()
    {
        GL_DRAW.RESET_SKEW();
        MX = Input.mousePosition.x / Screen.width;
        MY = Input.mousePosition.y / Screen.height;
        MX_EASED += (MX - MX_EASED) / M_EASE;
        MY_EASED += (MY - MY_EASED) / M_EASE;
        currentScreen.SetMouse(MX, MY, MX_EASED, MY_EASED);
        mat.SetPass(0);
        GL.LoadOrtho();

        screenTimer -= Time.fixedDeltaTime;
        if (screenTimer < 0)
        {
            GotoNextScreen();
        }
        CheckKeys();
        Draw();

    }
    void CheckKeys()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GotoPreviousScreen();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GotoNextScreen();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GotoScreen(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GotoScreen(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GotoScreen(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GotoScreen(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            GotoScreen(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            GotoScreen(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            GotoScreen(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            GotoScreen(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            GotoScreen(8);
        }
    }
    public void Draw()
    {
        GL_DRAW.RESET_SKEW();
        currentScreen.Draw();
        Palette _P = currentScreen.P;
        Color _COL = _P.Get(4);

        GL_DRAW.RESET_SKEW();
        // screen name
        GL_TXT.Txt(currentScreen.title, SCREEN_TITLE_X, SCREEN_TITLE_Y, DEFAULT_TXT_CELL_HEIGHT, _COL);
        string _PAGE_STR = (currentScreen_index + 1) + "/" + totalScreens;

        // screen index status
        GL_TXT.Txt(_PAGE_STR, 1f - (((_PAGE_STR.Length + 2) * 3) * DEFAULT_TXT_CELL_HEIGHT), SCREEN_TITLE_Y, DEFAULT_TXT_CELL_HEIGHT, _COL);

        // time remaining line
        currentScreen.timeRemaining = 1f - (screenTimer / currentScreen.duration);
        GL_DRAW.Draw_RECT_FILL(0f, 0f, 0.005f, currentScreen.timeRemaining, _COL);
    }
}
