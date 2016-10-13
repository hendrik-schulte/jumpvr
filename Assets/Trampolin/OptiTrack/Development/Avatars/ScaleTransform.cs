using UnityEngine;
using System.Collections;

//scales the avatar so it fits the size of the person
public class ScaleTransform : MonoBehaviour
{
    private Transform _head, _rightHand, _leftHand;
    private float _goalHead, _goalRightHand, _goalLeftHand;

    private int _tryCalibrationCounter;

    public GameObject CalibrationErrorText;

	void Start ()
    {
        _head = TrackingManager.Instance.Head.transform;
        _rightHand = TrackingManager.Instance.RightHand.transform;
        _leftHand = TrackingManager.Instance.LeftHand.transform;
        _goalRightHand = _rightHand.localPosition.x;
        _goalLeftHand = _leftHand.localPosition.x;
        _goalHead = _head.localPosition.y;

        _tryCalibrationCounter = 0;
        
	}

    public void CalibrateScale()
    {
        //float xNewScale = _rightHand.localPosition.x / (_goalRightHand / transform.localScale.x);
        float xNewScale = (_rightHand.localPosition.x / (_goalRightHand / transform.localScale.x)) + (_leftHand.localPosition.x / (_goalLeftHand/transform.localScale.x));
        xNewScale = xNewScale / 2;

        //catch misstransformation 'fat' error sometimes occuring
        if (xNewScale >= 1.75f)
        {
            //try it 3 times
            if(_tryCalibrationCounter<3)
            {
                _tryCalibrationCounter++;
                CalibrateScale();
            }
            //still hasn't worked? show message to restart app
            else
            {
                CalibrationErrorText.SetActive(true);
            }
        }
        else
        {
            _rightHand.localPosition += new Vector3(_goalRightHand, 0, 0) - new Vector3(_rightHand.localPosition.x, 0, 0);
            _leftHand.localPosition += new Vector3(_goalLeftHand, 0, 0) - new Vector3(_leftHand.localPosition.x, 0, 0);

            float yNewScale = _head.localPosition.y / (_goalHead / transform.localScale.y);

            //transform.localScale = new Vector3(xNewScale, yNewScale, transform.localScale.z);
            transform.localScale = new Vector3(xNewScale, yNewScale, xNewScale);

            // float yNewScale =
            //(OptiTrackManager.Instance.GetPosition(OptiTrackManager.Instance.GetIdByName(System.Enum.GetName(typeof(TrackingManager.JointType), TrackingManager.JointType.Head))).y-TrackingManager.Instance.Head.transform.parent.position.y) / (_goalHead / transform.localScale.y);
            //_head.position += new Vector3(0, _goalHead, 0) - new Vector3(0, _head.position.y, 0);
        }
    }

}
