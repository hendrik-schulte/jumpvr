using UnityEngine;

//[ExecuteInEditMode]
public class CameraFacingBillboard : MonoBehaviour
{
    public Camera m_Camera;

//    [SerializeField]
//    [Range(0, 1)]
//    private float RotationFactor = 0.5f;

//    private Quaternion StartingRotation;

    void Start()
    {
//        StartingRotation = transform.rotation;
    }


    void Update()
    {
        //        transform.LookAt(transform.position + m_Camera.transform.rotation * -Vector3.forward,
        //            m_Camera.transform.rotation * Vector3.up);

        transform.LookAt(m_Camera.transform.position, Vector3.up);

//        transform.rotation = Quaternion.Euler(
//            Mathf.Lerp(-180, transform.rotation.eulerAngles.x, RotationFactor),
//            transform.rotation.eulerAngles.y, 
//            transform.rotation.eulerAngles.z);

//        transform.LookAt(m_Camera.transform.position,
//    Quaternion.Slerp(m_Camera.transform.rotation, StartingRotation, RotationFactor) * Vector3.up);
    }
}
