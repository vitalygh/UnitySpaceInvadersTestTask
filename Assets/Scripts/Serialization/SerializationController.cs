//USING_ZENJECT
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
#if USING_ZENJECT
using Zenject;
#endif

public class SerializationController :
#if !USING_ZENJECT
    MonoBehaviour,
#else
    IInitializable,
#endif
    ISerializationController
{
    private ILogController logController = null;
    private IGameController gameController = null;
    private ISerializableSpaceshipController spaceshipController = null;
    private ISerializableInvadersController invadersController = null;
    private ISerialization<SerializedGame> serialization = null;
    private string Filename { get => Application.persistentDataPath + "/game"; }

#if USING_ZENJECT
    [Inject]
#endif
    private void Construct(ILogController logController,
        IGameController gameController,
        ISerializableSpaceshipController spaceshipController,
        ISerializableInvadersController invadersController,
        ISerialization<SerializedGame> serialization)
    {
        this.logController = logController;
        this.gameController = gameController;
        this.spaceshipController = spaceshipController;
        this.invadersController = invadersController;
        this.serialization = serialization;
    }

    public bool Load()
    {
        Init();
        if (!CheckDependencies())
            return false;
        var path = Filename + "." + serialization.Format;
        SerializedGame loadedGame = null;
        if (!File.Exists(path))
        {
            logController.Notify("Save file doesn't exist");
            return false;
        }
        try
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                loadedGame = serialization.Deserialize(fileStream);
            }
        }
        catch (Exception ex)
        {
            logController.Error("Deserialization failed: " + ex.ToString());
            return false;
        }
        if (loadedGame.Spaceship == null)
        {
            logController.Error(nameof(loadedGame.Spaceship) + " is null");
            return false;
        }
        if (loadedGame.Invaders == null)
        {
            logController.Error(nameof(loadedGame.Invaders) + " is null");
            return false;
        }
        if (loadedGame.Projectiles == null)
        {
            logController.Error(nameof(loadedGame.Projectiles) + " is null");
            return false;
        }
        gameController.HP = loadedGame.HP;
        gameController.Level = loadedGame.Level;
        gameController.Score = loadedGame.Score;
        var idToEntity = new Dictionary<int, IControllableEntity>();
        spaceshipController.Restart();
        spaceshipController.Spaceship.Position = loadedGame.Spaceship.position;
        idToEntity.Add(loadedGame.Spaceship.id, spaceshipController.Spaceship);
        invadersController.CleanUp();
        invadersController.MoveDirection = loadedGame.MoveDirection;
        foreach (var loadedInvader in loadedGame.Invaders)
        {
            var invader = invadersController.CreateInvader(loadedInvader.prefab, loadedInvader.position, loadedInvader.column);
            idToEntity.Add(loadedInvader.id, invader);
        }
        Transform parent =
#if !USING_ZENJECT
            transform;
#else
            null;
