using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }
    public float displayTime = 4f;
    VisualElement healthBar, dialogue, winScreen, loseScreen;
    Label counter, switchTimer;
    float dialogueTimer;
    void Awake() => instance = this;
    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        healthBar = root.Q<VisualElement>("HealthBar");
        dialogue = root.Q<VisualElement>("NPCDialogue");
        winScreen = root.Q<VisualElement>("WinScreenContainer");
        loseScreen = root.Q<VisualElement>("LoseScreenContainer");
        counter = root.Q<Label>("RobotCounter");
        switchTimer = root.Q<Label>("SwitchTimer");
        dialogue.style.display = DisplayStyle.None;
        winScreen.style.opacity = 0;
        loseScreen.style.opacity = 0;
        SetHealthValue(1f);
    }
    void OnDestroy() { if (instance == this) instance = null; }
    void Update()
    {
        if (dialogueTimer <= 0) return;
        dialogueTimer -= Time.deltaTime;
        if (dialogueTimer <= 0) dialogue.style.display = DisplayStyle.None;
    }
    public void SetHealthValue(float percentage)
    {
        if (healthBar != null) healthBar.style.width = Length.Percent(100 * Mathf.Clamp01(percentage));
    }
    public void SetCounter(int repaired, int total)
    {
        if (counter != null) counter.text = $"{repaired} / {total}";
    }
    public void SetSwitchTime(float seconds)
    {
        if (switchTimer != null) switchTimer.text = seconds > 0 ? $"Двері відчинені: {seconds:0.0} с" : "";
    }
    public void DisplayDialogue()
    {
        dialogue.style.display = DisplayStyle.Flex; dialogueTimer = displayTime;
    }
    public void DisplayWinScreen() => ShowEnd(winScreen);
    public void DisplayLoseScreen() => ShowEnd(loseScreen);
    void ShowEnd(VisualElement screen)
    {
        dialogue.style.display = DisplayStyle.None; dialogueTimer = 0; screen.style.opacity = 1;
    }
}
