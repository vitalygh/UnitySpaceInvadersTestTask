//USING_ZENJECT
using UnityEngine;
using UnityEngine.Events;
#if USING_ZENJECT
using Zenject;
#endif

public class TouchInputController :
#if USING_ZENJECT
    IInitializable, ITickable, 
#else
    MonoBehaviour,
#endif
    IInputController

{
    private int fingerId = -1;
    private Vector2 position = Vector2.zero;
    private float Sensitivity { get => 2.0f; }
    public UnityAction<Vector2> OnMove { set; get; }
    public UnityAction OnFire { set; get; }

#if USING_ZENJECT
    public void Initialize()
    {
        Start();
    }

    public void Tick()
    {
        Update();
    }
#endif

    // Start is called before the first frame update
    void Start()
    {
        position = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount <= 0)
            fingerId = -1;
        else
        {
            var touch = Input.GetTouch(0);
            if ((touch.phase == TouchPhase.Began) || (touch.fingerId != fingerId))
            {
                position = touch.position;
                fingerId = touch.fingerId;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                var newPosition = touch.position;
                var delta = newPosition - position;
                if (delta.magnitude > Mathf.Epsilon)
                {
                    OnMove?.Invoke(Sensitivity * Time.deltaTime * delta);
                    position = newPosition;
                }
            }
            if (Input.touchCount > 1)
                OnFire?.Invoke();
        }
    }
}