#endif
        IEnumerable<GameObject> invaderPrefabs = null;
        foreach (var loadedProjectile in loadedGame.Projectiles)
        {
            if (idToEntity.TryGetValue(loadedProjectile.shooter, out var entity))
            {
                var serializableGun = entity.Gun as ISerializableGun;
                if (serializableGun == null)
                {
                    logController.Error("Cast " + nameof(entity.Gun) + " to " + nameof(ISerializableGun) + " failed");
                    return false;
                }
                var createdProjectile = serializableGun.CreateProjectile(loadedProjectile.prefab, loadedProjectile.direction, loadedProjectile.position, parent);
                if (entity == spaceshipController.Spaceship)
                {
                    spaceshipController.CurrentProjectile = createdProjectile;
                    createdProjectile.OnDie += (projectile) => spaceshipController.CurrentProjectile = null;
                }
                else
                    invadersController.AddProjectile(createdProjectile);
                continue;
            }
            IEntity projectile = null;
            foreach (var column in invadersController.Invaders)
            {
                if (column == null)
                    continue;
                foreach (var invader in column)
                {
                    var serializableGun = invader.Gun as ISerializableGun;
                    if (serializableGun == null)
                    {
                        logController.Error("Cast " + nameof(entity.Gun) + " to " + nameof(ISerializableGun) + " failed");
                        continue;
                    }
                    if (serializableGun.HasProjectilePrefab(loadedProjectile.prefab))
                    {
                        projectile = serializableGun.CreateProjectile(loadedProjectile.prefab, loadedProjectile.direction, loadedProjectile.position, parent);
                        invadersController.AddProjectile(projectile);
                        break;
                    }
                }
                if (projectile != null)
                    break;
            }
            if (projectile == null)
            {
                if (invaderPrefabs == null)
                    invaderPrefabs = invadersController.UniqueInvaderPrefabs;
                foreach (var prefab in invaderPrefabs)
                {
                    if (prefab == null)
                    {
                        logController.Warning("Invader prefabs array contains null");
                        return false;
                    }
                    var controllableEntity = prefab.GetComponent<IControllableEntity>();
                    if (controllableEntity == null)
                    {
                        logController.Error("Prefab \"" + prefab.name + " doesn't contain component " + nameof(IControllableEntity));
                        return false;
                    }
                    var gun = controllableEntity.Gun as ISerializableGun;
                    if (gun == null)
                    {
                        logController.Error("Invader prefab \"" + prefab.name + " doesn't have " + nameof(ISerializableGun));
                        return false;
                    }
                    if (gun.HasProjectilePrefab(loadedProjectile.prefab))
                    {
                        invadersController.CreateTemporaryInvader(prefab, (entity) =>
                        {
                            var serializableGun = entity.Gun as ISerializableGun;
                            if (serializableGun == null)
                            {
                                logController.Error("Cast " + nameof(entity.Gun) + " to " + nameof(ISerializableGun) + " failed");
                                return;
                            }
                            if (serializableGun.HasProjectilePrefab(loadedProjectile.prefab))
                            {
                                projectile = serializableGun.CreateProjectile(loadedProjectile.prefab, loadedProjectile.direction, loadedProjectile.position, parent);
                                invadersController.AddProjectile(projectile);
                                return;
                            }
                        });
                        if (projectile != null)
                            break;
                    }
                }
            }
            if (projectile == null)
            {
                var gun = spaceshipController.Spaceship.Gun as ISerializableGun;
                if (gun == null)
                {
                    logController.Error(nameof(spaceshipController.Spaceship) + " doesn't have " + nameof(ISerializableGun));
                    return false;
                }
                if (gun.HasProjectilePrefab(loadedProjectile.prefab))
                {
                    projectile = gun.CreateProjectile(loadedProjectile.prefab, loadedProjectile.direction, loadedProjectile.position, parent);
                    spaceshipController.CurrentProjectile = projectile;
                    projectile.OnDie += (projectile) => spaceshipController.CurrentProjectile = null;
                    continue;
                }
            }
            if (projectile == null)
            {
                logController.Error("Can't create projectile with prefab name \"" + loadedProjectile.prefab + "\"");
                return false;
            }
        }
        logController.Notify("Game state loaded successful");
        return true;
    }

    public bool Save()
    {
        Init();
        if (!CheckDependencies())
            return false;
        var currentGame = new SerializedGame();
        currentGame.HP = gameController.HP;
        currentGame.Level = gameController.Level;
        currentGame.Score = gameController.Score;
        currentGame.MoveDirection = invadersController.MoveDirection;
        var entityToId = new Dictionary<IEntity, int>();
        var currentId = 0;
        var spaceship = spaceshipController.Spaceship;
        var serializableSpaceship = spaceship as ISerializableEntity;
        if (serializableSpaceship == null)
        {
            logController.Error("Cast " + nameof(spaceship) + " to " + nameof(ISerializableEntity) + " failed");
            return false;
        }
        currentGame.Spaceship = new SerializedControlledEntity()
        {
            prefab = serializableSpaceship.Name,
            position = spaceship.Position,
            id = currentId,
        };
        entityToId.Add(spaceship, currentId++);
        var invaders = invadersController.Invaders;
        var serializedInvaders = new List<SerializedInvader>();
        var columnIndex = 0;
        foreach (var column in invaders)
        {
            if (column == null)
                continue;
            foreach (var invader in column)
            {
                var serializableInvader = invader as ISerializableEntity;
                if (serializableInvader == null)
                {
                    logController.Error("Cast " + nameof(invader) + " to " + nameof(ISerializableEntity) + " failed");
                    return false;
                }
                serializedInvaders.Add(new SerializedInvader()
                {
                    prefab = serializableInvader.Name,
                    position = invader.Position,
                    id = currentId,
                    column = columnIndex,
                });
                entityToId.Add(invader, currentId++);
            }
            columnIndex += 1;
        }
        currentGame.Invaders = serializedInvaders.ToArray();
        var projectiles = new List<SerializedProjectile>();
        if (spaceshipController.CurrentProjectile != null)
        {
            var spaceshipProjectileEntity = spaceshipController.CurrentProjectile as ISerializableEntity;
            if (spaceshipProjectileEntity == null)
            {
                logController.Error("Cast " + nameof(spaceshipController.CurrentProjectile) + " to " + nameof(ISerializableEntity) + " failed");
                return false;
            }
            if (spaceshipProjectileEntity != null)
            {
                var spaceshipProjectile = spaceshipProjectileEntity as ISerializableProjectile;
                if (spaceshipProjectile == null)
                {
                    logController.Error("Cast " + nameof(spaceshipProjectileEntity) + " to " + nameof(ISerializableProjectile) + " failed");
                    return false;
                }
                if (spaceshipProjectileEntity != null)
                    projectiles.Add(new SerializedProjectile()
                    {
                        prefab = spaceshipProjectileEntity.Name,
                        position = spaceshipProjectile.Position,
                        direction = spaceshipProjectile.Direction,
                        shooter = (spaceshipProjectile.Shooter != null) && entityToId.TryGetValue(spaceshipProjectile.Shooter, out var id) ? id : -1
                    });
            }
        }
        var invaderProjectils = invadersController.GetProjectiles();
        foreach (var projectileEntity in invaderProjectils)
        {
            var serializableProjectileEntity = projectileEntity as ISerializableEntity;
            if (serializableProjectileEntity == null)
            {
                logController.Error("Cast " + nameof(projectileEntity) + " to " + nameof(ISerializableEntity) + " failed");
                return false;
            }
            var projectile = projectileEntity as ISerializableProjectile;
            if (projectile == null)
            {
                logController.Error("Cast " + nameof(projectileEntity) + " to " + nameof(ISerializableProjectile) + " failed");
                return false;
            }
            projectiles.Add(new SerializedProjectile()
            {
                prefab = serializableProjectileEntity.Name,
                position = projectile.Position,
                direction = projectile.Direction,
                shooter = (projectile.Shooter != null) && entityToId.TryGetValue(projectile.Shooter, out var id) ? id : -1
            });
        }
        currentGame.Projectiles = projectiles.ToArray();
        var path = Filename + "." + serialization.Format;
        try
        {
            using (var memoryStream = new MemoryStream())
            {
                serialization.Serialize(currentGame, memoryStream);
                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    memoryStream.Position = 0;
                    memoryStream.WriteTo(fileStream);
                    fileStream.Flush();
                }
            }
        }
        catch (Exception ex)
        {
            logController.Error("Serialization failed: " + ex.ToString());
            return false;
        }
        logController.Notify("Game state saved successful");
        return true;
    }

#if USING_ZENJECT
    public void Initialize()
    {
        Start();
    }
#endif

    private bool CheckDependencies()
    {
        if (gameController == null)
        {
            logController.Error(nameof(gameController) + " is null");
            return false;
        }
        if (spaceshipController == null)
        {
            logController.Error(nameof(spaceshipController) + " is null");
            return false;
        }
        if (invadersController == null)
        {
            logController.Error(nameof(invadersController) + " is null");
            return false;
        }
        if (serialization == null)
        {
            logController.Error(nameof(serialization) + " is null");
            return false;
        }
        return true;
    }

    private void Init()
    {
        if (logController != null)
            return;
#if !USING_ZENJECT
        Construct(GetComponent<ILogController>(),
            GetComponent<IGameController>(),
            GetComponent<ISerializableSpaceshipController>(),
            GetComponent<ISerializableInvadersController>(),
            GetComponent<ISerialization<SerializedGame>>());
#endif
    }

    private void Start()
    {
        Init();
        CheckDependencies();
    }
}
