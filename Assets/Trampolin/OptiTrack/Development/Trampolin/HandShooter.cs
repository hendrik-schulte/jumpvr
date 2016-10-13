using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Level;

public class HandShooter : MonoBehaviour {

    private bool _inRange;
    public float ActivationRange = 0.1f;
    
    List<Vector3> forwardDirections = new List<Vector3>();
    [SerializeField, Tooltip("Threshold for looking in past and smooth throw direction")]
    int smoothForward = 7;

    private Transform _parent;
    private Runner _runner;
    private ParticleSystem _particleSystem;

	// Use this for initialization
	void Start ()
    {
        _inRange = false;
        _parent = transform.parent;
        transform.parent = null;

        _runner = FindObjectOfType<Runner>();
        _particleSystem = GetComponent<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!_inRange)
        {
            if (Vector3.Distance(Trampolin.Instance.RightHand.position, Trampolin.Instance.LeftHand.position) <= ActivationRange)
            {
                _inRange = true;
                _particleSystem.Play();
            }
        }
        else
        {
            if (Vector3.Distance(Trampolin.Instance.RightHand.position, Trampolin.Instance.LeftHand.position) > ActivationRange)
            {
                _inRange = false;
                _particleSystem.Stop();
            }

            //transform.forward = _parent.forward;

            Vector3 sum = Vector3.zero;
            foreach (Vector3 direction in forwardDirections)
            {
                // sum over all last directions
                sum += direction;
            }
            transform.forward = sum.normalized;

            transform.position = _parent.position;
        }

        if (forwardDirections.Count > smoothForward)
        {
            // remove first added element
            forwardDirections.RemoveAt(0);
        }
        forwardDirections.Add(_parent.forward);

        _particleSystem.startSpeed = 1.2f + _runner.runnerSpeed;
    }
}
