using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WorldUIController : MonoBehaviour
{
    public PlayerController Player;
    public GameObject SuperMeter;
    public Image SuperMeterForeground;

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
            Player.SuperEnergy, 0.25f);

        if (animator != null)
        {
            animator.SetFloat("SuperEnergy", Player.SuperEnergy);
        }
    }
}
