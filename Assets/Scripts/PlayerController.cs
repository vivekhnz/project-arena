using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float MovementSpeed = 0.3f;
    public GameObject Bullet;
    public float MaxBulletSpread = 10.0f;
    public float MinBulletSpread = 0.0f;
    public float BloomDurationSeconds = 3.0f;
    public float BloomResetDurationSeconds = 1.5f;
    public AnimationCurve BloomCurve;
    public AnimationCurve BloomResetCurve;

    private float currentBloom = 0.0f;
    private float startFireTime = 0.0f;
    private float releaseFireTime = 0.0f;
    private bool wasFiring = false;

    void Start()
    {
    }

    void Update()
    {
        bool isFiring = false;
        ManageInput(out isFiring);
        UpdateBloom(isFiring);
    }

    private void ManageInput(out bool isFiring)
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

        // fire the player's weapon if the fire button is pressed
        if (Input.GetButton("Fire") || Input.GetAxis("FireAxis") > 0.0f)
        {
            Fire();
            isFiring = true;
        }
        else
        {
            isFiring = false;
        }
    }

    private void UpdateBloom(bool isFiring)
    {
        if (isFiring && !wasFiring)
        {
            // Player has just started firing
            // Calculate the starting fire time based on the current bloom value
            // This value will be the current time if bloom has fully reset since the last time fired
            startFireTime = Time.time - (GetCurveTimeForValue(BloomCurve, currentBloom, 10) * BloomDurationSeconds);
            wasFiring = true;
        }
        else if (!isFiring && wasFiring)
        {
            // Player just stopped firing
            // Store the time the player stopped firing
            releaseFireTime = Time.time;
            wasFiring = false;
        }

        if (isFiring)
        {
            // increase bloom the longer the player fires
            currentBloom = BloomCurve.Evaluate((Time.time - startFireTime) / BloomDurationSeconds);
        }
        else
        {
            // calculate the bloom at the time of release
            float bloomAtRelease = BloomCurve.Evaluate((releaseFireTime - startFireTime) / BloomDurationSeconds);
            // reset bloom over time
            currentBloom = bloomAtRelease * BloomResetCurve.Evaluate((Time.time - releaseFireTime) / BloomResetDurationSeconds);
        }
    }

    void Fire()
    {
        if (Bullet != null)
        {
            // instantiate a bullet
            var bullet = Instantiate(Bullet);
            var controller = bullet.GetComponent<BulletController>();

            // calculate bullet spread based on current bloom value
            float currentBulletSpread = Mathf.Lerp(MinBulletSpread, MaxBulletSpread, currentBloom);
            // calculate aim direction based on bullet spread
            float aim = transform.rotation.eulerAngles.z + Random.Range(-currentBulletSpread, currentBulletSpread);
            controller.Initialize(this.transform.position, aim);
        }
    }

    void OnDrawGizmosSelected()
    {
        // draw aim cones in Unity editor
        Gizmos.color = new Color(0.9f, 0.9f, 0.9f);
        DrawAimSpread(MaxBulletSpread);
        Gizmos.color = Color.red;
        DrawAimSpread(Mathf.Lerp(MinBulletSpread, MaxBulletSpread, currentBloom));
    }

    void DrawAimSpread(float spread)
    {
        float rayLength = 10.0f;

        Gizmos.DrawRay(transform.position, new Vector3(
            Mathf.Cos((transform.rotation.eulerAngles.z + spread) * Mathf.Deg2Rad) * rayLength,
            Mathf.Sin((transform.rotation.eulerAngles.z + spread) * Mathf.Deg2Rad) * rayLength, 0.0f));
        Gizmos.DrawRay(transform.position, new Vector3(
            Mathf.Cos((transform.rotation.eulerAngles.z - spread) * Mathf.Deg2Rad) * rayLength,
            Mathf.Sin((transform.rotation.eulerAngles.z - spread) * Mathf.Deg2Rad) * rayLength, 0.0f));
    }

    // calculates the time from a given value on a curve (reverse evaluate)
    // based on: http://stackoverflow.com/questions/25527855/animationcurve-evaluate-get-time-by-value
    public float GetCurveTimeForValue(AnimationCurve curve, float value, int accuracy)
    {
        float startTime = curve.keys[0].time;
        float endTime = curve.keys[curve.length - 1].time;

        float nearestTime = startTime;
        float step = endTime - startTime;

        for (int i = 0; i < accuracy; i++)
        {
            float valueAtNearestTime = curve.Evaluate(nearestTime);
            float distanceToValueAtNearestTime = Mathf.Abs(value - valueAtNearestTime);

            float timeToCompare = nearestTime + step;
            float valueAtTimeToCompare = curve.Evaluate(timeToCompare);
            float distanceToValueAtTimeToCompare = Mathf.Abs(value - valueAtTimeToCompare);

            if (distanceToValueAtTimeToCompare < distanceToValueAtNearestTime)
            {
                nearestTime = timeToCompare;
                valueAtNearestTime = valueAtTimeToCompare;
            }
            step = Mathf.Abs(step * 0.5f) * Mathf.Sign(value - valueAtNearestTime);
        }

        return nearestTime;
    }
}