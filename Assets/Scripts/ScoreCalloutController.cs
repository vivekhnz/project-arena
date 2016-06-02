using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreCalloutController : PooledObject
{
    private Vector2 velocity;

    public void Initialize(Transform parent,
        Vector3 position, int scoreValue, Vector2 velocity)
    {
        transform.position = position;
        transform.SetParent(parent);

        var calloutText = GetComponent<Text>();
        calloutText.text = string.Format("{0}{1}",
            scoreValue < 0 ? "-" : "+", scoreValue);

        int scoreCap = 250;
        float amount = Mathf.Clamp((float)scoreValue / (float)scoreCap, 0.0f, 1.0f);
        float scale = Mathf.Lerp(0.04f, 0.1f, amount);
        transform.localScale = new Vector3(scale, scale);

        this.velocity = velocity;
    }

    void FixedUpdate()
    {
        transform.position = new Vector3(
            transform.position.x + velocity.x,
            transform.position.y + velocity.y,
            transform.position.z);
    }

    public override void CleanupInstance()
    {
        transform.SetParent(GameObject.Find("ScoreCallout Pool").transform);

        base.CleanupInstance();
    }
}
