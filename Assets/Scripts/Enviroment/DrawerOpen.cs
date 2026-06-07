using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerOpen : MonoBehaviour
{
    public float openDistance = 0.2f; // Distance to move when the drawer is open
    private float smoothTime = 0.2f; // Speed of the drawer movement
    private Vector3 initialPosition; // Initial position of the drawer
    public bool isOpen = false; // Track whether the drawer is open or closed
    private Coroutine moveCoroutine; // Coroutine for moving the drawer
    private bool isMoving = false; // Track whether the drawer is currently moving
    private Vector3 velocity = Vector3.zero; // Velocity for SmoothDamp

    public bool reverse = false;
    void Start()
    {
        initialPosition = transform.position; // Store the initial position
    }

    public void ToggleDrawer()
    {
        if (isMoving) return;

        if (moveCoroutine != null) // If the drawer is moving, stop the coroutine
        {
            StopCoroutine(moveCoroutine);
        }

        Vector3 targetPosition;
        if (isOpen)
        {
            targetPosition = initialPosition; // Move to initial position
        }
        else
        {
            if (reverse)
            {
                targetPosition = initialPosition - transform.parent.forward * openDistance; // Move backward by openDistance
            }
            else
            {
                targetPosition = initialPosition + transform.parent.forward * openDistance; // Move forward by openDistance
            }
               
        }

        moveCoroutine = StartCoroutine(MoveDrawer(targetPosition));
        isOpen = !isOpen;
    }

    private IEnumerator MoveDrawer(Vector3 target)
    {
        isMoving = true;
        while (Vector3.Distance(transform.position, target) > 0.001f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
            yield return null;
        }
        transform.position = target; // Ensure the final position is set
        isMoving = false;
    }
}