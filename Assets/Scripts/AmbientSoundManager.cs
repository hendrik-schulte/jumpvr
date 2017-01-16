using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource AudioSource;

    [SerializeField]
    private AudioClip Ambient;

    [SerializeField]
    private AudioClip[] IdleSounds;

    [SerializeField]
    [Range(0, 10)]
    private float Intensity;

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }
}