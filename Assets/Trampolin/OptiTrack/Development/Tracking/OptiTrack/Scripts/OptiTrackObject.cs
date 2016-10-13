/**
 * Adapted from johny3212
 * Written by Matt Oskamp
 */
using UnityEngine;

public class OptiTrackObject : MonoBehaviour {

    [SerializeField]
    private enum IdentifyType
    {
        ByID,
        ByName
    }
    [SerializeField]
    private IdentifyType _idType = IdentifyType.ByID;
    [SerializeField]
    private string _rigidBodyName;
    [SerializeField]
    private int _rigidBodyIndex;

    void Update()
    {
        if (_idType == IdentifyType.ByName)
        {
            _rigidBodyIndex = OptiTrackManager.Instance.GetIdByName(_rigidBodyName);
        }
        else if (_idType == IdentifyType.ByID)
        {
            _rigidBodyName = OptiTrackManager.Instance.GetNameById(_rigidBodyIndex);
        }

        if (_rigidBodyIndex > -1)
        {
            Vector3 pos = OptiTrackManager.Instance.GetPosition(_rigidBodyIndex);
            Quaternion rot = OptiTrackManager.Instance.GetOrientation(_rigidBodyIndex);

            transform.position = pos;
            transform.rotation = rot;
        }
    }
}
