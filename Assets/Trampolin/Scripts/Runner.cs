using UnityEngine;
using System.Collections;

namespace Level
{
    public class Runner : MonoBehaviour
    {
        //public Controller controller;
        public Transform runner;
        public float runnerSpeed = 0f;
        public float preSwitchTime = 0f;
        public float switchSpeed = 0f;
        public bool useSlerp = false;

        public bool switchLeft = false;
        public bool switchRight = false;
        public float footstepSpeedFactor = 1;
        public AudioClip[] footsteps;

        //private RunnerLane[] _lanes;
        private int _currentLaneId = 0;
        private Coroutine _switchCoroutine;
        private AudioSource _audioSource;

        private Coroutine _increaseSpeedCoroutine;
        private bool _increasingSpeed;
        private float _startSpeed;

        void Start()
        {
            _increasingSpeed = false;
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource != null)
            {
                StartCoroutine(PlayFootsteps());
            }
            //GetAndSortRunnerLanes();

            _startSpeed = runnerSpeed;
        }

        void Update()
        {
            //if (switchRight)
            //{
            //    switchRight = false;
            //    SwitchRigth();
            //}
            //if (switchLeft)
            //{
            //    switchLeft = false;
            //    SwitchLeft();
            //}

            //if (runnerSpeed != 0)
            //{
            //    controller.Move(runnerSpeed * Time.deltaTime);
            //}
        }

        //public void SwitchLeft()
        //{
        //    if (_currentLaneId == 0) return;
        //    if (_switchCoroutine == null)
        //    {
        //        _switchCoroutine = StartCoroutine(Switch(true));
        //    }
        //}

        //public void SwitchRigth()
        //{
        //    //if (_currentLaneId == _lanes.Length - 1) return;
        //    if (_switchCoroutine == null)
        //    {
        //        _switchCoroutine = StartCoroutine(Switch(false));
        //    }
        //}

        //private IEnumerator Switch(bool left)
        //{
        //    yield return new WaitForSeconds(preSwitchTime);
        //    float t = 0;
        //    int neighbourId = left ? _currentLaneId - 1 : _currentLaneId + 1;
        //    //RunnerLane neighbour = _lanes[neighbourId];
        //    //RunnerLane current = _lanes[_currentLaneId];
        //    Vector3 pos = current.transform.position;
        //    while (t < 1)
        //    {
        //        if (useSlerp)
        //            pos = Vector3.Slerp(current.transform.position, neighbour.transform.position, t);
        //        else
        //            pos = Vector3.Lerp(current.transform.position, neighbour.transform.position, t);
        //        SetRunnerPosition(pos);
        //        t += switchSpeed * Time.deltaTime;
        //        yield return new WaitForEndOfFrame();
        //    }
        //    SetRunnerPosition(neighbour.transform.position);
        //    _currentLaneId = neighbourId;
        //    _switchCoroutine = null;
        //}

        //private void GetAndSortRunnerLanes()
        //{
        //    _lanes = GetComponentsInChildren<RunnerLane>();
        //    // Bubblesort: the array start with the most left and ends with the most right lane
        //    RunnerLane currentLane, nextLane;
        //    bool isFinished = false;
        //    do
        //    {
        //        isFinished = true;
        //        for (int i = 0; i < _lanes.Length - 1; i++)
        //        {
        //            currentLane = _lanes[i];
        //            nextLane = _lanes[i + 1];
        //            if (nextLane.transform.position.x < currentLane.transform.position.x)
        //            {
        //                // Switch
        //                _lanes[i] = nextLane;
        //                _lanes[i + 1] = currentLane;
        //                isFinished = false;
        //            }
        //        }
        //    } while (!isFinished);
        //    _currentLaneId = _lanes.Length / 2;
        //    SetRunnerPosition(_lanes[_currentLaneId].transform.position);
        //}

        //private void SetRunnerPosition(Vector3 position)
        //{
        //    if (runner != null)
        //    {
        //        runner.position = position;
        //    }
        //}

        private IEnumerator PlayFootsteps()
        {
            AudioClip oldRandomClip = null;
            AudioClip nextRandomClip = null;
            while (true)
            {
                if (footsteps.Length > 0)
                {
                    for (int i = 0; i < 100 && nextRandomClip == oldRandomClip; i++)
                    {
                        nextRandomClip = footsteps[Random.Range(0, footsteps.Length)];
                    }
                    _audioSource.clip = nextRandomClip;
                    _audioSource.Play();
                    oldRandomClip = nextRandomClip;
                }
                yield return new WaitForSeconds(footstepSpeedFactor / (runnerSpeed * Time.deltaTime));
            }
        }

        public void IncreaseSpeed(float speedAmount, float increaseDuration, float holdHighSpeedDuration)
        {
            if(increaseDuration==0.0f)
            {
                //only add when powerup is not active
                if(!_increasingSpeed)
                {
                    runnerSpeed += speedAmount;
                }
                
            }
            else
            {
                if(_increaseSpeedCoroutine!=null)
                {
                    StopCoroutine(_increaseSpeedCoroutine);
                }
                _increasingSpeed = true;
               _increaseSpeedCoroutine = StartCoroutine(IncreaseSpeedCor(speedAmount, increaseDuration, holdHighSpeedDuration));
            }
        }

        public void AssBomb()
        {
            _increasingSpeed = false;
            if(_increaseSpeedCoroutine!=null)
             StopCoroutine(_increaseSpeedCoroutine);
            runnerSpeed = _startSpeed;
        }

        private IEnumerator IncreaseSpeedCor(float speedAmount, float increaseDuration, float holdHighSpeedDuration)
        {
            float currentSpeed = runnerSpeed;
            float newSpeed = currentSpeed + speedAmount;
            float t = 0;
            while (t <= 1.0f)
            {
                runnerSpeed = Mathf.Lerp(currentSpeed, newSpeed, t);
                t += Time.deltaTime / increaseDuration;
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(holdHighSpeedDuration);
            t = 0;
            while (t <= 1.0f)
            {
                runnerSpeed = Mathf.Lerp(newSpeed, currentSpeed, t);
                t += Time.deltaTime / increaseDuration;
                yield return new WaitForEndOfFrame();
            }
            _increasingSpeed = false;
        }
    }
}