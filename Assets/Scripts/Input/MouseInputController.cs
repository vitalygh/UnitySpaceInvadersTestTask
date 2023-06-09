//USING_ZENJECT
using UnityEngine;
using UnityEngine.Events;
#if USING_ZENJECT
using Zenject;
#endif

public class MouseInputController :
#if USING_ZENJECT
    IInitializable, ITickable,
#else
    MonoBehaviour,
#endif
    IInputController
{
    private bool isEnabled = false;
    private Vector2 position = Vector2.zero;
    private float Sensitivity { get => 3.0f; }
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
        if (Input.touchCount > 0)
            position = Input.mousePosition;
        else
        {
            if (Input.GetMouseButtonDown(1))
                isEnabled = !isEnabled;
            if (!isEnabled)
            {
                position = Input.mousePosition;
                return;
            }
            Vector2 newPosition = Input.mousePosition;
            var delta = newPosition - position;
            if (delta.magnitude > Mathf.Epsilon)
            {
                OnMove?.Invoke(Sensitivity * Time.deltaTime * delta);
                position = newPosition;
            }
            if (Input.GetMouseButton(0))
                OnFire?.Invoke();
        }
    }
}
