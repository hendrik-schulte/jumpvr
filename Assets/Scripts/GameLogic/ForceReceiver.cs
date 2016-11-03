using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class ForceReceiver : MonoBehaviour
{
    public abstract void OnHit(Vector3 impulse);
}
