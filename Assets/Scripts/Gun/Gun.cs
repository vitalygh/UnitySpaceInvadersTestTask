using UnityEngine;

public class Gun : MonoBehaviour, IGun
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
        var projectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity, transform.parent);
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
