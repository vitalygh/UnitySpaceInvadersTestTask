//USING_ZENJECT
using UnityEngine;
#if USING_ZENJECT
using Zenject;
#endif

public class Spaceship : ControllableEntity
{
    public GameObject explosionEffect = null;

    protected override ILogController logController { get; set; }
    private IGameController gameController = null;
    private bool isDead = false;

#if USING_ZENJECT
    [Inject]
#endif
    private void Construct(ILogController logController, IGameController gameController)
    {
        this.logController = logController;
        this.gameController = gameController;
    }

    void Damage()
    {
        if (isDead)
            return;
        isDead = true;
        Explode(explosionEffect);
        if (gameController == null)
        {
            logController.Error(nameof(gameController) + " is null");
            return;
        }
        gameController.HP -= 1;
        if (gameController.HP > 0)
            return;
        gameController.GameOver();
    }

    public override void OnCollide(IEntity entity)
    {
        Damage();
    }

    public override void OnCollide(IProjectile projectile)
    {
        if (ReferenceEquals(projectile.Shooter, this))
            return;
        Damage();
    }


    private void Start()
    {
#if !USING_ZENJECT
        Construct(transform.parent.GetComponent<ILogController>(),
            transform.parent.GetComponent<IGameController>());
#endif
        if (gameController == null)
            logController.Error(nameof(gameController) + " is null");
    }


#if USING_ZENJECT
    public class Factory : PlaceholderFactory<Object, Vector3, IControllableEntity> { }
#endif
}
