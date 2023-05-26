using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour, ISerializableEntity, IEntity
{
    public string Name { get => gameObject.name; }
    public UnityAction<IEntity> OnDie { get; set; }

    public void Kill()
    {
        OnDie?.Invoke(this);
        OnDie = null;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        OnDie?.Invoke(this);
        OnDie = null;
    }
}
