using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator anim;

    [SerializeField] private float moveSpeed;

    private float xInput;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  
        anim = GetComponentInChildren<Animator>();     
    }

    // Update is called once per frame
    void Update()
    {
        xInput = Input.GetAxis("Horizontal");

        HandleAnimation();
        HandleMovement();
    }

    private void HandleAnimation(){
        anim.SetBool("IsRunning", rb.velocity.x != 0);
    }
    private void HandleMovement(){
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }
}
