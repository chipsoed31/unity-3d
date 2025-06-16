// Скрипт для подбора и переноса предметов в Unity, как в Half-Life 2 (ручной захват)

using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 5f;
    public float holdDistance = 2f;
    public float moveForce = 250f;
    public float throwForce = 500f;
    public LayerMask pickupLayer;

    [Header("References")]
    public Transform holdPoint;
    public Camera playerCamera;

    private Rigidbody heldObject;
    private bool isHolding => heldObject != null;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHolding)
                TryPickup();
            else
                DropObject();
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (isHolding)
                ThrowObject();
        }
    }

    void FixedUpdate()
    {
        if (isHolding)
        {
            Vector3 directionToHold = (holdPoint.position - heldObject.position);
            heldObject.linearVelocity = directionToHold * moveForce * Time.fixedDeltaTime;
        }
    }

    void LateUpdate()
    {
        holdPoint.position = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;
    }

    void TryPickup()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupLayer))
        {
            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null)
            {
                heldObject = rb;
                heldObject.useGravity = false;
                heldObject.linearDamping = 0.5f;
            }
        }
    }

    void DropObject()
    {
        if (heldObject)
        {
            heldObject.useGravity = true;
            heldObject.linearDamping = 0.1f;
            heldObject = null;
        }
    }

    void ThrowObject()
    {
        if (heldObject)
        {
            heldObject.useGravity = true;
            heldObject.linearDamping = 0.1f;
            heldObject.AddForce(playerCamera.transform.forward * throwForce);
            heldObject = null;
        }
        
    }
}
