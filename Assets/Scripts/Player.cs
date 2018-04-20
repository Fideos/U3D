using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    public float speed;
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
    [SerializeField]
    private float rotation;

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

        // Arreglar con mouse.
        if (Input.GetKey(KeyCode.Q))
        {
            rotation = -1;
        }
        else
        {
            if (Input.GetKey(KeyCode.E))
            {
                rotation = 1;
            }
            else
            {
                rotation = 0;
            }
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
        controller = GetComponent<CharacterController>();
        if (!controller)
        {
            Debug.LogError("No se pudo encontrar controller");
        }
    }

    void Update ()
    {
        GetInput(ref inputX, ref inputY, ref inputZ);
        Movement();
        Rotate(rotation);
    }
}
