using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreCalloutController : PooledObject
{
    public void Initialize(Transform parent, Vector3 position, int scoreValue)
    {
        transform.position = position;
        transform.SetParent(parent);

        var calloutText = GetComponent<Text>();
        calloutText.text = string.Format("{0}{1}",
            scoreValue < 0 ? "-" : "+", scoreValue);
    }

    public override void CleanupInstance()
    {
        transform.SetParent(GameObject.Find("ScoreCallout Pool").transform);

        base.CleanupInstance();
    }
}
