using UnityEngine;
using UnityEngine.Events;

public class MouseInputController : MonoBehaviour, IInputController
{
    private bool isEnabled = true;
    private Vector2 position = Vector2.zero;
    private float Sensitivity { get => 3.0f; }
    public UnityAction<Vector2> OnMove { set; get; }
    public UnityAction OnFire { set; get; }

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
                return;
            Vector2 newPosition = Input.mousePosition;
            var delta = newPosition - position;
            if (delta.magnitude > Mathf.Epsilon)
            {
                OnMove?.Invoke(delta * Sensitivity * Time.deltaTime);
                position = newPosition;
            }
            if (Input.GetMouseButton(0))
                OnFire?.Invoke();
        }
    }
}
