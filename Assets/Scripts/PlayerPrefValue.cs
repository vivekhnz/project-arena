using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerPrefValue : MonoBehaviour
{
    public string PreferenceName;
    public bool IsPersisted = false;

    private Text text;

    // Use this for initialization
    void Start ()
    {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (text != null && !string.IsNullOrEmpty(PreferenceName))
        {
            text.text = ApplicationModel.GetValue(PreferenceName, IsPersisted).ToString();
        }
	}
}
