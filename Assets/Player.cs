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
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround; 
    private bool isGrounded;
    private bool isAirBorne;
    private bool isWallSliding;

    private float xInput;
    private float yInput;

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


        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollision();
        HandleAnimation();
    }

    private void HandleWallSlide() {
        bool canWallSlide = isWallSliding && rb.velocity.y < 0; 
        float yModifier = yInput < 0 ? 1 : .05f;

        if (canWallSlide == false) {
            return;
        }

        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * yModifier);
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
        isWallSliding = Physics2D.Raycast(transform.position, Vector2.right * faceInDir, wallCheckDistance, whatIsGround);
    }

    private void HandleInput(){
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
            JumpButton();
    }



    private void HandleMovement(){

        if(isWallSliding){
            return;
        }

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
        if (xInput < 0 && faceInRight || xInput > 0 && !faceInRight) {
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
        anim.SetBool("isWallDetected", isWallSliding);
    }
    private void OnDrawGizmos(){
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckRadius));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * faceInDir), transform.position.y));
    }
}
