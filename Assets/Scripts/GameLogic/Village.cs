using UnityEngine;

public class Village : ForceReceiver
{
    [SerializeField]
    [Range(0, 50)]
    private float ForceThreshold = 5;

    void Awake()
    {
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public override void OnHit(Vector3 impulse)
    {
        if(impulse.magnitude >= ForceThreshold) Destroy(gameObject);
    }
}
