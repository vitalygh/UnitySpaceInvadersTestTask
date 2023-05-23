using UnityEngine;

public class ScaleXY : MonoBehaviour
{
    public float MaxScale = 0.4f;
    public float MinScale = 0.2f;
    public float Speed = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var time = Time.time;
        var value = MinScale + (MaxScale - MinScale) * Mathf.Abs(Mathf.Sin(Speed * time));
        transform.localScale = Vector2.one * value;
    }
}
