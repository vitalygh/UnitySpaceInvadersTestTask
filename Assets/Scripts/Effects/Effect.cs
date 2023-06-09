//USING_ZENJECT
using UnityEngine;
#if USING_ZENJECT
using Zenject;
#endif

public class Effect : Entity
{
    public float lifeTime = 0.25f;
    private float startTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    void UpdateScale()
    {
        var time = Time.time - startTime;
        var p = lifeTime > 0.0f ? time / lifeTime : 1.0f;
        if (p >= 1.0f)
        {
            Kill();
            return;
        }
        p = Mathf.Max(0.0f, 1.0f - p);
        transform.localScale = Vector3.one * p;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScale();
    }

#if USING_ZENJECT
    public class Factory : PlaceholderFactory<Object, Vector3, IEntity> { }
#endif
}
