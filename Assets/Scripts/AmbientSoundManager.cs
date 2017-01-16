using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource AudioSource;

    [SerializeField]
    private AudioClip Ambient;

    [SerializeField]
    private AudioClip[] IdleSounds;

    [Header("Idle Sounds Logic")]
    public AnimationCurve IdleProbabilityCurve;
    public float TryToPlayEvery;
    

    [SerializeField]
    [Range(0, 10)]
    private float Intensity;

    private float elapsedTime;
    private float currentCheckTime;
    private float[] idleSoundPropabilities;

    void Awake()
    {

    }

    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        idleSoundPropabilities = new float[IdleSounds.Length];
        for (int i = 0; i < idleSoundPropabilities.Length; i++)
        {
            idleSoundPropabilities[i] = 1f / idleSoundPropabilities.Length;
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        currentCheckTime += Time.deltaTime;

        if (currentCheckTime >= TryToPlayEvery)
        {
            currentCheckTime = 0f;
            if (Random.value <= IdleProbabilityCurve.Evaluate(elapsedTime))
            {
                elapsedTime = 0f;
                float addedProbs = 0f;
                for (int i = 0; i < idleSoundPropabilities.Length; i++)
                {
                    addedProbs += idleSoundPropabilities[i];
                    if (Random.value <= addedProbs || i == idleSoundPropabilities.Length - 1)
                    {
                        AudioSource.PlayOneShot(IdleSounds[i]);
                        adjustProbabilities(i);
                        break;
                    }
                }
            }
        }
    }

    private void adjustProbabilities(int indexThatWasPlayed)
    {

        float probAdjust = (idleSoundPropabilities[indexThatWasPlayed] * 0.5f) ;
        for (int i = 0; i < idleSoundPropabilities.Length; i++)
        {
            if (i == indexThatWasPlayed)
            {
                idleSoundPropabilities[i] *= 0.5f;
            }
            else
            {
                idleSoundPropabilities[i] += probAdjust / (idleSoundPropabilities.Length - 1);
            }
        }
    }
}