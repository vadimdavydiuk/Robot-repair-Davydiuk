using System.Collections;
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
    SpriteRenderer spriteRenderer;
    Color normalColor;
    Coroutine flash;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) normalColor = spriteRenderer.color;
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
        int nextHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (nextHealth == currentHealth) return;
        if (amount < 0)
        {
            damageCooldown = timeInvincible;
            if (animator != null && animator.runtimeAnimatorController != null) animator.SetTrigger("Hit");
        }
        currentHealth = nextHealth;
        var audio = GameAudio.instance;
        if (audio != null) audio.Play(amount < 0 ? audio.hit : audio.heal);
        if (flash != null) StopCoroutine(flash);
        flash = StartCoroutine(Flash(amount < 0 ? new Color(1, 0.35f, 0.3f) : new Color(0.4f, 1, 0.65f)));
        UIHandler.instance?.SetHealthValue(currentHealth / (float)maxHealth);
    }
    IEnumerator Flash(Color color)
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = color;
        yield return new WaitForSeconds(0.16f);
        spriteRenderer.color = normalColor;
        flash = null;
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
        var audio = GameAudio.instance;
        if (audio != null) audio.Play(audio.launch);
        if (animator != null && animator.runtimeAnimatorController != null) animator.SetTrigger("Launch");
    }
}
