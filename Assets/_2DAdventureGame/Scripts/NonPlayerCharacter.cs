using UnityEngine;
public class NonPlayerCharacter : MonoBehaviour
{
    public GameObject dialogueBubble;
    void Start() => ShowPrompt(false);
    public void ShowPrompt(bool visible)
    {
        if (dialogueBubble != null) dialogueBubble.SetActive(visible);
    }
    public void DisplayDialog() => UIHandler.instance?.DisplayDialogue();
}
