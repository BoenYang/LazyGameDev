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

    public bool isAttachWall = false;

    private int moveDir = 0;

    private int preFrameMoveDir = 0;

    public float wallJumpFlyTime = 0.2f;

    private float wallJumpFlyTimer = 0;

    private bool isWallJump = false;

    private Vector3 wallJumpVelocity;

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
        moveDir = 0;

        moveVelocity = Vector3.zero;
        isPress = false;

        if (isWallJump)
        {
            wallJumpFlyTimer -= Time.deltaTime;
            if (wallJumpFlyTimer <= 0)
            {
                isWallJump = false;
                wallJumpFlyTimer = wallJumpFlyTime;
                wallJumpVelocity = Vector3.zero;
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDir = -1;
            isPress = true;
            if (moveDir != preFrameMoveDir && isWallJump)
            {
                isWallJump = false;
                wallJumpVelocity = Vector3.zero;
            }

            if (!isWallJump)
            {
                if (controller.isGrounded)
                {
                    moveVelocity = new Vector3(-MoveSpeed, 0, 0);
                }
                else
                {
                    moveVelocity = new Vector3(-AirMoveSpeed, 0, 0);
                }
            }
         
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveDir = 1;
            isPress = true;
            if (moveDir != preFrameMoveDir && isWallJump)
            {
                isWallJump = false;
                wallJumpVelocity = Vector3.zero;
            }

            if (!isWallJump)
            {
                if (controller.isGrounded)
                {
                    moveVelocity = new Vector3(MoveSpeed, 0, 0);
                }
                else
                {
                    moveVelocity = new Vector3(AirMoveSpeed, 0, 0);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpVelocity = InternalJump();
            if (isAttachWall && isPress)
            {
                wallJumpVelocity = new Vector3(- MoveSpeed * moveDir * 2,0,0);
                wallJumpFlyTimer = wallJumpFlyTime;
                isWallJump = true;
                Debug.Log("move jump");
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

        velocity = moveVelocity + jumpVelocity + wallJumpVelocity;
        controller.Move(velocity * Time.deltaTime);
        if (Mathf.Abs(controller.velocity.x) < 0.1f && isPress)
        {
            //Debug.Log("attact wall " + controller.velocity.x);
            isAttachWall = true;
            isInAir = false;
            jumpVelocity = new Vector3(0, Mathf.Abs(moveVelocity.x * 2), 0);
            transform.rotation = Quaternion.identity;
        }
        else if(Mathf.Abs(controller.velocity.x) > 0.1f)
        {
            //Debug.Log("not attack wall" + controller.velocity.x);
            isAttachWall = false;
        }

        preFrameMoveDir = moveDir;
        // Debug.Log(controller.velocity);
        // rigidbody.velocity = velocity;
    }

    protected virtual void OnAir()
    {
        if (!isAttachWall)
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
