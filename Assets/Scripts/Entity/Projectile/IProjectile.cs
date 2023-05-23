using UnityEngine;

public interface IProjectile
{
    Vector2 Direction { get; set; }
    IEntity Shooter { get; set; }
}
