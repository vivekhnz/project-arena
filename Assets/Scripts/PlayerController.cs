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
        ManageInput();
    }

    private void ManageInput()
    {
        float xMove = Input.GetAxis("Horizontal");
        float yMove = Input.GetAxis("Vertical");

        // move the player
        transform.position = new Vector3(
            transform.position.x + (xMove * MovementSpeed),
            transform.position.y + (yMove * MovementSpeed),
            transform.position.z);

        float playerRotation = transform.eulerAngles.z;
        // is the player moving?
        if (Mathf.Abs(xMove) + Mathf.Abs(yMove) > 0.5f)
        {
            // calculate the rotation based on the player's movement direction
            playerRotation = Mathf.Atan2(yMove, xMove) * Mathf.Rad2Deg;
        }

        // is a gamepad connected?
        var joysticks = Input.GetJoystickNames();
        if (joysticks.Length > 0 && !string.IsNullOrEmpty(joysticks[0]))
        {
            // get aim from right thumbstick
            float xAim = Input.GetAxis("HorizontalAim");
            float yAim = Input.GetAxis("VerticalAim");

            // is the player aiming with the right thumbstick?
            if (Mathf.Abs(xAim) + Mathf.Abs(yAim) > 0.5f)
            {
                // calculate the rotation based on the player's aim direction
                playerRotation = Mathf.Atan2(yAim, xAim) * Mathf.Rad2Deg;
            }
        }
        else
        {
            // perform raycast to calculate world position of mouse cursor
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.forward, Vector3.zero);
            float distance = 0.0f;
            if (plane.Raycast(ray, out distance))
            {
                Vector3 mousePos = ray.GetPoint(distance);

                // calculate rotation based on direction to mouse cursor
                Vector3 directionToMouse = mousePos - transform.position;
                playerRotation = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
            }
        }

        // set the player rotation
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, playerRotation);
    }
}