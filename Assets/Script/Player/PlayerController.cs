using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayController : MonoBehaviour
{
    public float runSpeed;
    public float jumpSpeed;
    public float doubleJumpSpeed;

    private Rigidbody2D myRigibody;
    private Animator myAnim;
    private BoxCollider2D myFeet;
    private bool isGround;
    private bool canDoubleJump;

    // Start is called before the first frame update
    void Start()
    {
        myRigibody = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        myFeet = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        Flip();
        Jump();
        //Attack();
        CheckGrounded();
        SwitchAnimation();
    }

    void CheckGrounded()
    {
        isGround = myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")) || myFeet.IsTouchingLayers(LayerMask.GetMask("MovingPlatform"));
        Debug.Log(isGround);
    }

    void Flip()
    {
        bool playerHasXAxisSpeed = Mathf.Abs(myRigibody.velocity.x) > Mathf.Epsilon;
        if (playerHasXAxisSpeed)
        {
            if(myRigibody.velocity.x > 0.1f)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            if (myRigibody.velocity.x < -0.1f)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    void Run()
    {
        float moveDir = Input.GetAxis("Horizontal");
        Vector2 playerVel = new Vector2(moveDir * runSpeed, myRigibody.velocity.y);
        myRigibody.velocity = playerVel;
        bool playerHasXAxisSpeed = Mathf.Abs(myRigibody.velocity.x) > Mathf.Epsilon;
        myAnim.SetBool("Run", playerHasXAxisSpeed);
    }

    void Jump()
    {
        if(Input.GetButtonDown("Jump"))
        {
            if (isGround)
            {
                myAnim.SetBool("Jump", true);
                Vector2 jumpVel = new Vector2(0.0f, jumpSpeed);
                myRigibody.velocity = Vector2.up * jumpVel;
                canDoubleJump = true;
            }
            else
            {
                if (canDoubleJump)
                {
                    myAnim.SetBool("DoubleJump", true);
                    Vector2 doubleJumpVel = new Vector2(0.0f, doubleJumpSpeed);
                    myRigibody.velocity = Vector2.up * doubleJumpVel;
                    canDoubleJump = false;
                }
            }
        }
    }

    //void Attack()
    //{
    //    if(Input.GetButtonDown("Attack"))
    //    {
    //        myAnim.SetTrigger("Attack");
    //    }
    //}

    void SwitchAnimation()
    {
        myAnim.SetBool("Idle", false);
        if(myAnim.GetBool("Jump"))
        {
            if(myRigibody.velocity.y < 0.0f)
            {
                myAnim.SetBool("Jump", false);
                myAnim.SetBool("Fall", true);
            }
        }
        else
        {
            if(isGround)
            {
                myAnim.SetBool("Fall", false);
                myAnim.SetBool("Idle", true);
            }
        }

        if (myAnim.GetBool("DoubleJump"))
        {
            if (myRigibody.velocity.y < 0.0f)
            {
                myAnim.SetBool("DoubleJump", false);
                myAnim.SetBool("DoubleFall", true);
            }
        }
        else
        {
            if (isGround)
            {
                myAnim.SetBool("DoubleFall", false);
                myAnim.SetBool("Idle", true);
            }
        }
    }
}

