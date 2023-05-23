using System.Collections.Generic;
using UnityEngine;

public class EffectsController : MonoBehaviour, IEffectsController
{
    private HashSet<IEntity> effects = new HashSet<IEntity>();

    public void Restart()
    {
        foreach (var effect in effects)
        {
            effect.OnDie -= OnEffectDie;
            effect.Kill();
        }
        effects.Clear();
    }

    public void SpawnEffect(GameObject effectPrefab, Vector2 position)
    {
        if (!enabled)
            return;
        if (effectPrefab == null)
            return;
        var effect = Instantiate(effectPrefab, position, Quaternion.identity, transform);
        effect.name = effectPrefab.name;
        var entity = effect.GetComponent<IEntity>();
        if (entity != null)
        {
            entity.OnDie += OnEffectDie;
            effects.Add(entity);
        }
    }

    private void OnEffectDie(IEntity effect)
    {
        effects.Remove(effect);
    }

    public void Start()
    {
        
    }
}
