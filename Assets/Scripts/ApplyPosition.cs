﻿using System.Collections;
using System.Linq.Expressions;
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
            transform.position = TargetTransform.position - new Vector3(0, 0.25f, 0);

            yield return null;
        }
    }
}