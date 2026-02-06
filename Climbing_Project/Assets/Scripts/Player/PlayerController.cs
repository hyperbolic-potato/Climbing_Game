using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Vector2 movementInput;
    private Vector2 respawnPoint;
    public float walkSpeed = 60f, jumpForce = 60f, climbSpeed, maxWalkSpeed = 5f, jumpClimbDelay, deathDelay;
    private float gravity;
    private Rigidbody2D rb;
    private Collider2D col;
    public LayerMask jumpableSurfaces, climbableSurfaces;
    bool isJumping, isClimbing, canClimb, isDelayed;
   

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

        respawnPoint = Vector2.zero;

        ll = GameObject.FindWithTag("LevelLoader").GetComponent<LevelLoader>();
    }

    private void Update()
    {
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
            
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, transform.localScale * 0.85f, 0f, Vector2.down, 0.1f, jumpableSurfaces);

            if (hit)
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
                isJumping = false;

            }
            else if (isClimbing)
            {
                isClimbing = false;
                isJumping = false;
                StartCoroutine(ClimbDelay());
                if (movementInput.y > -0.5)
                {
                    rb.linearVelocityY = 0;
                    rb.AddForceY(jumpForce, ForceMode2D.Impulse);
                }
                
                
            }
        }

        //climbing

        

        //determine if the character is climbing via a toggle. Jumping escapes the climb

        if(canClimb && movementInput.y >= 0.5f)
        {
            if(!isClimbing) rb.linearVelocity = Vector2.zero;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Hazard"))
        {
            StartCoroutine(Death());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            respawnPoint = collision.transform.position;
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if(!isDelayed) canClimb = collision.CompareTag("Climbable");
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(!col.IsTouchingLayers(climbableSurfaces))
            canClimb = false;
        
    }

    

    IEnumerator ClimbDelay()
    {
        isDelayed = true;
        canClimb = false;
        yield return new WaitForSeconds(jumpClimbDelay);
        isDelayed = false;
    }

}
