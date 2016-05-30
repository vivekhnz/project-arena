using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SuperMeterController : MonoBehaviour
{
    public PlayerController Player;
    public Image SuperMeterForeground;

	void Start ()
    {

	}
	
	void FixedUpdate ()
    {
        transform.position = Player.transform.position;
        SuperMeterForeground.fillAmount = Mathf.Lerp(
            SuperMeterForeground.fillAmount,
            Player.SuperEnergy, 0.5f);
	}
}
