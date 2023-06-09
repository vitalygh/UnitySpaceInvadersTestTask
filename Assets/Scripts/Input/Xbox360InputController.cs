//USING_ZENJECT
using UnityEngine;
using UnityEngine.Events;
#if USING_ZENJECT
using Zenject;
#endif

public class Xbox360InputController :
#if USING_ZENJECT
    ITickable,
#else
    MonoBehaviour,
#endif
    IInputController
{
    private readonly string LeftJoyX = "LeftJoyX";
    private readonly string LeftJoyY = "LeftJoyY";
    private readonly string JoyA = "JoyA";

    private float Sensitivity { get => 10.0f; }
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
        var move = new Vector2(Input.GetAxis(LeftJoyX), -Input.GetAxis(LeftJoyY));
        if (move.magnitude > Mathf.Epsilon)
            OnMove?.Invoke(Sensitivity * Time.deltaTime * move);
        if (Input.GetButton(JoyA))
            OnFire?.Invoke();
    }
}
