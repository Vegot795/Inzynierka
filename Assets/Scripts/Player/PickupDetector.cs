using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerInventory))]
public class PickupDetector : MonoBehaviour
{
    private PlayerInventory inventory;
    private Camera mainCamera;

    private readonly List<PickupBase> nearbyPickups = new();

    private PickupBase hoveredPickup;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        UpdateHoveredPickup();
    }

    private void OnPickup(InputValue value)
    {
        if (hoveredPickup == null)
        {
            return;
        }

        hoveredPickup.OnPickedUp(inventory);
        nearbyPickups.Remove(hoveredPickup);
        hoveredPickup = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PickupBase pickup = other.GetComponent<PickupBase>();
        if (pickup != null && !nearbyPickups.Contains(pickup))
        {
            nearbyPickups.Add(pickup);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PickupBase pickup = other.GetComponent<PickupBase>();
        if (pickup != null)
        {
            if (pickup == hoveredPickup)
            {
                hoveredPickup = null;
            }
            pickup.OnPlayerExit();
            nearbyPickups.Remove(pickup);
        }
    }

    private void UpdateHoveredPickup()
    {
        nearbyPickups.RemoveAll(p => p == null);

        Vector2 mouseWorld = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        PickupBase closest = null;
        float closestDistance = float.MaxValue;

        foreach (var pickup in nearbyPickups)
        {
            float distance = Vector2.Distance(mouseWorld, pickup.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = pickup;
            }
        }

        if (closest != hoveredPickup)
        {
            hoveredPickup?.OnPlayerExit();
            hoveredPickup = closest;
            hoveredPickup?.OnPlayerEnter();
        }
    }
}
