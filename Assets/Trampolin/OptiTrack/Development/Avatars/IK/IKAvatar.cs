using UnityEngine;

public class IKAvatar : VirtualAvatar
{
    [SerializeField]
    private bool _isIKActive = false;

    //here, you could maybe try different weight values for the hints,
    //other values might provide a better solution
    //However, note that we experienced a worse performance with hints than without any hints!
    [SerializeField]
    [Tooltip("If you use IKHints (Knees,Elbows), how strong should the weight be (0-1)?")]
    private float _IKHintWeight = 0.5f;

    private Animator _animator;

    [SerializeField]
    private bool DebugLog = false;

    void Awake()
    {
        AvatarType = AvatarManager.AvatarType.IK;
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void OnAnimatorIK()
    {
        //if the IK is active, set the position and rotation directly to the goal. 
        if (_isIKActive)
        {
            SetIK(AvatarIKGoal.LeftFoot, AvatarManager.Instance.LeftFoot);
            SetIK(AvatarIKGoal.RightFoot, AvatarManager.Instance.RightFoot);
            SetIK(AvatarIKGoal.LeftHand, AvatarManager.Instance.LeftHand);
            SetIK(AvatarIKGoal.RightHand, AvatarManager.Instance.RightHand);
            _animator.SetLookAtPosition(AvatarManager.Instance.Head.position + AvatarManager.Instance.Head.forward);
            _animator.SetLookAtWeight(1);

            SetIKHint(AvatarIKHint.LeftElbow, AvatarManager.Instance.LeftElbow);
            SetIKHint(AvatarIKHint.RightElbow, AvatarManager.Instance.RightElbow);
            SetIKHint(AvatarIKHint.LeftKnee, AvatarManager.Instance.LeftKnee);
            SetIKHint(AvatarIKHint.RightKnee, AvatarManager.Instance.RightKnee);
        }

        //if the IK is not active, set the position and rotation of the hand and head back to the original position
        else
        {
            SetIK(AvatarIKGoal.LeftFoot, null);
            SetIK(AvatarIKGoal.RightFoot, null);
            SetIK(AvatarIKGoal.LeftHand, null);
            SetIK(AvatarIKGoal.RightHand, null);

            SetIKHint(AvatarIKHint.LeftElbow, null);
            SetIKHint(AvatarIKHint.RightElbow, null);
            SetIKHint(AvatarIKHint.LeftKnee, null);
            SetIKHint(AvatarIKHint.RightKnee, null);

            _animator.SetLookAtWeight(0);
        }

    }

    private void SetIK(AvatarIKGoal ikLink, Transform goal)
    {
        if (goal != null)
        {
            _animator.SetIKPosition(ikLink, goal.position);
            _animator.SetIKRotation(ikLink, goal.rotation);
            _animator.SetIKPositionWeight(ikLink, 1);
            _animator.SetIKRotationWeight(ikLink, 1);
        }
        else
        {
            _animator.SetIKPositionWeight(ikLink, 0);
            _animator.SetIKRotationWeight(ikLink, 0);
        }
    }

    private void SetIKHint(AvatarIKHint ikHint, Transform goal)
    {
        if (goal != null)
        {
            _animator.SetIKHintPosition(ikHint, goal.position);
            _animator.SetIKHintPositionWeight(ikHint, _IKHintWeight);
        }
        else
        {
            _animator.SetIKHintPositionWeight(ikHint, 0);
        }
    }
}