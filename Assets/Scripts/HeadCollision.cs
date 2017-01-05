using ExtensionMethods;
using UnityEngine;

public class HeadCollision : MonoBehaviour
{
    public LayerMask BlurLayers;


    void OnTriggerEnter(Collider collider)
    {
        if (BlurLayers.ContainsLayer(collider.gameObject.layer))
        {
            GameManager.Instance.Blur();
            Destroy(collider.gameObject);
        }
    }
}