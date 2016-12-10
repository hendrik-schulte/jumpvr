using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Destructible : MonoBehaviour, ForceReceiver
{
    public delegate void Callback();

    [SerializeField]
    private Animator Animator;

    [SerializeField]
    private ParticleSystem ParticleSystem;

    [SerializeField]
    private AudioSource AudioSource;

    [Header("Gameplay Values")]
    [SerializeField]
    [Range(0, 10)]
    private float ForceThreshold = 5;

    [SerializeField]
    [NonEditable]
    private bool IsLiving = true;

    [SerializeField]
    private bool DoNotDestroy = false;

    [Header("Debug Values")]
    [SerializeField]
    private bool DebugKeys;

    private GameEvent OnDestroyEvent;

    public Destructible()
    {
        OnDestroyEvent = new GameEvent();
    }

    void Awake()
    {
        if (!Animator) Animator = GetComponent<Animator>();
        if (!AudioSource) AudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!IsLiving || !DebugKeys) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DestroyVillage();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Animator.SetTrigger(Animator.StringToHash("Spawn"));
        }
    }

    public bool IsAlive()
    {
        return IsLiving;
    }

    public void Spawn()
    {
        Animator.SetTrigger(Animator.StringToHash("Spawn"));
    }

    public void DestroyAnimation()
    {
        Animator.SetTrigger(Animator.StringToHash("Destroy"));
        ParticleSystem.time = 0;
        ParticleSystem.Play(true);
    }

    public void DestroySound()
    {
        if (!AudioSource) return;

        AudioSource.PlayOneShot(AudioSource.clip);
    }


    public void OnHit(Vector3 impulse)
    {
        if (impulse.magnitude >= ForceThreshold) DestroyVillage(); ;
    }

    void DestroyVillage()
    {
        if (!IsLiving) return;

        IsLiving = false;
        DestroyAnimation();
        DestroySound();
        OnDestroyEvent.Invoke();

        if (!DoNotDestroy) Timer(2, delegate
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

    public void AddOnDestroyListener(UnityAction Action)
    {
        OnDestroyEvent.AddListener(Action);
    }
}
