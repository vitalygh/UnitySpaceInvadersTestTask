using UnityEngine;

public abstract class ControllableEntity : Entity, IControllableEntity
{
    private IGun gun = null;
    private IEffectsController effectController = null;

    public Vector2 Position
    {
        get => new Vector2(transform.position.x, transform.position.y);
        set => transform.position = value;
    }

    public IGun Gun
    {
        get
        {
            if (gun == null)
                gun = GetComponent<IGun>();
            return gun;
        }
    }

    public void Explode(GameObject effectPrefab = null)
    {
        if (effectPrefab != null)
        {
            if ((effectController == null) && (transform.parent != null))
                effectController = transform.parent.GetComponent<IEffectsController>();
            if (effectController != null)
                effectController.SpawnEffect(effectPrefab, Position);
        }
        var collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;
        Kill();
    }

    public abstract void OnCollide(IEntity entity);

    public abstract void OnCollide(IProjectile projectile);
}
