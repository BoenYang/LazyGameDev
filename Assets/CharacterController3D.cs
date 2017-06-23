using UnityEngine;
using System.Collections;

public class CharacterController3D : MonoBehaviour
{

    public float JumpStartV;

    public float MoveSpeed;

    public float AirMoveSpeed = 3;

    public float Gravity = 14;

    private CharacterController controller;

    private Rigidbody rigidbody;

    private Vector3 horizontalVelocity = Vector3.zero;

    private Vector3 verticalVelocity = Vector3.zero;

    private bool isJumping = false;

    public Vector3 velocity;

    public float rotationSpeed;

    private bool isPress;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        rigidbody = GetComponent<Rigidbody>();
    }


    public void Update()
    {

        if (controller.isGrounded)
        {
            isJumping = false;
            OnGrounded();
        }
        else
        {
            OnAir();
        }

        horizontalVelocity = Vector3.zero;
        verticalVelocity += new Vector3(0,-Gravity,0) * Time.deltaTime;

        isPress = false;
  
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            verticalVelocity = new Vector3(0, JumpStartV, 0) ;
        }

        if (Input.GetKey(KeyCode.A))
        {
            isPress = true;
            if (controller.isGrounded)
            {
                horizontalVelocity = new Vector3(-MoveSpeed, 0, 0);
            }
            else
            {
                horizontalVelocity = new Vector3(-AirMoveSpeed, 0, 0);
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            isPress = true;
            if (controller.isGrounded)
            {
                horizontalVelocity = new Vector3(MoveSpeed, 0, 0);
            }
            else
            {
                horizontalVelocity = new Vector3(AirMoveSpeed, 0, 0);
            }
        }

        velocity = horizontalVelocity + verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
        if (Mathf.Approximately(controller.velocity.x,0) && isPress)
        {
            verticalVelocity = new Vector3(0,Mathf.Abs(horizontalVelocity.x * 2),0);
            horizontalVelocity = Vector3.zero;
        }
        Debug.Log(controller.velocity);
    }

    protected void OnAir()
    {
        transform.eulerAngles = transform.eulerAngles + new Vector3(0, 0, rotationSpeed) * Time.deltaTime;
    }

    protected void OnGrounded()
    {
        transform.rotation = Quaternion.identity;
    }
}
