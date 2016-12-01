using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    [SerializeField]
    private Destructible StartGameVillage;

    [SerializeField]
    private Animator StartGameAnimator;

    [SerializeField]
    [Range(0, 10)]
    private float FadeOutTime = 3;

    private RealWorldButton[] Buttons;

    public MainMenu()
    {
        if (!Instance) Instance = this;
    }

    private void Awake()
    {
        Buttons = GetComponentsInChildren<RealWorldButton>();
    }

    public void Activated()
    {
        print("Activated menu");
        StopAllCoroutines();
        gameObject.SetActive(true);

        StartGameAnimator.SetBool("Enabled", true);
        StartGameVillage.Spawn();
        foreach (var Button in Buttons)
        {
            Button.Activated();
        }
    }

    public void Deactivated()
    {
        //        if (!gameObject.activeSelf) return;
        print("Deactivated menu");

        StartGameAnimator.SetBool("Enabled", false);
        StartGameVillage.DestroyAnimation();
        foreach (var Button in Buttons)
        {
            Button.Deactivated();
        }

        Timer(FadeOutTime, delegate
        {
            gameObject.SetActive(false);
        });
    }

    public void AddStartGameListener(UnityAction Action)
    {
        StartGameVillage.AddOnDestroyListener(Action);
    }

    private void Timer(float time, Destructible.Callback callback)
    {
        StartCoroutine(TimerCR(time, callback));
    }

    IEnumerator TimerCR(float time, Destructible.Callback callback)
    {
        yield return new WaitForSeconds(time);

        callback();
    }
}