using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerPrefValue : MonoBehaviour
{
    public string PreferenceName;

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
            text.text = PlayerPrefs.GetInt(PreferenceName, 0).ToString();
        }
	}
}
