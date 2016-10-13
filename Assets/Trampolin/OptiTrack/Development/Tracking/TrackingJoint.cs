using UnityEngine;
using System.Collections;

public abstract class TrackingJoint : MonoBehaviour 
{
    [SerializeField]
    protected TrackingManager.JointType _jointType;


    public abstract void CalculateOffset();

    public TrackingManager.JointType GetJointType()
    {
        return _jointType;
    }
}
