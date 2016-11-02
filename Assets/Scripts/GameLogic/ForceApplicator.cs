using UnityEngine;

public class ForceApplicator : MonoBehaviour
{
//    [SerializeField]
//    private LayerMask CollisionLayers;

    private Collider ForceCollider;

    void Awake()
    {
        ForceCollider = GetComponent<Collider>();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
//        print("Collision with " + collision.gameObject.name);
//        print("Col layer: " + LayerMask.LayerToName(collision.gameObject.layer));
//        print("allowed layer: " + CollisionLayers.value + ", " + LayerMask.LayerToName(CollisionLayers.value));


        if (collision.gameObject.layer != LayerMask.NameToLayer("Destructable")) return;

        print("Hit with force: " + collision.impulse);

        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white, 1f);
        }
    }
}
