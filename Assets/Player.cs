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


    [Header("Buffer & Cayote")]
    [SerializeField] private float jumpBufferWindow = .25f;
    private float jumpBufferActivated = -1;
    [SerializeField] private float cayoteJumpWindow = .5f;
    private float cayoteJumpActivated = -1;

    [Header("Wall interaction")]
    [SerializeField] private float wallJumpDuration = .6f;
    [SerializeField] private Vector2 wallJumpForce;
    private bool isWallJumping; 


    [Header("Knocked")]
    [SerializeField] private float knockedDuration = 1;
    [SerializeField] private Vector2 knockedPower;
    private bool isKnocked;


    [Header("Collision")]
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

        if (Input.GetKeyDown(KeyCode.R)) {
            Knocked();
        }

        UpdateAirBorneJump();

        if (isKnocked) {
            return; 
        }

        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollision();
        HandleAnimation();
    }

    public void Knocked(){
        if (isKnocked) {
            return;
        }

        StartCoroutine(KnockedRoutine());
        anim.SetTrigger("knocked");

        rb.velocity = new Vector2(knockedPower.x * -faceInDir, knockedPower.y);
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

        AttemptBufferJump();
    }

    private void HandleCollision(){
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckRadius, whatIsGround);
        isWallSliding = Physics2D.Raycast(transform.position, Vector2.right * faceInDir, wallCheckDistance, whatIsGround);
    }

    private void HandleInput(){
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space)){
            JumpButton();
            RequestBufferJump();
        }

    }

    private void RequestBufferJump() {
        if (isAirBorne) {
            jumpBufferActivated = Time.time;
        }
    }
    private void AttemptBufferJump() {
        if (Time.time < jumpBufferActivated + jumpBufferWindow) {
            jumpBufferActivated = 0;
            Jump();
        }
    }

    private void HandleMovement(){

        if(isWallSliding){
            return;
        }
        if(isWallJumping){
            return;
        }

        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void JumpButton() {
        if (isGrounded){
            Jump();
        } 
        else if (isWallSliding && !isGrounded) {
            WallJump();
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

    private void WallJump(){

        canDoubleJump = true;
        rb.velocity = new Vector2(wallJumpForce.x * -faceInDir, wallJumpForce.y);
        Flip();
        StopAllCoroutines();
        StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine(){
        isWallJumping = true;

        yield return new WaitForSeconds(wallJumpDuration);

        isWallJumping = false;
    }

    private IEnumerator KnockedRoutine(){
        isKnocked = true;

    yield return new WaitForSeconds(knockedDuration);

        isKnocked = false;
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
