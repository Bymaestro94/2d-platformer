using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
  
    [Header("DoubleJump")]
    [SerializeField] private float doubleJumpForce;
    public bool canDoubleJump;


    [Header("Collision Info")]
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask whatIsGround; 
    private bool isGrounded;
    private bool isAirBorne;

    private float xInput;

    private bool faceInRight = true;
    private int faceInDir = 1;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  
        anim = GetComponentInChildren<Animator>();     
    }

    // Update is called once per frame
    void Update()
    {

        UpdateAirBorneJump();

        HandleCollision();
        HandleInput();
        HandleMovement();
        HandleFlip();
        HandleAnimation();
    }

    private void UpdateAirBorneJump(){
        if (isGrounded && isAirBorne){
            HandleLanding();
        }

        if (!isGrounded && !isAirBorne){
            isAirBorne = true;
        }
    }

    private void HandleLanding(){
        isAirBorne = false;
        canDoubleJump = true;
    }

    private void HandleCollision(){
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckRadius, whatIsGround);
    }

    private void HandleInput(){
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
            JumpButton();
    }



    private void HandleMovement(){
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void JumpButton() {
        if (isGrounded){
            Jump();
        } 
        else if (canDoubleJump) {
        DoubleJump();
        }
    }

    private void Jump() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    private void DoubleJump() {
        canDoubleJump = false;
        rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
    }

    private void HandleFlip(){
        if (rb.velocity.x < 0 && faceInRight || rb.velocity.x > 0 && !faceInRight) {
            Flip();
        }
    }

    private void Flip(){
        faceInDir = faceInDir * -1;
        transform.Rotate(0, 180, 0);
        faceInRight = !faceInRight;
    }

    private void HandleAnimation(){
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
    }
    private void OnDrawGizmos(){
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckRadius));
    }
}
