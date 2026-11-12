using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public UIHandler uiHandler;
    public float restartDelay = 3f;
    public bool gameEnded { get; private set; }
    EnemyController[] enemies;
    void Start()
    {
        enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        uiHandler.SetCounter(0, enemies.Length);
    }
    void Update()
    {
        if (gameEnded) return;
        if (player.health <= 0) { EndGame(false); return; }
        int repaired = 0;
        foreach (var enemy in enemies) if (enemy != null && !enemy.isBroken) repaired++;
        uiHandler.SetCounter(repaired, enemies.Length);
        if (enemies.Length > 0 && repaired == enemies.Length) EndGame(true);
    }
    void EndGame(bool won)
    {
        gameEnded = true; player.LockControls();
        foreach (var enemy in enemies) if (enemy != null) enemy.StopMoving();
        if (won)
        {
            uiHandler.DisplayWinScreen();
            var audio = GameAudio.instance;
            if (audio != null) audio.Play(audio.victory);
        }
        else uiHandler.DisplayLoseScreen();
        StartCoroutine(Restart());
    }
    IEnumerator Restart()
    {
        yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
