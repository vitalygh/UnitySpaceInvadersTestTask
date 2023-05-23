using UnityEngine;

public class InvaderAnimation : MonoBehaviour
{
    public Material[] materials = null;
    private int current = 0;
    private SpriteRenderer spriteRenderer = null;
    private Vector3 position = Vector3.zero;

    public void UpdateFrame()
    {
        if (spriteRenderer == null)
            return;
        if ((materials == null) && (materials.Length <= 0))
            return;
        current = (current + 1) % materials.Length;
        spriteRenderer.material = materials[current];
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - position).sqrMagnitude > Mathf.Epsilon)
        {
            position = transform.position;
            UpdateFrame();
        }
    }
}
