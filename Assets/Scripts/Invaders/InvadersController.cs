using System.Collections.Generic;
using UnityEngine;

public class InvadersController : MonoBehaviour, IEntityController
{
    public GameObject[] InvaderPrefabs = null;

    private ILogController logController = null;
    private IGameController gameController = null;

    private const int HorizontalCount = 11;

    private Vector2 invaderBorders { get; } = new Vector2(9.0f, 8.0f);
    private Vector2 SpawnDistance { get; } = new Vector2(1.5f, 1.5f);
    private Vector2 ShootDirection { get; } = -Vector2.up;
    private int invadersCount = 0;
    private float MoveTimeoutMax { get; } = 2.0f;
    private float MoveTimeoutMin { get; } = 0.05f;
    private float ShootTimeoutMax { get; } = 3.0f;
    private float ShootTimeoutMin { get; } = 0.75f;

    private float lastShootTime = 0.0f;
    private float lastMoveTime = 0.0f;
    private float invadersMultiplier
    { 
        get => (float)Mathf.Max(0, invadersCount - (gameController?.Level ?? 0)) / (HorizontalCount * InvaderPrefabs.Length);
    }

    private float shootTimeout
    {
        get => ShootTimeoutMin + (ShootTimeoutMax - MoveTimeoutMin) * invadersMultiplier;
    }

    private float moveSpeedHorz { get; } = 0.5f;
    private float moveSpeedVert { get; } = 0.75f;
    private Vector2 moveDirection { get; set; } = Vector2.right;

    private float moveTimeout
    {
        get => MoveTimeoutMin + (MoveTimeoutMax - MoveTimeoutMin) * invadersMultiplier;
    }

    private List<IControllableEntity>[] invaders = new List<IControllableEntity>[HorizontalCount];
    private HashSet<IEntity> projectiles = new HashSet<IEntity>();

    public void CleanUp()
    {
        moveDirection = Vector2.right;
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
                entity.OnDie -= OnInviderDie;
                entity.Kill();
            }
            column.Clear();
        }
    }

    private void SpawnInvaders()
    {
        invadersCount = 0;
        var offset = new Vector2(0.5f * SpawnDistance.x * (HorizontalCount - 1), 0.0f);
        for (var j = 0; j < InvaderPrefabs.Length; j++)
            for (var i = 0; i < HorizontalCount; i++)
            {
                var position = offset;
                position.x -= i * SpawnDistance.x;
                position.y += j * SpawnDistance.y;
                var inviderPrefab = InvaderPrefabs[j];
                if (inviderPrefab == null)
                {
                    logController.Error(nameof(inviderPrefab) + " is null");
                    return;
                }
                var invaderObject = Instantiate(inviderPrefab, position, Quaternion.identity, transform);
                invaderObject.name = InvaderPrefabs[j].name;
                var invader = invaderObject.GetComponent<IControllableEntity>();
                if (invaders[i] == null)
                    invaders[i] = new List<IControllableEntity>();
                invader.OnDie += OnInviderDie;
                invaders[i].Add(invader);
                invadersCount += 1;
            }
    }

    private void OnProjectileDie(IEntity projectile)
    {
        projectiles.Remove(projectile);
    }

    private void OnInviderDie(IEntity entity)
    {
        invadersCount -= 1;
        for (var i = 0; i < HorizontalCount; i++)
            invaders[i].Remove(entity as IControllableEntity);
        if (invadersCount <= 0)
            gameController.Win();
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
        var entity = top[Random.Range(0, top.Count)];
        var projectile = entity.Gun.Fire(ShootDirection);
        projectiles.Add(projectile);
        projectile.OnDie += OnProjectileDie;
    }

    private void UpdatePosition()
    {
        var time = Time.time;
        if (time < lastMoveTime + moveTimeout)
            return;
        lastMoveTime = time;
        var dir = moveDirection;
        var offset = dir * moveSpeedHorz;
        var outOfBounds = false;
        foreach (var column in invaders)
        {
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
            moveDirection = dir;
            offset = dir * moveSpeedHorz * Time.deltaTime;
            //if (moveDirection.x > 0.0f)
                offset.y -= moveSpeedVert;
            outOfBounds = false;
        }
        foreach (var column in invaders)
            foreach (var invader in column)
            {
                var position = invader.Position;
                position += offset;
                if (position.y < -invaderBorders.y)
                    outOfBounds = true;
                invader.Position = position;
            }
        if (outOfBounds)
            gameController.GameOver();
    }

    public void Restart()
    {
        CleanUp();
        SpawnInvaders();
    }


    // Start is called before the first frame update
    void Start()
    {
        logController = GetComponent<ILogController>();
        gameController = GetComponent<IGameController>();
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
            foreach (var invader in column)
                invader.OnDie -= OnInviderDie;
    }
}