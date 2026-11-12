using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction LaunchAction;
    public InputAction TalkAction;
    public float speed = 3f;
    public int maxHealth = 5;
    public float timeInvincible = 2f;
    public float timeBetweenShots = 0.5f;
    public float talkDistance = 1.5f;
    public float projectileForce = 300f;
    public GameObject projectilePrefab;
    public int health => currentHealth;
    public bool controlsLocked { get; private set; }
    Rigidbody2D body;
    Animator animator;
    Vector2 move;
    Vector2 moveDirection = Vector2.right;
    int currentHealth;
    float damageCooldown;
    float shotTimer;
    NonPlayerCharacter nearbyNPC;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }
    void OnEnable()
    {
        MoveAction.Enable(); LaunchAction.Enable(); TalkAction.Enable();
    }
    void OnDisable()
    {
        MoveAction.Disable(); LaunchAction.Disable(); TalkAction.Disable();
        if (nearbyNPC != null) nearbyNPC.ShowPrompt(false);
    }
    void Start() => UIHandler.instance?.SetHealthValue(health / (float)maxHealth);
    void Update()
    {
        if (controlsLocked) return;
        move = Vector2.ClampMagnitude(MoveAction.ReadValue<Vector2>(), 1f);
        if (move.sqrMagnitude > 0.001f) moveDirection = move.normalized;
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.SetFloat("Look X", moveDirection.x);
            animator.SetFloat("Look Y", moveDirection.y);
            animator.SetFloat("Speed", move.magnitude);
        }
        damageCooldown = Mathf.Max(0, damageCooldown - Time.deltaTime);
        shotTimer = Mathf.Max(0, shotTimer - Time.deltaTime);
        if (LaunchAction.WasPressedThisFrame() && shotTimer <= 0 && projectilePrefab != null)
        {
            Launch(); shotTimer = timeBetweenShots;
        }
        var hit = Physics2D.Raycast(body.position + Vector2.up * 0.2f,
            moveDirection, talkDistance, LayerMask.GetMask("NPC"));
        var npc = hit.collider != null ? hit.collider.GetComponentInParent<NonPlayerCharacter>() : null;
        if (nearbyNPC != npc && nearbyNPC != null) nearbyNPC.ShowPrompt(false);
        nearbyNPC = npc;
        if (nearbyNPC != null)
        {
            nearbyNPC.ShowPrompt(true);
            if (TalkAction.WasPressedThisFrame()) nearbyNPC.DisplayDialog();
        }
    }
    void FixedUpdate()
    {
        if (!controlsLocked) body.MovePosition(body.position + move * speed * Time.fixedDeltaTime);
    }
    public void ChangeHealth(int amount)
    {
        if (controlsLocked || currentHealth <= 0 || (amount < 0 && damageCooldown > 0)) return;
        if (amount < 0)
        {
            damageCooldown = timeInvincible;
            if (animator != null && animator.runtimeAnimatorController != null) animator.SetTrigger("Hit");
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance?.SetHealthValue(currentHealth / (float)maxHealth);
    }
    public void LockControls()
    {
        controlsLocked = true; move = Vector2.zero; body.linearVelocity = Vector2.zero;
        if (nearbyNPC != null) nearbyNPC.ShowPrompt(false);
        if (animator != null && animator.runtimeAnimatorController != null) animator.SetFloat("Speed", 0);
    }
    void Launch()
    {
        var obj = Instantiate(projectilePrefab, body.position + Vector2.up * 0.5f, Quaternion.identity);
        obj.GetComponent<Projectile>().Launch(moveDirection, projectileForce);
        if (animator != null && animator.runtimeAnimatorController != null) animator.SetTrigger("Launch");
    }
}
