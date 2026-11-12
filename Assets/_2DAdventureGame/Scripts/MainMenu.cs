using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenu : MonoBehaviour
{
    public string levelScene = "MyLevel";
    Button start, quit;
    bool loading;
    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        start = root.Q<Button>("StartButton");
        quit = root.Q<Button>("QuitButton");
        start.clicked += StartGame;
        quit.clicked += Quit;
    }
    void OnDisable()
    {
        start.clicked -= StartGame;
        quit.clicked -= Quit;
    }
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame) StartGame();
    }
    public void StartGame()
    {
        if (loading) return;
        loading = true;
        start.SetEnabled(false);
        SceneManager.LoadScene(levelScene);
    }
    void Quit() => Application.Quit();
}
