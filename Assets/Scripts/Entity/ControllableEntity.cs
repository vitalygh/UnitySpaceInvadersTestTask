//USING_ZENJECT
using UnityEngine;
#if USING_ZENJECT
using Zenject;
#endif

public abstract class ControllableEntity : Entity, IControllableEntity
{
    private IGun gun = null;
    protected abstract ILogController logController { get; set; }
    private IEffectsController effectController = null;

#if USING_ZENJECT
    [Inject]
    private void Construct(IEffectsController effectController)
    {
        this.effectController = effectController;
    }
#endif

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
#if !USING_ZENJECT
            if ((effectController == null) && (transform.parent != null))
                effectController = transform.parent.GetComponent<IEffectsController>();
#endif
            if (effectController != null)
                effectController.SpawnEffect(effectPrefab, Position);
            else
                logController.Error(nameof(effectController) + " is null");
        }
        var collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;
        Kill();
    }

    public abstract void OnCollide(IEntity entity);

    public abstract void OnCollide(IProjectile projectile);
}
