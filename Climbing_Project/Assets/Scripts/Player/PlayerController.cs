using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector2 movementInput;
    public float walkSpeed = 60f, jumpForce = 60f, maxWalkSpeed = 5f;
    private Rigidbody2D rb;
    public LayerMask jumpableSurfaces;
    bool isJumping;

    public bool hasGrapple;
    GameObject grapple;

    Camera mainCam;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        grapple = transform.GetChild(0).gameObject;
        mainCam = Camera.main;
    }

    private void Update()
    {
        //walking left & right
        Vector2 walkForce = Vector2.zero;
        walkForce.x += movementInput.x;
        walkForce = walkForce.normalized * walkSpeed;

        //if speed is less than max speed...
        if (Mathf.Abs(rb.linearVelocity.x) < maxWalkSpeed
        //...or the two x values are opposing...
            || rb.linearVelocity.x * walkForce.x <= 0)
        {
            //...then input is accepted
            rb.AddForce(walkForce);
        }
        //jumping
        if (isJumping)
        {
            
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, transform.localScale, 0f, Vector2.down, 0.1f, jumpableSurfaces);

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
        }

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
}
