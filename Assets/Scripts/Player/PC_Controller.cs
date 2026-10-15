using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PC_Controller : CharacterBase
{
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public Vector2 mousePos;
    private bool canMove = true;
    private bool hasDied = false;

    Vector2 movementInput;
    Vector3 mouseRotation;
    Rigidbody2D rb;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    SpriteRenderer spriteRenderer;
    GameControllerScript GCS;

    public override void Awake() 
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseMoveSpeed = 10f;
        gameObject.layer = LayerMask.NameToLayer("Player");
        GCS = GameControllerScript.GCS;
    }

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
        currentMoveSpeed = baseMoveSpeed * moveSpeedModif;
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
                    (currentMoveSpeed)* Time.deltaTime + collisionOffset);
                if (count == 0)
                {
                    rb.MovePosition(rb.position + movementInput * (baseMoveSpeed * moveSpeedModif) * Time.deltaTime);
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

    protected override void Die(CharacterBase killer)
    {
        if (hasDied)
        {
            return;
        }
        hasDied = true;
        Time.timeScale = 0;
        var UIC = UIController.UIC;
        if (UIC == null)
        {
            Debug.Log("[PLayer] - UIC was assigned");
        }
        UIC.ShowDeathScreen();
        GCS.UpdatePlayerData();
    }
}
