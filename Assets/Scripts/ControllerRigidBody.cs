using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerRigidBody : MonoBehaviour {

    [SerializeField]
    public float speed;
    [SerializeField]
    public float rotationSpeed;
    [SerializeField]
    public float jumpForce;
    [SerializeField]
    public float jumpsCap;

    private float jumpCount = 0;

    private Rigidbody controller;
    private Vector3 vectorDir;

    [SerializeField]
    private float inputX;
    [SerializeField]
    private float inputZ;
    //[SerializeField]
    //private float inputY;
    [SerializeField]
    private float rotation;


    private bool IsGrounded()
    {
        bool answer = Physics.Raycast(transform.position, Vector3.down, 0.5f);
        return answer;
    }

    private void Rotate(float rot)
    {
        Quaternion deltaRotation = Quaternion.Euler(0, rotationSpeed * rot* Time.deltaTime, 0);
        controller.MoveRotation(controller.rotation * deltaRotation);
    }

    private void Jump()
    {
        if (jumpCount < jumpsCap)
        {
            controller.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
        }
    }

    private void GetInput(ref float x, ref float z)
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
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
        vectorDir.y = 0;
        controller.MovePosition(transform.position + (vectorDir * Time.deltaTime));
    }

    void Awake()
    {
        controller = GetComponent<Rigidbody>();
        if (!controller)
        {
            Debug.LogError("No se pudo encontrar controller");
        }
    }

    private void FixedUpdate()
    {
        Movement();
        Rotate(rotation);
    }

    void Update ()
    {
        GetInput(ref inputX, ref inputZ);
        if (IsGrounded())
        {
             jumpCount = 0;
        }
    }
}
