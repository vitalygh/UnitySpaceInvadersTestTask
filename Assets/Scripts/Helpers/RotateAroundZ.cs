using UnityEngine;

public class RotateAroundZ : MonoBehaviour
{
    public float Speed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation *= Quaternion.Euler(0.0f, 0.0f, Speed);
    }
}
