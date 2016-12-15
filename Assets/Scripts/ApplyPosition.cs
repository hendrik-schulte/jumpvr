using System.Collections;
using UnityEngine;

public class ApplyPosition : MonoBehaviour
{
    public Transform TargetTransform;


    void OnEnable()
    {
        StartCoroutine(UpdateAll());
    }

    IEnumerator UpdateAll()
    {
        while (true)
        {
            transform.position = TargetTransform.position;

            yield return null;
        }
    }
}