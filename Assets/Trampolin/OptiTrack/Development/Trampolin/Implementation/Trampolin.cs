using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Plotter;
using TrampolinComponents;
using UnityEngine.VR;

public delegate void CalibrationHandler();

public class Trampolin : MonoBehaviour
{
    //vars in inspector
    [SerializeField]
    [Tooltip("Every frame difference between two positions. Is this difference > jumpThreshold, State will be Jumping")]
    private float _jumpDifferenceThreshold;
    [SerializeField]
    [Tooltip("cm, how much necessary for it being a jump")]
    private float _jumpRecognitionOffset;
    [SerializeField]
    [Tooltip("cm, how much necessary for it being recognized as landing again")]
    private float _landingOffset;
    [SerializeField]
    [Tooltip("cm difference from last frame, how little necessary for it being recognized as floating in the air and not jumping up anymore")]
    private float _floatingDifferenceThreshold;
    [SerializeField]
    [Tooltip("Time to go from Landing to Standing, when not jumping again")]
    private float _landToStandTime;
    [SerializeField]
    [Tooltip("Key for Calibration. Hit when person is Standing in T-Pose in 0,0,0 and correct direction")]
    private KeyCode _keyToCalibrate = KeyCode.C;
    [SerializeField]
    [Tooltip("Key for Resetting the Statistics, e.g. when some has performed some test jumps")]
    private KeyCode _keyToResetStatistics = KeyCode.R;
    [SerializeField]
    [Tooltip("Starts Calibration automatically after x seconds")]
    private float _secondsTillCalibration = 1.0f;
    [SerializeField]
    [Tooltip("Starts Calibration when in TPose after x seconds")]
    private float _timeInTPoseNeeded = 5.0f;



    [SerializeField]
    private Transform _userContainer;

    private Vector3 _userContainerStart;

    //all joint transforms
    public Transform Head {get; private set;}
    public Transform RightHand {get; private set;}
    public Transform LeftHand {get; private set;}
    public Transform RightFoot {get; private set;}
    public Transform LeftFoot { get; private set; }
    public Transform RightElbow { get; private set; }
    public Transform LeftElbow { get; private set; }
    public Transform RightKnee { get; private set; }
    public Transform LeftKnee { get; private set; }
    //head, hands, foots in a list
    private List<Transform> _joints;

    public float JumpMultiplier { get; set; }

    //vars regarding measurement
    public float Acceleration { get { return _measurement.Acceleration; } }
    public float Velocity { get { return _measurement.Velocity; } }
    public float CurrentJumpDuration { get { return _measurement.CurrentJumpDuration; } }
    public float LastJumpDuration { get { return _measurement.LastJumpDuration; } }
    public float CurrentJumpHeight { get { return _measurement.CurrentJumpHeight; } }
    public float LastJumpHeight { get { return _measurement.LastJumpHeight; } }
    public float MaxJumpHeight { get { return _measurement.MaxJumpHeight; } }
    public float MinJumpHeight { get { return _measurement.MinJumpHeight; } }
    public float MaxJumpDuration { get { return _measurement.MaxJumpDuration; } }
    public float MinJumpDuration { get { return _measurement.MinJumpDuration; } }
    public float AverageJumpHeight { get { return _measurement.AverageJumpHeight; } }
    public float AverageJumpDuration { get { return _measurement.AverageJumpDuration; } }
    public float TakeOffIntensity { get { return _measurement.TakeOffIntensity; } }
    public float UserHeight { get { return _measurement.UserHeight; } }
    //vars regarding interaction
    public Vector3 LeaningDirection { get { return _interaction.LeaningDirection; }}
    public Vector3 JumpPlacementOffset { get { return _interaction.JumpPlacementOffset; }}
    public float HandDifferenceZ { get { return _interaction.HandDifferenceZ; }}
    public float WalkStrengthZ { get { return _interaction.WalkStrengthZ; } }

    //vars regarding statecontroller
    public UserState CurrentState { get { return _stateController.CurrentState; } }
    
    //other vars
    private bool _isCalibrated;
    //private ValuePlotterController vpc;
    private Measurement _measurement;
    private Interaction _interaction;
    private StateController _stateController;

    public event CalibrationHandler CalibrationDone;

    //singleton
    private static Trampolin _instance;
    public static Trampolin Instance
    {
        get { return _instance; }
    }

    private Coroutine _staysInTPose;
    private bool _isInTPose;
    private void Awake()
    {
        JumpMultiplier = 0;
        _instance = this;
        //init trampolin components
        _measurement = gameObject.AddComponent<Measurement>();
        _interaction = gameObject.AddComponent<Interaction>();
        _stateController = gameObject.AddComponent<StateController>();
        UpdateStateControllerVars();
    }

