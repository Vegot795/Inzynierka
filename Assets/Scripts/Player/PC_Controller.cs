using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Rendering.Canvas.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class PC_Controller : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public Vector2 mousePos;

    private bool canMove = true;


    Vector2 movementInput;
    Vector3 mouseRotation;
    Rigidbody2D rb;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    SpriteRenderer spriteRenderer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = PlayerRotation();
    }

    private void FixedUpdate()
    {
        if (movementInput != Vector2.zero)
        {
            bool success = TryMove(movementInput);
            if (!success)
            {
                success = TryMove(new Vector2(movementInput.x, 0));
            }
            if (!success)
            {
                success = TryMove(new Vector2(0, movementInput.y));
            }
        }
    }

#region ---Input System---
    private void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    private bool TryMove(Vector2 direction)
    {
       if(canMove)
        {
            if (direction != Vector2.zero)
            {
                int count = rb.Cast(
                    movementInput,
                    movementFilter,
                    castCollisions,
                    moveSpeed * Time.deltaTime + collisionOffset);
                if (count == 0)
                {
                    rb.MovePosition(rb.position + movementInput * moveSpeed * Time.deltaTime);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
       else
        {
            return false;
        }
    }
#endregion

    public Vector2 PlayerRotation()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 direction = (mouseWorldPosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, angle + 90f);

        return direction;
    }
}
