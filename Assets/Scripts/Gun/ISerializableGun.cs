using System.Collections.Generic;
using UnityEngine;

public interface ISerializableGun
{
    bool HasProjectilePrefab(string prefabName);
    IEntity CreateProjectile(string prefabName, Vector2 direction, Vector2 position, Transform parent);
}
