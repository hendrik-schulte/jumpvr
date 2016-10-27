using UnityEngine;

public class ForceApplicator : MonoBehaviour
{
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
}
