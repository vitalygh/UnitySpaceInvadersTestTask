using UnityEngine;

public class Projectile : Entity, ISerializableProjectile
{
    private ILogController logController = null;
    private ISpaceshipController spaceshipController = null;
    private IEffectsController effectController = null;

    public float Speed  = 20.0f;
    public GameObject explosionEffect = null;

    public Vector2 Position { get => transform.position; }
    public Vector2 Direction { get; set; }
    public IEntity Shooter { get; set; }

    private void SpawnEffect()
    {
        if (explosionEffect == null)
            return;
        if ((effectController == null) && (transform.parent != null))
            effectController = transform.parent.GetComponent<IEffectsController>();
        if (effectController != null)
            effectController.SpawnEffect(explosionEffect, transform.position);
    }

    private void Explode()
    {
        Kill();
    }

    void UpdateMove()
    {
        if (spaceshipController == null)
            return;
        var position = transform.position;
        var move = Direction * Speed * Time.deltaTime;
        position.x += move.x;
        position.y += move.y;
        transform.position = position;
        if (spaceshipController.IsPositionOutOfBorders(position))
        {
            SpawnEffect();
            Explode();
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        logController = transform.parent.GetComponent<ILogController>();
        spaceshipController = transform.parent.GetComponent<ISpaceshipController>();
        if (spaceshipController == null)
            logController.Error(nameof(spaceshipController) + " is null");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMove();
    }

    private void OnCollide()
    {
        var collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;
        Explode();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var entity = collision.GetComponent<IControllableEntity>();
        if ((entity != null) && (entity != Shooter))
        {
            SpawnEffect();
            entity.OnCollide((IProjectile)this);
            OnCollide();
            return;
        }
        var projectile = collision.GetComponent<IProjectile>();
        if (projectile != null)
        {
            SpawnEffect();
            OnCollide();
            return;
        }
    }
}
