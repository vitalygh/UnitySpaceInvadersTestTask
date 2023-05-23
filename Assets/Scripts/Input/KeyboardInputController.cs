using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class KeyboardInputController : MonoBehaviour, IInputController
{
    private float Sensitivity { get => 10.0f; }
    private readonly Dictionary<KeyCode, Vector2> keyMove = new Dictionary<KeyCode, Vector2>()
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var kvp in keyMove)
            if (Input.GetKey(kvp.Key))
                OnMove?.Invoke(kvp.Value * Sensitivity * Time.deltaTime);
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl))
            OnFire?.Invoke();
    }
}
