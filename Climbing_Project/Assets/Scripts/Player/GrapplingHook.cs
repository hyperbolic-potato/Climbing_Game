using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingHook : MonoBehaviour
{
    LineRenderer lr;
    Rigidbody2D rb;

    public LayerMask grappleableObjects;
    public float maxGrappleDistance, grappleStrength, spoolSpeed;
    private float grappleLength;
    bool isGrappled, isReeling;
    Vector2 grapplePoint, grappleVector;
    Rigidbody2D grappledRB;
    GameObject dynamicGrapplePoint;
    

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        rb = transform.parent.parent.gameObject.GetComponent<Rigidbody2D>();
        isGrappled = false;
        dynamicGrapplePoint = new GameObject(); //only making this for its transform
    }

    private void Update()
    {
        
        if (isGrappled)
        {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, grapplePoint);

            //get the vector from the character to the point it is grappling to
            grappleVector = grapplePoint - (Vector2)transform.position;

            GrappleLengthCorrection();


            if (isReeling)
            {
                // get the angle between the grapple line and the pointing direction
                float shear = Vector2.Angle(grappleVector.normalized, transform.up);

                //turn that into a number between 1 & -1 depending on the sharpness of said angle
                shear = (shear / 180 - 0.5f) * 2f;
                

                //multiply the shear coefficient accordingly to make the grapple spool out when pointed towards and in when pointed away
                if (grappleLength > 0) grappleLength += Time.deltaTime * -shear * spoolSpeed;
                else grappleLength = 0;
            }
            
        }

        lr.enabled = isGrappled;

    }

    public void Grapple(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            
            if (!isGrappled)
            {
                
                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, maxGrappleDistance, grappleableObjects);

                if (hit)
                {
                    grapplePoint = hit.point;


                    grappleLength = (grapplePoint - (Vector2)transform.position).magnitude;

                    isGrappled = true;

                    if (hit.collider.gameObject.GetComponent<Rigidbody2D>() != null)
                    {
                        grappledRB = hit.collider.gameObject.GetComponent<Rigidbody2D>();
                        //localGrappleOffset = grapplePoint - (Vector2)grappledRB.transform.position;
                        dynamicGrapplePoint.transform.parent = grappledRB.transform;
                        dynamicGrapplePoint.transform.position = grapplePoint;
                        
                    }
                    else grappledRB = null;
                    
                }
            }
            else
            {
                isGrappled = false;
            }
        }
        

        
    }

    public void GrappleLengthCorrection() // will keep the grapple from getting longer than it was when it was fired by applying forces to the character
    {

        Vector2 error = grappleVector - grappleVector.normalized * grappleLength; //error is actual length minus expected length
        if (grappledRB != null && grappledRB.bodyType != RigidbodyType2D.Dynamic)
        {
            if (grappleVector.magnitude > grappleLength) rb.AddForceAtPosition(error * grappleStrength, transform.position);
            //if the other RB isn't dynamic, just the player moves
        }
        else
        {
            grapplePoint = dynamicGrapplePoint.transform.position;

            if (grappleVector.magnitude > grappleLength)
            {
                rb.AddForceAtPosition(error * grappleStrength/2, transform.position);
                grappledRB.AddForceAtPosition(-error * grappleStrength/2, grapplePoint);
                
            }
            //otherwise, both the grappled object and the player are affected in opposite directions at half strength. Physics, baby.
        }

    }

    public void GrappleReel(InputAction.CallbackContext context) // the input function that will spool in the grapple
    {
        if(context.performed) isReeling = true;
        else isReeling = false;
    }
}
