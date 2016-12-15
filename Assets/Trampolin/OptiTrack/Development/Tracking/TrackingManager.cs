using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public abstract class TrackingManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Select and add joints you want to be active (no double entrys)")]
    private JointType[] _useJoint;

    [SerializeField]
    [Tooltip("Activate or deactivate tracking")]
    private bool _isTrackingActive = true;

    protected bool IsTrackingActive
    {
        get
        {
            return _isTrackingActive;
        }
        set
        {
            _isTrackingActive = value;
            SetAllJoints(_isTrackingActive);
        }
    }

    public enum JointType
    {
        Head,
        LeftFoot,
        LeftHand,
        RightFoot,
        RightHand,
        LeftElbow,
        RightElbow,
        LeftKnee,
        RightKnee,
        Balloon

    }

    protected List<TrackingJoint> _trackingJoints;

    public TrackingJoint Camera { get; private set; }
    public TrackingJoint Head { get; private set; }
    public TrackingJoint RightHand { get; private set; }
    public TrackingJoint RightFoot { get; private set; }
    public TrackingJoint LeftHand { get; private set; }
    public TrackingJoint LeftFoot { get; private set; }
    public TrackingJoint LeftKnee { get; private set; }
    public TrackingJoint RightKnee { get; private set; }
    public TrackingJoint LeftElbow { get; private set; }
    public TrackingJoint RightElbow { get; private set; }
    public TrackingJoint Balloon { get; private set; }

    private Dictionary<JointType, TrackingJoint> _typeToJointObj;

    protected static TrackingManager _instance;

    public static TrackingManager Instance
    {
        get { return _instance; }
    }

    protected void Awake()
    {
        _instance = this;
        _trackingJoints = new List<TrackingJoint>();
        _typeToJointObj = new Dictionary<JointType, TrackingJoint>();

        foreach (TrackingJoint trackingJoint in GetComponentsInChildren<TrackingJoint>())
        {
            JointType currentJointType = trackingJoint.GetJointType();
            _typeToJointObj.Add(currentJointType, null);
            if (Array.IndexOf(_useJoint, currentJointType) != -1)
            {
                _trackingJoints.Add(trackingJoint);
                _typeToJointObj[currentJointType] = trackingJoint;
                //switch (trackingJoint.GetJointType())
                //{
                //    case JointType.Head:
                //        Head = trackingJoint;
                //        break;
                //    case JointType.LeftFoot:
                //        LeftFoot = trackingJoint;
                //        break;
                //    case JointType.LeftHand:
                //        LeftHand = trackingJoint;
                //        break;
                //    case JointType.RightFoot:
                //        RightFoot = trackingJoint;
                //        break;
                //    case JointType.RightHand:
                //        RightHand = trackingJoint;
                //        break;
                //    case JointType.LeftKnee:
                //        LeftKnee = trackingJoint;
                //        break;
                //    case JointType.LeftElbow:
                //        LeftElbow = trackingJoint;
                //        break;
                //    case JointType.RightKnee:
                //        RightKnee = trackingJoint;
                //        break;
                //    case JointType.RightElbow:
                //        RightElbow = trackingJoint;
                //        break;
                //}
            }
        }

        Head = _typeToJointObj[JointType.Head];
        LeftHand = _typeToJointObj[JointType.LeftHand];
        RightHand = _typeToJointObj[JointType.RightHand];
        LeftFoot = _typeToJointObj[JointType.LeftFoot];
        RightFoot = _typeToJointObj[JointType.RightFoot];
        LeftElbow = _typeToJointObj[JointType.LeftElbow];
        RightElbow = _typeToJointObj[JointType.RightElbow];
        LeftKnee = _typeToJointObj[JointType.LeftKnee];
        RightKnee = _typeToJointObj[JointType.RightKnee];
        Balloon = _typeToJointObj[JointType.Balloon];

        SetAllJoints(_isTrackingActive);
    }

    private void OnValidate()
    {
        IsTrackingActive = _isTrackingActive;
    }

    public void CalibrateJoints()
    {
        foreach (TrackingJoint tj in _trackingJoints)
        {
            tj.CalculateOffset();
        }
    }

    public void SetAllJoints(bool active)
    {
        if (Application.isPlaying && _trackingJoints!=null)
        {
            foreach (TrackingJoint tj in _trackingJoints)
            {
                tj.enabled = active;
            }
        }
    }

    public TrackingJoint GetJoint(TrackingManager.JointType jointType)
    {
        return _typeToJointObj[jointType];
    }
}
