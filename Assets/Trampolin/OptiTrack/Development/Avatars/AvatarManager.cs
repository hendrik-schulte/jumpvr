using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class AvatarManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Which avatartype should be active in the beginning?")]
    private AvatarType _firstActiveAvatar;

    [SerializeField]
    [Tooltip("Key to switch to the next avatar")]
    private KeyCode _switchToNextAvatarKey = KeyCode.N;

    [SerializeField]
    [Tooltip("Key to switch to the next perspective")]
    private KeyCode _switchToNextPerspectiveKey = KeyCode.P;

    [SerializeField]
    [Tooltip("Transform containing childs with different camera perspectives")]
    private Transform _cameraPerspectives;

    [SerializeField]
    [Tooltip("Which perspective is active?")]
    private int _currentPerspectiveIndex = 0;

    public Mesh AvatarMeshComplete, AvatarMeshHeadless;

    public enum AvatarType
    {
        IK,
        HandsFoots,
        Invisible,
        Static,
        FixedAnimations
    }

    public Transform Camera { get; private set; }
    public Transform Head { get; private set; }
    public Transform RightHand { get; private set; }
    public Transform RightFoot { get; private set; }
    public Transform LeftHand { get; private set; }
    public Transform LeftFoot { get; private set; }
    public Transform RightElbow { get; private set; }
    public Transform RightKnee { get; private set; }
    public Transform LeftElbow { get; private set; }
    public Transform LeftKnee { get; private set; }

    private int _currentAvatarIndex;
    private Dictionary<AvatarType, VirtualAvatar> _avatars;
    private ScaleTransform _scaleTransform;

    private static AvatarManager _instance;
    public static AvatarManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {

        Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;

        //add transforms from tracking
        Head = TrackingManager.Instance.Head != null ? TrackingManager.Instance.Head.transform : null;
        LeftFoot = TrackingManager.Instance.LeftFoot != null ? TrackingManager.Instance.LeftFoot.transform : null;
        RightFoot = TrackingManager.Instance.RightFoot != null ? TrackingManager.Instance.RightFoot.transform : null;
        LeftHand = TrackingManager.Instance.LeftHand != null ? TrackingManager.Instance.LeftHand.transform : null;
        RightHand = TrackingManager.Instance.RightHand != null ? TrackingManager.Instance.RightHand.transform : null;
        LeftKnee = TrackingManager.Instance.LeftKnee != null ? TrackingManager.Instance.LeftKnee.transform : null;
        RightKnee = TrackingManager.Instance.RightKnee != null ? TrackingManager.Instance.RightKnee.transform : null;
        LeftElbow = TrackingManager.Instance.LeftElbow != null ? TrackingManager.Instance.LeftElbow.transform : null;
        RightElbow = TrackingManager.Instance.RightElbow != null ? TrackingManager.Instance.RightElbow.transform : null;

        //add all avatars from children to this list
        _avatars = new Dictionary<AvatarType, VirtualAvatar>();
        foreach (VirtualAvatar avatar in GetComponentsInChildren<VirtualAvatar>())
        {
            _avatars.Add(avatar.AvatarType, avatar);
        }

        //let avatars initialize first and deactivate them after short time period
        StartCoroutine(DeactivateAvatars());

        //activate first avatar
        _currentAvatarIndex = (int)_firstActiveAvatar;

        _scaleTransform = gameObject.GetComponent<ScaleTransform>();

        
        SetPerspective();
        //StartCoroutine(SwitchPerspectives());
    }

    //IEnumerator SwitchPerspectives()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(10f);
    //        _currentPerspectiveIndex++;
    //        SetPerspective();
    //    }
    //}

    private void Update()
    {
        if (Input.GetKeyDown(_switchToNextAvatarKey))
        {
            SwitchToNextAvatar();
        }

        if (Input.GetKeyDown(_switchToNextPerspectiveKey))
        {
            _currentPerspectiveIndex++;
            SetPerspective();
        }
    }

    public void CalibrateAvatars()
    {
        _scaleTransform.CalibrateScale();
        GetComponent<ApplySimpleDifference>().Calibrate();
    }

    IEnumerator DeactivateAvatars()
    {
        yield return new WaitForSeconds(0.9f);
        foreach (VirtualAvatar avatar in GetComponentsInChildren<VirtualAvatar>())
        {
            avatar.gameObject.SetActive(false);
        }
        _avatars[_firstActiveAvatar].gameObject.SetActive(true);
    }

    public void SwitchToNextAvatar()
    {
        _avatars[(AvatarType)_currentAvatarIndex].gameObject.SetActive(false);
        _currentAvatarIndex++;
        _currentAvatarIndex = _currentAvatarIndex %_avatars.Count;
        _avatars[(AvatarType)_currentAvatarIndex].gameObject.SetActive(true);
        SetMesh();
    }

    public void SwitchToAvatar(AvatarType avatarType)
    {
        _avatars[(AvatarType)_currentAvatarIndex].gameObject.SetActive(false);
        _avatars[avatarType].gameObject.SetActive(true);
        _currentAvatarIndex = (int)avatarType;
        SetMesh();
    }

    public void SwitchToNextPerspective()
    {
        _currentPerspectiveIndex++;
        SetPerspective();
    }

    public void SetPerspective(int newIndex)
    {
        _currentPerspectiveIndex = newIndex;
        SetPerspective();
    }

    public void SetPerspective()
    {
        _currentPerspectiveIndex = _currentPerspectiveIndex % _cameraPerspectives.childCount;
        Camera.parent.parent.localPosition = _cameraPerspectives.GetChild(_currentPerspectiveIndex).localPosition;
        Camera.parent.parent.localRotation = _cameraPerspectives.GetChild(_currentPerspectiveIndex).localRotation;
        SetMesh();
    }

    public void SetMesh()
    {
        //is it an avatar with body? 
        if(_avatars[(AvatarType)_currentAvatarIndex].transform.FindChild("Body") != null)
        {
            //first person? headless mesh
            if (_currentPerspectiveIndex == 0)
            {
                _avatars[(AvatarType)_currentAvatarIndex].transform.FindChild("Body").GetComponent<SkinnedMeshRenderer>().sharedMesh = AvatarMeshHeadless;
                _avatars[(AvatarType)_currentAvatarIndex].transform.FindChild("Eyes").gameObject.SetActive(false);
            }
            else
            {
                _avatars[(AvatarType)_currentAvatarIndex].transform.FindChild("Body").GetComponent<SkinnedMeshRenderer>().sharedMesh = AvatarMeshComplete;
                _avatars[(AvatarType)_currentAvatarIndex].transform.FindChild("Eyes").gameObject.SetActive(true);
            }
        }

    }

    //changing var inspector, apply new perspective
    private void OnValidate()
    {
        if(Application.isPlaying && Camera!=null)
        {
            SetPerspective(_currentPerspectiveIndex);
        }

    }

}
