using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
    {
    private Rigidbody2D rb;

    private Animator anim;
    
    [Header("Move info")]
    private bool runBegun;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float jumpForce;
    
    
    
    [Header("Collision info")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance;
    private bool isGrounded;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()

    {
        if (runBegun)
            rb.velocity = new Vector2(moveSpeed,rb.velocity.y);

            
        CheckCollision();
        CheckInput();
        AnimatorsController();
        
      }
    
    private void CheckInput(){
        if (Input.GetKeyDown(KeyCode.Mouse1))
            runBegun = true;
        
        if (Input.GetKeyDown(KeyCode.Space)&& isGrounded)
            rb.velocity = new Vector2(rb.velocity.x,jumpForce);
        
        
    }
    private void OnDrawGizmos(){
        Gizmos.DrawLine(transform.position,new Vector2(transform.position.x,transform.position.y - groundCheckDistance));
    }
    private void CheckCollision(){
        isGrounded = Physics2D.Raycast(transform.position,Vector2.down,groundCheckDistance,whatIsGround);
    }

    private void AnimatorsController()
    {
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("xVelocity", rb.velocity.x);
    }

}



