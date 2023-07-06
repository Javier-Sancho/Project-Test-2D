using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PrimerScript : MonoBehaviour
{
    #region Variables
    // public Fields
    public float speed = 150;
    public SpriteRenderer spr;

    // Private Fields
    Rigidbody2D rb;
    Animator animator;
    [SerializeField] Collider2D standigCollider;
    [SerializeField] Transform overheadCheckColl;
    [SerializeField] LayerMask groundLayer;

    const float overheadCheckRadius = 0.2f;
    float runSpeedModifier = 1.5f;
    float crouchSpeedModifier = .5f;
    [SerializeField] float jumpPower = 500f;
    float horizontalValue;

    bool jump;
    [SerializeField] bool isRunning;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isCrouched;

    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        #region Move && Run
        // Store the horizontal value
        horizontalValue = Input.GetAxisRaw("Horizontal");

        //If left shift click enable isRunning
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;
        }

        //If left shift release disable isRunning
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }
        #endregion

        #region Jump
        //If we press Jump Button enable jump
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        //Otherwise disable it
        else if (Input.GetButtonUp("Jump")) 
        {
            jump = false;
        }
              
        //Set the yVelocity in the animator
        animator.SetFloat("yVelocity", rb.velocity.y);

        #endregion

        #region Crouch
        //If we press Crouch Button enable crouch
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouched = true;

        }
        //Otherwise disable it
        else if (Input.GetButtonUp("Crouch"))
        {
            isCrouched = false;
        }
        #endregion

    }
    public void OnCollisionEnter2D(Collision2D coll)
    {
        isGrounded = false;
        if (coll.gameObject.tag == "Suelo")
        {
            isGrounded = true;
        }
        else if (coll.gameObject == null) 
        {
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        Move(horizontalValue, jump, isCrouched);
        
    }


    void Move(float dir, bool jumpFlag, bool crouchFlag)
    {
        #region Crouch & Jump

        //If we are crouching disable crouching
        //Check overhead for collision with Ground items
        //If there are any, remain crouched

        if(!crouchFlag)
        {
            if (Physics2D.OverlapCircle(overheadCheckColl.position, overheadCheckRadius, groundLayer)) 
            { 
                crouchFlag = true;
            }
        }


        if (isGrounded)
        {
            
            animator.SetBool("Jump", false);

            //If the player is grounded and press crouch disable standing collider 
            //+ animated crouch + Reduce the speed
            //If relased resume the original speed + enable the standig collider

            standigCollider.enabled = !crouchFlag;

            //If the player is grounded and pressed space JUMP
            if (jumpFlag)
            {
                isGrounded = false;
                jumpFlag = false;
                //Add Jump force
                rb.AddForce(new Vector2(0f, jumpPower));
            }
        }
        else { 
            animator.SetBool("Jump", true);
            
        }
        

        animator.SetBool("Crouch", crouchFlag);
        #endregion

        #region Move & Run
        //Set value of x using dir and speed
        float xVal = dir * speed * Time.deltaTime;
        //If we are running multiply with the running modifier
        if (isRunning)
            xVal *= runSpeedModifier;

        if (crouchFlag)
            xVal *= crouchSpeedModifier;

        //Create Vec2 for the velocity
        Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
        //Set the player´s velocity
        rb.velocity = targetVelocity;

        //Store the current scale value
        Vector3 currentScale = transform.localScale;
        //if looking right and clicket left(flip to the left)
        if (dir < 0) {
            spr.flipX = true;
        }
        else if (dir > 0)
        {
            spr.flipX = false;
        }

        // 0 iddle, 4 walking, 6 running
        //Set the float xVelocity according to the x value of the Rigibody2D velocity
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        #endregion

    }

}
