using UnityEngine;

public class Wall : MonoBehaviour
{
    [HideInInspector] public bool isActive = false;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Called by the CollisionManager to enable/disable this wall.
    /// </summary>
    public void SetActive(bool active)
    {
        isActive = active;
        gameObject.SetActive(active);
    }

    /// <summary>
    /// Called by the Bullet when it hits this wall.
    /// </summary>
    public void OnHit()
    {
        RandomWall.Instance.OnWallHit(this);
    }
}
