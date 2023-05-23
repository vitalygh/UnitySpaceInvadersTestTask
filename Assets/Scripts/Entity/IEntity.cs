using UnityEngine.Events;

public interface IEntity
{
    void Kill();
    UnityAction<IEntity> OnDie { get; set; }
}
