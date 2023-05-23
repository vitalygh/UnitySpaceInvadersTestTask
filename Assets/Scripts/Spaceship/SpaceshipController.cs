using UnityEngine;

public class SpaceshipController : MonoBehaviour, ISpaceshipController
{
    public GameObject SpaceshipPrefab = null;

    private Vector2 spaceshipBorders { get; } = new Vector2(10.0f, 10.0f);
    private Vector2 fireDirection { get; } = Vector2.up;
    public ILogController logController = null;
    private IInputController[] inputControllers = null;
    private IControllableEntity spaceship = null;
    private IEntity currentProjectile = null;
    private bool cooldown { get => currentProjectile != null; }

    public void Restart()
    {
        Init();
        var initialPosition = new Vector2(0.0f, -spaceshipBorders.y);
        if (spaceship == null)
        {
            if (SpaceshipPrefab != null)
            {
                var spaceshipObject = Instantiate(SpaceshipPrefab, initialPosition, Quaternion.identity, transform);
                spaceshipObject.name = SpaceshipPrefab.name;
                spaceship = spaceshipObject.GetComponent<IControllableEntity>();
                spaceship.OnDie += OnSpaceshipDie;
                if (spaceship == null)
                    logController.Error(nameof(IEntity) + " is null");
            }
            else
                logController.Error(nameof(SpaceshipPrefab) + " is null");
        }
        if (spaceship != null)
            spaceship.Position = initialPosition;
        if (currentProjectile != null)
            currentProjectile.Kill();
    }

    public void Init()
    {
        logController = GetComponent<ILogController>();
        if (inputControllers == null)
        {
            inputControllers = GetComponents<IInputController>();
            if ((inputControllers == null) || (inputControllers.Length <= 0))
                logController.Warning(nameof(inputControllers) + " is null or empty");
            foreach (var ic in inputControllers)
            {
                ic.OnMove += Move;
                ic.OnFire += Fire;
            }
        }
    }

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
