using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class GameEvent : UnityEvent { }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool DEBUG;


    public Text Text;
    public MainMenu MainMenu { get { return MainMenu.Instance; } }
    public GameObject Camera;
    public GameObject VillagePrefab;
    public Transform GameModels;

    public int maxNumberVillages;


    //bool[] spawned = new bool[] { false, false, false, false, false, false };
    List<Vector3> villagesToSpawn = new List<Vector3>();
    List<Vector3> villagesSpawned = new List<Vector3>();
    List<GameObject> villageObjects = new List<GameObject>();
    bool gamePaused = false;
    bool maxNumberReached = false;

    private Animator CameraAnimator;

    void Awake()
    {
        if (!Instance) Instance = this;
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
        while (!gamePaused)
        {
            if (!maxNumberReached)
            {
                int newVillage = (int)(Random.value * villagesToSpawn.Count);
                Vector3 temp = villagesToSpawn[newVillage];
                villagesSpawned.Add(villagesToSpawn[newVillage]);
                villagesToSpawn.Remove(villagesToSpawn[newVillage]);
                createVillage(temp);
            }

            if(villagesToSpawn.Count == 0)
            {
                maxNumberReached = true;
            }


            yield return new WaitForSeconds(5);
        }
        yield return null;
    }

    public void createVillage(Vector3 position)
    {
        var Village = Instantiate(VillagePrefab, position, Quaternion.identity, GameModels) as GameObject;
        villageObjects.Add(Village);
    }

/*    IEnumerator Villages()
    {
        while (!gamePaused)
        {
            bool noWait = false;
            if (!maxNumberReached)
            {
                //random neue Village setzen
                int newVillage = (int)(Random.value * 5);
                switch (newVillage)
                {
                    case 0:
                        if (spawned[newVillage] == true)
                        {
                            noWait = true;
                        }
                        else
                        {
                            Instantiate(Village, posFront, Quaternion.identity, GameModel);
                            spawned[newVillage] = true;
                        }
                        break;
                    case 1:
                        if (spawned[newVillage] == true)
                        {
                            noWait = true;
                        }
                        else
                        {
                            Instantiate(Village, posFrontRight, Quaternion.identity, GameModel);
                            spawned[newVillage] = true;
                        }
                        break;
                    case 2:
                        if (spawned[newVillage] == true)
                        {
                            noWait = true;
                        }
                        else
                        {
                            Instantiate(Village, posBackRight, Quaternion.identity, GameModel);
                            spawned[newVillage] = true;
                        }
                        break;
                    case 3:
                        if (spawned[newVillage] == true)
                        {
                            noWait = true;
                        }
                        else
                        {
                            Instantiate(Village, posBack, Quaternion.identity, GameModel);
                            spawned[newVillage] = true;
                        }
                        break;
                    case 4:
                        if (spawned[newVillage] == true)
                        {
                            noWait = true;
                        }
                        else
                        {
                            Instantiate(Village, posBackLeft, Quaternion.identity, GameModel);
                            spawned[newVillage] = true;
                        }
                        break;
                    case 5:
                        if (spawned[newVillage] == true)
                        {
                            noWait = true;
                        }
                        else
                        {
                            Instantiate(Village, posFrontLeft, Quaternion.identity, GameModel);
                            spawned[newVillage] = true;
                        }
                        break;
                }
            }
            //Array auszählen
            for (int i = 0; i < spawned.Length; i++)
            {
                int count = 0;
                if (spawned[i] == true)
                {
                    count++;
                }
                if (count >= maxNumberVillages)
                {
                    maxNumberReached = true;
                    Debug.Log("Maximum Number of Villages Reached");
                }
            }
            //Wartezeit zwischen neuen Dörfern
            if (!noWait)
            {
                yield return new WaitForSeconds(10);
            }
            else
            {
                yield return null;
            }
        }

    }*/

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
        
        Vector3 posFront = new Vector3(0, 0, (float)0.5);
        Vector3 posFrontRight = new Vector3((float)0.5, 0, (float)0.25);
        Vector3 posBackRight = new Vector3((float)0.5, 0, (float)-0.25);
        Vector3 posBack = new Vector3(0, 0, (float)-0.5);
        Vector3 posBackLeft = new Vector3((float)-0.5, 0, (float)-0.25);
        Vector3 posFrontLeft = new Vector3((float)-0.5, 0, (float)0.25);
        villagesToSpawn.Add(posFront);
        villagesToSpawn.Add(posFrontRight);
        villagesToSpawn.Add(posBackRight);
        villagesToSpawn.Add(posBack);
        villagesToSpawn.Add(posBackLeft);
        villagesToSpawn.Add(posFrontLeft);
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
    }
}