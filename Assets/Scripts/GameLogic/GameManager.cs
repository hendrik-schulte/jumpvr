using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Text Text;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void CalibrationDone()
    {
        
    }
}