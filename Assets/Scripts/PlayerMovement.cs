using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    // Player movement vars
    Rigidbody rb;
    public float accelerationSpeed = 5.0f;
    public float maxSpeed = 10.0f;
    public float velocityMultiplier = 1.0f;

    // Player jump vars
    public Collider groundCheck;
    public bool isGrounded;
    public float jumpForce = 10;
    Vector3 jumpVector;

    // Camera vars
    public Camera cam;
    public Transform hRotationHelper;
    public Transform vRotationHelper;

    public float camSmooth = 0.3f;
    float yVelocity = 0.0f;
    float xVelocity = 0.0f;
    float xRotOld;
    float xRotation = 0.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        hRotationHelper.localRotation = transform.localRotation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        #region Player

        // Gather keyboard input
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");

        // Store keyboard input in a Vector3, normalize and tweak speed
        Vector3 move = (transform.right * xMove + transform.forward * zMove).normalized * accelerationSpeed * Time.deltaTime;

        // Exponentially increase movement speed, apply movement
        velocityMultiplier = Mathf.Clamp(rb.velocity.magnitude / 4, 1, 50);

        rb.velocity += move * velocityMultiplier;

        // Jumping
        jumpVector = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z) * (rb.velocity.magnitude + 1);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity += jumpVector;
        }

        // Check if player let off keyboard, decelerate if so
        if (move.x == 0 && rb.velocity.x != 0)
        {
            rb.velocity += new Vector3(-rb.velocity.x / 2 * accelerationSpeed * Time.deltaTime, 0, 0);
        }

        if (move.z == 0 && rb.velocity.z != 0)
        {
            rb.velocity += new Vector3(0, 0, -rb.velocity.z / 2 * accelerationSpeed * Time.deltaTime);
        }

        // Clamp velocity to maxSpeed variable
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;

        }
        #endregion

        #region Camera

        // Gather mouse input for camera
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Calculate up and down cam rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // Rotate helper for smooth cam rotation
        hRotationHelper.Rotate(Vector3.up * mouseX, Space.Self);

        // Apply cam rotation
        transform.localRotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, hRotationHelper.eulerAngles.y, ref yVelocity, camSmooth), 0);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        #endregion
    }
}
