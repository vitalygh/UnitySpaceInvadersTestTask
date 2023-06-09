using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ISerializableInvadersController
{
    void CleanUp();
    IEnumerable<GameObject> UniqueInvaderPrefabs { get; }
    IEnumerable<IEnumerable<IControllableEntity>> Invaders { get; }
    public Vector2 MoveDirection { get; set; }
    IControllableEntity CreateInvader(string prefabName, Vector2 position, int column);
    void CreateTemporaryInvader(GameObject prefab, UnityAction<IControllableEntity> whileEntityCreated);
    IEnumerable<IEntity> GetProjectiles();
    void AddProjectile(IEntity projectile);
}
