using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class GameEvent : UnityEvent { }

public class GameManager : MonoBehaviour
{
    [Serializable]
    public class BuildingSite
    {
        public BuildingSite(Vector3 position)
        {
            Position = position;
        }

        public Vector3 Position;
    }

    public static GameManager Instance;

    [SerializeField]
    [Range(0.1f, 10)]
    private float UpdateInterval = 5;

    public bool DEBUG;


    [SerializeField]
    private Text Text;
    [SerializeField]
    private MainMenu MainMenu { get { return MainMenu.Instance; } }
    [SerializeField]
    private GameObject Camera;
    [SerializeField]
    private GameObject VillagePrefab;
    [SerializeField]
    private GameObject CatapultPrefab;
    [SerializeField]
    private Transform GameModels;
    [SerializeField]
    private Transform BottomModels;
    [SerializeField]
    private GameObject WaypointManager;
    
    public Transform HeadTrackerPosition;

    public int maxNumberVillages;


    //bool[] spawned = new bool[] { false, false, false, false, false, false };
    List<BuildingSite> villagesToSpawn = new List<BuildingSite>();
    List<BuildingSite> villagesSpawned = new List<BuildingSite>();
    List<Destructible> villageObjects = new List<Destructible>();
    bool gamePaused = false;

    private Animator CameraAnimator;

    void Awake()
    {
        if (!Instance) Instance = this;

        Vector3 posFront = new Vector3(0, 0, (float)0.5);
        Vector3 posFrontRight = new Vector3((float)0.5, 0, (float)0.25);
        Vector3 posBackRight = new Vector3((float)0.5, 0, (float)-0.25);
        Vector3 posBack = new Vector3(0, 0, (float)-0.5);
        Vector3 posBackLeft = new Vector3((float)-0.5, 0, (float)-0.25);
        Vector3 posFrontLeft = new Vector3((float)-0.5, 0, (float)0.25);
        villagesToSpawn.Add(new BuildingSite(posFront));
        villagesToSpawn.Add(new BuildingSite(posFrontRight));
        villagesToSpawn.Add(new BuildingSite(posBackRight));
        villagesToSpawn.Add(new BuildingSite(posBack));
        villagesToSpawn.Add(new BuildingSite(posBackLeft));
        villagesToSpawn.Add(new BuildingSite(posFrontLeft));
    }

    void Start()
    {
        CameraAnimator = Camera.GetComponent<Animator>();

        SwipeDetectorGear.Instance.AddSwipeTopListener(Recalibrate);
        SwipeDetectorGear.Instance.AddSwipeLeftListener(ExitToMenu);
        MainMenu.AddStartGameListener(StartGame);

        //		StartCoroutine("Villages");
    }

    IEnumerator Villages()
    {
        yield return new WaitForSeconds(1.5f);

        while (!gamePaused)
        {

            if (villagesToSpawn.Count > 0)
            {
                int newVillage = (int)(UnityEngine.Random.value * villagesToSpawn.Count);
                var temp = villagesToSpawn[newVillage];
                villagesSpawned.Add(villagesToSpawn[newVillage]);
                villagesToSpawn.Remove(villagesToSpawn[newVillage]);


                if (UnityEngine.Random.Range(0f, 1f) < 0.75) createBuilding(temp, VillagePrefab);
                else createBuilding(temp, CatapultPrefab);

            }

            
            yield return new WaitForSeconds(UpdateInterval);
        }
    }

    public void createBuilding(BuildingSite site, GameObject prefab)
    {
        var Village = Instantiate(prefab, site.Position, Quaternion.LookRotation(site.Position), BottomModels).GetComponent<Destructible>();
        Village.transform.localPosition = new Vector3(Village.transform.localPosition.x, 0, Village.transform.localPosition.z);
        villageObjects.Add(Village);
        Village.AddOnDestroyListener(delegate ()
        {
            villageObjects.Remove(Village);

            Timer(2, delegate
            {
                villagesToSpawn.Add(site);
                villagesSpawned.Remove(site);
            });
        });
    }


    public void CalibrationDone()
    {
        MainMenu.Activated();
    }

    public void Recalibrate()
    {
        MainMenu.Deactivated();

        Trampolin.Instance.Recalibrate();
    }

    public void CloseMenu()
    {
        MainMenu.Deactivated();
    }

    public void StartGame()
    {
        MainMenu.Deactivated();

        WaypointManager.SetActive(false);

        StartCoroutine(Villages());
    }


    public void Credits()
    {

    }

    public void Blur()
    {
        CameraAnimator.SetTrigger("Blurry");
    }

    public void ExitToMenu()
    {
        gamePaused = true;

        MainMenu.Activated();

        WaypointManager.SetActive(false);
    }

    private void Timer(float time, UnityAction action)
    {
        StartCoroutine(TimerCR(time, action));
    }

    IEnumerator TimerCR(float time, UnityAction action)
    {
        yield return new WaitForSeconds(time);

        action();
    }
}