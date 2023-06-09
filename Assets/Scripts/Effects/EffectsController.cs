//USING_ZENJECT
using System;
using System.Collections.Generic;
using UnityEngine;
#if USING_ZENJECT
using Zenject;
#endif

public class EffectsController :
#if USING_ZENJECT
    IInitializable,
#else
    MonoBehaviour,
#endif
    IEffectsController
{
    private ILogController logController = null;
    private readonly HashSet<IEntity> effects = new();

#if USING_ZENJECT
    private Effect.Factory effectFactory = null;

    [Inject]
    private void Construct(Effect.Factory effectFactory)
    {
        this.effectFactory = effectFactory;
    }
#endif

#if USING_ZENJECT
    [Inject]
#endif
    private void Construct(ILogController logController)
    {
        this.logController = logController;
    }

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
        if (effectPrefab == null)
        {
            logController.Error(nameof(effectPrefab) + " is null");
            return;
        }
#if USING_ZENJECT
        if (effectFactory == null)
        {
            logController.Error(nameof(effectFactory) + " is null");
            return;
        }
        var effectEntity = effectFactory.Create(effectPrefab, position);
#else
        var effectObject = Instantiate(effectPrefab, position, Quaternion.identity, transform);
        effectObject.name = effectPrefab.name;
        var effectEntity = effectObject.GetComponent<IEntity>();
#endif
        if (effectEntity != null)
        {
            effectEntity.OnDie += OnEffectDie;
            effects.Add(effectEntity);
        }
    }

    private void OnEffectDie(IEntity effect)
    {
        effects.Remove(effect);
    }

#if USING_ZENJECT
    public void Initialize()
    {
        Start();
    }
#endif

    // Start is called before the first frame update
    private void Start()
    {
#if !USING_ZENJECT
        Construct(GetComponent<ILogController>());
#else
        if (effectFactory == null)
            logController.Error(nameof(effectFactory) + " is null");
#endif
    }
}
