//USING_ZENJECT
using System;
using UnityEngine;
#if USING_ZENJECT
using Zenject;
#endif

public class SpaceshipController :
#if USING_ZENJECT
    IInitializable, IDisposable,
#else
    MonoBehaviour,
#endif
    ISerializableSpaceshipController
{
    [Serializable]
    public class Settings
    {
        public GameObject SpaceshipPrefab = null;
    }
    public Settings settings = null;

    private Vector2 spaceshipBorders { get; } = new Vector2(10.0f, 10.0f);
    private Vector2 fireDirection { get; } = Vector2.up;
    public ILogController logController = null;
#if USING_ZENJECT
    private Spaceship.Factory spaceshipFactory = null;
#endif
    private IInputController[] inputControllers = null;
    private IControllableEntity spaceship = null;
    private IEntity currentProjectile = null;
    private bool cooldown { get => currentProjectile != null; }

#if USING_ZENJECT
    [Inject]
    private void Construct(Spaceship.Factory spaceshipFactory, Settings settings)
    {
        this.spaceshipFactory = spaceshipFactory;
        this.settings = settings;
    }
#endif

#if USING_ZENJECT
    [Inject]
#endif
    private void Construct(ILogController logController, IInputController[] inputControllers)
    {
        this.logController = logController;
        this.inputControllers = inputControllers;
        if (inputControllers != null)
            foreach (var inputController in inputControllers)
            {
                inputController.OnMove += Move;
                inputController.OnFire += Fire;
            }
    }

    public void Restart()
    {
        Init();
        var initialPosition = new Vector2(0.0f, -spaceshipBorders.y);
        if (spaceship == null)
        {
            if (settings.SpaceshipPrefab == null)
                logController.Error(nameof(settings.SpaceshipPrefab) + " is null");
            else
            {
#if USING_ZENJECT
                spaceship = spaceshipFactory.Create(settings.SpaceshipPrefab, initialPosition);
#else
                var spaceshipObject = Instantiate(settings.SpaceshipPrefab, initialPosition, Quaternion.identity, transform);
                spaceshipObject.name = settings.SpaceshipPrefab.name;
                spaceship = spaceshipObject.GetComponent<IControllableEntity>();
#endif
                if (spaceship == null)
                    logController.Error(nameof(IEntity) + " is null");
                else
                    spaceship.OnDie += OnSpaceshipDie;
            }
        }
        if (spaceship != null)
            spaceship.Position = initialPosition;
    }

    private void Init()
    {
#if !USING_ZENJECT
        if (logController == null)
            Construct(GetComponent<ILogController>(),
                GetComponents<IInputController>());
#else
        if (spaceshipFactory == null)
            logController.Warning(nameof(spaceshipFactory) + " is null");
#endif
        if ((inputControllers == null) || (inputControllers.Length <= 0))
            logController.Warning(nameof(inputControllers) + " is null or empty");
        if (settings == null)
            logController.Warning(nameof(settings) + " is null");
    }

    public IEntity CurrentProjectile
    { 
        get => currentProjectile;
        set => currentProjectile = value;
    }
    public IControllableEntity Spaceship { get => spaceship; }

#if USING_ZENJECT
    public void Initialize()
    {
        Start();
    }

    public void Dispose()
    {
        OnDestroy();
    }
#endif


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void OnDestroy()
    {
        if (inputControllers != null)
            foreach (var ic in inputControllers)
            {
                ic.OnMove -= Move;
                ic.OnFire -= Fire;
            }
        if (spaceship != null)
            spaceship.OnDie -= OnSpaceshipDie;
    }

    public bool IsPositionOutOfBorders(Vector2 position)
    {
        if (position.x < -spaceshipBorders.x)
            return true;
        if (position.x > spaceshipBorders.x)
            return true;
        if (position.y < -spaceshipBorders.y)
            return true;
        if (position.y > spaceshipBorders.y)
            return true;
        return false;
    }

    private void Move(Vector2 offset)
    {
        if (spaceship == null)
            return;
        var position = spaceship.Position;
        position += offset;
        position.x = Mathf.Min(position.x, spaceshipBorders.x);
        position.x = Mathf.Max(position.x, -spaceshipBorders.x);
        position.y = Mathf.Min(position.y, spaceshipBorders.y);
        position.y = Mathf.Max(position.y, -spaceshipBorders.y);
        spaceship.Position = position;
    }

    private void Fire()
    {
        if (spaceship == null)
            return;
        if (cooldown)
            return;
        var gun = spaceship.Gun;
        if (gun == null)
            return;
        currentProjectile = gun.Fire(fireDirection);
        currentProjectile.OnDie += (projectile) => currentProjectile = null;
    }

    private void OnSpaceshipDie(IEntity entity)
    {
        spaceship = null;
        Restart();
    }
}
