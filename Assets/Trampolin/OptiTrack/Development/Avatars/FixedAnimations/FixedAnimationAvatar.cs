using UnityEngine;
using System.Collections;

//this avatar shows the whole body, but plays fixed animations for different events
public class FixedAnimationAvatar : VirtualAvatar
{
    private Animator _animator;

    private void Awake()
    {
        AvatarType = AvatarManager.AvatarType.FixedAnimations;
    }

    private void Start()
    {
        Trampolin.Instance.RegisterOnStateChanged(PlayFixedAnimationsStates);
        Trampolin.Instance.RegisterOnGesture(PlayFixedAnimationsGestures);
    }

    //setting the animator values depending on the current trampolin states, so appropriate
    //animations get played back
    private void PlayFixedAnimationsStates()
    {
        _animator = GetComponent<Animator>();
        if (Trampolin.Instance.CurrentState == UserState.JumpingUp)
        {
            _animator.SetBool("JumpUp", true);
            _animator.SetBool("Landing", false);
        }
        else if (Trampolin.Instance.CurrentState == UserState.Falling)
        {
            _animator.SetBool("JumpUp", false);
        }
        else if (Trampolin.Instance.CurrentState == UserState.Landing)
        {
            _animator.SetBool("Landing", true);
        }
    }

    private void PlayFixedAnimationsGestures(InteractionGestureEventArgs igArgs)
    {
        InteractionGesture performedGesture = igArgs.PerformedGesture;

        if(performedGesture == InteractionGesture.AssCombo)
        {
            //...
            //here you could play some animation depending on the gesture, e.g. an ass bomb, like it is performed in PlayFixedAnimationsState()
        }
    }


}
