using UnityEngine;

public class Spaceship : ControllableEntity
{
    public GameObject explosionEffect = null;

    private ILogController logController = null;
    private IGameController gameController = null;
    private bool isDead = false;

    void Damage()
    {
        if (isDead)
            return;
        isDead = true;
        Explode(explosionEffect);
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
        logController = transform.parent.GetComponent<ILogController>();
        gameController = transform.parent.GetComponent<IGameController>();
        if (gameController == null)
            logController.Error(nameof(gameController) + " is null");
    }
}
