using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public enum InteractionGesture
{
    AssCombo
}

public class InteractionGestureEventArgs : EventArgs
{
    public InteractionGestureEventArgs(InteractionGesture ig)
    {
        PerformedGesture = ig;
    }

    public InteractionGesture PerformedGesture { get; private set; }
}

public delegate void PerformedInteractionGestureHandler(InteractionGestureEventArgs e);

namespace TrampolinComponents
{
    public class Interaction : MonoBehaviour
    {
        //used to detect asscombo
        private float _minLegHeight;

        private float _walkForwardLength;

        //leftright with height difference between foots, forwardbackward with difference foot/head
        public Vector3 LeaningDirection { get { return GetLeaningDirection(); } private set { LeaningDirection = value; } }
        //gives the difference from the start position and the current position from a top view perspective. This way one can
        //analyse where on the trampolin surface the user is jumping, depending on the offset from the middle point where he is calibrating
        public Vector3 JumpPlacementOffset { get { return GetJumpPlacementOffset(); } private set { JumpPlacementOffset = value; } }
        //maybe can be used for rotations?
        public float HandDifferenceZ { get { return GetHandGestureDifference(); } private set { HandDifferenceZ = value; } }
        public float WalkStrengthZ { get; private set; }

        private Vector3 _startPosition;

        //will be used so interactions won't be triggered multiple times during one jump cycle
        private Dictionary<InteractionGesture, bool> _isLockedUntilNextJump;

        public event PerformedInteractionGestureHandler PerformedInteractionGesture;

        private void Start()
        {
            _startPosition = Vector3.zero;
            _minLegHeight = 0.4f;
            _walkForwardLength = 0.4f;
            _isLockedUntilNextJump = new Dictionary<InteractionGesture, bool>();

            foreach (InteractionGesture ig in Enum.GetValues(typeof(InteractionGesture)))
            {
                _isLockedUntilNextJump.Add(ig, false);
            }
            
            Trampolin.Instance.RegisterOnStateChanged(ChangedState);
        }

        public void RegisterOnGesture(PerformedInteractionGestureHandler method)
        {
            PerformedInteractionGesture += method;
        }

        private void OnGesturePerformed(InteractionGestureEventArgs ig)
        {
            _isLockedUntilNextJump[ig.PerformedGesture] = true;
            if(PerformedInteractionGesture != null) PerformedInteractionGesture(ig);
        }

        public void Calibrate()
        {
            _startPosition = Trampolin.Instance.LeftFoot.localPosition + (Trampolin.Instance.RightFoot.localPosition - Trampolin.Instance.LeftFoot.localPosition) / 2;
            _startPosition.y = Trampolin.Instance.Head.position.y;
        }

        public void UpdateInteraction()
        {
            
            //gets greater than 0 when it is higher than walkForwardLength
            WalkStrengthZ = Mathf.Max(Mathf.Abs(Trampolin.Instance.LeftFoot.localPosition.z - Trampolin.Instance.RightFoot.localPosition.z)-_walkForwardLength, 0);
            if (Trampolin.Instance.CurrentState == UserState.Landing || Trampolin.Instance.CurrentState == UserState.Standing)
            {
                //check for ass combo - this needs to be refined when other gestures are added with similar movements.
                if (Trampolin.Instance.Head.localPosition.z+_minLegHeight < (Trampolin.Instance.LeftFoot.localPosition.z+Trampolin.Instance.RightFoot.localPosition.z)/2)
                {
                    if (!_isLockedUntilNextJump[InteractionGesture.AssCombo])
                    {
                        Debug.Log("ass");
                        OnGesturePerformed(new InteractionGestureEventArgs(InteractionGesture.AssCombo));
                    }
                }
            }
            
            //zeit messen wie lang es gedauert hat von linken auf rechten fuß zu wechseln, bool
            //coroutine die darauf wartet das sich links nach oben bewegt, auch wieder mit mindestwert sodass man beine auch anheben muss
            //sie dreht boolean um und andersherum wird geguckt
            
        }

        private void ChangedState()
        {
            if (Trampolin.Instance.CurrentState == UserState.JumpingUp)
            {
                ResetInteractionGestureLock();
            }

            if(Trampolin.Instance.CurrentState == UserState.Landing)
            {

            }
        }

        private void ResetInteractionGestureLock()
        {
            foreach (InteractionGesture ig in Enum.GetValues(typeof(InteractionGesture)))
            {
                _isLockedUntilNextJump[ig] = false;
            }
        }

        private Vector3 GetLeaningDirection()
        {
            if (Trampolin.Instance.CurrentState == UserState.Standing)
            {
                //for raising foots
                float differenceHeightFoots = Trampolin.Instance.LeftFoot.position.y - Trampolin.Instance.RightFoot.position.y;

                //head position from top view perspective in relation to the foot positions
                Vector2 leftFoot = new Vector2(Trampolin.Instance.LeftFoot.localPosition.x, Trampolin.Instance.LeftFoot.localPosition.z);
                Vector2 rightFoot = new Vector2(Trampolin.Instance.RightFoot.localPosition.x, Trampolin.Instance.RightFoot.localPosition.z);
                Vector2 middlePointFootsTopPersp = leftFoot + (rightFoot - leftFoot) / 2;
                return new Vector3(differenceHeightFoots + (Trampolin.Instance.Head.localPosition.x - middlePointFootsTopPersp.x), 0, (Trampolin.Instance.Head.localPosition.z - middlePointFootsTopPersp.y));
            }
            return Vector3.zero;
        }

        private Vector3 GetJumpPlacementOffset()
        {
            Vector3 currentPositionTopView = Trampolin.Instance.LeftFoot.localPosition + (Trampolin.Instance.RightFoot.localPosition - Trampolin.Instance.LeftFoot.localPosition) / 2;
            currentPositionTopView.y = 0;
            Vector3 offset = currentPositionTopView - _startPosition;
            offset.y = 0;
            return offset;
        }

        private float GetHandGestureDifference()
        {
            return Trampolin.Instance.LeftHand.localPosition.z - Trampolin.Instance.RightHand.localPosition.z;
        }

    }

}
