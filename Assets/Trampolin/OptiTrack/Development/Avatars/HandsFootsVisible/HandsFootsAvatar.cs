using UnityEngine;
using System.Collections;

//this avatar displays only meshes for hands/foots
public class HandsFootsAvatar : VirtualAvatar
{
    [SerializeField]
    private Transform _rightFootAnchor, _rightHandAnchor, _leftFootAnchor, _leftHandAnchor;

    //private vars and properties will get coupled (private vars for inspector, properties for scripting)
    [SerializeField]
    [Tooltip("Shall Hands/Foots be visible?")]
    private bool _displayFoots, _displayHands;

    public bool DisplayFoots
    {
        get
        {
            return _displayFoots;
        }
        set
        {
            _displayFoots = value;
            SetFoots(_displayFoots);
        }
    }

    public bool DisplayHands
    {
        get
        {
            return _displayHands;
        }
        set
        {
            _displayHands = value;
            SetHands(_displayHands);
        }
    }

    private void Awake()
    {
        AvatarType = AvatarManager.AvatarType.HandsFoots;
    }

    private void Start()
    {
        SetFoots(DisplayFoots);
        SetHands(DisplayHands);
    }

    private void Update()
    {
        if (_displayFoots)
        {
            ApplyPositionRotation(_rightFootAnchor, AvatarManager.Instance.RightFoot);
            ApplyPositionRotation(_leftFootAnchor, AvatarManager.Instance.LeftFoot);
        }

        if (_displayHands)
        {
            ApplyPositionRotation(_rightHandAnchor, AvatarManager.Instance.RightHand);
            ApplyPositionRotation(_leftHandAnchor, AvatarManager.Instance.LeftHand);
        }
    }

    private void SetFoots(bool active)
    {
        _rightFootAnchor.gameObject.SetActive(active);
        _leftFootAnchor.gameObject.SetActive(active);
    }

    private void SetHands(bool active)
    {
        _rightHandAnchor.gameObject.SetActive(active);
        _leftHandAnchor.gameObject.SetActive(active);
    }

    private void ApplyPositionRotation(Transform oldTransform, Transform newTransform)
    {
        oldTransform.position = newTransform.position;
        oldTransform.rotation = newTransform.rotation;
    }

    //if script gets changed apply private vars to properties
    private void OnValidate() 
    {
        DisplayHands = _displayHands;
        DisplayFoots = _displayFoots;
    }
}
