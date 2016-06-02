using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class WorldUIController : MonoBehaviour
{
    public PlayerController Player;
    public GameObject SuperMeter;
    public Image SuperMeterForeground;
    public ScoreCalloutController ScoreCallout;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        SuperMeter.transform.position = Player.transform.position;
        SuperMeterForeground.fillAmount = Mathf.Lerp(
            SuperMeterForeground.fillAmount,
            (float)Player.SuperEnergy, 0.25f);

        if (animator != null)
        {
            animator.SetFloat("SuperEnergy", (float)Player.SuperEnergy);
            animator.SetBool("IsSuperActive", Player.IsSuperActive);
        }
    }

    public void CreateScoreCallout(int scoreValue, Vector3 position)
    {
        var callout = ScoreCallout.Fetch<ScoreCalloutController>();
        callout.Initialize(transform, position, scoreValue);
    }
}
