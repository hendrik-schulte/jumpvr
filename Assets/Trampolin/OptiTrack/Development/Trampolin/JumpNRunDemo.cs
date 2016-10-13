using UnityEngine;
using System.Collections;
using Level;

/// <summary>
/// Short demonstration of events and runner interaction
/// </summary>
public class JumpNRunDemo : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Threshold necessary to trigger line changing (1=1meter)")]
    private float _xCenterOffsetThreshold=0.2f;

    [SerializeField]
    [Tooltip("Time necessary at the beginning after calibration to speed up")]
    private float _timeSpeedUp = 2.0f;

    //change this value to not directly manipulate the jumpmultiplier (e.g. during midjump), but just as the user will jump up
    public float JumpMultiplierValue { get; set; }

    public Runner runner;
    private float _runnerSpeed;

    void Start()
    {
        Trampolin.Instance.RegisterOnStateChanged(ReactToLanding);
        Trampolin.Instance.RegisterOnCalibration(ReactToCalibration);
        Trampolin.Instance.RegisterOnGesture(ReactToGesture);
        JumpMultiplierValue = 0.0f;
        Trampolin.Instance.JumpMultiplier = JumpMultiplierValue;
        //save the runnerSpeed from Runner script set in the inspector 
        _runnerSpeed = runner.runnerSpeed;
        //...and stop it so it doesn't go ham immedtiately
        runner.runnerSpeed = 0.0f;
    }

    private void ReactToGesture(InteractionGestureEventArgs ig)
    {
        if(ig.PerformedGesture == InteractionGesture.AssCombo)
        {
            runner.AssBomb();
            GetComponent<AudioSource>().Play();
        }
    }

    private void ReactToLanding()
    {
        if (Trampolin.Instance.CurrentState == UserState.Landing)
        {
            float xCenterOffset = Trampolin.Instance.JumpPlacementOffset.x;
            if (Mathf.Abs(xCenterOffset) > _xCenterOffsetThreshold)
            {
                if (xCenterOffset > 0)
                {
                    runner.switchRight = true;
                }
                else
                {
                    runner.switchLeft = true;
                }
            }

            if(Trampolin.Instance.WalkStrengthZ>=0)
            {
                runner.IncreaseSpeed(Trampolin.Instance.WalkStrengthZ*2.0f,0,0);
            }
        }
        //if the multiplier gets changed, it will be applied as the user jumps up, not during the jump immediately
        else if (Trampolin.Instance.CurrentState == UserState.JumpingUp)
        {
            Trampolin.Instance.JumpMultiplier = JumpMultiplierValue;
        }
    }

    private void ReactToCalibration()
    {
        StartCoroutine(StartRunning());
    }

    IEnumerator StartRunning()
    {
        float t = 0;
        while(t<=1.0f)
        {
            runner.runnerSpeed = _runnerSpeed * t;
            t += Time.deltaTime / _timeSpeedUp;
            yield return new WaitForEndOfFrame();
        }
    }

    void Update()
    {

    }
}
