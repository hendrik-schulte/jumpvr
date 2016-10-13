using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrampolinDebug : MonoBehaviour
{
    private GameObject _canvas;
    private Text[] _texts;

    private string _currentStateText = "Standing";
    private string _lastGestureText = "None";
    private string _calibrationText = "Not Calibrated yet";

    private void Start()
    {
        //get GUI elements
        _canvas = FindObjectOfType<Canvas>().gameObject;
        _texts = _canvas.GetComponentsInChildren<Text>();

        //register for trampolin gesture and state events 
        Trampolin.Instance.RegisterOnStateChanged(UpdateStateText);
        Trampolin.Instance.RegisterOnGesture(UpdateGestureText);
        Trampolin.Instance.RegisterOnCalibration(UpdateCalibrationInfo);
    }

    //react to trampolin gesture and state events (set strings)
    private void UpdateGestureText(InteractionGestureEventArgs ig)
    {
        _lastGestureText = ig.PerformedGesture.ToString();
    }

    private void UpdateStateText()
    {
        _currentStateText = Trampolin.Instance.CurrentState.ToString();
    }

    private void UpdateCalibrationInfo()
    {
        _calibrationText = "Calibrated!";
    }

    //display various trampolin information
    private void Update()
    {
        _texts[0].text = "CurrentState: " + _currentStateText;
        _texts[1].text = "Current Jump Height: " + Trampolin.Instance.CurrentJumpHeight;
        _texts[2].text = "Jump Duration: " + Trampolin.Instance.LastJumpDuration;
        _texts[3].text = "Max Jump Height: " + Trampolin.Instance.MaxJumpHeight;
        _texts[4].text = "Min Jump Height: " + Trampolin.Instance.MinJumpHeight;
        _texts[5].text = "Take Off Intensity: " + Trampolin.Instance.TakeOffIntensity;
        _texts[6].text = "User Height: " + Trampolin.Instance.UserHeight;
        _texts[7].text = "Velocity: " + Trampolin.Instance.Velocity;
        _texts[8].text = "Acceleration: " + Trampolin.Instance.Acceleration;
        _texts[9].text = "Last Gesture: " + _lastGestureText;
        _texts[10].text = "Last Jump Height: " + Trampolin.Instance.LastJumpHeight;
        _texts[11].text = "Average Jump Height: " + Trampolin.Instance.AverageJumpHeight;
        _texts[12].text = "Average Jump Duration: " + Trampolin.Instance.AverageJumpDuration;
        _texts[13].text = "LeaningDirection: " + Trampolin.Instance.LeaningDirection;
        _texts[14].text = "HandDifferenceZ: " + Trampolin.Instance.HandDifferenceZ;
        _texts[15].text = "Jump Placement Offset: " + Trampolin.Instance.JumpPlacementOffset;
        _texts[16].text = "WalkStrengthZ: " + Trampolin.Instance.WalkStrengthZ;
        _texts[17].text = "Last Gesture: " + _lastGestureText;
        _texts[18].text = "Calibration State: " + _calibrationText;
    }

    //old GUI implementation
    //void OnGUI()
    //{
        //GUI.Label(new Rect(0, 0, 200, 100), "CurrentState: "+CurrentStateText);
        //GUI.Label(new Rect(0, 15, 200, 100), "Current Jump Height: " + Trampolin.Instance.CurrentJumpHeight);
        //GUI.Label(new Rect(0, 30, 200, 100), "Jump Duration: " + Trampolin.Instance.JumpDuration);
        //GUI.Label(new Rect(0, 45, 200, 100), "Max Jump Height: " + Trampolin.Instance.MaxJumpHeight);
        //GUI.Label(new Rect(0, 60, 200, 100), "Min Jump Height: " + Trampolin.Instance.MinJumpHeight);
        //GUI.Label(new Rect(0, 75, 200, 100), "Take Off Intensity: " + Trampolin.Instance.TakeOffIntensity);
        //GUI.Label(new Rect(0, 90, 200, 100), "User Height: " + Trampolin.Instance.UserHeight);
        //GUI.Label(new Rect(0, 105, 200, 100), "Velocity: " + Trampolin.Instance.Velocity);
        //GUI.Label(new Rect(0, 120, 200, 100), "Acceleration: " + Trampolin.Instance.Acceleration);
        //GUI.Label(new Rect(0, 135, 200, 100), "Last Gesture: " + LastGestureText);
        //GUI.Label(new Rect(0, 150, 200, 100), "Last Jump Height: " + Trampolin.Instance.LastJumpHeight);
        //GUI.Label(new Rect(0, 165, 200, 100), "Average Jump Height: " + Trampolin.Instance.AverageJumpHeight);
        //GUI.Label(new Rect(0, 180, 200, 100), "Average Jump Duration: " + Trampolin.Instance.AverageJumpDuration);
        //GUI.Label(new Rect(0, 195, 200, 100), "LeaningDirection: " + Trampolin.Instance.LeaningDirection);
        //GUI.Label(new Rect(0, 210, 200, 100), "HandDifferenceZ: " + Trampolin.Instance.HandDifferenceZ);
        //GUI.Label(new Rect(0, 225, 200, 100), "Jump Placement Offset: " + Trampolin.Instance.JumpPlacementOffset);
    //}


}
