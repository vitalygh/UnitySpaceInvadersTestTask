//USING_ZENJECT
using UnityEngine;
#if USING_ZENJECT
using Zenject;
#endif

public class Invader : ControllableEntity
{
    protected override ILogController logController { get; set; }
    private IGameController gameController = null;
    
    public int Score = 10;
    public GameObject explosionEffect = null;

#if USING_ZENJECT
    [Inject]
#endif
    private void Construct(ILogController logController, IGameController gameController)
    {
        this.logController = logController;
        this.gameController = gameController;
    }

    public override void OnCollide(IEntity entity)
    {
    }

    public override void OnCollide(IProjectile projectile)
    {
        if (ReferenceEquals(projectile.Shooter, this))
            return;
        if (gameController != null)
            gameController.Score += Score;
        Explode(explosionEffect);
    }

    // Start is called before the first frame update
    void Start()
    {
#if !USING_ZENJECT
        Construct(transform.parent.GetComponent<ILogController>(),
            transform.parent.GetComponent<IGameController>());
#endif
        if (gameController == null)
            logController.Error(nameof(gameController) + " is null");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var entity = collision.GetComponent<IControllableEntity>();
        if (entity != null)
            entity.OnCollide(this);
    }

#if USING_ZENJECT
    public class Factory : PlaceholderFactory<Object, Vector3, IControllableEntity> { }
#endif
}
