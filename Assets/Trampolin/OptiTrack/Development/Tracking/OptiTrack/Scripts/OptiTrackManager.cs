using UnityEngine;
using OptiTrackManagement;

public class OptiTrackManager : TrackingManager
{
    [SerializeField]
    [Tooltip("Receive packets every frame or not?")]
    private bool _framerateLocked = true;
    //[SerializeField]
    //[Tooltip("Scale Rigidbody movement?")]
    //private float _scale = 1.0f;

    private DirectMulticastSocketClient _dmsClient;

    private static new OptiTrackManager _instance;

    public static new OptiTrackManager Instance
    {
        get { return _instance; }
    }

    new void Awake()
    {
        base.Awake();
        _instance = this;
    }

    void Start()
    {
        Debug.Log("OptiTrackManager: Init");
        _dmsClient = new DirectMulticastSocketClient(this);
        _dmsClient.StartClient();
        Application.runInBackground = true;
    }

    ~OptiTrackManager()
    {
        _dmsClient.Close();
    }

    void OnApplicationQuit()
    {
        _dmsClient.Close();
    }

    void Update()
    {
        //base.Update()
        _dmsClient.Unlock();
    }

    public bool IsFramerateLocked()
    {
        return _framerateLocked;
    }

    public OptiTrackRigidBody GetOptiTrackRigidBody(int rigidbodyIndex)
    {
        if(_dmsClient.IsInit())
        {
            DataStream networkData = _dmsClient.GetDataStream();
            return networkData.getRigidbody(rigidbodyIndex);
        }
        else
        {
            _dmsClient.StartClient();
            return GetOptiTrackRigidBody(rigidbodyIndex);
        }
    }

    public string GetNameById(int rigidbodyIndex)
    {
        if(_dmsClient.IsInit() && rigidbodyIndex >=0 && rigidbodyIndex<200)
        {
            OptiTrackRigidBody otrb = GetOptiTrackRigidBody(rigidbodyIndex);
            if(otrb.used)
            {
                return otrb.name;
            }
        }

        return "#not set";
    }

    public int GetIdByName(string name)
    {
        if(_dmsClient.IsInit())
        {
            DataStream networkData = _dmsClient.GetDataStream();
            return networkData.GetIDByName(name);
        }

        return -1;
    }

    public Vector3 GetPosition(int rigidbodyIndex)
    {
        if(_dmsClient.IsInit())
        {
            Vector3 pos = GetOptiTrackRigidBody(rigidbodyIndex).position;
            pos.x = -pos.x;
            return pos;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Quaternion GetOrientation(int rigidbodyIndex)
    {
        if(_dmsClient.IsInit())
        {
            Quaternion rot = GetOptiTrackRigidBody(rigidbodyIndex).orientation;

            Vector3 euler = rot.eulerAngles;
            rot.eulerAngles = new Vector3(euler.x, -euler.y, -euler.z);
            return rot;
        }
        else
        {
            return Quaternion.identity;
        }
    }

}