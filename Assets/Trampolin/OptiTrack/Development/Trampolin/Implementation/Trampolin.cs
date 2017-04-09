using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TrampolinComponents;
using UnityEngine.VR;
using UnityEngine.UI;

public delegate void CalibrationHandler();
[RequireComponent(typeof(AudioSource))]
public class Trampolin : MonoBehaviour
{
    public GameObject tPoseDummy; //Mesh that shows the T-Pose the user should perform
    public Text tPoseText; //Used to display the instructions
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
    private float _secondsTillCalibration = 5.0f;
    [SerializeField]
    [Tooltip("Starts Calibration when in TPose after x seconds")]
    private float _timeInTPoseNeeded = 3.0f;
    [SerializeField, Tooltip("Sound for calibration start")]
    AudioClip _initialize;
    [SerializeField, Tooltip("Sound for calibration start")]
    AudioClip _goIntoTPose;
    [SerializeField, Tooltip("Sound for calibration start")]
    AudioClip _startCalibrationClip;
    [SerializeField, Tooltip("Sound for calibration finish")]
    AudioClip _finishCalibrationClip;
    [SerializeField, Tooltip("Voice output for calibration finish")]
    AudioClip _finishCalibrationVoice;
    [SerializeField, Tooltip("Seconds before auto calibration")]
    private float _preDelay = 10.0f;
    AudioSource _audioSource;
    private bool _calibrationStarted = false;

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
    private bool scalingEnabled = true;
    

    private void Awake()
    {
        
        if (_instance == null)
        {
//            DontDestroyOnLoad(transform.gameObject);
            _instance = this;
        }
        else if (_instance != this)
        {
            //var tPointer = *trampolin;
            Destroy(this.gameObject);
        }


        JumpMultiplier = 0;
        //_instance = this;
        _audioSource = GetComponent<AudioSource>();
        //init trampolin components
        _measurement = gameObject.AddComponent<Measurement>();
        _interaction = gameObject.AddComponent<Interaction>();
        _stateController = gameObject.AddComponent<StateController>();
        UpdateStateControllerVars();
        
        
        
    }

    private void Start()
    {
        //init my vars
        _isCalibrated = false;
        RegisterOnStateChanged(ChangedState);
        //vpc = new ValuePlotterController(this.gameObject, new Rect(0, 0, 100, 100), Color.black, Color.white, -30, 30);
        StartCoroutine(VoiceGuidance());
        StartCoroutine(WaitAndStartCalibration(_preDelay, _secondsTillCalibration));
    }


    IEnumerator VoiceGuidance()
    {
        PlayAudioClip(_initialize);
        yield return new WaitForSeconds(_initialize.length);
        PlayAudioClip(_goIntoTPose);
    }

    IEnumerator WaitAndStartCalibration(float waitForSeconds, float calibrationDelay)
    {
        tPoseDummy.SetActive(true);
        tPoseText.text = "Nehmen Sie die gezeigte T-Pose ein.";
        //yield return new WaitForSeconds(3);
        //
        //yield return new WaitForSeconds(3);
        yield return new WaitForSeconds(waitForSeconds);
        /*
        if (!_isCalibrated)
            StartCoroutine(StartCalibration(calibrationDelay));

        */
        tPoseText.text = "";
        tPoseDummy.SetActive(false);
        GameManager.Instance.CalibrationDone();
    }

    public void Recalibrate()
    {
        scalingEnabled = false;
        StartCoroutine(StartCalibration());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator StartCalibration(float delay = 0)
    {
        Debug.Log("StartCalibration() "+ _calibrationStarted);
        if (!_calibrationStarted)
        {
            //tPoseDummy.SetActive(true);
            _calibrationStarted = true;
            Debug.Log("Start Calibration");
            tPoseText.text = "Halten Sie die T-Pose.";

            PlayAudioClip(_startCalibrationClip);
        
            yield return new WaitForSeconds(_startCalibrationClip.length);
            
            yield return new WaitForSeconds(delay);
            

            _isCalibrated = true;
           
            TrackingManager.Instance.CalibrateJoints();
            AvatarManager.Instance.CalibrateAvatars(scalingEnabled);
            _measurement.Calibrate();
            _interaction.Calibrate();
            InputTracking.Recenter();
            Debug.Log("grea-uuuht success! (Calibrated)");
            //PlayAudioClip(_finishCalibrationClip);
            if(CalibrationDone != null) CalibrationDone();
            yield return new WaitForSeconds(_finishCalibrationClip.length);
            //PlayAudioClip(_finishCalibrationVoice);
            
            _calibrationStarted = false;
            tPoseText.text = "";
            GameManager.Instance.CalibrationDone();
            tPoseDummy.SetActive(false);
        }
        
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
            Debug.Log("Tpose erkannt");
            if(!_calibrationStarted)
                StartCoroutine(StartCalibration(2f));
        } 
    }

    private bool IsInTPose()
    {
        //hand rigidbody have to be carried, not layed on the ground


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

    void PlayAudioClip(AudioClip clip)
    {
        if(clip != null)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }

    public bool IsCalibrated()
    {
        return _isCalibrated;
    }

    public void ReCalibrate()
    {
        if (!_calibrationStarted)
        {
            Debug.Log("Recalibrate()");
            
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
            _isCalibrated = false;

            scalingEnabled = false;

            StartCoroutine(VoiceGuidance());
            StartCoroutine(WaitAndStartCalibration(_preDelay, _secondsTillCalibration)); 
        }
    }
}
