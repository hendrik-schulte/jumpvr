using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Initializer : MonoBehaviour
{
    [SerializeField]
    private SphereCollider Collider;

    [SerializeField]
    private Transform Prefab;

    [SerializeField]
    [Range(0, 200)]
    private int Amount;

    [SerializeField]
    [Range(0, 30)]
    private float MinDistance;

    [SerializeField]
    [Range(0, 10)]
    private float MinScale = 0.9f;

    [SerializeField]
    [Range(0, 10)]
    private float MaxScale = 1.1f;

    void Awake()
    {
        InsideCircle(Amount);
    }

    private void InsideCircle(int Amount)
    {
        var i = 0;
        var Spread = Collider.radius * transform.lossyScale.x;

        if (Spread < MinDistance) return;


        while (i < Amount)
        {
            var randomPos = Vector3.zero;

            while (randomPos.magnitude < MinDistance)
            {
                randomPos = transform.position + Random.insideUnitSphere * Random.value * Spread;
            }

            //            var randomAngle = Random.Range(0, 360f);
            //            var randomScale = ;

            var instance = Instantiate(Prefab, randomPos, Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up), transform);

            instance.localScale = new Vector3(Random.Range(MinScale, MaxScale), Random.Range(MinScale, MaxScale), Random.Range(MinScale, MaxScale));

            i++;
        }
    }
}