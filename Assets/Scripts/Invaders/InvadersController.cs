//USING_ZENJECT
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if USING_ZENJECT
using Zenject;
#endif

public class InvadersController :
#if USING_ZENJECT
    IInitializable, ITickable, IDisposable,
#else
    MonoBehaviour,
#endif
    IEntityController, ISerializableInvadersController
{
    [Serializable]
    public class Settings
    {
        public GameObject[] InvaderPrefabs = null;
    }

    public Settings settings = null;

#if USING_ZENJECT
    private Invader.Factory invaderFactory = null;
#endif

    private HashSet<GameObject> uniqueInvaderPrefabs = null;
    private ILogController logController = null;
    private IGameController gameController = null;

    private const int HorizontalCount = 11;

    private static Vector2 invaderBorders { get; } = new Vector2(9.0f, 8.0f);
    private static Vector2 SpawnDistance { get; } = new Vector2(1.5f, 1.5f);
    private static Vector2 ShootDirection { get; } = -Vector2.up;
    private static Vector2 TemporaryInvaderSpawnPosition { get; } = new Vector2(0.0f, 2.0f * invaderBorders.y);
    private int invadersCount = 0;
    private float MoveTimeoutMax { get; } = 2.0f;
    private float MoveTimeoutMin { get; } = 0.05f;
    private float ShootTimeoutMax { get; } = 3.0f;
    private float ShootTimeoutMin { get; } = 0.75f;

    private float lastShootTime = 0.0f;
    private float lastMoveTime = 0.0f;
    private float invadersMultiplier
    { 
        get => (float)Mathf.Max(0, invadersCount - (gameController?.Level ?? 0)) / (HorizontalCount * settings.InvaderPrefabs.Length);
    }

    private float shootTimeout
    {
        get => ShootTimeoutMin + (ShootTimeoutMax - MoveTimeoutMin) * invadersMultiplier;
    }

    private float moveSpeedHorz { get; } = 0.5f;
    private float moveSpeedVert { get; } = 0.75f;
    public Vector2 MoveDirection { get; set; } = Vector2.right;

    private float moveTimeout
    {
        get => MoveTimeoutMin + (MoveTimeoutMax - MoveTimeoutMin) * invadersMultiplier;
    }

    private readonly List<IControllableEntity>[] invaders = new List<IControllableEntity>[HorizontalCount];
    private readonly HashSet<IEntity> projectiles = new();

#if USING_ZENJECT
    [Inject]
    private void Construct(Invader.Factory invaderFactory, Settings settings)
    {
        this.invaderFactory = invaderFactory;
        this.settings = settings;
    }
#endif

#if USING_ZENJECT
    [Inject]
#endif
    private void Construct(ILogController logController, IGameController gameController)
    {
        this.logController = logController;
        this.gameController = gameController;
    }

    public void CleanUp()
    {
        MoveDirection = Vector2.right;
        foreach (var projectile in projectiles)
        {
            projectile.OnDie -= OnProjectileDie;
            projectile.Kill();
        }
        projectiles.Clear();
        foreach (var column in invaders)
        {
            if (column == null)
                continue;
            foreach (var entity in column)
            {
                entity.OnDie -= OnInvaderDie;
                entity.Kill();
            }
            column.Clear();
        }
        invadersCount = 0;
    }

    private void SpawnInvaders()
    {
        var offset = new Vector2(0.5f * SpawnDistance.x * (HorizontalCount - 1), 0.0f);
        for (var j = 0; j < settings.InvaderPrefabs.Length; j++)
            for (var i = 0; i < HorizontalCount; i++)
            {
                var position = offset;
                position.x -= i * SpawnDistance.x;
                position.y += j * SpawnDistance.y;
                var invaderPrefab = settings.InvaderPrefabs[j];
                if (invaderPrefab == null)
                {
                    logController.Error(nameof(settings.InvaderPrefabs) + " contains null");
                    return;
                }
                CreateInvader(invaderPrefab, position, i);
            }
    }

    public IEnumerable<GameObject> UniqueInvaderPrefabs
    {
        get
        {
            if (uniqueInvaderPrefabs == null)
            {
                uniqueInvaderPrefabs = new HashSet<GameObject>();
                foreach (var invaderPrefab in settings.InvaderPrefabs)
                    if (invaderPrefab != null)
                        uniqueInvaderPrefabs.Add(invaderPrefab);
            }
            return uniqueInvaderPrefabs;
        }
    }
    public IEnumerable<IEnumerable<IControllableEntity>> Invaders => invaders;

    public IControllableEntity CreateInvader(string prefabName, Vector2 position, int column)
    {
        foreach(var prefab in UniqueInvaderPrefabs)
        {
            if (prefab == null)
                continue;
            if (prefab.name != prefabName)
                continue;
            return CreateInvader(prefab, position, column);
        }
        logController.Error("Invader prefab with name \"" + prefabName + "\" not found");
        return null;
    }

    public void CreateTemporaryInvader(GameObject prefab, UnityAction<IControllableEntity> whileEntityCreated)
    {
        var invader = CreateInvader(prefab, TemporaryInvaderSpawnPosition);
        if (invader != null)
        {
            whileEntityCreated?.Invoke(invader);
            invader.Kill();
        }
    }

    private IControllableEntity CreateInvader(GameObject invaderPrefab, Vector2 position, int column)
    {
        if ((column < 0) || (column >= invaders.Length))
            return null;
        var invader = CreateInvader(invaderPrefab, position);
        if (invader == null)
            return null;
        invader.Position = position;
        if (invaders[column] == null)
            invaders[column] = new List<IControllableEntity>();
        invader.OnDie += OnInvaderDie;
        invaders[column].Add(invader);
        invadersCount += 1;
        return invader;
    }

    private IControllableEntity CreateInvader(GameObject invaderPrefab, Vector2 position)
    {
#if USING_ZENJECT
        var invaderEntity = invaderFactory.Create(invaderPrefab, position);
#else
        var invaderObject = Instantiate(invaderPrefab, position, Quaternion.identity, transform);
        invaderObject.name = invaderPrefab.name;
        var invaderEntity = invaderObject.GetComponent<IControllableEntity>();
#endif
        return invaderEntity;
    }

    public IEnumerable<IEntity> GetProjectiles() => projectiles;

    private void OnProjectileDie(IEntity projectile)
    {
        projectiles.Remove(projectile);
    }

    private void OnInvaderDie(IEntity entity)
    {
        for (var i = 0; i < HorizontalCount; i++)
        {
            var column = invaders[i];
            if (column == null)
                continue;
            if (invaders[i].Remove(entity as IControllableEntity))
            {
                invadersCount -= 1;
                break;
            }
        }
        if (invadersCount <= 0)
            gameController?.Win();
    }

    private void UpdateShoot()
    {
        var time = Time.time;
        if (shootTimeout + lastShootTime > time)
            return;
        lastShootTime = time;
        if (invadersCount <= 0)
            return;
        var top = new List<IControllableEntity>();
        foreach (var column in invaders)
        {
            if (column == null)
                continue;
            if (column.Count <= 0)
                continue;
            top.Add(column[0]);
        }
        if (top.Count <= 0)
            return;
        var entity = top[UnityEngine.Random.Range(0, top.Count)];
        var projectile = entity.Gun.Fire(ShootDirection);
        AddProjectile(projectile);
    }

    public void AddProjectile(IEntity projectile)
    {
        if (projectile == null)
            return;
        projectiles.Add(projectile);
        projectile.OnDie += OnProjectileDie;
    }

    private void UpdatePosition()
    {
        var time = Time.time;
        if (time < lastMoveTime + moveTimeout)
            return;
        lastMoveTime = time;
        var dir = MoveDirection;
        var offset = dir * moveSpeedHorz;
        var outOfBounds = false;
        foreach (var column in invaders)
        {
            if (column == null)
                continue;
            foreach (var invader in column)
            {
                var position = invader.Position;
                if (((position + offset).x > invaderBorders.x) || ((position + offset).x < -invaderBorders.x))
                {
                    outOfBounds = true;
                    break;
                }
            }
            if (outOfBounds)
                break;
        }
        if (outOfBounds)
        {
            dir.x = -dir.x;
            MoveDirection = dir;
            offset = moveSpeedHorz * Time.deltaTime * dir;
            //if (MoveDirection.x > 0.0f)
                offset.y -= moveSpeedVert;
            outOfBounds = false;
        }
        foreach (var column in invaders)
        {
            if (column == null)
                continue;
            foreach (var invader in column)
            {
                var position = invader.Position;
                position += offset;
                if (position.y < -invaderBorders.y)
                    outOfBounds = true;
                invader.Position = position;
            }
        }
        if (outOfBounds)
            gameController?.GameOver();
    }

    public void Restart()
    {
        CleanUp();
        SpawnInvaders();
    }

#if USING_ZENJECT
    public void Initialize()
    {
        Start();
    }

    public void Tick()
    {
        Update();
    }

    public void Dispose()
    {
        OnDestroy();
    }
#endif

    // Start is called before the first frame update
    private void Start()
    {
#if !USING_ZENJECT
        Construct(GetComponent<ILogController>(), GetComponent<IGameController>());
#else
        if (invaderFactory == null)
            logController.Warning(nameof(invaderFactory) + " is null");
#endif
        if (gameController == null)
            logController.Warning(nameof(gameController) + " is null");
        if (settings == null)
            logController.Warning(nameof(settings) + " is null");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateShoot();
        UpdatePosition();
    }

    private void OnDestroy()
    {
        foreach (var column in invaders)
        {
            if (column == null)
                continue;
            foreach (var invader in column)
                invader.OnDie -= OnInvaderDie;
        }
    }
}
