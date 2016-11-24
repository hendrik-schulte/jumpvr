using System.Collections;
using UnityEngine;

public class Destructible : MonoBehaviour, ForceReceiver
{
    public delegate void Callback();

    [SerializeField]
    private Animator Animator;
    int destroy = Animator.StringToHash("Destroy");
//    public ParticleSystem particles;
    int spawn = Animator.StringToHash("Spawn");

    [SerializeField]
    private ParticleSystem ParticleSystem;

    [Header("Gameplay Values")]
    [SerializeField]
    [Range(0, 50)]
    private float ForceThreshold = 5;

    [SerializeField]
    private bool IsLiving = true;

    [Header("Debug Values")]
    [SerializeField]
    private bool DebugKeys;


    // Use this for initialization
    void Start()
    {
        if(!Animator) Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLiving || !DebugKeys) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DestroyVillage();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Animator.SetTrigger(spawn);
        }
    }

    public bool IsAlive()
    {
        return IsLiving;
    }


    public void OnHit(Vector3 impulse)
    {
        if (impulse.magnitude >= ForceThreshold) DestroyVillage(); ;
    }

    void DestroyVillage()
    {
        if (!IsLiving) return;

        IsLiving = false;
        Animator.SetTrigger(destroy);
        ParticleSystem.Play(true);

        Timer(2, delegate
        {
//            ParticleSystem.Stop(true);
//            ParticleSystem.Clear(true);
            Destroy(gameObject);
        });
    }

    private void Timer(float time, Callback callback)
    {
        StartCoroutine(TimerCR(time, callback));
    }

    IEnumerator TimerCR(float time, Callback callback)
    {
        yield return new WaitForSeconds(time);

        callback();
    }
}
