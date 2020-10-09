using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Button ButtonPrefab;
    public GameObject RootObject;
    public GameObject Background;
    public string[] Scenes;

    private Button[] buttons;
    private int loadedScene = -1;

    private void Start()
    {
        // DontDestroyOnLoad(RootObject);
        
        buttons = new Button[Scenes.Length];
        for (int i = 0; i < Scenes.Length; i++)
        {
            var button = Instantiate(ButtonPrefab, transform, false);
            var capturedIndex = i;
            button.onClick.AddListener(() => LoadScene(capturedIndex));

            button.GetComponentInChildren<Text>().text = Scenes[i];
            
            buttons[i] = button;
        }
    }

    private void LoadScene(int index)
    {
        if (loadedScene == index)
            return;

        if (loadedScene != -1)
            SceneManager.UnloadSceneAsync(Scenes[loadedScene]);
        else
            Background.SetActive(false);
        
        loadedScene = index;
        
        SceneManager.LoadScene(Scenes[loadedScene]);
        
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = i != loadedScene;
        }
    }
}
