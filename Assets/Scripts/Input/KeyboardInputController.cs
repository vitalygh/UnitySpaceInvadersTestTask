//USING_ZENJECT
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
#if USING_ZENJECT
using Zenject;
#endif

public class KeyboardInputController:
#if USING_ZENJECT
    ITickable,
#else
    MonoBehaviour,
#endif
    IInputController
{
    private float Sensitivity { get => 10.0f; }
    private readonly Dictionary<KeyCode, Vector2> keyMove = new ()
    {
        { KeyCode.W, Vector2.up },
        { KeyCode.S, -Vector2.up },
        { KeyCode.A, -Vector2.right },
        { KeyCode.D, Vector2.right },

        { KeyCode.UpArrow, Vector2.up },
        { KeyCode.DownArrow, -Vector2.up },
        { KeyCode.LeftArrow, -Vector2.right },
        { KeyCode.RightArrow, Vector2.right },
    };

    public UnityAction<Vector2> OnMove { set; get; }
    public UnityAction OnFire { set; get; }

#if USING_ZENJECT
    public void Tick()
    {
        Update();
    }
#endif

    // Update is called once per frame
    void Update()
    {
        foreach (var kvp in keyMove)
            if (Input.GetKey(kvp.Key))
                OnMove?.Invoke(Sensitivity * Time.deltaTime * kvp.Value);
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl))
            OnFire?.Invoke();
    }
}
