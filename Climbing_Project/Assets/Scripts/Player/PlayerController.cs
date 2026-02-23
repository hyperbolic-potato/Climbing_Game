using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Vector2 movementInput;
    public float walkSpeed = 60f, jumpForce = 60f, climbSpeed, maxWalkSpeed = 5f, jumpClimbDelay, deathDelay, walkDecceleration, jumpDecceleration, lethalFallDistance;
    private float gravity, fallDistance;
    private Rigidbody2D rb;
    private Collider2D col;
    public LayerMask jumpableSurfaces, climbableSurfaces;
    bool isJumping, isGrounded, isClimbing, canClimb, isDelayed, isDead, hasWon, wasClimbing;
   

    public bool hasGrapple;
    GameObject grapple;

    Camera mainCam;

    LevelLoader ll;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        grapple = transform.GetChild(0).gameObject;
        mainCam = Camera.main;

        gravity = rb.gravityScale;
        

        ll = GameObject.FindWithTag("LevelLoader").GetComponent<LevelLoader>();
        transform.position = ll.respawnPos;
        isDead = false;
    }

    private void Update()
    {
        if (!(isDead || hasWon))
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, transform.localScale * 0.85f, 0f, Vector2.down, 0.1f, jumpableSurfaces);
            
            
            isGrounded = hit;

            

            //walking left & right
            Vector2 walkForce = Vector2.zero;
            walkForce.x += movementInput.x;
            walkForce = walkForce.normalized * walkSpeed;

            //if speed is less than max speed...
            if (!isClimbing && (Mathf.Abs(rb.linearVelocity.x) < maxWalkSpeed
                //...or the two x values are opposing...
                || rb.linearVelocity.x * walkForce.x <= 0))
            {
                //...then input is accepted
                rb.AddForce(walkForce);
            }
            //jumping
            if (isJumping)
            {
                if (isGrounded && rb.linearVelocityY <= 0)
                {
                    //rb.linearVelocityY = 0;
                    //im sure this isn't going to be complicit in some game-breaking physics exploits...
                    //it was :/
                    Rigidbody2D otherRB = hit.collider.gameObject.GetComponent<Rigidbody2D>();
                    if (otherRB != null && otherRB.bodyType != RigidbodyType2D.Dynamic)
                    {
                        rb.linearVelocityY = 0;
                        rb.AddForceY(jumpForce, ForceMode2D.Impulse);

                    }
                    else
                    {
                        rb.AddForceY(jumpForce, ForceMode2D.Impulse);
                        otherRB.AddForceY(-jumpForce, ForceMode2D.Impulse);
                        //prevents the player from flying via rigidbody abuse

                    }
                }
                else if (isClimbing || wasClimbing)
                {
                    isClimbing = false;
                    StartCoroutine(ClimbDelay());
                    if (movementInput.y > -0.5)
                    {
                        rb.linearVelocityY = 0;
                        rb.AddForceY(jumpForce, ForceMode2D.Impulse);
                    }


                }


            }

            if(!isJumping && rb.linearVelocityY > 0f)
            {
                rb.linearVelocityY *= jumpDecceleration;
            }

            //fatal falls

            if (rb.linearVelocityY < 0f)
            {
                fallDistance -= rb.linearVelocityY * Time.deltaTime;
            }
            else
            {
                
                if (fallDistance > lethalFallDistance) StartCoroutine(Death());
                fallDistance = 0f;
            }

            Debug.Log(fallDistance);

            if (fallDistance > lethalFallDistance)
            {
                canClimb = false;
                
            }

            //climbing



            //determine if the character is climbing via a toggle. Jumping escapes the climb

            if (canClimb && (movementInput.y >= 0.5f || wasClimbing))
            {
                if (!isClimbing) rb.linearVelocity = Vector2.zero;

                isClimbing = true;
                isJumping = false;

            }

            if (!canClimb || isJumping)
            {
                isClimbing = false;
            }

            if (isClimbing)
            {
                rb.gravityScale = 0f;
                rb.MovePosition(transform.position + (Vector3)movementInput * (climbSpeed / 60f));
            }
            else
            {
                rb.gravityScale = gravity;
            }

            //grappling is handled by an object childed to this gameobject
            grapple.SetActive(hasGrapple);

            //ground friction
            if (isGrounded && (movementInput.x == 0f || rb.linearVelocity.x * movementInput.x <= 0))
            {
                rb.linearVelocityX *= walkDecceleration;
            }

            
        }

        
        
        
        
    }

    IEnumerator Death()
    {
        //death animation/behavior goes here
        isDead = true;

        GetComponent<SpriteRenderer>().color = Color.red;

        yield return new WaitForSeconds(deathDelay);

        ll.LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void GetMovementInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isJumping = true;
            

        }
        if (context.canceled)
        {
            isJumping = false;
        }
    }

    public void Victory()
    {
        //this is pure fun
        hasWon = true;
        rb.linearVelocity = Vector2.up * 7f;
        rb.gravityScale = 0f;
        gravity = 0f;
        GetComponent<SpriteRenderer>().color = Color.green;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Hazard"))
        {
            StartCoroutine(Death());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDelayed) canClimb = collision.CompareTag("Climbable");

        if (collision.CompareTag("Checkpoint"))
        {
            ll.respawnPos = collision.transform.position;
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("Victory"))
        {
            Victory();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!col.IsTouchingLayers(climbableSurfaces))
        {
            canClimb = false;
            if(isClimbing) StartCoroutine(ClimbJumpGrace());
        }
            
        
    }

    IEnumerator ClimbJumpGrace()
    {
        wasClimbing = true;
        yield return new WaitForSeconds(0.05f);
        while (rb.linearVelocityY < 0)
        {
            yield return new WaitForEndOfFrame();
        }
        wasClimbing = false;

    }

    IEnumerator ClimbDelay()
    {
        isDelayed = true;
        canClimb = false;
        yield return new WaitForSeconds(jumpClimbDelay);
        isDelayed = false;
    }

}
