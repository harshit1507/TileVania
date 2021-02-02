using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    // Config params
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climpSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(25f, 25f);

    // State
    bool isAlive = true;

    // Cached reference
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider2D;
    BoxCollider2D myFeet;
    float gravityScaleAtStart;
    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
        myFeet = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
            return;        

        Run();
        FlipSprite();
        Jump();
        ClimbLadder();
        Die();
    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 runVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = runVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("Running", playerHasHorizontalSpeed);
    }

    private void ClimbLadder()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myAnimator.SetBool("Climbing", false);
            myRigidBody.gravityScale = gravityScaleAtStart;
            return;
        }
            

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climpSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("Climbing", playerHasVerticalSpeed);
    }

    private void Jump()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")))
            return;

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity += jumpVelocityToAdd;
        }
    }

    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        // if the player is moving horizontally
        if(playerHasHorizontalSpeed)
        {
            // reverse the current scaling of x-axis
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
        }
    }

    private void Die()
    {
        if(myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            GetComponent<Rigidbody2D>().velocity = deathKick;
        }
    }
}
