using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TrampolinComponents
{
    public class Measurement : MonoBehaviour
    {
        public float Velocity { get; private set; }
        public float Acceleration { get; private set; }
        public float CurrentJumpDuration { get; private set; }
        public float LastJumpDuration { get; private set; }
        public float DifferenceHeadY { get; private set; }
        public float StartHeadY { get; private set; }
        public float CurrentJumpHeight { get; private set; }
        public float LastJumpHeight { get; private set; }
        public float MaxJumpHeight { get; private set; }
        public float MaxJumpDuration { get; private set; }
        public float MinJumpHeight { get; private set; }
        public float MinJumpDuration { get; private set; }
        public float AverageJumpDuration { get { return CalcAverageJumpDuration(); } private set { AverageJumpDuration = value; } }
        public float AverageJumpHeight { get { return CalcAverageJumpHeight(); } private set { AverageJumpHeight = value; } }
        //how many cm did the user go down on the trampolin before jumping up again
        public float TakeOffIntensity { get; private set; }
        public float UserHeight { get; private set; }

        private List<MeasureStatisticPoint> _statisticsList;

        private struct MeasureStatisticPoint
        {
            public float jumpDuration;
            public float jumpHeight;
        }

        private struct MeasurePoint
        {
            public float position;
            public float velocity;
        }
        private MeasurePoint _lastMeasurePoint;
        private float _timeJumpStarted;
        private float _trackingJumpHeight;

        private void Start()
        {
            Trampolin.Instance.RegisterOnStateChanged(ChangedState);

            Velocity = 0;
            Acceleration = 0;
            DifferenceHeadY = 0;
            LastJumpDuration = 0;
            StartHeadY = 0;
            CurrentJumpHeight = 0;
            CurrentJumpDuration = 0;
            LastJumpHeight = 0;
            _trackingJumpHeight = 0;
            TakeOffIntensity = 0;
            _timeJumpStarted = 0;
            UserHeight = 0;
            MaxJumpHeight = float.MinValue;
            MinJumpHeight = float.MaxValue;
            MinJumpDuration = float.MaxValue;
            MaxJumpDuration = float.MinValue;

            _lastMeasurePoint.position = Trampolin.Instance.Head.localPosition.y;
            _lastMeasurePoint.velocity = 0;

            ResetStatistics();
        }

        public void ResetStatistics()
        {
            _statisticsList = new List<MeasureStatisticPoint>();
        }

        private float CalcAverage(bool calcDuration)
        {
            if (_statisticsList != null)
            {
                if (_statisticsList.Count > 0)
                {
                    float sum = 0;
                    foreach (MeasureStatisticPoint msc in _statisticsList)
                    {
                        if(calcDuration)
                        {
                            sum += msc.jumpDuration;
                        }
                        else
                        {
                            sum += msc.jumpHeight;
                        }
    
                    }

                    return sum / _statisticsList.Count;
                }

            }
            return -1;
        }

        public float CalcAverageJumpDuration()
        {
            return CalcAverage(true);
        }

        public float CalcAverageJumpHeight()
        {
            return CalcAverage(false);
        }

        //todo: could be implemented  in the future
        public float EstimateJumpTime()
        {
            if (_statisticsList != null)
            {

            }
            return -1;
        }

        //todo: could be implemented in the future
        public float CalculateUserMass()
        {
            if (_statisticsList != null)
            {

            }
            return -1;
        }

        public void Calibrate()
        {
            UserHeight = MeasureUserHeight();
            StartHeadY = Trampolin.Instance.Head.position.y;
        }

        public float MeasureUserHeight()
        {
            if (Trampolin.Instance.CurrentState == UserState.Standing && Trampolin.Instance.LeftFoot != null && Trampolin.Instance.RightFoot != null)
            {
                float midValueFoots = (Trampolin.Instance.LeftFoot.position.y + Trampolin.Instance.RightFoot.position.y) / 2;
                float differenceHeadFoots = Trampolin.Instance.Head.position.y - midValueFoots;
                //approx. adjustments because trackers are positioned on the foots
                return differenceHeadFoots + 0.07f;
            }
            return -1;
        }

        public void UpdateMeasurements()
        {
            //setting JumpHeight and Jumpduration
            if (Trampolin.Instance.CurrentState == UserState.JumpingUp || Trampolin.Instance.CurrentState == UserState.Falling || Trampolin.Instance.CurrentState == UserState.Floating)
            {
                if (Trampolin.Instance.Head.position.y > StartHeadY)
                {
                    CurrentJumpHeight = Trampolin.Instance.Head.position.y - StartHeadY;
                    CurrentJumpDuration = Time.time - _timeJumpStarted;
                }
            }
            else
            {
                CurrentJumpDuration = 0;
                CurrentJumpHeight = 0;
            }

            //set take off intensity
            if (Trampolin.Instance.CurrentState == UserState.Landing || Trampolin.Instance.CurrentState == UserState.Standing)
            {
                float tempIntensity = StartHeadY - Trampolin.Instance.Head.position.y;
                if (tempIntensity > TakeOffIntensity)
                {
                    TakeOffIntensity = tempIntensity;
                }
            }

            //Measuring velocity & acceleration
            DifferenceHeadY = Trampolin.Instance.Head.localPosition.y - _lastMeasurePoint.position;
            Velocity = (DifferenceHeadY) / (Time.deltaTime);
            Acceleration = (Velocity - _lastMeasurePoint.velocity) / (Time.deltaTime);
            //save measurements for next update
            _lastMeasurePoint.position = Trampolin.Instance.Head.localPosition.y;
            _lastMeasurePoint.velocity = Velocity;
        }
        private void ChangedState()
        {
            if (Trampolin.Instance.CurrentState == UserState.JumpingUp)
            {
                _trackingJumpHeight = 0;
                _timeJumpStarted = Time.time;
            }
            else if (Trampolin.Instance.CurrentState == UserState.Landing)
            {
                LastJumpHeight = _trackingJumpHeight;
                LastJumpDuration = Time.time - _timeJumpStarted;
                MinJumpDuration = Mathf.Min(MinJumpDuration, LastJumpDuration);
                MaxJumpDuration = Mathf.Max(MaxJumpDuration, LastJumpDuration);
                TakeOffIntensity = 0;

                MeasureStatisticPoint msc = new MeasureStatisticPoint();
                msc.jumpDuration = LastJumpDuration;
                msc.jumpHeight = _trackingJumpHeight;
                _statisticsList.Add(msc);
            }
            else if (Trampolin.Instance.CurrentState == UserState.Floating)
            {
                MinJumpHeight = Mathf.Min(MinJumpHeight, CurrentJumpHeight);
                MaxJumpHeight = Mathf.Max(MaxJumpHeight, CurrentJumpHeight);
                _trackingJumpHeight = Mathf.Max(_trackingJumpHeight, CurrentJumpHeight);
            }

        }

    }
}
