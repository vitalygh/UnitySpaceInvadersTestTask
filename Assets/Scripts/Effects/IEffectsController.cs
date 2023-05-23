using UnityEngine;

public interface IEffectsController : IEntityController
{
    void SpawnEffect(GameObject prefab, Vector2 position);
}
