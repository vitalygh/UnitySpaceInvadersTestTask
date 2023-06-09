//USING_ZENJECT
using UnityEngine;
#if USING_ZENJECT
using Zenject;
#endif

public class Gun : MonoBehaviour, IGun, ISerializableGun
{
    private ILogController logController = null;
    public GameObject[] projectilePrefabs = null;
    private IEntity shooter = null;

#if USING_ZENJECT
    private Projectile.Factory projectileFactory = null;
    [Inject]
    private void Construct(ILogController logController, Projectile.Factory projectileFactory)
    {
        this.logController = logController;
        this.projectileFactory = projectileFactory;
    }
#endif

    public IEntity Fire(Vector2 direction)
    {
        if ((projectilePrefabs == null) || (projectilePrefabs.Length <= 0))
        {
            logController.Error(nameof(projectilePrefabs) + " is null or empty");
            return null;
        }
        var index = Random.Range(0, projectilePrefabs.Length);
        var projectilePrefab = projectilePrefabs[index];
        if (projectilePrefab == null)
        {
            logController.Error(nameof(projectilePrefab) + " with index " + index + " is null");
            return null;
        }
        return CreateProjectile(projectilePrefab, direction, transform.position, transform.parent);
    }

    public bool HasProjectilePrefab(string prefabName)
    {
        foreach (var projectilePrefab in projectilePrefabs)
            if ((projectilePrefab != null) && (projectilePrefab.name == prefabName))
                return true;
        return false;
    }

    public IEntity CreateProjectile(string prefabName, Vector2 direction, Vector2 position, Transform parent)
    {
        if ((projectilePrefabs == null) || (projectilePrefabs.Length <= 0))
        {
            logController.Error(nameof(projectilePrefabs) + " is null or empty");
            return null;
        }
        foreach (var projectilePrefab in projectilePrefabs)
            if ((projectilePrefab != null) && (projectilePrefab.name == prefabName))
                return CreateProjectile(projectilePrefab, direction, position, parent);
        logController.Error("Projectile prefab with name \"" + prefabName + "\" not found");
        return null;
    }

    private IEntity CreateProjectile(GameObject projectilePrefab, Vector2 direction, Vector2 position, Transform parent)
    {
        if (projectilePrefab == null)
        {
            logController.Error(nameof(projectilePrefab) + " is null");
            return null;
        }
#if USING_ZENJECT
        if (projectileFactory == null)
        {
            logController.Error(nameof(projectileFactory) + " is null");
            return null;
        }
        var projectileEntity = projectileFactory.Create(projectilePrefab, position);
#else
        var projectileObject = Instantiate(projectilePrefab, position, Quaternion.identity, parent);
        projectileObject.name = projectilePrefab.name;
        var projectileEntity = projectileObject.GetComponent<IEntity>();
#endif
        if (projectileEntity == null)
            return null;
        if (projectileEntity is IProjectile projectile)
        {
            projectile.Direction = direction;
            projectile.Shooter = shooter;
            return projectileEntity;
        }
        logController.Error("Failed to create projectile from prefab \"" + projectilePrefab.name + "\"");
        projectileEntity.Kill();
        return null;
    }

    // Start is called before the first frame update
    void Start()
    {
#if !USING_ZENJECT
        logController = transform.parent.GetComponent<ILogController>();
#else        
        if (projectileFactory == null)
            logController.Error(nameof(projectileFactory) + " is null");
#endif
        shooter = GetComponent<IEntity>();
    }
}
