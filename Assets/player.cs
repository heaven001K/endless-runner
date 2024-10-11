using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
    {
    private Rigidbody2D rb;

    private Animator anim;
    
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
    private bool wallDetected;
    private bool isGrounded;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
    }


    // Update is called once per frame
    void Update()

    {
        if (playerUnlocked && !wallDetected)
            Movement();
        if (isGrounded)
        {
            canDoubleJump = true;
        }
            
        CheckCollision();
        CheckInput();
        AnimatorsController();
        
      }

    private void Movement()
    {
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }


    private void CheckInput(){
        if (Input.GetKeyDown(KeyCode.Mouse1))
            playerUnlocked = true;
        
        if (Input.GetKeyDown(KeyCode.Space))
            jumpButton();
        
        
    }

    private void jumpButton()
    {
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

    private void OnDrawGizmos(){
        Gizmos.DrawLine(transform.position,new Vector2(transform.position.x,transform.position.y - groundCheckDistance));
        Gizmos.DrawWireCube(wallCheck.position,wallSize);
    }
    private void CheckCollision(){
        isGrounded = Physics2D.Raycast(transform.position,Vector2.down,groundCheckDistance,whatIsGround);
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallSize,0,Vector2.zero,whatIsGround); 
    }

    private void AnimatorsController()
    {
        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("xVelocity", rb.velocity.x);
    }

}



