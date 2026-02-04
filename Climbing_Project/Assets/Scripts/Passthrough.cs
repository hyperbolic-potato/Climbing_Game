using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Passthrough : MonoBehaviour
{
    Collider2D col;
    Collider2D otherCol;
    private void Start()
    {
        col = GetComponent<Collider2D>();
    }

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        otherCol = collision.collider;
        Debug.Log(collision.contacts[0].normal);
        if (collision.collider.CompareTag("Player") && collision.contacts[0].normal.y != -1f)
        {
            Physics2D.IgnoreCollision(collision.collider, col, true);
            CancelPassthrough();
        }
    }

    IEnumerator CancelPassthrough()
    {
        yield return new WaitForEndOfFrame();
        Physics2D.IgnoreCollision(otherCol, col, true);
    }
}
