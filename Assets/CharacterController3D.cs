using UnityEngine;

public class CharacterController3D : MonoBehaviour
{

    public float JumpStartV;

    public float MoveSpeed;

    public float AirMoveSpeed = 3;

    public float Gravity = 14;

    private CharacterController controller;

    private Vector3 moveVelocity = Vector3.zero;

    private Vector3 jumpVelocity = Vector3.zero;

    private bool isInAir = false;

    public Vector3 velocity;

    public float rotationSpeed;

    private bool isPress;

    private bool isAttackWall = false;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }


    public void Update()
    {
        
        if (controller.isGrounded && isInAir)
        {
            isInAir = false;
            OnGrounded();
        }
        else if(isInAir)
        {
            OnAir();
        }

        moveVelocity = Vector3.zero;
        isPress = false;
  
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpVelocity = InternalJump();
        }

        if (Input.GetKey(KeyCode.A))
        {
            isPress = true;
            if (controller.isGrounded)
            {
                moveVelocity = new Vector3(-MoveSpeed, 0, 0);
            }
            else
            {
                moveVelocity = new Vector3(-AirMoveSpeed, 0, 0);
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            isPress = true;
            if (controller.isGrounded)
            {
                moveVelocity = new Vector3(MoveSpeed, 0, 0);
            }
            else
            {
                moveVelocity = new Vector3(AirMoveSpeed, 0, 0);
            }
        }
        if (jumpVelocity.y > -Gravity)
        {
            jumpVelocity += new Vector3(0, -Gravity, 0)*Time.deltaTime;
        }
        else
        {
            jumpVelocity.y = -Gravity;
        }
        velocity = moveVelocity + jumpVelocity;
        controller.Move(velocity * Time.deltaTime);
        if (Mathf.Abs(controller.velocity.x) < 0.1f && isPress)
        {
            Debug.Log("attact wall " + controller.velocity.x);
            isAttackWall = true;
            isInAir = false;
            jumpVelocity = new Vector3(0, Mathf.Abs(moveVelocity.x * 2), 0);
            moveVelocity = Vector3.zero;
        }
        else
        {
            Debug.Log("not attack wall" + controller.velocity.x);
            isAttackWall = false;
        }
        //Debug.Log(controller.velocity);
       // rigidbody.velocity = velocity;
    }

    protected virtual void OnAir()
    {
        if (!isAttackWall)
        {
            transform.eulerAngles = transform.eulerAngles + new Vector3(0, 0, rotationSpeed)*Time.deltaTime;
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }

    protected virtual void OnGrounded()
    {
        transform.rotation = Quaternion.identity;
    }

    protected Vector3 Jump()
    {
        return new Vector3(0, JumpStartV, 0);
    }

    private Vector3 InternalJump()
    {
        isInAir = true;
        return Jump();
    }
}
