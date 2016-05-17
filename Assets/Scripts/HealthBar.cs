using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public DamageableObject Target;

    private Slider slider;
    private float targetValue = 0.0f; 

	void Start ()
    {
        slider = GetComponent<Slider>();
        if (slider != null && Target != null)
        {
            Target.HealthChanged += OnHealthChanged;
            slider.minValue = 0;
            slider.maxValue = Target.MaxHealth;
            slider.value = Target.CurrentHealth;
            targetValue = Target.CurrentHealth;
        }
	}

    void Update ()
    {
        slider.value = Mathf.Lerp(slider.value, targetValue, 0.2f);
    }

    private void OnHealthChanged(object sender, System.EventArgs e)
    {
        if (slider != null)
        {
            targetValue = Target.CurrentHealth;
        }
    }
}
