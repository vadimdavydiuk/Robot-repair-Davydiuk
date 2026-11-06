using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
    public float displayTime = 4.0f;
    public GameObject dialogueBubble; // Сюди перетягуємо хмарку "Press X" над головою
    float timerDisplay;

    void Start()
    {
        if (dialogueBubble != null)
            dialogueBubble.SetActive(false);

        timerDisplay = -1.0f;
    }

    void Update()
    {
        if (timerDisplay > 0)
        {
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                if (dialogueBubble != null)
                    dialogueBubble.SetActive(false);
            }
        }
    }

    public void DisplayDialog()
    {
        timerDisplay = displayTime;

        // Показуємо хмарку "Press X" над головою NPC
        if (dialogueBubble != null)
            dialogueBubble.SetActive(true);

        // Відкриваємо основний діалог HUD внизу екрана
        if (UIHandler.instance != null)
        {
            UIHandler.instance.DisplayDialogue();
        }
    }
}