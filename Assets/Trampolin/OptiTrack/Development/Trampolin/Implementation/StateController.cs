using UnityEngine;
using System.Collections;
using System;

public enum UserState
{
    Standing,
    JumpingUp,
    Floating,
    Falling,
    Landing
}
public delegate void ChangedTrampolinStateHandler();

namespace TrampolinComponents
{
    public class StateController : MonoBehaviour
    {

        public UserState CurrentState { get; private set; }
        public float LandToStandTime { get; set; }
        public float JumpDifferenceThreshold { get; set; }
        public float JumpRecognitionOffset { get; set; }
        public float LandingOffset { get; set; }
        public float FloatingDifferenceThreshold { get; set; }

        public event ChangedTrampolinStateHandler ChangedState;
        private Coroutine _landingCoroutine;
        private Measurement _measurement;

        private void Awake()
        {
            _measurement = GetComponent<Measurement>();
        }

        private void Start()
        {
            OnStateChanged(UserState.Standing);
        }

        private IEnumerator Landing()
        {
            yield return new WaitForSeconds(LandToStandTime);
            OnStateChanged(UserState.Standing);
        }

        public void RegisterOnStateChanged(ChangedTrampolinStateHandler method)
        {
            ChangedState += method;
        }

        public void OnStateChanged(UserState newState)
        {
            CurrentState = newState;
            ChangedState();
        }

        //this method is based on jumping offset and calibrated starting height
        public void UpdateStatesFixed()
        {
            //user higher than his calibrated position + offset
            if (Trampolin.Instance.Head.position.y > (_measurement.StartHeadY + JumpRecognitionOffset))
            {
                if ((CurrentState == UserState.Standing || CurrentState == UserState.Landing))
                {
                    if (_landingCoroutine != null)
                    {
                        StopCoroutine(_landingCoroutine);
                    }
                    OnStateChanged(UserState.JumpingUp);
                }


            }

            if (Trampolin.Instance.Head.position.y > (_measurement.StartHeadY + LandingOffset))
            {
                if (CurrentState == UserState.JumpingUp && _measurement.DifferenceHeadY < FloatingDifferenceThreshold)
                {
                    OnStateChanged(UserState.Floating);
                }

                if (Mathf.Abs(_measurement.DifferenceHeadY) > FloatingDifferenceThreshold && CurrentState == UserState.Floating)
                {
                    OnStateChanged(UserState.Falling);
                }
            
            }

            if(Trampolin.Instance.Head.position.y < (_measurement.StartHeadY + LandingOffset))
            { 
                if (CurrentState == UserState.Falling)
                {
                    OnStateChanged(UserState.Landing);
                    _landingCoroutine = StartCoroutine(Landing());
                }
            }

            //neu notfall absicherung
            //if (Trampolin.Instance.Head.position.y <= (_measurement.StartHeadY + JumpRecognitionOffset) && CurrentState == TrampolinState.Floating)
            //{
            //    OnStateChanged(TrampolinState.Falling);
            //}
        }

        //this state changing method is based on the assumption of velocity, that the player is jumping or falling
        //when his position changes (difference) enough
        //this needs to be called from fixedupdate though
        //public void UpdateStatesFixed()
        //{
        //    //going up
        //    if (_measurement.DifferenceHeadY >= JumpDifferenceThreshold)
        //    {
        //        if (CurrentState == TrampolinState.Standing || CurrentState == TrampolinState.Landing)
        //        {
        //            if (_landingCoroutine != null)
        //            {
        //                StopCoroutine(_landingCoroutine);
        //            }
        //            OnStateChanged(TrampolinState.JumpingUp);
        //        }
        //    }
        //    //going down
        //    else if (_measurement.DifferenceHeadY <= -JumpDifferenceThreshold)
        //    {
        //        if (CurrentState == TrampolinState.Floating)
        //        {
        //            OnStateChanged(TrampolinState.Falling);
        //        }
        //    }
        //    else
        //    {
        //        if (CurrentState == TrampolinState.JumpingUp)
        //        {
        //            //max reached
        //            //Debug.Log("MaxReached, nearly");
        //            OnStateChanged(TrampolinState.Floating);
        //        }
        //        else if (CurrentState == TrampolinState.Falling)
        //        {
        //            //low reached
        //            OnStateChanged(TrampolinState.Landing);
        //            _landingCoroutine = StartCoroutine(Landing());
        //        }
        //    }
        //}

    }
}
