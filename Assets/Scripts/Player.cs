using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    private float speed;
    [SerializeField]
    public float rotationSpeed;
    [SerializeField]
    public float jumpForce;
    [SerializeField]
    public float gravity;
    [SerializeField]
    public float jumpsCap;

    private float jumpCount = 0;

    private CharacterController controller;
    private Vector3 vectorDir;

    [SerializeField]
    private float inputX;
    [SerializeField]
    private float inputZ;
    [SerializeField]
    private float inputY;

    public GunController handReference;

    /* Game Manager y UI */

    private bool isDead;

    public bool IsDead()
    {
        return isDead;
    }

    public void Die()
    {
        isDead = true;
    }

    /* */

    public bool GetMovement()
    {
        if (controller.velocity != new Vector3(0, 0, 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Rotate(float rot)
    {
        transform.Rotate(0, rot * rotationSpeed * Time.deltaTime, 0);
    }

    private void Jump(ref float y)
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
        }
        if (jumpCount < jumpsCap)
        {
            y = jumpForce;
            jumpCount++;
        }
    }

    private void GetInput(ref float x, ref float y, ref float z)
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        if (Input.GetButtonDown("Jump"))
        {
            Jump(ref y);
        }
    }

    private void Movement()
    {
        vectorDir = new Vector3(inputX, 0, inputZ);
        vectorDir = transform.TransformDirection(vectorDir);
        vectorDir *= speed;
        if (!controller.isGrounded)
        {
            inputY -= gravity * Time.deltaTime;
        }
        vectorDir.y = inputY;
        controller.Move(vectorDir * Time.deltaTime);
    }

    void Awake()
    {
        isDead = false;
        controller = GetComponent<CharacterController>();
        if (!controller)
        {
            Debug.LogError("No se pudo encontrar controller");
        }
    }

    void Update ()
    {
        if (!isDead)
        {
            GetInput(ref inputX, ref inputY, ref inputZ);
            Movement();
        }
    }
}
