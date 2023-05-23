using UnityEngine;
using UnityEngine.Events;

public class Xbox360InputController : MonoBehaviour, IInputController
{
    private readonly string LeftJoyX = "LeftJoyX";
    private readonly string LeftJoyY = "LeftJoyY";
    private readonly string JoyA = "JoyA";

    private float Sensitivity { get => 10.0f; }
    public UnityAction<Vector2> OnMove { set; get; }
    public UnityAction OnFire { set; get; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var move = new Vector2(Input.GetAxis(LeftJoyX), -Input.GetAxis(LeftJoyY));
        if (move.magnitude > Mathf.Epsilon)
            OnMove?.Invoke(move * Sensitivity * Time.deltaTime);
        if (Input.GetButton(JoyA))
            OnFire?.Invoke();
    }
}
