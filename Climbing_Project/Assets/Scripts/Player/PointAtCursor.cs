using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointAtCursor : MonoBehaviour
{
    bool isXInput;
    Vector2 thumbstickDirection;
    public float lerpCoeff = 0.5f;
    void Update()
    {
        if (isXInput)
        {
            float thumbstickAngle = Vector2.SignedAngle(Vector2.up, thumbstickDirection);

            if(thumbstickDirection != Vector2.zero) transform.rotation = //Quaternion.AngleAxis(thumbstickAngle, transform.forward);
                Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(thumbstickAngle, transform.forward), lerpCoeff);


            Cursor.lockState = CursorLockMode.Locked;

            
        }
        else
        {
            //get the position of the mouse cursor in the world
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            //turn the vector into an angle
            float mouseAngle = Vector2.SignedAngle(Vector2.up, mousePos - (Vector2)transform.position);

            //use that angle to rotate the gameObject
            transform.rotation = //Quaternion.AngleAxis(thumbstickAngle, transform.forward);
                Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(mouseAngle, transform.forward), lerpCoeff);

            //transform.rotation = Quaternion.AngleAxis(Vector2.SignedAngle(transform.position, (Vector2)transform.position - mousePos), transform.forward);
            //I LOVE QUATERNIONS I LOVE QUATERNIONS ASFHASKJDHAKSHASFA8ASDF

            Cursor.lockState = CursorLockMode.None;
        }

    }

    public void ToggleLook(InputAction.CallbackContext context)
    {
        isXInput = context.control.device is Gamepad;

        if (isXInput)
        {
            thumbstickDirection = context.ReadValue<Vector2>();
        }
    }
}
