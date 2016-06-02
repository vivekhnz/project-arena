using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float MovementSpeed = 0.3f;
    public BulletController Bullet;
    public ShockwaveController Shockwave;
    public float BulletSpread = 10.0f; // degrees
    public float RateOfFire = 60.0f; // bullets per minute
    public float SuperDuration = 5.0f; // seconds
    public float InvincibilityDuration = 2.0f; // seconds

    public double SuperEnergy { get; private set; }
    public bool IsSuperActive { get; private set; }

    private GameStateManager gameStateManager;
    private Animator animator;
    private float bulletFiredTime = 0.0f;
    private float invincibilityTime;
    private bool isInvincible = false;
    private bool superchargedNotified = false;

    void Start()
    {
        SuperEnergy = 0.0f;
        superchargedNotified = false;
        IsSuperActive = false;
        animator = GetComponent<Animator>();

        isInvincible = true;
        invincibilityTime = Time.time;

        var gameStateManagerObj = GameObject.FindGameObjectWithTag("GameStateManager");
        if (gameStateManagerObj != null)
        {
            gameStateManager = gameStateManagerObj.GetComponent<GameStateManager>();
        }
    }

    void FixedUpdate()
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

        // fire the player's weapon if the fire button is pressed
        if (Input.GetButton("Fire") || Input.GetAxis("FireAxis") > 0.0f)
        {
            Fire();
        }
        
        if (IsSuperActive)
        {
            DepleteSuper();
        }
        else if (Input.GetButton("Super") && SuperEnergy >= 1.0)
        {
            ActivateSuper();
        }

        if (isInvincible && Time.time - invincibilityTime > InvincibilityDuration)
        {
            isInvincible = false;
        }

        if (animator != null)
        {
            animator.SetBool("IsSuperActive", IsSuperActive);
            animator.SetBool("IsInvincible", isInvincible);
        }
    }

    private void CreateShockwave()
    {
        if (Shockwave != null)
        {
            var shockwave = Shockwave.Fetch<ShockwaveController>();
            shockwave.Initialize(transform.position);
        }
    }

    private void ActivateSuper()
    {
        IsSuperActive = true;
        CreateShockwave();
    }

    private void DepleteSuper()
    {
        superchargedNotified = false;

        float decreaseAmount = 1.0f / (60.0f * SuperDuration);
        SuperEnergy = Mathf.Clamp((float)SuperEnergy - decreaseAmount, 0.0f, 1.0f);
        if (SuperEnergy <= 0.0)
        {
            SuperEnergy = 0.0;
            IsSuperActive = false;
        }
    }

    void Fire()
    {
        if (Bullet != null)
        {
            float timeBetweenBullets = 60.0f / RateOfFire;
            if (Time.time - bulletFiredTime > timeBetweenBullets)
            {
                int bulletsToFire = (int)Mathf.Ceil(RateOfFire / 3600.0f);
                for (int i = 0; i < bulletsToFire; i++)
                {
                    // fetch a bullet instance from the object pool
                    var bullet = Bullet.Fetch<BulletController>();
                    // calculate aim direction based on bullet spread
                    float aim = transform.rotation.eulerAngles.z + Random.Range(-BulletSpread, BulletSpread);
                    // calculate offset
                    Vector3 offset = new Vector3(
                        Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad),
                        Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad),
                        0) * transform.localScale.x;
                    bullet.Initialize(this.transform.position + offset, aim);
                    // store the current time for rate of fire calculation
                    bulletFiredTime = Time.time;
                }
            }
        }
    }

    public void AddSuperEnergy(double amount)
    {
        SuperEnergy += amount;
        if (SuperEnergy >= 1.0)
        {
            if (gameStateManager != null && !superchargedNotified)
            {
                gameStateManager.NotifySupercharged();
                superchargedNotified = true;
            }
            SuperEnergy = 1.0;
        }
    }

    public void Kill()
    {
        if (!isInvincible)
        {
            if (gameStateManager != null)
            {
                gameStateManager.AddScore(-gameStateManager.DeathScorePenalty);
            }

            SuperEnergy = 0.0;
            IsSuperActive = false;
            superchargedNotified = false;

            CreateShockwave();

            isInvincible = true;
            invincibilityTime = Time.time;
        }
    }

    void OnDrawGizmosSelected()
    {
        // draw aim cones in Unity editor
        Gizmos.color = Color.red;
        DrawAimSpread(BulletSpread);
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
}