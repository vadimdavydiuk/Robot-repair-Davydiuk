using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public int amount = 1; // Поле для налаштування в Inspector

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null && controller.health < controller.maxHealth)
        {
            controller.ChangeHealth(amount);
            Destroy(gameObject);
        }
    }
}