using System.Collections;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [Range(0, 10)]
    public float Speed;

    void Awake()
    {
        
    }

    void OnEnable()
    {
        StartCoroutine(UpdateAll());
    }


    IEnumerator UpdateAll()
    {
        while (true)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + Time.deltaTime * Speed, transform.rotation.eulerAngles.z);

            yield return null;
        }
    }
}