    private void Start()
    {
        //adding joints
        Head = TrackingManager.Instance.Head != null ? TrackingManager.Instance.Head.transform : null;
        LeftFoot = TrackingManager.Instance.LeftFoot != null ? TrackingManager.Instance.LeftFoot.transform : null;
        RightFoot = TrackingManager.Instance.RightFoot != null ? TrackingManager.Instance.RightFoot.transform : null;
        LeftHand = TrackingManager.Instance.LeftHand != null ? TrackingManager.Instance.LeftHand.transform : null;
        RightHand = TrackingManager.Instance.RightHand != null ? TrackingManager.Instance.RightHand.transform : null;
        LeftKnee = TrackingManager.Instance.LeftKnee != null ? TrackingManager.Instance.LeftKnee.transform : null;
        RightKnee = TrackingManager.Instance.RightKnee != null ? TrackingManager.Instance.RightKnee.transform : null;
        LeftElbow = TrackingManager.Instance.LeftElbow != null ? TrackingManager.Instance.LeftElbow.transform : null;
        RightElbow = TrackingManager.Instance.RightElbow != null ? TrackingManager.Instance.RightElbow.transform : null;

        //put joints in list
        _joints = new List<Transform>();
        _joints.Add(Head);
        _joints.Add(RightHand);
        _joints.Add(LeftHand);
        _joints.Add(RightFoot);
        _joints.Add(LeftFoot);
        _joints.Add(RightElbow);
        _joints.Add(LeftElbow);
        _joints.Add(RightKnee);
        _joints.Add(LeftKnee);
        _joints.RemoveAll(item => item == null);

        //init my vars
        _isCalibrated = false;
        RegisterOnStateChanged(ChangedState);
        //vpc = new ValuePlotterController(this.gameObject, new Rect(0, 0, 100, 100), Color.black, Color.white, -30, 30);

        //StartCoroutine(StartCalibration(_secondsTillCalibration));
    }

    IEnumerator StartCalibration(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isCalibrated = true;
        TrackingManager.Instance.CalibrateJoints();
        AvatarManager.Instance.CalibrateAvatars();
        _measurement.Calibrate();
        _interaction.Calibrate();
        InputTracking.Recenter();
        CalibrationDone();
    }

    private void Update()
    {
        if (Input.GetKeyDown(_keyToCalibrate))
        {
            StartCoroutine(StartCalibration(0f));
        }

        if(Input.GetKeyDown(_keyToResetStatistics))
        {
            _measurement.ResetStatistics();
        }

        if (_isCalibrated)
        {
            //add jumpmultiplier
            if (CurrentState == UserState.JumpingUp || CurrentState == UserState.Floating || CurrentState == UserState.Falling)
            {
                _userContainer.position = new Vector3(_userContainer.position.x, _userContainerStart.y + CurrentJumpHeight * JumpMultiplier, _userContainer.position.z);
            }
            //_measurement.UpdateMeasurements();
            _interaction.UpdateInteraction();
            //_stateController.UpdateStates();
        }
        else
        {
            _isInTPose = IsInTPose();
            if (_isInTPose)
            {
                if (_staysInTPose == null)
                {
                    _staysInTPose = StartCoroutine(StaysInTPose());
                }
            }
            else
            {
                if (_staysInTPose != null)
                {
                    StopCoroutine(_staysInTPose);
                    _staysInTPose = null;
                }
            }
        }
    }

    IEnumerator StaysInTPose()
    {
        float counter = 0;
        while (_isInTPose && counter<=_timeInTPoseNeeded)
        {
            counter += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (_isInTPose)
        {
            //Debug.Log("CALIBRATION!");
            StartCoroutine(StartCalibration(0f));
        } 
    }

    private bool IsInTPose()
    {
        //hand rigidbody have to be carried, not layed on the ground
        if(RightHand.localPosition.y > 0.5 && LeftHand.localPosition.y > 0.5)
        {
            //hands need certain distance
            if (Mathf.Abs(RightHand.localPosition.x - LeftHand.localPosition.x) > 1.2)
            {
                //they have to be in nearly same y and z position
                if (Mathf.Abs(RightHand.localPosition.z - LeftHand.localPosition.z) < 0.2 && Mathf.Abs(RightHand.localPosition.y - LeftHand.localPosition.y) < 0.2)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void ChangedState()
    {
        if(CurrentState==UserState.JumpingUp)
        {
            _userContainerStart = new Vector3(0, _userContainer.position.y, 0);
        }
        else if(CurrentState==UserState.Landing)
        {
            _userContainer.position = new Vector3(_userContainer.position.x, _userContainerStart.y, _userContainer.position.z);
        }
    }

    private void FixedUpdate()
    {
        if (_isCalibrated)
        {
            _measurement.UpdateMeasurements();
            //_interaction.UpdateInteraction();
            _stateController.UpdateStatesFixed();
            //vpc.AddValue(_acceleration);
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying && _stateController!=null)
        {
            UpdateStateControllerVars();
        }
    }

    private void UpdateStateControllerVars() 
    {
        _stateController.LandToStandTime = _landToStandTime;
        _stateController.JumpDifferenceThreshold = _jumpDifferenceThreshold;
        _stateController.JumpRecognitionOffset = _jumpRecognitionOffset;
        _stateController.LandingOffset = _landingOffset;
        _stateController.FloatingDifferenceThreshold = _floatingDifferenceThreshold;
    }

    public void RegisterOnStateChanged(ChangedTrampolinStateHandler method)
    {
        _stateController.RegisterOnStateChanged(method);
    }
    public void RegisterOnGesture(PerformedInteractionGestureHandler method)
    {
        _interaction.RegisterOnGesture(method);
    }

    public void RegisterOnCalibration(CalibrationHandler method)
    {
        CalibrationDone += method;
    }

}
