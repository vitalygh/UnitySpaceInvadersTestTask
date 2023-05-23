using UnityEngine;

public interface IControllableEntity: IEntity
{
    Vector2 Position { get; set; }
    IGun Gun { get; }
    void OnCollide(IProjectile projectile);
    void OnCollide(IEntity entity);
}
