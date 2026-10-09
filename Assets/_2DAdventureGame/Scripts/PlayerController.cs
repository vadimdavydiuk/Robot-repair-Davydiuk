using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction LaunchAction;

    public float speed = 3.0f;
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;
    public GameObject projectilePrefab;

    public int health { get { return currentHealth; } }

    Rigidbody2D rigidbody2d;
    Animator animator;

    Vector2 move;
    Vector2 moveDirection = new Vector2(1, 0);
    int currentHealth;
    bool isInvincible;
    float damageCooldown;

    // Таймер затримки між пострілами (ДЗ)
    public float timeBetweenShots = 0.5f;
    float shotTimer;

    void Start()
    {
        MoveAction.Enable();
        LaunchAction.Enable();

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        if (animator != null)
        {
            animator.SetFloat("Look X", moveDirection.x);
            animator.SetFloat("Look Y", moveDirection.y);
            animator.SetFloat("Speed", move.magnitude);
        }

        // Таймер невразливості
        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        // Оновлення затримки пострілу
        if (shotTimer > 0)
        {
            shotTimer -= Time.deltaTime;
        }

        // Постріл на клавішу C
        if (LaunchAction.WasPressedThisFrame() && shotTimer <= 0)
        {
            Launch();
            shotTimer = timeBetweenShots; // Запуск перезарядки
        }
    }

    void FixedUpdate()
    {
        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            damageCooldown = timeInvincible;
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        
        if (projectile != null)
        {
            projectile.Launch(moveDirection, 300);
        }

        if (animator != null)
        {
            animator.SetTrigger("Launch");
        }
    }
}