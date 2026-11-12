using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class TimedSwitch : MonoBehaviour
{
    public GameObject door;
    [Min(0.1f)] public float openTime = 4f;
    public Color activeColor = new Color(0.2f, 1f, 0.5f);
    public float remainingTime { get; private set; }
    SpriteRenderer plate;
    Color idleColor;
    BoxCollider2D doorCollider;
    void Awake()
    {
        plate = GetComponent<SpriteRenderer>();
        if (plate != null) idleColor = plate.color;
        if (door != null) doorCollider = door.GetComponent<BoxCollider2D>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (door == null || other.GetComponentInParent<PlayerController>() == null) return;
        remainingTime = openTime; door.SetActive(false);
        if (plate != null) plate.color = activeColor;
    }
    void Update()
    {
        if (remainingTime <= 0 || door == null) return;
        remainingTime = Mathf.Max(0, remainingTime - Time.deltaTime);
        if (remainingTime <= 0)
        {
            var player = FindFirstObjectByType<PlayerController>();
            var collider = player != null ? player.GetComponent<Collider2D>() : null;
            var bounds = doorCollider != null ? new Bounds(door.transform.TransformPoint(doorCollider.offset),
                Vector3.Scale(doorCollider.size, door.transform.lossyScale)) : new Bounds();
            if (collider != null && bounds.Intersects(collider.bounds)) remainingTime = 0.1f;
            else
            {
                door.SetActive(true);
                if (plate != null) plate.color = idleColor;
            }
        }
        UIHandler.instance?.SetSwitchTime(remainingTime);
    }
    void OnDisable()
    {
        remainingTime = 0;
        if (door != null) door.SetActive(true);
        if (plate != null) plate.color = idleColor;
        UIHandler.instance?.SetSwitchTime(0);
    }
}
