using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float MovementSpeed = 0.3f;

    void Start()
    {

    }

    void Update()
    {
        float xTranslate = Input.GetAxis("Horizontal") * MovementSpeed;
        float yTranslate = Input.GetAxis("Vertical") * MovementSpeed;

        transform.Translate(xTranslate, yTranslate, 0.0f);
    }
}