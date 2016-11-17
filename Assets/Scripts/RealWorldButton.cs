using UnityEngine;

public class RealWorldButton : MonoBehaviour
{
    public string Action;

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.layer == LayerMask.NameToLayer("Player"))

        GameManager.Instance.SendMessage(Action);
    }
}