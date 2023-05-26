using System.Collections.Generic;
using UnityEngine;

public interface ISerializableInvadersController
{
    void CleanUp();
    IEnumerable<GameObject> UniqueInvaderPrefabs { get; }
    IEnumerable<IEnumerable<IControllableEntity>> Invaders { get; }
    public Vector2 MoveDirection { get; set; }
    IControllableEntity CreateInvader(string prefabName, Vector2 position, int column);
    IEnumerable<IEntity> GetProjectiles();
    void AddProjectile(IEntity projectile);
}
