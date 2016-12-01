using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class GameEvent : UnityEvent { }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    public Text Text;
    public MainMenu MainMenu { get { return MainMenu.Instance; } }
    public GameObject Camera;
    public GameObject Village;

    public int maxNumberVillages;

    //0
    static Vector3 posFront = new Vector3(0, 0, (float)0.5);
    //1
    static Vector3 posFrontRight = new Vector3((float)0.5, 0, (float)0.25);
    //2
    static Vector3 posBackRight = new Vector3((float)0.5, 0, (float)-0.25);
    //3
    static Vector3 posBack = new Vector3(0, 0, (float)-0.5);
    //4
    static Vector3 posBackLeft = new Vector3((float)-0.5, 0, (float)-0.25);
    //5
    static Vector3 posFrontLeft = new Vector3((float)-0.5, 0, (float)0.25);
    bool[] spawned = new bool[] { false, false, false, false, false, false };
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
                            Instantiate(Village, posFront, Quaternion.identity);
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
                            Instantiate(Village, posFrontRight, Quaternion.identity);
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
                            Instantiate(Village, posBackRight, Quaternion.identity);
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
                            Instantiate(Village, posBack, Quaternion.identity);
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
                            Instantiate(Village, posBackLeft, Quaternion.identity);
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
                            Instantiate(Village, posFrontLeft, Quaternion.identity);
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

        StartCoroutine(Villages());
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