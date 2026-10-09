using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Поля налаштування патруля
    public float speed = 1.0f;
    public bool vertical;
    public float changeTime = 3.0f;

    // Внутрішні компоненти та змінні
    Rigidbody2D rigidbody2d;
    Animator animator;
    float timer;
    int direction = 1;
    bool broken = true;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;
    }

    void Update()
    {
        // Таймер патрулювання
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            direction = -direction; // Зміна напрямку (1 на -1, і навпаки)
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        // Якщо ворог полагоджений — зупиняємо рух
        if (!broken)
        {
            return;
        }

        Vector2 position = rigidbody2d.position;

        if (vertical)
        {
            position.y = position.y + speed * direction * Time.deltaTime;
            if (animator != null)
            {
                animator.SetFloat("Move X", 0);
                animator.SetFloat("Move Y", direction);
            }
        }
        else
        {
            position.x = position.x + speed * direction * Time.deltaTime;
            if (animator != null)
            {
                animator.SetFloat("Move X", direction);
                animator.SetFloat("Move Y", 0);
            }
        }

        rigidbody2d.MovePosition(position);
    }

    // Нанесення шкоди при дотику
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    // Метод ремонту ворога
    public void Fix()
    {
        broken = false;
        rigidbody2d.simulated = false; // Вимикаємо фізику та тригери
        if (animator != null)
        {
            animator.SetTrigger("Fixed");
        }
    }
}