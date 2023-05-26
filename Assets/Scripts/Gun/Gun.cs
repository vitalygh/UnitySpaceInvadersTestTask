using UnityEngine;

public class Gun : MonoBehaviour, IGun, ISerializableGun
{
    private ILogController logController = null;
    public GameObject[] projectilePrefabs = null;
    private IEntity shooter = null;

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
        var projectileObject = Instantiate(projectilePrefab, position, Quaternion.identity, parent);
        projectileObject.name = projectilePrefab.name;
        var projectile = projectileObject.GetComponent<IProjectile>();
        projectile.Direction = direction;
        projectile.Shooter = shooter;
        return projectileObject.GetComponent<IEntity>();
    }

    // Start is called before the first frame update
    void Start()
    {
        logController = transform.parent.GetComponent<ILogController>();
        shooter = GetComponent<IEntity>();
    }
}
