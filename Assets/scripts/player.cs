using System.Collections;
using System.Collections.Generic;
using Autodesk.Fbx;
using Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
    {
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private bool isDead;
    private bool playerUnlocked;
    
    [Header("KnockBack Info")]
    [SerializeField] private Vector2 knockBackDir;
    private bool isKnocked;
    private bool canBeKnocked = true;
    
    
    [Header("Speed Info")]
    [SerializeField] public float moveSpeed;
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


    [Header("Jump Info")]
    [SerializeField] private float doubleJumpForce;
    private float defaultJumpForce;
    [SerializeField] private float jumpForce;
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
        sr = GetComponent<SpriteRenderer>();
        
        speedMilestone = milestoneIncreaser;
        defaultSpeed = moveSpeed;
        defaultMilestoneIncreaser = milestoneIncreaser;

        
    }


    // Update is called once per frame
    void Update()

    {
        slideTimeCounter -= Time.deltaTime;
        slideCooldownTimer -= Time.deltaTime;

        if (isDead)
        {
            return;
        }
        if (isKnocked)
        {
            return;
        }
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


    private IEnumerator invincibility()
    {
        Color originalColor = sr.color;
        Color newColor = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f); // Adjusted alpha value

        canBeKnocked = false;
        sr.color = newColor;
        yield return new WaitForSeconds(.1f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.1f);

        sr.color = newColor;
        yield return new WaitForSeconds(.15f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.15f);

        sr.color = newColor;
        yield return new WaitForSeconds(.25f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.25f);

        sr.color = newColor;
        yield return new WaitForSeconds(.3f);

        sr.color = originalColor;
        canBeKnocked = true;
    }
    private void KnockedBack()
    {
        if (!canBeKnocked)
        {
            return;
        }
        StartCoroutine(invincibility());
        isKnocked = true;
        rb.velocity = knockBackDir;
        
    }
    private void cancelKnockBack()
    {
        isKnocked = false;
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
        
        
        if (Input.GetKeyDown(KeyCode.K) && isGrounded)
            KnockedBack();
        
        if (Input.GetKeyDown(KeyCode.O) && !isDead)
            StartCoroutine(Die());
        
        if (Input.GetKeyDown(KeyCode.Mouse1))
            playerUnlocked = true;
        
        if (Input.GetKeyDown(KeyCode.Space))
            jumpButton();

        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            slideButton();
        }


    }

    private IEnumerator Die()
    {
        isDead = true;
        canBeKnocked = false;
        rb.velocity = knockBackDir;
        anim.SetBool("isDead", true);

        yield return new WaitForSeconds(.5f);
        rb.velocity = new Vector2(0, 0);
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
        anim.SetBool("isKnocked", isKnocked);
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



