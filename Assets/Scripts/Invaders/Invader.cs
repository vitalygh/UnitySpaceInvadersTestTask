using UnityEngine;

public class Invader : ControllableEntity
{
    private ILogController logController = null;
    private IGameController gameController = null;
    
    public int Score = 10;
    public GameObject explosionEffect = null;

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
        logController = transform.parent.GetComponent<ILogController>();
        gameController = transform.parent.GetComponent<IGameController>();
        if (gameController == null)
            logController.Error(nameof(gameController) + " is null");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var entity = collision.GetComponent<IControllableEntity>();
        if (entity != null)
            entity.OnCollide(this);
    }
}
