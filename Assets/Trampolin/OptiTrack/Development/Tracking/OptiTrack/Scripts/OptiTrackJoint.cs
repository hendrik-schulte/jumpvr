using UnityEngine;
using System;
using System.Collections;

public class OptiTrackJoint : TrackingJoint
{

    [SerializeField]
    private bool _setRotation, _setPosition;

    private string _rigidBodyName;
    private int _rigidBodyIndex;

    private Vector3 _startPos, _offset;

    private void Awake()
    {
        _startPos = transform.localPosition;
    }

    private void Start()
    {
        _rigidBodyName = Enum.GetName(typeof(TrackingManager.JointType), _jointType);
    }

    public override void CalculateOffset()
    {
        _offset = _startPos - transform.localPosition;
        //get scaled in x and y anyway
        //_offset.x = 0;
        _offset.y = 0;

        //Debug.Log(_offset.z + _rigidBodyName);
    }

    private void Update()
    {
        _rigidBodyIndex = OptiTrackManager.Instance.GetIdByName(_rigidBodyName);
        if (_rigidBodyIndex > -1)
        {
            Vector3 pos = OptiTrackManager.Instance.GetPosition(_rigidBodyIndex);
            Quaternion rot = OptiTrackManager.Instance.GetOrientation(_rigidBodyIndex);

            //consider parents translation and rotation
            if (_setPosition)
            {
                if (_rigidBodyName.Equals(Enum.GetName(typeof(TrackingManager.JointType), TrackingManager.JointType.Head)))
                {
                    transform.localPosition = pos + _offset;
                }
                else
                {
                    transform.localPosition = pos;
                }

                //transform.localPosition = pos;
            }

            if (_setRotation)
            {
                transform.localRotation = rot;
            }
        }
        else
        {
            Debug.LogError("Not finding joint: "+ _rigidBodyName);
        }
    }

}
