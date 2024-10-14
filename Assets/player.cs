using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour
    {
    private Rigidbody2D rb;

    private Animator anim;
    
    [Header("Speed Info")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float milestoneIncreaser;
    [SerializeField] private float speedMultiplier;
    private float defaultSpeed;
    private float defaultMilestoneIncreaser;
    private float speedMilestone;
    
    [Header("Ledge info")]
    [SerializeField] private Vector2 offset1;
    [SerializeField] private Vector2 offset2;
    private Vector2 climbBegunPosition;
    private Vector2 climbAfterPosition;
    private bool canGrabLedge = true;
    private bool canClimb;
    
    
    [Header("Sliding info")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideTime;
    [SerializeField] private float slideCooldown;
    private float slideCooldownTimer;
    private bool isSliding;
    private float slideTimeCounter;
    
    
    [Header("Move info")]
    [SerializeField] public float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private float defaultJumpForce;
    private bool playerUnlocked;
    private bool canDoubleJump;
    
    
    [Header("Collision info")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallSize;
    [SerializeField] private float ceillingCheckDistance;
    private bool ceilingDetected;
    private bool wallDetected;
    private bool isGrounded;
    [HideInInspector]public bool ledgeDetected;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        speedMilestone = milestoneIncreaser;
        defaultSpeed = moveSpeed;
        defaultMilestoneIncreaser = milestoneIncreaser;
        
    }


    // Update is called once per frame
    void Update()

    {
        slideTimeCounter -= Time.deltaTime;
        slideCooldownTimer -= Time.deltaTime;
        
        
        if (playerUnlocked)
            Movement();
        if (isGrounded)
        {
            canDoubleJump = true;
        }
            
        
        SpeedController();
        CheckCollision();
        CheckForLedge();
        CheckForSlide();
        CheckInput();
        AnimatorsController();
        
        
      }
    private void SpeedController()
    {
        if (moveSpeed == maxSpeed)
        {
            return;
        }
        if (transform.position.x > speedMilestone)
        {
            speedMilestone += milestoneIncreaser;
            moveSpeed *= speedMultiplier;
            milestoneIncreaser  = milestoneIncreaser * speedMultiplier;

            if (moveSpeed > maxSpeed)
            {
                moveSpeed = maxSpeed;
            }


        }
        
    }
    private void SpeedReset()
    {
        moveSpeed = defaultSpeed;
        milestoneIncreaser = defaultMilestoneIncreaser;
    }
    private void Movement()
    {
        if (wallDetected)
        {
            SpeedReset();
            return;
        }
        if (isSliding)
        {
            rb.velocity = new Vector2(slideSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
    }


    private void CheckInput(){
        if (Input.GetKeyDown(KeyCode.Mouse1))
            playerUnlocked = true;
        
        if (Input.GetKeyDown(KeyCode.Space))
            jumpButton();

        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            slideButton();
        }


    }
    private void CheckForSlide()
    {
        if (slideTimeCounter < 0 && !ceilingDetected)
        {
            isSliding = false;
        }
    }

    private void slideButton()
    {
        if (rb.velocity.x != 0 && slideCooldownTimer < 0 )
        {
            isSliding = true;
            slideTimeCounter = slideTime;
            slideCooldownTimer = slideCooldown;
        }
    }

    private void jumpButton()
    {
        if (isSliding)
        {
            return;
        }
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else if (canDoubleJump)
        {
            canDoubleJump = false;
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
        }
    }

    private void CheckForLedge()
    {
        if (canGrabLedge && ledgeDetected)
        {
            canGrabLedge = false;
            Vector2 ledgePosition = GetComponentInChildren<LedgeDetection>().transform.position;

            climbBegunPosition = ledgePosition + offset1;
            climbAfterPosition = ledgePosition + offset2;

            canClimb = true;
        }

        if (canClimb)
        {
            transform.position = climbBegunPosition;
        }
    }

    private void LedgeOver()
    {
        canClimb = false;
        transform.position = climbAfterPosition;
        Invoke("AllowLedgeGrab", .1f);
    }
    
    private void AllowLedgeGrab()
    {
        canGrabLedge = true;
    }

    
    
    private void OnDrawGizmos(){
        Gizmos.DrawLine(transform.position,new Vector2(transform.position.x,transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position,new Vector2(transform.position.x,transform.position.y + ceillingCheckDistance)); 
        Gizmos.DrawWireCube(wallCheck.position,wallSize);
    }
    private void CheckCollision(){
        isGrounded = Physics2D.Raycast(transform.position,Vector2.down,groundCheckDistance,whatIsGround);
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallSize,0,Vector2.zero,whatIsGround); 
        ceilingDetected = Physics2D.Raycast(transform.position,Vector2.up,ceillingCheckDistance,whatIsGround);

        
    }



    private void AnimatorsController()
    {
        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("canClimb", canClimb);
        if (rb.velocity.y < 20)
        {
            anim.SetBool("canRoll", true);
        }
        
    }

    private void RollAnimationFinished()
    {
        anim.SetBool("canRoll", false);
    }
}



