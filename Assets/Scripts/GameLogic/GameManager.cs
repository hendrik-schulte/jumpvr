using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Text Text;
    public GameObject MainMenu;
    public GameObject Camera;

    private Animator CameraAnimator;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        CameraAnimator = Camera.GetComponent<Animator>();

        SwipeDetectorGear.Instance.AddSwipeTopListener(Recalibrate);
    }

    public void CalibrationDone()
    {
        print("Calibration Done!");
        MainMenu.SetActive(true);
    }

    public void Recalibrate()
    {
        MainMenu.SetActive(false);

        Trampolin.Instance.Recalibrate();
    }

    public void StartGame()
    {
        print("Starting Game!");
        MainMenu.SetActive(false);

    }

    public void Blur()
    {
        CameraAnimator.SetTrigger("Blurry");
    }
}