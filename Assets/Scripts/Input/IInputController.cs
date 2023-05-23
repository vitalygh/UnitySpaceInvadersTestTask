using UnityEngine;
using UnityEngine.Events;

public interface IInputController
{
    UnityAction<Vector2> OnMove { set; get; }
    UnityAction OnFire { set; get; }
}
